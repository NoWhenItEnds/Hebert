using Godot;
using Hebert.Types.Singletons;

namespace Hebert.Managers
{
    /// <summary> A manager that translates the player's input to world commands. </summary>
    public partial class InputManager : SingletonNode<InputManager>
    {
        /// <inheritdoc/>
        public override void _Ready()
        {
            GD.Print("Hello World");
        }
    }
}
