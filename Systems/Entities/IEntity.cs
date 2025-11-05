using System;
using Godot;
using Hebert.Types;

namespace Hebert.Entities
{
    /// <summary> A data representation of a physical entity within the world. </summary>
    public interface IEntity
    {
        /// <summary> Gets the entity's current cell position within the world. </summary>
        public Vector3I Position { get; }

        /// <summary> Whether the entity blocks movement through it. </summary>
        public Boolean BlocksMovement { get; }

        /// <summary> Whether the entity blocks sight. </summary>
        public Boolean BlocksSight { get; }
    }
}
