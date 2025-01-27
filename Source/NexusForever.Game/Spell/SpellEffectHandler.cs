using System.Numerics;
using Microsoft.Extensions.DependencyInjection;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Housing;
using NexusForever.Game.Abstract.Spell;
using NexusForever.Game.Combat;
using NexusForever.Game.Entity;
using NexusForever.Game.Housing;
using NexusForever.Game.Map;
using NexusForever.Game.Static.Entity;
using NexusForever.Game.Static.Spell;
using NexusForever.GameTable;
using NexusForever.GameTable.Model;
using NexusForever.Network.World.Message.Model;
using NexusForever.Shared;
using NLog;

namespace NexusForever.Game.Spell
{
    public static class SpellHandler
    {
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        [SpellEffectHandler(SpellEffectType.Damage)]
        public static void HandleEffectDamage(ISpell spell, IUnitEntity target, ISpellTargetEffectInfo info)
        {
            if (!target.CanAttack(spell.Caster))
                return;

            uint damage = 0;
            damage += DamageCalculator.Instance.GetBaseDamageForSpell(spell.Caster, info.Entry.ParameterType00, info.Entry.ParameterValue00);
            damage += DamageCalculator.Instance.GetBaseDamageForSpell(spell.Caster, info.Entry.ParameterType01, info.Entry.ParameterValue01);
            damage += DamageCalculator.Instance.GetBaseDamageForSpell(spell.Caster, info.Entry.ParameterType02, info.Entry.ParameterValue02);
            damage += DamageCalculator.Instance.GetBaseDamageForSpell(spell.Caster, info.Entry.ParameterType03, info.Entry.ParameterValue03);

            DamageCalculator.Instance.CalculateDamage(spell.Caster, target, spell, ref info, (DamageType)info.Entry.DamageType, damage);

            target.TakeDamage(spell.Caster, info.Damage);
        }

        [SpellEffectHandler(SpellEffectType.Resurrect)]
        public static void HandleEffectResurrect(ISpell spell, IUnitEntity target, ISpellTargetEffectInfo info)
        {
            if (target is not IPlayer player)
                return;

            player.ResurrectionManager.ResurrectRequest(spell.Caster.Guid);
        }

        [SpellEffectHandler(SpellEffectType.Proxy)]
        public static void HandleEffectProxy(ISpell spell, IUnitEntity target, ISpellTargetEffectInfo info)
        {
            // Some Proxies can be triggered only a certain amount of times per cast, by any target, and we evaluate all targets at once to apply Proxy effects.
            // This checks that value to ensure we've not exceeded the unique number of times this can fire.
            // A good example of this is for the Esper Ability Telekinetic Strike, it has a Proxy that grants Psi point when it hits an enemy.
            // However, Esper's can only generate a maximum of 1 Psi Point per cast. This tracks that value that seems to indicate it's a 1-time effect per cast.
            if (spell.GetEffectTriggerCount(info.Entry.Id, out uint count))
                if (count >= info.Entry.DataBits04)
                    return;

            spell.AddProxy(new Proxy(target, info.Entry, spell, spell.Parameters));
        }

        [SpellEffectHandler(SpellEffectType.Disguise)]
        public static void HandleEffectDisguise(ISpell spell, IUnitEntity target, ISpellTargetEffectInfo info)
        {
            Creature2Entry creature2 = GameTableManager.Instance.Creature2.GetEntry(info.Entry.DataBits02);
            if (creature2 == null)
                return;

            Creature2DisplayGroupEntryEntry displayGroupEntry = GameTableManager.Instance.Creature2DisplayGroupEntry.Entries.FirstOrDefault(d => d.Creature2DisplayGroupId == creature2.Creature2DisplayGroupId);
            if (displayGroupEntry == null)
                return;

            target.DisplayInfo = displayGroupEntry.Creature2DisplayInfoId;
        }

        [SpellEffectHandler(SpellEffectType.SummonMount)]
        public static void HandleEffectSummonMount(ISpell spell, IUnitEntity target, ISpellTargetEffectInfo info)
        {
            // TODO: handle NPC mounting?
            if (target is not IPlayer player)
                return;

            if (!player.CanMount())
                return;

            // TODO: needs to be replaced once spell effect handlers aren't static
            var factory = LegacyServiceProvider.Provider.GetService<IEntityFactory>();

            var mount = factory.CreateEntity<IMountEntity>();
            mount.Initialise(player, spell.Parameters.SpellInfo.Entry.Id, info.Entry.DataBits00, info.Entry.DataBits01, info.Entry.DataBits04);
            mount.EnqueuePassengerAdd(player, VehicleSeatType.Pilot, 0);

            // usually for hover boards
            /*if (info.Entry.DataBits04 > 0u)
            {
                mount.SetAppearance(new ItemVisual
                {
                    Slot      = ItemSlot.Mount,
                    DisplayId = (ushort)info.Entry.DataBits04
                });
            }*/

            var position = new MapPosition
            {
                Position = player.Position
            };

            if (player.Map.CanEnter(mount, position))
                player.Map.EnqueueAdd(mount, position);

            // FIXME: also cast 52539,Riding License - Riding Skill 1 - SWC - Tier 1,34464
            // FIXME: also cast 80530,Mount Sprint  - Tier 2,36122

            player.CastSpell(52539, new SpellParameters());
            player.CastSpell(80530, new SpellParameters());
        }

        [SpellEffectHandler(SpellEffectType.Teleport)]
        public static void HandleEffectTeleport(ISpell spell, IUnitEntity target, ISpellTargetEffectInfo info)
        {
            if (!(target is Player player))
                return;

            WorldLocation2Entry locationEntry = GameTableManager.Instance.WorldLocation2.GetEntry(info.Entry.DataBits00);
            if (locationEntry == null)
                return;

            // Handle Housing Teleport
            if (locationEntry.WorldId == 1229)
            {
                IResidence residence = GlobalResidenceManager.Instance.GetResidenceByOwner(player.Name);
                if (residence == null)
                    residence = GlobalResidenceManager.Instance.CreateResidence(player);

                IResidenceEntrance entrance = GlobalResidenceManager.Instance.GetResidenceEntrance(residence.PropertyInfoId);
                if (player.CanTeleport())
                {
                    player.Rotation = entrance.Rotation.ToEulerDegrees();
                    player.TeleportTo(entrance.Entry, entrance.Position, residence.Parent?.Id ?? residence.Id);
                    return;
                }
            }

            if (player.CanTeleport())
            {
                player.Rotation = new Quaternion(locationEntry.Facing0, locationEntry.Facing1, locationEntry.Facing2, locationEntry.Facing3).ToEulerDegrees();
                player.TeleportTo((ushort)locationEntry.WorldId, locationEntry.Position0, locationEntry.Position1, locationEntry.Position2);
            }
        }

        [SpellEffectHandler(SpellEffectType.FullScreenEffect)]
        public static void HandleFullScreenEffect(ISpell spell, IUnitEntity target, ISpellTargetEffectInfo info)
        {
            // TODO/FIXME: Add duration into the queue so that the spell will automatically finish at the correct time. This is a workaround for Full Screen Effects.
            //events.EnqueueEvent(new Event.SpellEvent(info.Entry.DurationTime / 1000d, () => { status = SpellStatus.Finished; SendSpellFinish(); }));
        }

        [SpellEffectHandler(SpellEffectType.RapidTransport)]
        public static void HandleEffectRapidTransport(ISpell spell, IUnitEntity target, ISpellTargetEffectInfo info)
        {
            TaxiNodeEntry taxiNode = GameTableManager.Instance.TaxiNode.GetEntry(spell.Parameters.TaxiNode);
            if (taxiNode == null)
                return;

            WorldLocation2Entry worldLocation = GameTableManager.Instance.WorldLocation2.GetEntry(taxiNode.WorldLocation2Id);
            if (worldLocation == null)
                return;

            if (target is not IPlayer player)
                return;

            if (!player.CanTeleport())
                return;

            var rotation = new Quaternion(worldLocation.Facing0, worldLocation.Facing0, worldLocation.Facing2, worldLocation.Facing3);
            player.Rotation = rotation.ToEulerDegrees();
            player.TeleportTo((ushort)worldLocation.WorldId, worldLocation.Position0, worldLocation.Position1, worldLocation.Position2);
        }

        [SpellEffectHandler(SpellEffectType.LearnDyeColor)]
        public static void HandleEffectLearnDyeColor(ISpell spell, IUnitEntity target, ISpellTargetEffectInfo info)
        {
            if (target is not IPlayer player)
                return;

            player.Account.GenericUnlockManager.Unlock((ushort)info.Entry.DataBits00);
        }

        [SpellEffectHandler(SpellEffectType.UnlockMount)]
        public static void HandleEffectUnlockMount(ISpell spell, IUnitEntity target, ISpellTargetEffectInfo info)
        {
            if (target is not IPlayer player)
                return;

            Spell4Entry spell4Entry = GameTableManager.Instance.Spell4.GetEntry(info.Entry.DataBits00);
            player.SpellManager.AddSpell(spell4Entry.Spell4BaseIdBaseSpell);

            player.Session.EnqueueMessageEncrypted(new ServerUnlockMount
            {
                Spell4Id = info.Entry.DataBits00
            });
        }

        [SpellEffectHandler(SpellEffectType.UnlockPetFlair)]
        public static void HandleEffectUnlockPetFlair(ISpell spell, IUnitEntity target, ISpellTargetEffectInfo info)
        {
            if (target is not IPlayer player)
                return;

            player.PetCustomisationManager.UnlockFlair((ushort)info.Entry.DataBits00);
        }

        [SpellEffectHandler(SpellEffectType.UnlockVanityPet)]
        public static void HandleEffectUnlockVanityPet(ISpell spell, IUnitEntity target, ISpellTargetEffectInfo info)
        {
            if (target is not IPlayer player)
                return;

            Spell4Entry spell4Entry = GameTableManager.Instance.Spell4.GetEntry(info.Entry.DataBits00);
            player.SpellManager.AddSpell(spell4Entry.Spell4BaseIdBaseSpell);

            player.Session.EnqueueMessageEncrypted(new ServerUnlockMount
            {
                Spell4Id = info.Entry.DataBits00
            });
        }

        [SpellEffectHandler(SpellEffectType.SummonVanityPet)]
        public static void HandleEffectSummonVanityPet(ISpell spell, IUnitEntity target, ISpellTargetEffectInfo info)
        {
            if (target is not IPlayer player)
                return;

            // enqueue removal of existing vanity pet if summoned
            if (player.VanityPetGuid != null)
            {
                IPetEntity oldVanityPet = player.GetVisible<IPetEntity>(player.VanityPetGuid.Value);
                oldVanityPet?.RemoveFromMap();
                player.VanityPetGuid = 0u;
            }

            // TODO: needs to be replaced once spell effect handlers aren't static
            var factory = LegacyServiceProvider.Provider.GetService<IEntityFactory>();

            var pet = factory.CreateEntity<IPetEntity>();
            pet.Initialise(player, info.Entry.DataBits00);

            var position = new MapPosition
            {
                Position = player.Position
            };

            if (player.Map.CanEnter(pet, position))
                player.Map.EnqueueAdd(pet, position);
        }

        [SpellEffectHandler(SpellEffectType.TitleGrant)]
        public static void HandleEffectTitleGrant(ISpell spell, IUnitEntity target, ISpellTargetEffectInfo info)
        {
            if (target is not IPlayer player)
                return;

            player.TitleManager.AddTitle((ushort)info.Entry.DataBits00);
        }

        [SpellEffectHandler(SpellEffectType.Fluff)]
        public static void HandleEffectFluff(ISpell spell, IUnitEntity target, ISpellTargetEffectInfo info)
        {
        }

        [SpellEffectHandler(SpellEffectType.UnitPropertyModifier)]
        public static void HandleEffectPropertyModifier(ISpell spell, IUnitEntity target, ISpellTargetEffectInfo info)
        {
            // TODO: I suppose these could be cached somewhere instead of generating them every single effect?
            SpellPropertyModifier modifier =
                new SpellPropertyModifier((Property)info.Entry.DataBits00,
                    info.Entry.DataBits01,
                    BitConverter.UInt32BitsToSingle(info.Entry.DataBits02),
                    BitConverter.UInt32BitsToSingle(info.Entry.DataBits03),
                    BitConverter.UInt32BitsToSingle(info.Entry.DataBits04));
            target.AddSpellModifierProperty(modifier, spell.Parameters.SpellInfo.Entry.Id);

            // TODO: Handle removing spell modifiers

            //if (info.Entry.DurationTime > 0d)
            //    events.EnqueueEvent(new SpellEvent(info.Entry.DurationTime / 1000d, () =>
            //    {
            //        player.RemoveSpellProperty((Property)info.Entry.DataBits00, parameters.SpellInfo.Entry.Id);
            //    }));
        }

        [SpellEffectHandler(SpellEffectType.Activate)]
        public static void HandleEffectActivate(ISpell spell, IUnitEntity target, ISpellTargetEffectInfo info)
        {
            spell.Parameters.ClientSideInteraction?.HandleSuccess(spell);
        }
    }
}
