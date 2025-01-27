using System.Numerics;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Map.Search;
using NexusForever.Game.Static.Entity;
using NexusForever.GameTable.Model;
using NexusForever.IO.Map;
using NexusForever.Network.Message;

namespace NexusForever.Game.Abstract.Map
{
    public interface IBaseMap : IMap
    {
        /// <summary>
        /// Distance between a <see cref="IPlayer"/> and a <see cref="IGridEntity"/> before the entity can be seen.
        /// </summary>
        float? VisionRange { get; }

        WorldEntry Entry { get; }
        MapFile File { get; }

        /// <summary>
        /// Enqueue <see cref="IGridEntity"/> to be removed from <see cref="IBaseMap"/>.
        /// </summary>
        void EnqueueRemove(IGridEntity entity);

        /// <summary>
        /// Enqueue <see cref="IGridEntity"/> to be relocated in <see cref="IBaseMap"/> to <see cref="Vector3"/>.
        /// </summary>
        void EnqueueRelocate(IGridEntity entity, Vector3 position);

        /// <summary>
        /// Return all <see cref="IGridEntity"/>'s from <see cref="Vector3"/> in range that satisfy <see cref="ISearchCheck{T}"/>.
        /// </summary>
        IEnumerable<T> Search<T>(Vector3 vector, float? radius, ISearchCheck<T> check) where T : IGridEntity;

        /// <summary>
        /// Return all <see cref="IMapGrid"/>'s from <see cref="Vector3"/> in range.
        /// </summary>
        void GridSearch(Vector3 vector, float? radius, out List<IMapGrid> intersectedGrids);

        /// <summary>
        /// Return <see cref="IGridEntity"/> by guid.
        /// </summary>
        T GetEntity<T>(uint guid) where T : IGridEntity;

        /// <summary>
        /// Enqueue broadcast of <see cref="IWritable"/> to all <see cref="IPlayer"/>'s on the map.
        /// </summary>
        void EnqueueToAll(IWritable message);

        /// <summary>
        /// Notify <see cref="IMapGrid"/> at coordinates of the addition of new <see cref="IPlayer"/> that is in vision range.
        /// </summary>
        void GridAddVisiblePlayer(uint gridX, uint gridZ);

        /// <summary>
        /// Notify <see cref="IMapGrid"/> at coordinates of the removal of an existing <see cref="IPlayer"/> that is no longer in vision range.
        /// </summary>
        void GridRemoveVisiblePlayer(uint gridX, uint gridZ);

        /// <summary>
        /// Return terrain height at supplied position.
        /// </summary>
        float? GetTerrainHeight(float x, float z);

        /// <summary>
        /// Return <see cref="ResurrectionType"/> applicable to this map.
        /// </summary>
        ResurrectionType GetResurrectionType();
    }
}