using System.Linq;
using NexusForever.Game;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Spell;
using NexusForever.Game.Spell;
using NexusForever.Game.Static.Entity;
using NexusForever.Game.Static.Quest;
using NexusForever.GameTable;
using NexusForever.GameTable.Model;
using NexusForever.Network;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model;
using NexusForever.Network.World.Message.Static;
using NLog;
using System;

namespace NexusForever.WorldServer.Network.Message.Handler
{
    public static class EntityHandler
    {
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        [MessageHandler(GameMessageOpcode.ClientEntityCommand)]
        public static void HandleEntityCommand(IWorldSession session, ClientEntityCommand entityCommand)
        {
            IWorldEntity mover = session.Player;
            if (mover == null)
                return;

            if (session.Player.ControlGuid != null)
                mover = session.Player.GetVisible<IWorldEntity>(session.Player.ControlGuid.Value);

            if (session.Player.IsEmoting)
                session.Player.IsEmoting = false;

            mover?.MovementManager.HandleClientEntityCommands(entityCommand.Commands, entityCommand.Time);
        }

        /// <remarks>
        /// Assuming this is a "Legacy" activate packet. It seems to only be used by Datacubes and some Path interact events.
        /// May be worth considering this a "PathActivate" packet, as Datacubes, while usable by any, had special events related to Scientists.
        /// </remarks>
        [MessageHandler(GameMessageOpcode.ClientActivateUnit)]
        public static void HandleActivateUnit(IWorldSession session, ClientActivateUnit unit)
        {
            IWorldEntity entity = session.Player.GetVisible<IWorldEntity>(unit.UnitId);
            if (entity == null)
                throw new InvalidPacketValueException();

            // TODO: sanity check for range etc.

            entity.OnActivate(session.Player);
        }

        [MessageHandler(GameMessageOpcode.ClientEntityInteract)]
        public static void HandleClientEntityInteraction(IWorldSession session, ClientEntityInteract entityInteraction)
        {
            IWorldEntity entity = session.Player.GetVisible<IWorldEntity>(entityInteraction.Guid);
            if (entity != null)
            {
                session.Player.QuestManager.ObjectiveUpdate(QuestObjectiveType.ActivateEntity, entity.CreatureId, 1u);
                session.Player.QuestManager.ObjectiveUpdate(QuestObjectiveType.TalkTo, entity.CreatureId, 1u);
                foreach (uint targetGroupId in AssetManager.Instance.GetTargetGroupsForCreatureId(entity.CreatureId) ?? Enumerable.Empty<uint>())
                    session.Player.QuestManager.ObjectiveUpdate(QuestObjectiveType.TalkToTargetGroup, targetGroupId, 1u);
            }

            switch (entityInteraction.Event)
            {
                case 37: // Quest NPC
                {
                    session.EnqueueMessageEncrypted(new Server0357
                    {
                        UnitId = entityInteraction.Guid
                    });
                    break;
                }
                case 49: // Handle Vendor
                    VendorHandler.HandleClientVendor(session, entityInteraction);
                    break;
                case 68: // "MailboxActivate"
                    var mailboxEntity = session.Player.Map.GetEntity<IMailboxEntity>(entityInteraction.Guid);
                    break;
                case 8: // "HousingGuildNeighborhoodBrokerOpen"
                case 40:
                case 41: // "ResourceConversionOpen"
                case 42: // "ToggleAbilitiesWindow"
                case 43: // "InvokeTradeskillTrainerWindow"
                case 45: // "InvokeShuttlePrompt"
                case 46:
                case 47:
                case 48: // "InvokeTaxiWindow"
                case 65: // "MannequinWindowOpen"
                case 66: // "ShowBank"
                case 67: // "ShowRealmBank"
                case 69: // "ShowDye"
                case 70: // "GuildRegistrarOpen"
                case 71: // "WarPartyRegistrarOpen"
                case 72: // "GuildBankerOpen"
                case 73: // "WarPartyBankerOpen"
                case 75: // "ToggleMarketplaceWindow"
                case 76: // "ToggleAuctionWindow"
                case 79: // "TradeskillEngravingStationOpen"
                case 80: // "HousingMannequinOpen"
                case 81: // "CityDirectionsList"
                case 82: // "ToggleCREDDExchangeWindow"
                case 84: // "CommunityRegistrarOpen"
                case 85: // "ContractBoardOpen"
                case 86: // "BarberOpen"
                case 87: // "MasterCraftsmanOpen"
                default:
                    log.Warn($"Received unhandled interaction event {entityInteraction.Event} from Entity {entityInteraction.Guid}");
                    break;
            }
        }

        [MessageHandler(GameMessageOpcode.ClientEntityInteractChair)]
        public static void HandleClientEntityInteractEmote(IWorldSession session, ClientEntityInteractChair interactChair)
        {
            IWorldEntity chair = session.Player.GetVisible<IWorldEntity>(interactChair.ChairUnitId);
            if (chair == null)
                throw new InvalidPacketValueException();

            Creature2Entry creatureEntry = GameTableManager.Instance.Creature2.GetEntry(chair.CreatureId);
            if ((creatureEntry.ActivationFlags & 0x200000) == 0)
                throw new InvalidPacketValueException();

            session.Player.Sit(chair);
        }

        [MessageHandler(GameMessageOpcode.ClientResurrectAccept)]
        public static void HandleClientResurrectAccept(IWorldSession session, ClientResurrectAccept clientResurrectAccept)
        {
            if (clientResurrectAccept.RezType == ResurrectionType.None)
                return;

            session.Player.ResurrectionManager.Resurrect(clientResurrectAccept.RezType);
        }

        [MessageHandler(GameMessageOpcode.ClientResurrectRequest)]
        public static void HandleClientResurrectRequest(IWorldSession session, ClientResurrectRequest _)
        {
            if (session.Player.TargetGuid == null)
                return;

            session.Player.ResurrectionManager.Resurrect(session.Player.TargetGuid.Value);
        }

        [MessageHandler(GameMessageOpcode.ClientActivateUnitCast)]
        public static void HandleActivateUnitCast(WorldSession session, ClientActivateUnitCast request)
        {
            IWorldEntity entity = session.Player.GetVisible<IWorldEntity>(request.ActivateUnitId);
            if (entity == null)
                throw new InvalidPacketValueException();

            entity.OnActivateCast(session.Player, request.ClientUniqueId);
        }

        /// <remarks>
        /// Possibly only used by Bindpoint entities
        /// </remarks>
        [MessageHandler(GameMessageOpcode.ClientActivateUnitInteraction)]
        public static void HandleActivateUnitDeferred(WorldSession session, ClientActivateUnitInteraction request)
        {
            IWorldEntity entity = session.Player.GetVisible<IWorldEntity>(request.ActivateUnitId);
            if (entity == null)
                throw new InvalidPacketValueException();

            entity.OnActivateCast(session.Player, request.ClientUniqueId);
        }

        [MessageHandler(GameMessageOpcode.ClientInteractionResult)]
        public static void HandleSpellDeferredResult(WorldSession session, ClientSpellInteractionResult result)
        {
            if (!(session.Player.HasSpell(x => x.CastingId == result.CastingId, out ISpell spell)))
                throw new ArgumentNullException($"Spell cast {result.CastingId} not found.");

            if (spell is not SpellClientSideInteraction spellCSI)
                throw new ArgumentNullException($"Spell missing a ClientSideInteraction.");

            switch (result.Result)
            {
                case 0:
                    spellCSI.FailClientInteraction();
                    break;
                case 1:
                    spellCSI.SucceedClientInteraction();
                    break;
                case 2:
                    spellCSI.CancelCast(CastResult.ClientSideInteractionFail);
                    break;
            }
        }
    }
}
