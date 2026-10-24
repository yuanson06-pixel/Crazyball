using Crazyball.CourseKit.Upgrades;
using UnityEngine;

namespace Crazyball.CourseKit.Player
{
    public sealed class PinballUpgradeReceiver : MonoBehaviour, IUpgradeEffectReceiver
    {
        [SerializeField] private PinballProjectile projectile;
        [SerializeField] private UpgradeEffectType effectType = UpgradeEffectType.PinballDamage;

        public UpgradeEffectType EffectType => effectType;

        public void Apply(float value, int level)
        {
            projectile?.ApplyUpgrade(effectType, value);
        }
    }
}
