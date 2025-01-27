﻿using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Entity.Movement.Command.Position;

namespace NexusForever.Script.Template
{
    public interface IWorldEntityScript : IGridEntityScript
    {
        /// <summary>
        /// Invoked when <see cref="IPositionCommand"/> is finalised.
        /// </summary>
        void OnPositionEntityCommandFinalise(IPositionCommand command)
        {
        }

        /// <summary>
        /// Invoked when <see cref="IWorldEntity"/> enters a zone.
        /// </summary>
        void OnEnterZone(IWorldEntity entity, uint zone)
        {
        }

        /// <summary>
        /// Invoked when this entity is interacted with.
        /// </summary>
        void OnInteract(IPlayer activator)
        {
        }

        /// <summary>
        /// Invoked when this entity receives a successful activation.
        /// </summary>
        void OnActivateSuccess(IPlayer activator)
        {
        }

        /// <summary>
        /// Invoked when this entity receives a failed activation.
        /// </summary>
        void OnActivateFail(IPlayer activator)
        {
        }
    }
}
