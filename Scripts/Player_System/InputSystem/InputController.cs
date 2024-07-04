using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace _Project.Scripts.Player_System.InputSystem
{
    public class InputController : MonoBehaviour
    {
        private InputActionMap _actionMap;

        private InputAction _moveInputAction;
        private InputAction _walkInputAction;
        private InputAction _sprintInputAction;
        private InputAction _crouchInputAction;
        private InputAction _jumpInputAction;
        private InputAction _lookInputAction;

        private const string MOVE_INPUT_ENTRY = "Move";
        private const string WALK_INPUT_ENTRY = "Walk";
        private const string SPRINT_INPUT_ENTRY = "Sprint";
        private const string CROUCH_INPUT_ENTRY = "Crouch";
        private const string JUMP_INPUT_ENTRY = "Jump";
        private const string LOOK_INPUT_ENTRY = "Look";

        public PlayerInput PlayerInput;
        public Vector2 Move { get; private set; }
        public Vector2 Look { get; private set; }

        public bool Walk { get; private set; }
        public bool Sprint { get; private set; }
        public bool Crouch { get; private set; }
        public bool Jump { get; private set; }

        private void Awake()
        {
            _actionMap = PlayerInput.currentActionMap;

            _moveInputAction = _actionMap.FindAction(MOVE_INPUT_ENTRY);
            _walkInputAction = _actionMap.FindAction(WALK_INPUT_ENTRY);
            _sprintInputAction = _actionMap.FindAction(SPRINT_INPUT_ENTRY);
            _crouchInputAction = _actionMap.FindAction(CROUCH_INPUT_ENTRY);
            _jumpInputAction = _actionMap.FindAction(JUMP_INPUT_ENTRY);
            _lookInputAction = _actionMap.FindAction(LOOK_INPUT_ENTRY);
        }

        private void OnEnable()
        {
            _actionMap.Enable();
            SubscribeContext();
        }

        private void SubscribeContext()
        {
            _moveInputAction.performed +=
                (InputAction.CallbackContext context) => Move = context.ReadValue<Vector2>();
            _moveInputAction.canceled +=
                (InputAction.CallbackContext context) => Move = context.ReadValue<Vector2>();
            
            _lookInputAction.performed +=
                (InputAction.CallbackContext context) => Look = context.ReadValue<Vector2>();
            _lookInputAction.canceled +=
                (InputAction.CallbackContext context) => Look = context.ReadValue<Vector2>();

            _walkInputAction.performed +=
                (InputAction.CallbackContext context) => Walk = context.ReadValueAsButton();
            _walkInputAction.canceled +=
                (InputAction.CallbackContext context) => Walk = context.ReadValueAsButton();

            _sprintInputAction.performed +=
                (InputAction.CallbackContext context) => Sprint = context.ReadValueAsButton();
            _sprintInputAction.canceled +=
                (InputAction.CallbackContext context) => Sprint = context.ReadValueAsButton();

            _crouchInputAction.performed +=
                (InputAction.CallbackContext context) => Crouch = context.ReadValueAsButton();
            _crouchInputAction.canceled +=
                (InputAction.CallbackContext context) => Crouch = context.ReadValueAsButton();

            _jumpInputAction.started +=
                (InputAction.CallbackContext context) => Jump = context.ReadValueAsButton();
            _jumpInputAction.canceled +=
                (InputAction.CallbackContext context) => Jump = context.ReadValueAsButton();
        }

        private void UnsubscribeContext()
        {
            _moveInputAction.performed -=
                (InputAction.CallbackContext context) => Move = context.ReadValue<Vector2>();
            _moveInputAction.canceled -=
                (InputAction.CallbackContext context) => Move = context.ReadValue<Vector2>();

            _walkInputAction.performed -=
                (InputAction.CallbackContext context) => Walk = context.ReadValueAsButton();
            _walkInputAction.canceled -=
                (InputAction.CallbackContext context) => Walk = context.ReadValueAsButton();

            _sprintInputAction.performed -=
                (InputAction.CallbackContext context) => Sprint = context.ReadValueAsButton();
            _sprintInputAction.canceled -=
                (InputAction.CallbackContext context) => Sprint = context.ReadValueAsButton();

            _crouchInputAction.performed -=
                (InputAction.CallbackContext context) => Crouch = context.ReadValueAsButton();
            _crouchInputAction.canceled -=
                (InputAction.CallbackContext context) => Crouch = context.ReadValueAsButton();

            _jumpInputAction.started -=
                (InputAction.CallbackContext context) => Jump = context.ReadValueAsButton();
            _jumpInputAction.canceled -=
                (InputAction.CallbackContext context) => Jump = context.ReadValueAsButton();
            
            _lookInputAction.performed -=
                (InputAction.CallbackContext context) => Look = context.ReadValue<Vector2>();
            
            _lookInputAction.canceled -=
                (InputAction.CallbackContext context) => Look = context.ReadValue<Vector2>();
        }

        private void OnDisable()
        {
            _actionMap.Disable();
        }

        private void OnDestroy()
        {
            UnsubscribeContext();
        }
    }
}