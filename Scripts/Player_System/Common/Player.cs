using _Project.Scripts.Player_System.InputSystem;
using UnityEngine;

namespace _Project.Scripts.Player_System.Common
{
    public class Player
    {
        private readonly Animator _animator;
        private readonly CharacterController _characterController;
        private readonly InputController _inputController;

        public Animator Animator
        {
            get => _animator;
        }

        public CharacterController Controller
        {
            get => _characterController;
        }

        public InputController Input
        {
            get => _inputController;
        }

        public Player(Animator animator, CharacterController controller, InputController inputController)
        {
            _animator = animator;
            _characterController = controller;
            _inputController = inputController;
        }
    }
}