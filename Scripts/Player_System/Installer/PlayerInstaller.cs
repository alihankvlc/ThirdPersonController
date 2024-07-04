using _Project.Scripts.Player_System.Common;
using _Project.Scripts.Player_System.InputSystem;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace _Project.Scripts.Player_System.Installer
{
    public class PlayerInstaller : MonoInstaller
    {
        [SerializeField] private GameObject _player;

        public override void InstallBindings()
        {
            _player ??= FindObjectOfType<PlayerBehaviour>().gameObject;

            if (_player == null)
            {
                Debug.LogWarning("Player is null");
                return;
            }

            Container.Bind<Animator>().FromComponentOn(_player).AsSingle();
            Container.Bind<CharacterController>().FromComponentOn(_player).AsSingle();
            Container.Bind<InputController>().FromComponentOn(_player).AsSingle();
            Container.Bind<PlayerInput>().FromComponentOn(_player).AsSingle();

            Container.Bind<Player>().AsSingle().WithArguments(
                Container.Resolve<Animator>(),
                Container.Resolve<CharacterController>(),
                Container.Resolve<InputController>()
            );

            InputController inputController = Container.Resolve<InputController>();
            PlayerInput playerInputComponent = Container.Resolve<PlayerInput>();
            
            inputController.PlayerInput = playerInputComponent;
        }
    }
}