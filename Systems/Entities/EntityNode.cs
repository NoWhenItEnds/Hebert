using System;
using Godot;
using Hebert.Managers;
using Hebert.Types;

namespace Hebert.Entities
{
    /// <summary> A node representing the entity within the game world. </summary>
    public partial class EntityNode : Node3D, IPoolable
    {
        /// <summary> The node the camera will use when following this entity. </summary>
        [ExportGroup("Nodes")]
        [Export] public Marker3D CameraPosition { get; private set; }

        /// <summary> The animated sprite used to display the entity. </summary>
        [Export] private AnimatedSprite3D _spriteNode;


        /// <summary> The entity data object this node represents. </summary>
        private IEntity? _entityData = null;

        /// <summary> How many meters each grid cell is. </summary>
        private Vector3 _cellSize;


        /// <inheritdoc/>
        public override void _Ready()
        {
            _cellSize = ChunkManager.Instance.CellSize;
        }


        /// <summary> Initialise the entity with new entity data. </summary>
        /// <param name="entity"> The entity data to attach to this node. </param>
        public void Initialise(IEntity entity)
        {
            _entityData = entity;
        }


        /// <summary> Get a reference to the entity data component of the node. </summary>
        /// <typeparam name="T"> The kind of entity the data represents. </typeparam>
        /// <returns> The associated entity component. </returns>
        /// <exception cref="ArgumentNullException"> If the entity data hasn't been initialised. </exception>
        /// <exception cref="InvalidCastException"> If the data component isn't of the given type. </exception>
        public T GetEntity<T>() where T : IEntity
        {
            if (_entityData == null)
            {
                throw new ArgumentNullException("The data component of the entity node hasn't been initialised.");
            }
            if (_entityData is T data)
            {
                return data;
            }
            else
            {
                throw new InvalidCastException($"Unable to cast the data to {typeof(T)}.");
            }
        }


        /// <inheritdoc/>
        public override void _Process(Double delta)
        {
            if (_entityData != null)
            {
                Vector3I entityPosition = _entityData.Position.GlobalPosition;

                // Need to break the vectors as godot-space is different.
                Vector3 godotPosition = new Vector3(entityPosition.X, entityPosition.Z, entityPosition.Y) * new Vector3(_cellSize.X, _cellSize.Z, _cellSize.Y);
                GlobalPosition = godotPosition;
            }
        }


        /// <inheritdoc/>
        public void FreeObject()
        {
            _entityData = null;
        }
    }
}
