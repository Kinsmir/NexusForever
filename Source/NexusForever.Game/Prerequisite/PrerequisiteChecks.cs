using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Static.Entity;
using NexusForever.Game.Static.Prerequisite;
using NexusForever.Game.Static.Quest;
using NexusForever.Game.Static.Reputation;
using Path = NexusForever.Game.Static.Entity.Path;

namespace NexusForever.Game.Prerequisite
{
    public sealed partial class PrerequisiteManager
    {
        [PrerequisiteCheck(PrerequisiteType.Level)]
        private static bool PrerequisiteCheckLevel(IPlayer player, PrerequisiteComparison comparison, uint value, uint objectId)
        {
            switch (comparison)
            {
                case PrerequisiteComparison.Equal:
                    return player.Level == value;
                case PrerequisiteComparison.NotEqual:
                    return player.Level != value;
                case PrerequisiteComparison.GreaterThan:
                    return player.Level > value;
                case PrerequisiteComparison.GreaterThanOrEqual:
                    return player.Level >= value;
                case PrerequisiteComparison.LessThan:
                    return player.Level < value;
                case PrerequisiteComparison.LessThanOrEqual:
                    return player.Level <= value;
                default:
                    log.Warn($"Unhandled PrerequisiteComparison {comparison} for {PrerequisiteType.Level}!");
                    return false;
            }
        }

        [PrerequisiteCheck(PrerequisiteType.Race)]
        private static bool PrerequisiteCheckRace(IPlayer player, PrerequisiteComparison comparison, uint value, uint objectId)
        {
            switch (comparison)
            {
                case PrerequisiteComparison.Equal:
                    return player.Race == (Race)value;
                case PrerequisiteComparison.NotEqual:
                    return player.Race != (Race)value;
                default:
                    log.Warn($"Unhandled PrerequisiteComparison {comparison} for {PrerequisiteType.Race}!");
                    return false;
            }
        }

        [PrerequisiteCheck(PrerequisiteType.Class)]
        private static bool PrerequisiteCheckClass(IPlayer player, PrerequisiteComparison comparison, uint value, uint objectId)
        {
            switch (comparison)
            {
                case PrerequisiteComparison.Equal:
                    return player.Class == (Class)value;
                case PrerequisiteComparison.NotEqual:
                    return player.Class != (Class)value;
                default:
                    log.Warn($"Unhandled PrerequisiteComparison {comparison} for {PrerequisiteType.Class}!");
                    return false;
            }
        }

        [PrerequisiteCheck(PrerequisiteType.Quest)]
        private static bool PrerequisiteCheckQuest(IPlayer player, PrerequisiteComparison comparison, uint value, uint objectId)
        {
            switch (comparison)
            {
                case PrerequisiteComparison.Equal:
                    return player.QuestManager.GetQuestState((ushort)objectId) == (QuestState)value;
                case PrerequisiteComparison.NotEqual:
                    return player.QuestManager.GetQuestState((ushort)objectId) != (QuestState)value;
                default:
                    log.Warn($"Unhandled PrerequisiteComparison {comparison} for {PrerequisiteType.Quest}!");
                    return false;
            }
        }

        [PrerequisiteCheck(PrerequisiteType.Prerequisite)]
        private static bool PrerequisiteCheckPrerequisite(IPlayer player, PrerequisiteComparison comparison, uint value, uint objectId)
        {
            switch (comparison)
            {
                case PrerequisiteComparison.NotEqual:
                    return !Instance.Meets(player, objectId);
                case PrerequisiteComparison.Equal:
                    return Instance.Meets(player, objectId);
                default:
                    log.Warn($"Unhandled {comparison} for {PrerequisiteType.Prerequisite}!");
                    return false;
            }
        }

        [PrerequisiteCheck(PrerequisiteType.Path)]
        private static bool PrerequisiteCheckPath(IPlayer player, PrerequisiteComparison comparison, uint value, uint objectId)
        {
            switch (comparison)
            {
                case PrerequisiteComparison.Equal:
                    return player.PathManager.IsPathActive((Path)value);
                default:
                    log.Warn($"Unhandled PrerequisiteComparison {comparison} for {PrerequisiteType.Path}!");

                    return false;
            }
        }

        [PrerequisiteCheck(PrerequisiteType.Achievement)]
        private static bool PrerequisiteCheckAchievement(IPlayer player, PrerequisiteComparison comparison, uint value, uint objectId)
        {
            switch (comparison)
            {
                case PrerequisiteComparison.NotEqual:
                    return !player.AchievementManager.HasCompletedAchievement((ushort)objectId);
                case PrerequisiteComparison.Equal:
                    return player.AchievementManager.HasCompletedAchievement((ushort)objectId);
                default:
                    log.Warn($"Unhandled PrerequisiteComparison {comparison} for {PrerequisiteType.Achievement}!");
                    return false;
            }
        }

        [PrerequisiteCheck(PrerequisiteType.ItemProficiency)]
        private static bool PrerequisiteCheckItemProficiency(IPlayer player, PrerequisiteComparison comparison, uint value, uint objectId)
        {
            switch (comparison)
            {
                case PrerequisiteComparison.NotEqual:
                    return (player.GetItemProficiencies() & (ItemProficiency)value) == 0;
                case PrerequisiteComparison.Equal:
                    return (player.GetItemProficiencies() & (ItemProficiency)value) != 0;
                default:
                    log.Warn($"Unhandled PrerequisiteComparison {comparison} for {PrerequisiteType.Achievement}!");
                    return false;
            }
        }

        [PrerequisiteCheck(PrerequisiteType.Gender)]
        private static bool PrerequisiteCheckGender(IPlayer player, PrerequisiteComparison comparison, uint value, uint objectId)
        {
            switch (comparison)
            {
                case PrerequisiteComparison.NotEqual:
                    return player.Sex != (Sex)value;
                case PrerequisiteComparison.Equal:
                    return player.Sex == (Sex)value;
                default:
                    log.Warn($"Unhandled PrerequisiteComparison {comparison} for {PrerequisiteType.Gender}!");
                    return false;
            }
        }

        [PrerequisiteCheck(PrerequisiteType.AccountItemClaimed)]
        private static bool PrerequisiteCheckAccountItemClaimed(IPlayer player, PrerequisiteComparison comparison, uint value, uint objectId, IUnitEntity target)
        {
            switch (comparison)
            {
                case PrerequisiteComparison.NotEqual:
                    //return !player.Inventory.HasItem(value);
                case PrerequisiteComparison.Equal:
                    //return player.Inventory.HasItem(value);
                default:
                    log.Warn($"Unhandled PrerequisiteComparison {comparison} for {PrerequisiteType.AccountItemClaimed}!");
                    return false;
            }
        }

        [PrerequisiteCheck(PrerequisiteType.SpellBaseId)]
        private static bool PrerequisiteCheckSpellBaseId(IPlayer player, PrerequisiteComparison comparison, uint value, uint objectId)
        {
            switch (comparison)
            {
                case PrerequisiteComparison.NotEqual:
                    return player.SpellManager.GetSpell(objectId) == null;
                case PrerequisiteComparison.Equal:
                    return player.SpellManager.GetSpell(objectId) != null;
                default:
                    log.Warn($"Unhandled PrerequisiteComparison {comparison} for {PrerequisiteType.Achievement}!");
                    return false;
            }
        }

        [PrerequisiteCheck(PrerequisiteType.CosmicRewards)]
        private static bool PrerequisiteCheckCosmicRewards(IPlayer player, PrerequisiteComparison comparison, uint value, uint objectId, IUnitEntity target)
        {
            switch (comparison)
            {
                case PrerequisiteComparison.NotEqual:
                    //return player.Session.AccountCurrencyManager.GetAmount(Account.Static.AccountCurrencyType.CosmicReward) != value;
                case PrerequisiteComparison.Equal:
                    //return player.Session.AccountCurrencyManager.GetAmount(Account.Static.AccountCurrencyType.CosmicReward) == value;
                case PrerequisiteComparison.GreaterThan:
                case PrerequisiteComparison.LessThanOrEqual: 
                    // The conditional below is intentionally "incorrect". It's possible PrerequisiteComparison 4 is actually GreaterThanOrEqual
                    //return player.Session.AccountCurrencyManager.GetAmount(Account.Static.AccountCurrencyType.CosmicReward) >= value;
                default:
                    log.Warn($"Unhandled PrerequisiteComparison {comparison} for {PrerequisiteType.CosmicRewards}!");
                    return false;
            }
        }

        [PrerequisiteCheck(PrerequisiteType.BaseFaction)]
        private static bool PrerequisiteCheckBaseFaction(IPlayer player, PrerequisiteComparison comparison, uint value, uint objectId)
        {
            switch (comparison)
            {
                case PrerequisiteComparison.Equal:
                    return player.Faction1 == (Faction)value;
                case PrerequisiteComparison.NotEqual:
                    return player.Faction1 != (Faction)value;
                default:
                    return false;
            }
        }

        [PrerequisiteCheck(PrerequisiteType.HoverboardFlair)]
        private static bool PrerequestCheckHoverboardFlair(IPlayer player, PrerequisiteComparison comparison, uint value, uint objectId)
        {
            switch (comparison)
            {
                case PrerequisiteComparison.Equal:
                    return player.PetCustomisationManager.GetCustomisation(PetType.HoverBoard, objectId) != null;
                case PrerequisiteComparison.NotEqual:
                    return player.PetCustomisationManager.GetCustomisation(PetType.HoverBoard, objectId) == null;
                default:
                    log.Warn($"Unhandled PrerequisiteComparison {comparison} for {PrerequisiteType.HoverboardFlair}!");
                    return false;
            }
        }

        [PrerequisiteCheck(PrerequisiteType.Stealth)]
        private static bool PrerequisiteCheckStealth(IPlayer player, PrerequisiteComparison comparison, uint value, uint objectId, IUnitEntity target)
        {
            // TODO: Add value to the check. It's a spell4 Id.

            switch (comparison)
            {
                case PrerequisiteComparison.Equal:
                    //return player.Stealthed == true; // TODO: Add OR check for Spell4 Effect
                case PrerequisiteComparison.NotEqual:
                    //return player.Stealthed == false; // TODO: Add AND check for Spell4 Effect
                default:
                    return false;
            }
        }

        [PrerequisiteCheck(PrerequisiteType.TargetIsPlayer)]
        private static bool PrerequisiteCheckTargetIsPlayer(IPlayer player, PrerequisiteComparison comparison, uint value, uint objectId, IUnitEntity target)
        {
            // TODO: Currently this is a wasted effort. We only evaluate prereq's against Players. This suggests we may need to start evaluating against all entities.

            switch (comparison)
            {
                case PrerequisiteComparison.Equal:
                    return player is IPlayer;
                case PrerequisiteComparison.NotEqual:
                    return !(player is IPlayer);
                default:
                    log.Warn($"Unhandled {comparison} for {PrerequisiteType.TargetIsPlayer}!");
                    return false;
            }
        }

        [PrerequisiteCheck(PrerequisiteType.Unhealthy)]
        private static bool PrerequesiteCheckUnhealthy(IPlayer player, PrerequisiteComparison comparison, uint value, uint objectId, IUnitEntity target)
        {
            // TODO: Investigate further. Unknown what the value and objectId refers to at this time.

            // Error message is "Cannot recall while in Unhealthy Time" when trying to use Rapid Transport & other recall spells
            switch (comparison)
            {
                case PrerequisiteComparison.NotEqual:
                    return !player.InCombat;
                default:
                    log.Warn($"Unhandled {comparison} for {PrerequisiteType.Unhealthy}!");
                    return true;
            }
        }

        [PrerequisiteCheck(PrerequisiteType.Vital)]
        private static bool PrerequisiteCheckVital(IPlayer player, PrerequisiteComparison comparison, uint value, uint objectId)
        {
            switch (comparison)
            {
                case PrerequisiteComparison.Equal:
                    return player.GetVitalValue((Vital)objectId) == value;
                case PrerequisiteComparison.NotEqual:
                    return player.GetVitalValue((Vital)objectId) != value;
                case PrerequisiteComparison.GreaterThanOrEqual:
                    return player.GetVitalValue((Vital)objectId) >= value;
                case PrerequisiteComparison.GreaterThan:
                    return player.GetVitalValue((Vital)objectId) > value;
                case PrerequisiteComparison.LessThanOrEqual:
                    return player.GetVitalValue((Vital)objectId) <= value;
                case PrerequisiteComparison.LessThan:
                    return player.GetVitalValue((Vital)objectId) < value;
                default:
                    log.Warn($"Unhandled {comparison} for {PrerequisiteType.Vital}!");
                    return false;
            }
        }

        [PrerequisiteCheck(PrerequisiteType.VitalPercent)]
        private static bool PrerequisiteCheckVitalPercent(IPlayer player, PrerequisiteComparison comparison, uint value, uint objectId, IUnitEntity target)
        {
            float max = 0;
            switch (objectId)
            {
                case 1:
                    max = player.GetPropertyValue(Property.BaseHealth);
                    break;
                case 3:
                    max = player.GetPropertyValue(Property.ShieldCapacityMax);
                    break;
                case 5:
                    max = player.GetPropertyValue(Property.ResourceMax0);
                    break;
                case 6:
                    max = player.GetPropertyValue(Property.ResourceMax1);
                    break;
                case 8:
                    max = player.GetPropertyValue(Property.ResourceMax3);
                    break;
                case 15:
                    max = player.GetPropertyValue(Property.BaseFocusPool);
                    break;
                default:
                    log.Warn($"Unhandled objectId: {objectId} for {PrerequisiteType.VitalPercent}");
                    break;
            }

            float percentage = player.GetVitalValue((Vital)objectId) / max * 100;

            switch (comparison)
            {
                case PrerequisiteComparison.GreaterThanOrEqual:
                    return percentage >= value;
                case PrerequisiteComparison.GreaterThan:
                    return percentage > value;
                case PrerequisiteComparison.LessThanOrEqual:
                    return percentage <= value;
                case PrerequisiteComparison.LessThan:
                    return percentage < value;
                default:
                    log.Warn($"Unhandled {comparison} for {PrerequisiteType.Vital}!");
                    return false;
            }
        }

        [PrerequisiteCheck(PrerequisiteType.ActiveSpellCount)]
        private static bool PrerequisiteCheckSpellActiveSpellCount(IPlayer player, PrerequisiteComparison comparison, uint value, uint objectId, IUnitEntity target)
        {
            switch (comparison)
            {
                case PrerequisiteComparison.Equal:
                    return player.GetActiveSpellCount(s => s.Spell4Id == objectId && !s.IsFinished) == value;
                case PrerequisiteComparison.LessThanOrEqual:
                    return player.GetActiveSpellCount(s => s.Spell4Id == objectId && !s.IsFinished) <= value;
                default:
                    log.Warn($"Unhandled {comparison} for {PrerequisiteType.ActiveSpellCount}");
                    return false;
            }
        }

        [PrerequisiteCheck(PrerequisiteType.SpellObj)]
        private static bool PrerequisiteCheckSpellObj(IPlayer player, PrerequisiteComparison comparison, uint value, uint objectId)
        {
            // TODO: Confirm how the objectId is calculated. It seems like this check always checks for a Spell that is determined by an objectId.

            // Error message is "Spell requirement not met"

            switch (comparison)
            {
                case PrerequisiteComparison.Equal:
                    return player.SpellManager.GetSpell(value) != null;
                case PrerequisiteComparison.NotEqual:
                    return player.SpellManager.GetSpell(value) == null;
                default:
                    log.Warn($"Unhandled {comparison} for {PrerequisiteType.SpellObj}!");
                    return false;
            }
        }

        [PrerequisiteCheck(PrerequisiteType.SpellMechanic)]
        private static bool PrerequisiteCheckSpellMechanic(IPlayer player, PrerequisiteComparison comparison, uint value, uint objectId, IUnitEntity target)
        {
            uint resource = 0;
            switch (objectId)
            {
                case 4:
                    resource = (uint)player.GetVitalValue(Vital.SpellSurge);
                    break;
                default:
                    log.Warn($"Unhandled objectId: {objectId} for {PrerequisiteType.SpellMechanic}");
                    break;
            }

            switch (comparison)
            {
                case PrerequisiteComparison.GreaterThan:
                    return resource > value;
                default:
                    log.Warn($"Unhandled {comparison} for {PrerequisiteType.SpellMechanic}");
                    return false;
            }
        }

        [PrerequisiteCheck(PrerequisiteType.PositionalRequirement)]
        private static bool PrerequisiteCheckPositionalRequirement(IPlayer player, PrerequisiteComparison comparison, uint value, uint objectId, IUnitEntity target)
        {
            if (target == null || objectId == 0)
                return false;

            PositionalRequirementEntry entry = GameTableManager.Instance.PositionalRequirement.GetEntry(objectId);

            float angle = (target.Position.GetRotationTo(player.Position) - target.Rotation).X.ToDegrees();
            float minBounds = entry.AngleCenter - entry.AngleRange / 2f;
            float maxBounds = entry.AngleCenter + entry.AngleRange / 2f;
            bool isAllowed = angle >= minBounds && angle <= maxBounds;
                 
            switch (comparison)
            {
                case PrerequisiteComparison.Equal:
                    return isAllowed;
                case PrerequisiteComparison.NotEqual:
                    return !isAllowed;
                default:
                    log.Warn($"Unhandled PrerequisiteComparison {comparison} for {PrerequisiteType.PositionalRequirement}!");
                    return false;
            }
        }

        [PrerequisiteCheck(PrerequisiteType.Entitlement)]
        private static bool PrerequisiteCheckEntitlement(IPlayer player, PrerequisiteComparison comparison, uint value, uint objectId, IUnitEntity target)
        {
            EntitlementEntry entry = GameTableManager.Instance.Entitlement.GetEntry(objectId);
            if (entry == null)
                throw new ArgumentException($"Invalid entitlement type {objectId}!");

            uint currentValue = 0;

            if (((EntitlementFlags)entry.Flags).HasFlag(EntitlementFlags.Character))
                currentValue = player.Session.EntitlementManager.GetCharacterEntitlement((EntitlementType)objectId)?.Amount ?? 0u;
            else
                currentValue = player.Session.EntitlementManager.GetAccountEntitlement((EntitlementType)objectId)?.Amount ?? 0u;

            switch (comparison)
            {
                case PrerequisiteComparison.Equal:
                    return currentValue == value;
                case PrerequisiteComparison.NotEqual:
                    return currentValue != value;
                case PrerequisiteComparison.GreaterThanOrEqual:
                    return currentValue >= value;
                case PrerequisiteComparison.GreaterThan:
                    return currentValue > value;
                case PrerequisiteComparison.LessThanOrEqual:
                    return currentValue <= value;
                case PrerequisiteComparison.LessThan:
                    return currentValue < value;
                default:
                    log.Warn($"Unhandled PrerequisiteComparison {comparison} for {PrerequisiteType.PositionalRequirement}!");
                    return false;
            }
        }

        [PrerequisiteCheck(PrerequisiteType.CostumeUnlocked)]
        private static bool PrerequisiteCheckCostumeUnlocked(IPlayer player, PrerequisiteComparison comparison, uint value, uint objectId, IUnitEntity target)
        {
            switch (comparison)
            {
                case PrerequisiteComparison.Equal:
                    return player.CostumeManager.HasCostumeItemUnlocked(objectId);
                case PrerequisiteComparison.NotEqual:
                    return !player.CostumeManager.HasCostumeItemUnlocked(objectId);
                default:
                    log.Warn($"Unhandled PrerequisiteComparison {comparison} for {PrerequisiteType.PositionalRequirement}!");
                    return false;
            }
        }

        [PrerequisiteCheck(PrerequisiteType.Unknown194)]
        private static bool PrerequisiteCheckUnknown194(IPlayer player, PrerequisiteComparison comparison, uint value, uint objectId)
        {
            // TODO: Only used in Mount check prerequisites. Its use is unknown.

            return true;
        }

        [PrerequisiteCheck(PrerequisiteType.Unknown195)]
        private static bool PrerequisiteCheckUnknown195(IPlayer player, PrerequisiteComparison comparison, uint value, uint objectId)
        {
            // TODO: Only used in Mount check prerequisites. Its use is unknown.

            return true;
        }

        [PrerequisiteCheck(PrerequisiteType.Plane)]
        private static bool PrerequisiteCheckPlane(IPlayer player, PrerequisiteComparison comparison, uint value, uint objectId)
        {
            // Unknown how this works at this time, but there is a Spell Effect called "ChangePlane". Could be related.
            // TODO: Investigate further.

            // Returning true by default as many mounts used this
            return true;
        }

        [PrerequisiteCheck(PrerequisiteType.PurchasedTitle)]
        private static bool PrerequisiteCheckPurchasedTitle(IPlayer player, PrerequisiteComparison comparison, uint value, uint objectId)
        {
            switch (comparison)
            {
                case PrerequisiteComparison.Equal:
                    return player.TitleManager.HasTitle((ushort)objectId);
                case PrerequisiteComparison.NotEqual:
                    return !player.TitleManager.HasTitle((ushort)objectId);
                default:
                    log.Warn($"Unhandled PrerequisiteComparison {comparison} for {(PrerequisiteType)288}!");
                    return true;
            }
        }
    }
}
