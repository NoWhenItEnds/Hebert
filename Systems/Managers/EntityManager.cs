using Godot;
using Hebert.Entities;
using Hebert.Types;
using Hebert.Types.Singletons;

namespace Hebert.Managers
{
    /// <summary> The manager for all the entities within the game world. </summary>
    public partial class EntityManager : SingletonNode3D<EntityManager>
    {
        /// <summary> The prefab to use when spawning entities. </summary>
        [ExportGroup("Resources")]
        [Export] private PackedScene _entityPrefab;


        /// <summary> The pool of entity nodes. </summary>
        private ObjectPool<EntityNode> _entityPool;


        public override void _Ready()
        {
            _entityPool = new ObjectPool<EntityNode>(this, _entityPrefab);
        }


        /// <summary> Create a new entity node with a new entity data object of the given type. </summary>
        /// <typeparam name="T"> The kind of entity data to assign it. </typeparam>
        /// <param name="worldPosition"> The global cell position to spawn it at. </param>
        /// <returns> The created entity node. </returns>
        public EntityNode CreateEntity<T>(Vector3I worldPosition) where T : IEntity
        {
            EntityNode entity = _entityPool.GetAvailableObject();
            Actor actor = new Actor(WorldPosition.Create(worldPosition));
            entity.Initialise(actor);
            return entity;
        }


        /// <summary> Create a new entity node to representing already existing entity data. </summary>
        /// <param name="data"> A reference to the data the node will represent. </param>
        /// <returns> The created entity node. </returns>
        public EntityNode CreateEntity(IEntity data)
        {
            EntityNode entity = _entityPool.GetAvailableObject();
            entity.Initialise(data);
            return entity;
        }


        /// <summary> Free the given entity and return it to the object pool for reuse. </summary>
        /// <param name="entity"> The entity node to free. </param>
        public void FreeEntity(EntityNode entity)
        {
            entity.FreeObject();
            _entityPool.FreeObject(entity);
        }
    }
}
