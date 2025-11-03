using System;
using System.Collections.Generic;
using Godot;

namespace Hebert.Entities
{
    /// <summary> A unit of world space that keeps track of entities within its bounds. </summary>
    public class Chunk : IEquatable<Chunk>
    {
        /// <summary> The coordinates in chunk space the chunk is positioned. </summary>
        public Vector3I ChunkPosition { get; private set; } = Vector3I.Zero;

        /// <summary> How many cells the chunk contains. </summary>
        public Vector3I ChunkSize { get; private set; } = Vector3I.Zero;


        /// <summary> A grid of all the entities within the chunk. </summary>
        private readonly List<IEntity>[,,] _entities;


        /// <summary> A unit of world space that keeps track of entities within its bounds. </summary>
        /// <param name="chunkPosition"> The position in chunk space to place the chunk. </param>
        /// <param name="chunkSize"> The number of cells the chunk should represent. </param>
        public Chunk(Vector3I chunkPosition, Vector3I chunkSize)
        {
            ChunkPosition = chunkPosition;
            ChunkSize = chunkSize;
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

            GD.Print($"Chunk @ {ChunkPosition} was initialised.");    // TODO - Use logger.
        }


        /// <summary> Get the position of a global position relative to the chunk, or in chunk coordinates. </summary>
        /// <param name="globalPosition"> The global position to convert. </param>
        /// <returns> The converted position in chunk coordinates. </returns>
        /// <exception cref="ArgumentOutOfRangeException"> If the position exceeds the bounds of the chunk. </exception>
        public Vector3I GetLocalPosition(Vector3I globalPosition)
        {
            Vector3I localPosition = globalPosition - (ChunkPosition * ChunkSize);
            if (localPosition < Vector3I.Zero || localPosition >= ChunkSize)
            {
                throw new ArgumentOutOfRangeException($"The given global position, {globalPosition}, when converted to local space, {localPosition}, is beyond the chunk bounds of {Vector3I.Zero} - {ChunkSize}.");
            }

            return localPosition;
        }


        /// <summary> Gets an array of the entities current occupying the given chunk position. </summary>
        /// <param name="localPosition"> The chunk position to get the entities from. </param>
        /// <returns> An array of the entities currently occupying the given position.</returns>
        /// <exception cref="ArgumentOutOfRangeException"> If the position exceeds the bounds of the chunk. </exception>
        public IEntity[] GetEntities(Vector3I localPosition)
        {
            if (localPosition < Vector3I.Zero || localPosition >= ChunkSize)
            {
                throw new ArgumentOutOfRangeException($"The given position, {localPosition}, is beyond the chunk bounds of {Vector3I.Zero} - {ChunkSize}.");
            }

            return _entities[localPosition.X, localPosition.Y, localPosition.Z].ToArray();
        }


        /// <inheritdoc/>
        public override Int32 GetHashCode() => HashCode.Combine(ChunkPosition);


        /// <inheritdoc/>
        public Boolean Equals(Chunk? other) => ChunkPosition == other?.ChunkPosition;
    }
}
