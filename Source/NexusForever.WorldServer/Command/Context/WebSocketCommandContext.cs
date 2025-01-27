﻿using System;
using System.Collections.Immutable;
using System.IO;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.RBAC;
using NexusForever.Game.RBAC;
using NexusForever.Game.Static;
using NexusForever.Game.Static.RBAC;
using NLog;

namespace NexusForever.WorldServer.Command.Context
{
    public class WebSocketCommandContext : ICommandContext
    {
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        public IWorldEntity Invoker { get; }
        public IWorldEntity Target { get; }

        public Language Language { get; } = Language.English;
        public ImmutableHashSet<Permission> Permissions { get; }

        private readonly WebSocket webSocket;

        /// <summary>
        /// Create a new <see cref="WebSocketCommandContext"/> with the <see cref="Permission"/>'s from the WebSocket <see cref="Role"/>.
        /// </summary>
        public WebSocketCommandContext(WebSocket webSocket)
        {
            this.webSocket = webSocket;

            // console role needs to exist in order for the websocket command context to work
            IRBACRole role = RBACManager.Instance.GetRole(Role.WebSocket);
            if (role == null)
                throw new InvalidDataException("WebSocket role doesn't exist!");

            Permissions = role.Permissions.Keys.ToImmutableHashSet();
        }

        /// <summary>
        /// Send information message containing the supplied string.
        /// </summary>
        public void SendMessage(string message)
        {
            SendWebSocketMessage(message, "info");
        }

        /// <summary>
        /// Send error message containing the supplied string.
        /// </summary>
        public void SendError(string message)
        {
            SendWebSocketMessage(message, "error");
        }

        /// <summary>
        /// Return <see cref="IWorldEntity"/> target, if no target is present return the <see cref="IWorldEntity"/> invoker.
        /// </summary>
        public T GetTargetOrInvoker<T>() where T : IWorldEntity
        {
            return default;
        }

        private async void SendWebSocketMessage(string text, string type)
        {
            if (webSocket.State != WebSocketState.Open)
                return;

            try
            {
                string message = JObject.FromObject(new { text, type }).ToString(Formatting.None);
                await webSocket.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes(message)), WebSocketMessageType.Text, true, CancellationToken.None)
                    .ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                log.Error(exception, $"Failed to send {type} message to websocket client, message was: {text}");
            }
        }
    }
}
