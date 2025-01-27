using System;
using System.Collections.Generic;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Social;
using NexusForever.Game.Static.Entity;
using NexusForever.Game.Static.Social;
using NexusForever.GameTable;
using NexusForever.GameTable.Model;
using NexusForever.Network;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model;
using NexusForever.WorldServer.Command;
using NexusForever.WorldServer.Command.Context;
using NLog;

namespace NexusForever.WorldServer.Network.Message.Handler
{
    public static class SocialHandler
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        private const string CommandPrefix = "!";

        [MessageHandler(GameMessageOpcode.ClientChat)]
        public static void HandleChat(IWorldSession session, ClientChat chat)
        {
            if (chat.Message.StartsWith(CommandPrefix))
            {
                try
                {
                    IWorldEntity target = null;
                    if (session.Player.TargetGuid != null)
                        target = session.Player.GetVisible<IWorldEntity>(session.Player.TargetGuid.Value);

                    var context = new WorldSessionCommandContext(session, target);
                    CommandManager.Instance.HandleCommand(context, chat.Message.Substring(CommandPrefix.Length));
                }
                catch (Exception e)
                {
                    log.Warn($"{e.Message}: {e.StackTrace}");
                }
            }
            else
                GlobalChatManager.Instance.HandleClientChat(session.Player, chat);
        }

        [MessageHandler(GameMessageOpcode.ClientEmote)]
        public static void HandleEmote(IWorldSession session, ClientEmote emote)
        {
            StandState standState = StandState.Stand;
            EmotesEntry entry = null;
            if (emote.EmoteId != 0)
            {
                entry = GameTableManager.Instance.Emotes.GetEntry(emote.EmoteId);
                if (entry == null)
                    throw (new InvalidPacketValueException("HandleEmote: Invalid EmoteId"));

                standState = (StandState)entry.StandState;
            }

            if (emote.EmoteId == 0 && session.Player.IsSitting)
                session.Player.Unsit();

            if (emote.EmoteId == 0)
                return;

            // TODO: Only set this when the Player has an "unlimited duration" emote active - like /sit, /sleep, /dance.
            session.Player.IsEmoting = true;
            session.Player.EnqueueToVisible(new ServerEntityEmote
            {
                EmotesId = (ushort)emote.EmoteId,
                Seed = emote.Seed,
                SourceUnitId = session.Player.Guid,
                TargetUnitId = emote.TargetUnitId,
                Targeted = emote.Targeted,
                Silent = emote.Silent
            });

            if (entry.NoArgAnim != 0)
                session.Player.EnqueueToVisible(new ServerEmote
                {
                    Guid = session.Player.Guid,
                    StandState = standState,
                    EmoteId = emote.EmoteId
                });
        }

        [MessageHandler(GameMessageOpcode.ClientWhoRequest)]
        public static void HandleWhoRequest(IWorldSession session, ClientWhoRequest request)
        {
            var players = new List<ServerWhoResponse.WhoPlayer>
            {
                new()
                {
                    Name    = session.Player.Name,
                    Level   = session.Player.Level,
                    Race    = session.Player.Race,
                    Class   = session.Player.Class,
                    Path    = session.Player.Path,
                    Faction = session.Player.Faction1,
                    Sex     = session.Player.Sex,
                    Zone    = session.Player.Zone.Id
                }
            };

            session.EnqueueMessageEncrypted(new ServerWhoResponse
            {
                Players = players
            });
        }

        [MessageHandler(GameMessageOpcode.ClientChatWhisper)]
        public static void HandleWhisper(IWorldSession session, ClientChatWhisper whisper)
        {
            GlobalChatManager.Instance.HandleWhisperChat(session.Player, whisper);
        }

        [MessageHandler(GameMessageOpcode.ClientChatJoin)]
        public static void HandleChatJoin(IWorldSession session, ClientChatJoin chatJoin)
        {
            ChatResult result = session.Player.ChatManager.CanJoin(chatJoin.Name, chatJoin.Password);
            if (result != ChatResult.Ok)
            {
                session.EnqueueMessageEncrypted(new ServerChatJoinResult
                {
                    Type   = chatJoin.Type,
                    Name   = chatJoin.Name,
                    Result = result
                });
                return;
            }

            session.Player.ChatManager.Join(chatJoin.Name, chatJoin.Password);
        }

        [MessageHandler(GameMessageOpcode.ClientChatLeave)]
        public static void HandleChatLeave(IWorldSession session, ClientChatLeave chatLeave)
        {
            ChatResult result = session.Player.ChatManager.CanLeave(chatLeave.Channel.ChatId);
            if (result != ChatResult.Ok)
            {
                GlobalChatManager.Instance.SendChatResult(session, chatLeave.Channel.Type, chatLeave.Channel.ChatId, result);
                return;
            }

            session.Player.ChatManager.Leave(chatLeave.Channel.ChatId, ChatChannelLeaveReason.Leave);
        }

        [MessageHandler(GameMessageOpcode.ClientChatKick)]
        public static void HandleChatKick(IWorldSession session, ClientChatKick chatKick)
        {
            ChatResult result = session.Player.ChatManager.CanKick(chatKick.Channel.ChatId, chatKick.CharacterName);
            if (result != ChatResult.Ok)
            {
                GlobalChatManager.Instance.SendChatResult(session, chatKick.Channel.Type, chatKick.Channel.ChatId, result);
                return;
            }

            session.Player.ChatManager.Kick(chatKick.Channel.ChatId, chatKick.CharacterName);
        }

        [MessageHandler(GameMessageOpcode.ClientChatList)]
        public static void HandleChatList(IWorldSession session, ClientChatList chatList)
        {
            ChatResult result = session.Player.ChatManager.CanListMembers(chatList.Channel.ChatId);
            if (result != ChatResult.Ok)
            {
                GlobalChatManager.Instance.SendChatResult(session, chatList.Channel.Type, chatList.Channel.ChatId, result);
                return;
            }

            session.Player.ChatManager.ListMembers(chatList.Channel.ChatId);
        }

        [MessageHandler(GameMessageOpcode.ClientChatPassword)]
        public static void HandleChatPassword(IWorldSession session, ClientChatPassword chatPassword)
        {
            ChatResult result = session.Player.ChatManager.CanSetPassword(chatPassword.Channel.ChatId, chatPassword.Password);
            if (result != ChatResult.Ok)
            {
                GlobalChatManager.Instance.SendChatResult(session, chatPassword.Channel.Type, chatPassword.Channel.ChatId, result);
                return;
            }

            session.Player.ChatManager.SetPassword(chatPassword.Channel.ChatId, chatPassword.Password);
        }

        [MessageHandler(GameMessageOpcode.ClientChatOwner)]
        public static void HandleChatOwner(IWorldSession session, ClientChatOwner chatOwner)
        {
            ChatResult result = session.Player.ChatManager.CanPassOwner(chatOwner.Channel.ChatId, chatOwner.CharacterName);
            if (result != ChatResult.Ok)
            {
                GlobalChatManager.Instance.SendChatResult(session, chatOwner.Channel.Type, chatOwner.Channel.ChatId, result);
                return;
            }

            session.Player.ChatManager.PassOwner(chatOwner.Channel.ChatId, chatOwner.CharacterName);
        }

        [MessageHandler(GameMessageOpcode.ClientChatModerator)]
        public static void HandleChatModerator(IWorldSession session, ClientChatModerator chatModerator)
        {
            ChatResult result = session.Player.ChatManager.CanMakeModerator(chatModerator.Channel.ChatId, chatModerator.CharacterName);
            if (result != ChatResult.Ok)
            {
                GlobalChatManager.Instance.SendChatResult(session, chatModerator.Channel.Type, chatModerator.Channel.ChatId, result);
                return;
            }

            session.Player.ChatManager.MakeModerator(chatModerator.Channel.ChatId, chatModerator.CharacterName, chatModerator.Status);
        }

        [MessageHandler(GameMessageOpcode.ClientChatMute)]
        public static void HandleChatMute(IWorldSession session, ClientChatMute chatMute)
        {
            ChatResult result = session.Player.ChatManager.CanMute(chatMute.Channel.ChatId, chatMute.CharacterName);
            if (result != ChatResult.Ok)
            {
                GlobalChatManager.Instance.SendChatResult(session, chatMute.Channel.Type, chatMute.Channel.ChatId, result);
                return;
            }

            session.Player.ChatManager.Mute(chatMute.Channel.ChatId, chatMute.CharacterName, chatMute.Status);
        }
    }
}
