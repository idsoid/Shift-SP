using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

//Commands
//TODO: create common abstract classes
#region
public class MovementAxisCommand
{
    public float HorizontalAxis { get; private set; }
    public float VerticalAxis { get; private set; }

    public MovementAxisCommand(float horizontalAxis, float verticalAxis)
    {
        HorizontalAxis = horizontalAxis;
        VerticalAxis = verticalAxis;
    }
}
public class MouseAxisCommand
{
    public float MouseX { get; private set; }
    public float MouseY { get; private set; }

    public MouseAxisCommand(float mouseX, float mouseY)
    {
        MouseX = mouseX;
        MouseY = mouseY;
    }
}
public class AttackButtonCommand
{
    public bool ShootButton { get; private set; }
    public bool AbilityButton { get; private set; }
    public bool ReloadButton { get; private set; }

    public AttackButtonCommand(bool shootButton, bool abilityButton, bool reloadButton)
    {
        ShootButton = shootButton;
        AbilityButton = abilityButton;
        ReloadButton = reloadButton;
    }
}
public class MovementButtonCommand
{
    public bool RunButton { get; private set; }
    public bool JumpButton { get; private set; }

    public MovementButtonCommand(bool runButton, bool jumpButton)
    {
        RunButton = runButton;
        JumpButton = jumpButton;
    }
}
public class ShiftButtonCommand
{
    public bool ShiftButton { get; private set; }
    public ShiftButtonCommand(bool shiftButton)
    {
        ShiftButton = shiftButton;
    }
}
#endregion

//Main
public class InputController : MonoBehaviour
{
    [SerializeField] private PlayerInput playerInput;
    private MovementAxisCommand _lastMovementAxisCommand = new(0, 0);
    private MouseAxisCommand _lastMouseAxisCommand = new(0, 0);
    private AttackButtonCommand _lastAttackCommand = new(false, false, false);
    private MovementButtonCommand _lastMovementButtonCommand = new(false, false);
    private ShiftButtonCommand _lastShiftButtonCommand = new(false);

    public bool TryGetMovementAxisInput(out MovementAxisCommand movementAxisCommand)
    {
        Vector2 direction = playerInput.actions.FindAction("Move").ReadValue<Vector2>();
        float horizontalAxis = direction.x;
        float verticalAxis = direction.y;
        bool hasAxisInputChanged = _lastMovementAxisCommand.HorizontalAxis != horizontalAxis 
            || _lastMovementAxisCommand.VerticalAxis != verticalAxis;

        if (hasAxisInputChanged)
            _lastMovementAxisCommand = new MovementAxisCommand(horizontalAxis, verticalAxis);

        movementAxisCommand = _lastMovementAxisCommand;

        return hasAxisInputChanged;
    }
    public bool TryGetMouseAxisInput(out MouseAxisCommand mouseAxisCommand)
    {
        Vector2 direction = playerInput.actions.FindAction("Look").ReadValue<Vector2>();
        float mouseX = direction.x;
        float mouseY = direction.y;
        bool hasAxisInputChanged = _lastMouseAxisCommand.MouseX != mouseX || _lastMouseAxisCommand.MouseY != mouseY;

        if (hasAxisInputChanged)
            _lastMouseAxisCommand = new MouseAxisCommand(mouseX, mouseY);

        mouseAxisCommand = _lastMouseAxisCommand;

        return hasAxisInputChanged;
    }
    public bool TryGetAttackButtonInput(out AttackButtonCommand attackButtonCommand)
    {
        bool hasAttackInputChanged = _lastAttackCommand.ShootButton != playerInput.actions.FindAction("Attack").IsPressed() 
            || _lastAttackCommand.AbilityButton != playerInput.actions.FindAction("Ability").triggered 
            || _lastAttackCommand.ReloadButton != playerInput.actions.FindAction("Reload").triggered;
        
        if (hasAttackInputChanged)
            _lastAttackCommand = new AttackButtonCommand(playerInput.actions.FindAction("Attack").IsPressed(), 
                playerInput.actions.FindAction("Ability").triggered, 
                playerInput.actions.FindAction("Reload").triggered);
        
        attackButtonCommand = _lastAttackCommand;

        return hasAttackInputChanged;
    }
    public bool TryGetMovementButtonInput(out MovementButtonCommand movementButtonCommand)
    {
        bool hasMovementInputChanged = _lastMovementButtonCommand.RunButton != playerInput.actions.FindAction("Sprint").IsPressed() 
            || _lastMovementButtonCommand.JumpButton != playerInput.actions.FindAction("Jump").triggered;
        
        if (hasMovementInputChanged)
            _lastMovementButtonCommand = new MovementButtonCommand(playerInput.actions.FindAction("Sprint").IsPressed(), 
                playerInput.actions.FindAction("Jump").triggered);
        
        movementButtonCommand = _lastMovementButtonCommand;

        return hasMovementInputChanged;
    }
    public bool TryGetShiftButtonInput(out ShiftButtonCommand shiftButtonCommand) 
    {
        bool hasShiftInputChanged = _lastShiftButtonCommand.ShiftButton != playerInput.actions.FindAction("Shift").triggered;

        if (hasShiftInputChanged)
            _lastShiftButtonCommand = new ShiftButtonCommand(playerInput.actions.FindAction("Shift").triggered);

        shiftButtonCommand = _lastShiftButtonCommand;

        return hasShiftInputChanged;
    }
}
