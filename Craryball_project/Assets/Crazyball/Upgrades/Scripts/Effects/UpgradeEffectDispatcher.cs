using System.Collections.Generic;
using UnityEngine;

namespace Crazyball.CourseKit.Upgrades
{
    public sealed class UpgradeEffectDispatcher : MonoBehaviour
    {
        [SerializeField] private InGameUpgradeManager upgradeManager;
        [SerializeField] private MonoBehaviour[] receiverComponents;

        private readonly Dictionary<UpgradeEffectType, IUpgradeEffectReceiver> receivers =
            new Dictionary<UpgradeEffectType, IUpgradeEffectReceiver>();

        private void Awake()
        {
            receivers.Clear();
            foreach (MonoBehaviour component in receiverComponents)
            {
                if (component is IUpgradeEffectReceiver receiver)
                    receivers[receiver.EffectType] = receiver;
            }
        }

        private void OnEnable()
        {
            if (upgradeManager != null)
                upgradeManager.UpgradePurchased += OnUpgradePurchased;
        }

        private void OnDisable()
        {
            if (upgradeManager != null)
                upgradeManager.UpgradePurchased -= OnUpgradePurchased;
        }

        private void OnUpgradePurchased(UpgradeDefinition definition, int level)
        {
            if (receivers.TryGetValue(definition.EffectType, out IUpgradeEffectReceiver receiver))
                receiver.Apply(definition.GetEffectValue(level), level);
        }
    }
}
