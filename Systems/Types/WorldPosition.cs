using Godot;
using Hebert.Entities;
using Hebert.Managers;

namespace Hebert.Types
{
    /// <summary> A cell position within the game world. </summary>
    public class WorldPosition
    {
        /// <summary> A reference to the world chunk this position is within. </summary>
        public Chunk ParentChunk { get; private set; }

        /// <summary> The local coordinates of the position within the chunk. </summary>
        public Vector3I LocalPosition { get; private set; }

        /// <summary> The global coordinates of the position. </summary>
        public Vector3I GlobalPosition { get; private set; }


        /// <summary> A cell position within the game world. </summary>
        /// <param name="parentChunk"> A reference to the world chunk this position is within. </param>
        /// <param name="localPosition"> The local coordinates of the position within the chunk. </param>
        /// <param name="globalPosition"> The global coordinates of the position. </param>
        internal WorldPosition(Chunk parentChunk, Vector3I localPosition, Vector3I globalPosition)
        {
            ParentChunk = parentChunk;
            LocalPosition = localPosition;
            GlobalPosition = globalPosition;
        }


        /// <summary> Update the coordinates to the new given world position. </summary>
        /// <param name="globalPosition"> The global coordinates of the position. </param>
        /// <remarks> This is simply a position in space, it's not its responsibility to check for collisions. </remarks>
        public void UpdatePosition(Vector3I globalPosition)
        {
            Chunk chunk = ChunkManager.Instance.GetChunkFromWorldPosition(globalPosition);
            Vector3I localPosition = chunk.GetLocalPosition(globalPosition);

            ParentChunk = chunk;
            LocalPosition = localPosition;
            GlobalPosition = globalPosition;
        }


        /// <summary> Update the coordinates to a position relative to the position's current coordinates. </summary>
        /// <param name="relativePosition"> The coordinates relative to the position. </param>
        /// <remarks> This is simply a position in space, it's not its responsibility to check for collisions. </remarks>
        public void UpdateRelativePosition(Vector3I relativePosition) => UpdatePosition(GlobalPosition + relativePosition);


        /// <summary> Gets an array of the entities current occupying the given relative position. </summary>
        /// <param name="relativePosition"> The position relative to these coordinates to get the entities from. </param>
        /// <returns> An array of the entities currently occupying the given position.</returns>
        public IEntity[] GetEntities(Vector3I relativePosition)
        {
            Chunk chunk = ChunkManager.Instance.GetChunkFromWorldPosition(GlobalPosition + relativePosition);
            return chunk.GetEntities(relativePosition);
        }


        /// <summary> Attempt to create a new instance of a world position. </summary>
        /// <param name="globalPosition"> The global coordinates of the position. </param>
        /// <returns> The created world position. </returns>
        public static WorldPosition Create(Vector3I globalPosition)
        {
            Chunk chunk = ChunkManager.Instance.GetChunkFromWorldPosition(globalPosition);
            Vector3I localPosition = chunk.GetLocalPosition(globalPosition);

            return new WorldPosition(chunk, localPosition, globalPosition);
        }
    }
}
