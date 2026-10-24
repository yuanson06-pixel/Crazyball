using System;
using Crazyball.CourseKit.Core;

namespace Crazyball.CourseKit.Upgrades
{
    public sealed class PersistentUpgradeService
    {
        private readonly PersistentGameData data;
        private readonly ISaveService saveService;

        public event Action<int> CoreBalanceChanged;
        public event Action<UpgradeDefinition, int> UpgradePurchased;

        public int CoreBalance => data.upgradeCore;

        public PersistentUpgradeService(PersistentGameData data, ISaveService saveService)
        {
            this.data = data ?? throw new ArgumentNullException(nameof(data));
            this.saveService = saveService ?? throw new ArgumentNullException(nameof(saveService));
        }

        public int GetLevel(UpgradeDefinition definition)
        {
            if (definition == null)
                return 0;

            UpgradeLevelRecord record = FindRecord(definition.UpgradeId);
            return record?.level ?? 0;
        }

        public bool IsUnlocked(UpgradeDefinition inGameUpgrade)
        {
            if (inGameUpgrade == null)
                return false;

            UpgradeDefinition requirement = inGameUpgrade.RequiredPersistentUnlock;
            return requirement == null || GetLevel(requirement) > 0;
        }

        public bool TryPurchase(UpgradeDefinition definition)
        {
            if (definition == null || definition.Category == UpgradeCategory.InGame)
                return false;

            int currentLevel = GetLevel(definition);
            if (currentLevel >= definition.MaxLevel)
                return false;

            int cost = definition.GetCostForLevel(currentLevel);
            if (cost == int.MaxValue || data.upgradeCore < cost)
                return false;

            data.upgradeCore -= cost;
            UpgradeLevelRecord record = FindRecord(definition.UpgradeId);
            if (record == null)
            {
                record = new UpgradeLevelRecord { upgradeId = definition.UpgradeId };
                data.persistentUpgrades.Add(record);
            }

            record.level = currentLevel + 1;
            saveService.Save(data);
            CoreBalanceChanged?.Invoke(data.upgradeCore);
            UpgradePurchased?.Invoke(definition, record.level);
            return true;
        }

        private UpgradeLevelRecord FindRecord(string upgradeId)
        {
            return data.persistentUpgrades.Find(record => record.upgradeId == upgradeId);
        }
    }
}
