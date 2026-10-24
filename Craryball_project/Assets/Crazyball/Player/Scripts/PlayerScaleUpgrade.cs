using Crazyball.CourseKit.Upgrades;
using UnityEngine;

namespace Crazyball.CourseKit.Player
{
    public sealed class PlayerScaleUpgrade : MonoBehaviour, IUpgradeEffectReceiver
    {
        private Vector3 initialScale;

        public UpgradeEffectType EffectType => UpgradeEffectType.PlayerScale;

        private void Awake()
        {
            initialScale = transform.localScale;
        }

        public void Apply(float value, int level)
        {
            transform.localScale = initialScale * Mathf.Clamp(value, 0.45f, 1f);
        }

        public void ResetForRun()
        {
            transform.localScale = initialScale;
        }
    }
}
