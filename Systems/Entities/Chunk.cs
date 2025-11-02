using System;
using System.Collections.Generic;
using Godot;

namespace Hebert.Entities
{
    /// <summary> A unit of world space that keeps track of entities within its bounds. </summary>
    public partial class Chunk : Node3D, IEquatable<Chunk>
    {
        /// <summary> The size of each cell within the chunk. This should represent a world metre. </summary>
        /// <remarks> X = width, Y = depth, Z = height. </remarks>
        [ExportGroup("Settings")]
        [Export] private Vector3 _cellSize = new Vector3(1f, 1f, 1f);


        /// <summary> The coordinates in chunk space the chunk is positioned. </summary>
        public Vector3I ChunkPosition { get; private set; } = Vector3I.Zero;


        /// <summary> A grid of all the entities within the chunk. </summary>
        private List<IEntity>[,,] _entities;


        /// <summary> Initialise the chunk within the game world. </summary>
        /// <param name="chunkPosition"> The position in chunk space to place the chunk. </param>
        /// <param name="chunkSize"> The number of cells the chunk should represent. </param>
        public void Initialise(Vector3I chunkPosition, Vector3I chunkSize)
        {
            ChunkPosition = chunkPosition;
            _entities = new List<IEntity>[chunkSize.X, chunkSize.Y, chunkSize.Z];

            // Initialise the cells.
            for (Int32 z = 0; z < chunkSize.Z; z++)
            {
                for (Int32 y = 0; y < chunkSize.Y; y++)
                {
                    for (Int32 x = 0; x < chunkSize.X; x++)
                    {
                        _entities[x, y, z] = new List<IEntity>();
                    }
                }
            }

            CallDeferred("DeferredInitialise", [chunkSize]);
        }


        /// <summary> The parts of the initialisation that need to be deferred to the main thread. Mostly Godot stuff. </summary>
        /// <param name="chunkSize"> The number of cells the chunk should represent. </param>
        private void DeferredInitialise(Vector3I chunkSize)
        {
            Name = $"Chunk [{ChunkPosition.X}, {ChunkPosition.Y}, {ChunkPosition.Z}]";
            GlobalPosition = ChunkPosition * chunkSize * _cellSize;
            GD.Print($"Chunk @ {ChunkPosition} completed deferred initialisation.");
        }


        /// <inheritdoc/>
        public override Int32 GetHashCode() => HashCode.Combine(ChunkPosition);


        /// <inheritdoc/>
        public Boolean Equals(Chunk? other) => ChunkPosition == other?.ChunkPosition;
    }
}
