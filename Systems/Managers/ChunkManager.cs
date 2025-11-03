using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using Hebert.Entities;
using Hebert.Types;
using Hebert.Types.Exceptions;
using Hebert.Types.Singletons;

namespace Hebert.Managers
{
    /// <summary> Orchestrates many chunks, ensuring the correct ones are loaded and processing. </summary>
    public partial class ChunkManager : SingletonNode<ChunkManager>
    {
        /// <summary> The number of cells within a single chunk. </summary>
        /// <remarks> X = width, Y = depth, Z = height. </remarks>
        [ExportGroup("Settings")]
        [Export] private Vector3I _chunkSize = new Vector3I(256, 256, 32);

        /// <summary> The size of each cell within the chunk. This should represent a world metre. </summary>
        [Export] public Vector3 CellSize { get; private set; } = new Vector3(1f, 1f, 1f);

        /// <summary> The number of chunks in the world. </summary>
        // TODO - Make this generated from a world schematic.
        [Export] private Vector3I _worldSize = new Vector3I(10, 10, 2);


        /// <summary> A reference to the local manager of tasks. </summary>
        private TaskManager _taskManager = new TaskManager();

        /// <summary> A reference to all the chunks within the game world. </summary>
        private HashSet<Chunk> _chunks = new HashSet<Chunk>();


        /// <inheritdoc/>
        public override void _Ready()
        {
            // TODO - Make this generated from a world schematic.
            for (Int32 z = 0; z < _worldSize.Z; z++)
            {
                for (Int32 y = 0; y < _worldSize.Y; y++)
                {
                    for (Int32 x = 0; x < _worldSize.X; x++)
                    {
                        Vector3I position = new Vector3I(x, y, z);
                        _taskManager.AddTask(Task.Run(() => CreateChunk(position)));
                    }
                }
            }
        }


        /// <summary> Attempt to create a new chunk at the given chunk coordinates. </summary>
        /// <param name="position"> The chunk coordinates to create the chunk at. </param>
        /// <returns> The newly created chunk. </returns>
        /// <exception cref="EntityException"> If the prefab couldn't be instantiated. </exception>
        private Chunk CreateChunk(Vector3I position)
        {
            if (_chunks.FirstOrDefault(x => x.ChunkPosition == position) != null)
            {
                throw new EntityException($"There is already a chunk at the coordinates {position}!");
            }

            Chunk chunk = new Chunk(position, _chunkSize);
            _chunks.Add(chunk);

            return chunk;
        }


        /// <summary> Get the chunk at the specified chunk coordinate. </summary>
        /// <param name="position"> The position of the chunk in chunk coordinates to get. </param>
        /// <returns> The found chunk. Will throw an error if the position is out of bounds. </returns>
        /// <exception cref="ArgumentOutOfRangeException"> If the given position is not within the bounds of a chunk. </exception>
        public Chunk GetChunkFromChunkPosition(Vector3I position)
        {
            Chunk? chunk = _chunks.FirstOrDefault(x => x.ChunkPosition == position) ?? null;
            if (chunk == null)
            {
                throw new ArgumentOutOfRangeException($"There is no chunk at the given chunk position of {position}.");
            }
            return chunk;
        }


        /// <summary> Get the chunk at the specified world coordinate. </summary>
        /// <param name="globalPosition"> The position of the chunk in world coordinates to get. </param>
        /// <returns> The found chunk. Will throw an error if the position is out of bounds. </returns>
        /// <exception cref="ArgumentOutOfRangeException"> If the given position is not within the bounds of a chunk. </exception>
        public Chunk GetChunkFromWorldPosition(Vector3I globalPosition) => GetChunkFromChunkPosition(globalPosition / _chunkSize);


        /// <summary> Gets an array of the entities current occupying the given global position. </summary>
        /// <param name="globalPosition"> The global position to get the entities from. </param>
        /// <returns> An array of the entities currently occupying the given position.</returns>
        public IEntity[] GetEntities(Vector3I globalPosition)
        {
            Chunk chunk = GetChunkFromWorldPosition(globalPosition);
            return chunk.GetEntities(globalPosition / _chunkSize);
        }
    }
}
