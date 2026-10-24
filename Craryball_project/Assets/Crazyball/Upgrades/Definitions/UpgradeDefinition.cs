using UnityEngine;

namespace Crazyball.CourseKit.Upgrades
{
    [CreateAssetMenu(fileName = "SO_Upgrade_New", menuName = "Crazyball/Upgrade Definition")]
    public sealed class UpgradeDefinition : ScriptableObject
    {
        [SerializeField] private string upgradeId = "upgrade.new";
        [SerializeField] private string displayName = "New Upgrade";
        [SerializeField, TextArea] private string description;
        [SerializeField] private Sprite icon;
        [SerializeField] private UpgradeCategory category;
        [SerializeField] private UpgradeEffectType effectType;
        [SerializeField, Min(1)] private int maxLevel = 3;
        [SerializeField] private int[] costs = { 1, 2, 3 };
        [SerializeField] private float[] effectValues = { 1f, 2f, 3f };
        [SerializeField] private UpgradeDefinition requiredPersistentUnlock;

        public string UpgradeId => upgradeId;
        public string DisplayName => displayName;
        public string Description => description;
        public Sprite Icon => icon;
        public UpgradeCategory Category => category;
        public UpgradeEffectType EffectType => effectType;
        public int MaxLevel => maxLevel;
        public UpgradeDefinition RequiredPersistentUnlock => requiredPersistentUnlock;

        public int GetCostForLevel(int currentLevel)
        {
            if (costs == null || currentLevel < 0 || currentLevel >= costs.Length)
                return int.MaxValue;

            return Mathf.Max(0, costs[currentLevel]);
        }

        public float GetEffectValue(int level)
        {
            int index = level - 1;
            if (effectValues == null || index < 0 || index >= effectValues.Length)
                return 0f;

            return effectValues[index];
        }

        private void OnValidate()
        {
            maxLevel = Mathf.Max(1, maxLevel);
        }
    }
}
