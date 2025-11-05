using System;
using Godot;
using Hebert.Managers;

namespace Hebert.Entities
{
    /// <summary> An entity with autonomous movement. Represents a creature within the game world. </summary>
    public class Actor : IEntity
    {
        /// <inheritdoc/>
        public Vector3I Position { get; private set; }

        /// <inheritdoc/>
        public bool BlocksMovement { get; private set; } = false;

        /// <inheritdoc/>
        public bool BlocksSight { get; private set; } = false;


        /// <summary> An entity with autonomous movement. Represents a creature within the game world. </summary>
        /// <param name="position"> The actor's initial cell position within the world. </param>
        public Actor(Vector3I position)
        {
            Position = position;
        }


        /// <summary> Attempt to move the actor to a new location relative to itself. It will move towards the position until something blocks it. </summary>
        /// <param name="relativePosition"> The cell coordinates of a position relative to the entity. </param>
        /// <returns> Whether the action was successfully completed. </returns>
        public Boolean Move(Vector3I relativePosition)
        {
            Vector3I desiredPosition = Position + relativePosition;
            Cell desiredCell = ChunkManager.Instance.GetCell(desiredPosition);
            Boolean isSuccessful = false;
            // TODO - Should use the astar.
            if(!desiredCell.BlocksMovement)
            {
                Position = desiredPosition;
                isSuccessful = true;
            }
            return isSuccessful;
        }
    }
}
