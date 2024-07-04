using System;
using _Project.Scripts.Player_System.InputSystem;
using _Project.Scripts.Player_System.Installer;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace _Project.Scripts.Player_System.Common
{
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(PlayerInput))]
    [RequireComponent(typeof(InputController))]
    public class PlayerBehaviour : MonoBehaviour
    {
        private void OnValidate()
        {
            SceneContext sceneContext = FindObjectOfType<SceneContext>();

            if (sceneContext == null)
            {
                GameObject sceneContextParentObject = new GameObject("SceneContext");
                sceneContext = sceneContextParentObject.AddComponent<SceneContext>();

                GameObject playerInstallerObject = new GameObject("PlayerInstaller");
                PlayerInstaller playerInstaller = playerInstallerObject.AddComponent<PlayerInstaller>();

                playerInstallerObject.transform.SetParent(sceneContext.transform);
                sceneContext.Installers = new MonoInstaller[] { playerInstaller };

                playerInstaller.InstallBindings();
            }
        }
    }
}