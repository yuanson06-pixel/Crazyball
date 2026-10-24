using Crazyball.CourseKit.Upgrades;
using UnityEngine;

namespace Crazyball.CourseKit.Player
{
    public sealed class PlayerResponsivenessUpgrade : MonoBehaviour, IUpgradeEffectReceiver
    {
        [SerializeField] private PlayerBallController controller;

        public UpgradeEffectType EffectType => UpgradeEffectType.InputResponsiveness;

        public void Apply(float value, int level)
        {
            if (controller != null)
                controller.SetFollowSmoothTime(Mathf.Max(0.015f, value));
        }
    }
}
