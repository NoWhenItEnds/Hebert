using Godot;
using Hebert.Types.Singletons;
using System;

namespace Hebert.Types
{
    /// <summary> The game world's camera node. This is moved to where the player is looking. </summary>
    public partial class WorldCamera : SingletonNode3D<WorldCamera>
    {
        /// <summary> The main camera through which the player will see the world. </summary>
        [ExportGroup("Nodes")]
        [Export] private Camera3D _mainCamera;


        /// <summary> The current node the camera is following. </summary>
        /// <remarks> The camera is orient itself exactly to the node's position and rotation via its transform. </remarks>
        private Node3D? _currentTarget = null;


        /// <summary> Set the camera to follow the given node. A null will stop it from following anything. </summary>
        public void SetTarget(Node3D? node) => _currentTarget = node;


        /// <inheritdoc/>
        public override void _Process(Double delta)
        {
            if (_currentTarget != null)
            {
                GlobalPosition = _currentTarget.GlobalPosition;
                GlobalRotation = _currentTarget.GlobalRotation;
            }
        }
    }
}
