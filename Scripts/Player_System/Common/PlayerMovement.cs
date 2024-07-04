using System;
using _Project.Scripts.Interface;
using UnityEngine;
using Zenject;

namespace _Project.Scripts.Player_System.Common
{
    public enum MovementType
    {
        Strafe,
        Directional
    }

    [Serializable]
    public class GeneralSettings
    {
        public float MouseSensitivity;
        public bool CursorLock;
    }

    [Serializable]
    public class MovementSettings
    {
        public MovementType MovementType;
        [Space, Header("Speed Settings")] public float SpeedChangeRate = 10f;
        public float WalkSpeed = 1.5f;
        public float RunSpeed = 3f;
        public float SprintSpeed = 6f;
        public float CrouchSpeed = 0.75f;
        public float RotationSpeed = 15f;
        [Header("Rotation Settings")] public float RotationSmoothDampTime = 0.12f;
        [Header("Jump Settings")] public float JumpHeight = 0.75f;
    }

    [Serializable]
    public class GroundSettings
    {
        public bool IsGrounded;
        public float Gravity;
        public float GroundOffset;
        public float GroundRadius;
        public float TimeInAir;

        public LayerMask GroundLayerMask;
    }

    [Serializable]
    public class CameraSettings
    {
        public GameObject CameraRigObject;
        [Range(-180f, 180f)] public float CameraTopClamp;
        [Range(-180f, 180f)] public float CameraBottomClamp;
    }

    public class PlayerMovement : MonoBehaviour
    {
        [SerializeField] private GeneralSettings _generalSettings;
        [SerializeField] private MovementSettings _movementSettings;
        [SerializeField] private GroundSettings _groundSettings;
        [SerializeField] private CameraSettings _cameraSettings;

        private Player _player;

        private Camera _mainCamera;
        private Transform _transform;

        private float _targetSpeed;
        private float _targetRotation;
        private float _verticalVelocity;
        private float _currentVelocityForRotation;

        private float _cameraTargetYaw;
        private float _cameraTargetPitch;

        private const int CAMERA_DELTA_MULTIPLIER = 1;
        private const float THRESHOLD = 0.1f;


        private readonly int SPEED_HASH_ID = Animator.StringToHash("Speed");
        private readonly int CROUCH_HASH_ID = Animator.StringToHash("IsCrouch");
        private readonly int JUMP_HASH_ID = Animator.StringToHash("IsJump");
        private readonly int FALLING_HASH_ID = Animator.StringToHash("IsFall");
        private readonly int FALLING_AIR_TIMER = Animator.StringToHash("InAirTimer");

        [Inject]
        private void Constructor(Player player)
        {
            _player = player;
        }

        private void Awake()
        {
            _mainCamera = Camera.main;
            _transform = transform;
        }

        private void Start()
        {
            Cursor.visible = _generalSettings.CursorLock;
            Cursor.lockState = _generalSettings.CursorLock ? CursorLockMode.Locked : CursorLockMode.None;
        }
        
        private void Update()
        {
            Movement();
            ApplyGravity();

            if (_player.Input.Jump && _groundSettings.IsGrounded)
                Jump();
        }

        private void LateUpdate()
        {
            CameraRotation();
        }

        private void Movement()
        {
            _targetSpeed = Mathf.MoveTowards(_targetSpeed, GetTargetSpeed(),
                _movementSettings.SpeedChangeRate * Time.deltaTime);

            Vector3 moveDirection = new Vector3(_player.Input.Move.x, 0.0f, _player.Input.Move.y).normalized;

            if (_player.Input.Move != Vector2.zero)
                Rotation(moveDirection);

            Vector3 targetDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;

            _player.Controller.Move(targetDirection * (_targetSpeed * Time.deltaTime) +
                                    new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);

            bool canCrouch = !_player.Input.Sprint && !_player.Input.Walk && _groundSettings.IsGrounded;
            _player.Animator.SetBool(CROUCH_HASH_ID, _player.Input.Crouch && canCrouch);
            _player.Animator.SetFloat(SPEED_HASH_ID, _targetSpeed);
        }

        private void Rotation(Vector3 moveDirection)
        {
            _targetRotation = Mathf.Atan2(moveDirection.x, moveDirection.z) * Mathf.Rad2Deg +
                              _mainCamera.transform.eulerAngles.y;

            float rotation = Mathf.SmoothDampAngle(_transform.eulerAngles.y, _targetRotation,
                ref _currentVelocityForRotation, _movementSettings.RotationSmoothDampTime);

            Quaternion lookAtRotation = Quaternion.Euler(0.0f, rotation, 0.0f);
            _transform.rotation =
                Quaternion.Slerp(_transform.rotation, lookAtRotation, _movementSettings.RotationSpeed);
        }

        private void Jump()
        {
            _verticalVelocity = Mathf.Sqrt(_movementSettings.JumpHeight * -2 * _groundSettings.Gravity);
            _player.Animator.Play(JUMP_HASH_ID);
        }

        private void CameraRotation()
        {
            if (_player.Input.Look.sqrMagnitude > THRESHOLD)
            {
                _cameraTargetPitch += _player.Input.Look.y * CAMERA_DELTA_MULTIPLIER;
                _cameraTargetYaw += _player.Input.Look.x * CAMERA_DELTA_MULTIPLIER;
            }

            _cameraTargetPitch = Mathf.Clamp(_cameraTargetPitch, _cameraSettings.CameraBottomClamp,
                _cameraSettings.CameraTopClamp);

            _cameraSettings.CameraRigObject.transform.rotation =
                Quaternion.Euler(_cameraTargetPitch, _cameraTargetYaw, 0.0f);
        }

        private bool GroundCheck()
        {
            _groundSettings.IsGrounded = Physics.CheckSphere(GetGroundCheckSpherePosition(),
                _groundSettings.GroundRadius, _groundSettings.GroundLayerMask);

            _groundSettings.TimeInAir = _groundSettings.IsGrounded ? 0f : _groundSettings.TimeInAir + Time.deltaTime;

            _player.Animator.SetFloat(FALLING_AIR_TIMER, _groundSettings.TimeInAir);
            _player.Animator.SetBool(FALLING_HASH_ID, !_groundSettings.IsGrounded);

            return _groundSettings.IsGrounded;
        }

        private void ApplyGravity()
        {
            if (GroundCheck() && _verticalVelocity < 0.0f)
                _verticalVelocity = -2f;

            _verticalVelocity += _groundSettings.Gravity * Time.deltaTime;
        }

        private Vector3 GetGroundCheckSpherePosition()
        {
            return _transform.position + _transform.up * _groundSettings.GroundOffset;
        }

        private float GetTargetSpeed()
        {
            if (_player.Input.Move == Vector2.zero)
                return 0.0f;

            float targetSpeed = _movementSettings.RunSpeed;

            if (_player.Input.Sprint)
                targetSpeed = _movementSettings.SprintSpeed;
            else if (_player.Input.Walk && !_player.Input.Crouch)
                targetSpeed = _movementSettings.WalkSpeed;
            else if (_player.Input.Crouch && !_player.Input.Walk)
                targetSpeed = _movementSettings.CrouchSpeed;

            return targetSpeed;
        }
    }
}