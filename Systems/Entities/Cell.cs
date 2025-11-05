using System;
using System.Collections.Generic;
using Godot;
using Hebert.Types.AStar;

namespace Hebert.Entities
{
    /// <summary> A single position within a grid. Holds reference to the entities within the position. </summary>
    public class Cell : IGraphable
    {
        /// <summary> The chunk the cell is a part of. </summary>
        public readonly Chunk Chunk;

        /// <summary> The local position of the cell within the chunk's grid. </summary>
        public readonly Vector3I ChunkPosition;

        /// <summary> The entities currently occupying the cell. </summary>
        public readonly List<IEntity> Entities = new List<IEntity>();


        /// <summary> A single position within a grid. Holds reference to the entities within the position. </summary>
        /// <param name="chunk"> The local position of the cell within the chunk's grid. </param>
        /// <param name="chunkPosition"> The entities currently occupying the cell. </param>
        public Cell(Chunk chunk, Vector3I chunkPosition)
        {
            Chunk = chunk;
            ChunkPosition = chunkPosition;
        }


        /// <inheritdoc/>
        public Single CalculateHeuristic(IGraphable other)
        {
            return 1;
        }


        /// <inheritdoc/>
        public IGraphable[] GetNeighbours() => Chunk.GetNeighbours(ChunkPosition);
    }
}
