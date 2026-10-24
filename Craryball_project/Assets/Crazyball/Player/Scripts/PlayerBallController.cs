using Crazyball.CourseKit.Input;
using UnityEngine;

namespace Crazyball.CourseKit.Player
{
    public sealed class PlayerBallController : MonoBehaviour
    {
        [SerializeField] private PlayerPointerInput pointerInput;
        [SerializeField] private PinballProjectile pinball;
        [SerializeField] private PlayerScaleUpgrade scaleUpgrade;
        [SerializeField] private PlayerWidthUpgrade widthUpgrade;
        [SerializeField, Min(0.1f)] private float moveSpeed = 7f;
        [SerializeField] private Vector2 horizontalLimits = new Vector2(-3.1f, 3.1f);

        private bool movementEnabled = true;
        private float horizontalVelocity;
        private Vector3 startingPosition;

        public float HorizontalVelocity => horizontalVelocity;

        private void Awake()
        {
            startingPosition = transform.position;
        }

        private void OnEnable()
        {
            if (pointerInput != null)
                pointerInput.LaunchRequested += LaunchPinball;
        }

        private void OnDisable()
        {
            if (pointerInput != null)
                pointerInput.LaunchRequested -= LaunchPinball;
        }

        private void Update()
        {
            float input = movementEnabled && pointerInput != null ? pointerInput.Horizontal : 0f;
            horizontalVelocity = input * moveSpeed;
            Vector3 position = transform.position;
            position.x = Mathf.Clamp(position.x + horizontalVelocity * Time.deltaTime, horizontalLimits.x, horizontalLimits.y);
            transform.position = position;
        }

        public void SetMovementEnabled(bool enabled)
        {
            movementEnabled = enabled;
            if (!enabled) horizontalVelocity = 0f;
        }

        public void ResetPinball()
        {
            pinball?.Catch();
        }

        public void ResetForRun()
        {
            horizontalVelocity = 0f;
            transform.position = startingPosition;
            scaleUpgrade?.ResetForRun();
            widthUpgrade?.ResetForRun();
            pinball?.ResetUpgrades();
        }

        public void SetFollowSmoothTime(float value)
        {
            moveSpeed = Mathf.Max(0.1f, value <= 1f ? 1f / Mathf.Max(0.05f, value) : value);
        }

        private void LaunchPinball()
        {
            if (movementEnabled)
                pinball?.Launch();
        }
    }

}
