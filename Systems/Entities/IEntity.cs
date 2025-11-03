using Hebert.Types;

namespace Hebert.Entities
{
    /// <summary> A data representation of a physical entity within the world. </summary>
    public interface IEntity
    {
        /// <summary> Gets the entity's current position within the world. </summary>
        public WorldPosition Position { get; }
    }
}
