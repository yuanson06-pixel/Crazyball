using Crazyball.CourseKit.Upgrades;
using UnityEngine;

namespace Crazyball.CourseKit.Player
{
    public sealed class PlayerWidthUpgrade : MonoBehaviour, IUpgradeEffectReceiver
    {
        private Vector3 initialScale;
        private float widthMultiplier = 1f;

        public UpgradeEffectType EffectType => UpgradeEffectType.PlayerWidth;

        private void Awake()
        {
            initialScale = transform.localScale;
        }

        public void Apply(float value, int level)
        {
            widthMultiplier = Mathf.Max(1f, value);
            ApplyWidth();
        }

        public void ResetForRun()
        {
            widthMultiplier = 1f;
            ApplyWidth();
        }

        private void LateUpdate()
        {
            ApplyWidth();
        }

        private void ApplyWidth()
        {
            Vector3 scale = transform.localScale;
            float uniformMultiplier = scale.y / Mathf.Max(.001f, initialScale.y);
            scale.x = initialScale.x * uniformMultiplier * widthMultiplier;
            transform.localScale = scale;
        }
    }
}
