using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Crazyball.CourseKit.Input
{
    public sealed class PlayerPointerInput : MonoBehaviour
    {
        [SerializeField] private InputActionReference moveAction;
        [SerializeField] private InputActionReference launchAction;
        [SerializeField] private InputActionReference pauseAction;

        public event Action LaunchRequested;
        public event Action PauseRequested;

        public float Horizontal { get; private set; }

        private void OnEnable()
        {
            Enable(moveAction);
            Enable(launchAction);
            Enable(pauseAction);

            if (moveAction != null)
            {
                moveAction.action.performed += OnMove;
                moveAction.action.canceled += OnMove;
            }
            if (launchAction != null)
                launchAction.action.performed += OnLaunch;
            if (pauseAction != null)
                pauseAction.action.performed += OnPause;
        }

        private void OnDisable()
        {
            if (moveAction != null)
            {
                moveAction.action.performed -= OnMove;
                moveAction.action.canceled -= OnMove;
            }
            if (launchAction != null)
                launchAction.action.performed -= OnLaunch;
            if (pauseAction != null)
                pauseAction.action.performed -= OnPause;

            Disable(moveAction);
            Disable(launchAction);
            Disable(pauseAction);
        }

        private void OnMove(InputAction.CallbackContext context)
        {
            Horizontal = context.ReadValue<float>();
        }

        private void OnLaunch(InputAction.CallbackContext context)
        {
            LaunchRequested?.Invoke();
        }

        private void OnPause(InputAction.CallbackContext context)
        {
            PauseRequested?.Invoke();
        }

        private static void Enable(InputActionReference actionReference)
        {
            actionReference?.action.Enable();
        }

        private static void Disable(InputActionReference actionReference)
        {
            actionReference?.action.Disable();
        }
    }
}
