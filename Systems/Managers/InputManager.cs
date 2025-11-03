using Godot;
using Hebert.Entities;
using Hebert.Types;
using Hebert.Types.Singletons;

namespace Hebert.Managers
{
    /// <summary> A manager that translates the player's input to world commands. </summary>
    public partial class InputManager : SingletonNode<InputManager>
    {
        // TODO - Move this to player controller.
        private Actor _player;


        /// <inheritdoc/>
        public override void _Input(InputEvent @event)
        {
            if (@event.IsActionPressed("action_wait"))
            {
                EntityNode node = EntityManager.Instance.CreateEntity<Actor>(new Vector3I(0, 0, 0));
                _player = node.GetEntity<Actor>();
                WorldCamera.Instance.SetTarget(node.CameraPosition);
            }

            if (@event.IsActionPressed("action_n"))
            {
                _player.Move(new Vector3I(0, -1, 0));
            }
            else if (@event.IsActionPressed("action_ne"))
            {
                _player.Move(new Vector3I(1, -1, 0));
            }
            else if (@event.IsActionPressed("action_e"))
            {
                _player.Move(new Vector3I(1, 0, 0));
            }
            else if (@event.IsActionPressed("action_se"))
            {
                _player.Move(new Vector3I(1, 1, 0));
            }
            else if (@event.IsActionPressed("action_s"))
            {
                _player.Move(new Vector3I(0, 1, 0));
            }
            else if (@event.IsActionPressed("action_sw"))
            {
                _player.Move(new Vector3I(-1, 1, 0));
            }
            else if (@event.IsActionPressed("action_w"))
            {
                _player.Move(new Vector3I(-1, 0, 0));
            }
            else if (@event.IsActionPressed("action_nw"))
            {
                _player.Move(new Vector3I(-1, -1, 0));
            }
        }
    }
}
