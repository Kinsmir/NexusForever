using System.Numerics;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using NexusForever.Database.Character;
using NexusForever.Database.Character.Model;
using NexusForever.Game.Abstract.Housing;
using NexusForever.Game.Static.Housing;
using NexusForever.GameTable.Model;
using NexusForever.Network.World.Message.Model;

namespace NexusForever.Game.Housing
{
    public class Decor : IDecor
    {
        /// <summary>
        /// Determines which fields need saving for <see cref="IDecor"/> when being saved to the database.
        /// </summary>
        [Flags]
        public enum DecorSaveMask
        {
            None               = 0x0000,
            Create             = 0x0001,
            Delete             = 0x0002,
            Type               = 0x0004,
            Position           = 0x0008,
            Rotation           = 0x0010,
            Scale              = 0x0020,
            DecorParentId      = 0x0040,
            ColourShiftId      = 0x0080,
            PlotIndex          = 0x0100
        }

        public ulong Id => Residence.Id;
        public ulong DecorId { get; }
        public HousingDecorInfoEntry Entry { get; }

        public DecorType Type
        {
            get => type;
            set
            {
                type = value;
                saveMask |= DecorSaveMask.Type;
            }
        }

        private DecorType type;

        public uint PlotIndex
        {
            get => plotIndex;
            set
            {
                plotIndex = value;
                saveMask |= DecorSaveMask.PlotIndex;
            }
        }

        private uint plotIndex;

        public Vector3 Position
        {
            get => position;
            set
            {
                position = value;
                saveMask |= DecorSaveMask.Position;
            }
        }

        private Vector3 position;

        public Quaternion Rotation
        {
            get => rotation;
            set
            {
                rotation = value;
                saveMask |= DecorSaveMask.Rotation;
            }
        }

        private Quaternion rotation;

        public float Scale
        {
            get => scale;
            set
            {
                scale = value;
                saveMask |= DecorSaveMask.Scale;
            }
        }

        private float scale;

        public ulong DecorParentId
        {
            get => decorParentId;
            set
            {
                decorParentId = value;
                saveMask |= DecorSaveMask.DecorParentId;
            }
        }

        private ulong decorParentId;

        public ushort ColourShiftId
        {
            get => colourShiftId;
            set
            {
                colourShiftId = value;
                saveMask |= DecorSaveMask.ColourShiftId;
            }
        }

        private ushort colourShiftId;

        public IResidence Residence { get; }

        private DecorSaveMask saveMask;

        /// <summary>
        /// Returns if <see cref="IDecor"/> is enqueued to be saved to the database.
        /// </summary>
        public bool PendingCreate => (saveMask & DecorSaveMask.Create) != 0;

        /// <summary>
        /// Returns if <see cref="IDecor"/> is enqueued to be deleted from the database.
        /// </summary>
        public bool PendingDelete => (saveMask & DecorSaveMask.Delete) != 0;

        /// <summary>
        /// Enqueue <see cref="IDecor"/> to be deleted from the database.
        /// </summary>
        public void EnqueueDelete(bool set)
        {
            saveMask = DecorSaveMask.Delete;
        }

        /// <summary>
        /// Create a new <see cref="IDecor"/> from an existing database model.
        /// </summary>
        public Decor(IResidence residence, ResidenceDecor model, HousingDecorInfoEntry entry)
        {
            DecorId       = model.DecorId;
            Entry         = entry;
            type          = (DecorType)model.DecorType;
            plotIndex     = model.PlotIndex;
            position      = new Vector3(model.X, model.Y, model.Z);
            rotation      = new Quaternion(model.Qx, model.Qy, model.Qz, model.Qw);
            scale         = model.Scale;
            decorParentId = model.DecorParentId;
            colourShiftId = model.ColourShiftId;
            Residence     = residence;

            saveMask = DecorSaveMask.None;
        }

        /// <summary>
        /// Create a new <see cref="IDecor"/> from a <see cref="HousingDecorInfoEntry"/> template.
        /// </summary>
        public Decor(IResidence residence, ulong decorId, HousingDecorInfoEntry entry)
        {
            DecorId   = decorId;
            Entry     = entry;
            type      = DecorType.Crate;
            position  = Vector3.Zero;
            rotation  = Quaternion.Identity;
            Residence = residence;

            saveMask = DecorSaveMask.Create;
        }

        /// <summary>
        /// Create a new <see cref="IDecor"/> from an existing <see cref="IDecor"/>.
        /// </summary>
        /// <remarks>
        /// Copies all data from the source <see cref="IDecor"/> with a new id.
        /// </remarks>
        public Decor(IResidence residence, IDecor decor, ulong decorId)
        {
            DecorId       = decorId;
            Entry         = decor.Entry;
            type          = decor.Type;
            plotIndex     = decor.PlotIndex;
            position      = decor.Position;
            rotation      = decor.Rotation;
            scale         = decor.Scale;
            decorParentId = decor.DecorParentId;
            colourShiftId = decor.ColourShiftId;
            Residence     = residence;

            saveMask = DecorSaveMask.Create;
        }

        public void Save(CharacterContext context)
        {
            if (saveMask == DecorSaveMask.None)
                return;

            if ((saveMask & DecorSaveMask.Create) != 0)
            {
                // decor doesn't exist in database, all infomation must be saved
                context.Add(new ResidenceDecor
                {
                    Id            = Id,
                    DecorId       = DecorId,
                    DecorInfoId   = Entry.Id,
                    DecorType     = (uint)Type,
                    PlotIndex     = PlotIndex,
                    X             = Position.X,
                    Y             = Position.Y,
                    Z             = Position.Z,
                    Qx            = Rotation.X,
                    Qy            = Rotation.Y,
                    Qz            = Rotation.Z,
                    Qw            = Rotation.W,
                    Scale         = Scale,
                    DecorParentId = DecorParentId,
                    ColourShiftId = ColourShiftId
                });
            }
            else if ((saveMask & DecorSaveMask.Delete) != 0)
            {
                var model = new ResidenceDecor
                {
                    Id      = Id,
                    DecorId = DecorId
                };

                context.Entry(model).State = EntityState.Deleted;
            }
            else
            {
                // decor already exists in database, save only data that has been modified
                var model = new ResidenceDecor
                {
                    Id      = Id,
                    DecorId = DecorId
                };

                // could probably clean this up with reflection, works for the time being
                EntityEntry<ResidenceDecor> entity = context.Attach(model);
                if ((saveMask & DecorSaveMask.Type) != 0)
                {
                    model.DecorType = (uint)Type;
                    entity.Property(p => p.DecorType).IsModified = true;
                }
                if ((saveMask & DecorSaveMask.PlotIndex) != 0)
                {
                    model.PlotIndex = PlotIndex;
                    entity.Property(p => p.PlotIndex).IsModified = true;
                }
                if ((saveMask & DecorSaveMask.Position) != 0)
                {
                    model.X = Position.X;
                    entity.Property(p => p.X).IsModified = true;
                    model.Y = Position.Y;
                    entity.Property(p => p.Y).IsModified = true;
                    model.Z = Position.Z;
                    entity.Property(p => p.Z).IsModified = true;
                }
                if ((saveMask & DecorSaveMask.Rotation) != 0)
                {
                    model.Qx = Rotation.X;
                    entity.Property(p => p.Qx).IsModified = true;
                    model.Qy = Rotation.Y;
                    entity.Property(p => p.Qy).IsModified = true;
                    model.Qz = Rotation.Z;
                    entity.Property(p => p.Qz).IsModified = true;
                    model.Qw = Rotation.W;
                    entity.Property(p => p.Qw).IsModified = true;
                }
                if ((saveMask & DecorSaveMask.Scale) != 0)
                {
                    model.Scale = Scale;
                    entity.Property(p => p.Scale).IsModified = true;
                }
                if ((saveMask & DecorSaveMask.DecorParentId) != 0)
                {
                    model.DecorParentId = DecorParentId;
                    entity.Property(p => p.DecorParentId).IsModified = true;
                }
                if ((saveMask & DecorSaveMask.ColourShiftId) != 0)
                {
                    model.ColourShiftId = ColourShiftId;
                    entity.Property(p => p.ColourShiftId).IsModified = true;
                }
            }

            saveMask = DecorSaveMask.None;
        }

        /// <summary>
        /// Move <see cref="IDecor"/> to supplied position.
        /// </summary>
        public void Move(DecorType type, Vector3 position, Quaternion rotation, float scale, uint plotIndex)
        {
            Type      = type;
            Position  = position;
            Rotation  = rotation;
            Scale     = scale;
            PlotIndex = plotIndex;
        }

        /// <summary>
        /// Move <see cref="IDecor"/> to the crate.
        /// </summary>
        public void Crate()
        {
            Move(DecorType.Crate, Vector3.Zero, Quaternion.Identity, 0f, int.MaxValue);
            DecorParentId = 0u;
        }

        public ServerHousingResidenceDecor.Decor Build()
        {
            return new()
            {
                RealmId       = RealmContext.Instance.RealmId,
                DecorId       = DecorId,
                ResidenceId   = Residence.Id,
                DecorType     = Type,
                PlotIndex     = PlotIndex,
                Scale         = Scale,
                Position      = Position,
                Rotation      = Rotation,
                DecorInfoId   = Entry.Id,
                ParentDecorId = DecorParentId,
                ColourShift   = ColourShiftId
            };
        }
    }
}
