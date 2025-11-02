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
    public partial class ChunkManager : SingletonNode3D<ChunkManager>
    {
        /// <summary> The number of cells within a single chunk. </summary>
        /// <remarks> X = width, Y = depth, Z = height. </remarks>
        [ExportGroup("Settings")]
        [Export] private Vector3I _chunkSize = new Vector3I(256, 256, 32);

        /// <summary> The number of chunks in the world. </summary>
        // TODO - Make this generated from a world schematic.
        [Export] private Vector3I _worldSize = new Vector3I(10, 10, 2);


        /// <summary> The prefab used to spawn additional chunks. </summary>
        [ExportGroup("Resources")]
        [Export] private PackedScene _chunkPrefab;


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
                        Chunk chunk = CreateChunk(position);
                        _taskManager.AddTask(Task.Run(() => chunk.Initialise(position, _chunkSize)));
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
            if(_chunks.FirstOrDefault(x => x.ChunkPosition == position) != null)
            {
                throw new EntityException($"There is already a chunk at the coordinates {position}!");
            }

            Chunk? chunk = _chunkPrefab.InstantiateOrNull<Chunk>();
            if (chunk == null)
            {
                throw new EntityException("The chunk prefab given to the chunk manager is of the incorrect type!");
            }

            AddChild(chunk);
            _chunks.Add(chunk);

            return chunk;
        }


        /// <summary> Get the chunk at the specified chunk coordinate. </summary>
        /// <param name="position"> The position of the chunk in chunk coordinates to get. </param>
        /// <returns> The found chunk. </returns>
        /// <exception cref="EntityException"> If a chunk does not exist at the given coordinates. </exception>
        private Chunk GetChunkFromChunkPosition(Vector3I position)
        {
            try
            {
                return _chunks.First(x => x.ChunkPosition == position);
            }
            catch (InvalidOperationException exception)
            {
                throw new EntityException($"The chunk manager does not have a chunk registered to the given position of '{position}'.", exception);
            }
        }


        /// <summary> Get the chunk at the specified world coordinate. </summary>
        /// <param name="position"> The position of the chunk in world coordinates to get. </param>
        /// <returns> The found chunk. </returns>
        private Chunk GetChunkFromWorldPosition(Vector3I position)
        {
            Vector3I chunkPosition = position / _chunkSize;
            return GetChunkFromChunkPosition(chunkPosition);
        }
    }
}
