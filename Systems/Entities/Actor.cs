using Godot;
using Hebert.Types;

namespace Hebert.Entities
{
    /// <summary> An entity with autonomous movement. Represents a creature within the game world. </summary>
    public class Actor : IEntity
    {
        /// <inheritdoc/>
        public WorldPosition Position { get; private set; }


        public Actor(WorldPosition position)
        {
            Position = position;
        }


        /// <summary> Attempt to move the actor to a new location relative to itself. It will move towards the position until something blocks it. </summary>
        /// <param name="relativePosition"> The cell coordinates of a position relative to the entity. </param>
        /// <returns> An array of all blocking entities. </returns>
        public IEntity[] Move(Vector3I relativePosition)
        {
            // TODO - This should include a AStar pather.
            IEntity[] entities = Position.GetEntities(Position.GlobalPosition + relativePosition);
            if(entities.Length == 0)
            {
                Position.UpdateRelativePosition(relativePosition);
            }
            return entities;
        }
    }
}
