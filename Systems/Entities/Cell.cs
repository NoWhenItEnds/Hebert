using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Hebert.Entities
{
    /// <summary> A single position within a grid. Holds reference to the entities within the position. </summary>
    public class Cell
    {
        /// <summary> The chunk the cell is a part of. </summary>
        public readonly Chunk Chunk;

        /// <summary> The local position of the cell within the chunk's grid. </summary>
        public readonly Vector3I ChunkPosition;

        /// <summary> The entities currently occupying the cell. </summary>
        public readonly List<IEntity> Entities = new List<IEntity>();


        /// <summary> Whether the cell contains an entity that blocks movement, stopping any movement through the cell. </summary>
        public Boolean BlocksMovement => Entities.Any(x => x.BlocksMovement);

        /// <summary> Whether the cell contains an entity that blocks sight, stopping any sight through the cell. </summary>
        public Boolean BlocksSight => Entities.Any(x => x.BlocksSight);


        /// <summary> A single position within a grid. Holds reference to the entities within the position. </summary>
        /// <param name="chunk"> The local position of the cell within the chunk's grid. </param>
        /// <param name="chunkPosition"> The entities currently occupying the cell. </param>
        public Cell(Chunk chunk, Vector3I chunkPosition)
        {
            Chunk = chunk;
            ChunkPosition = chunkPosition;
        }
    }
}
