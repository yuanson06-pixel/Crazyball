using System.Collections.Generic;
using Crazyball.CourseKit.Core;
using Crazyball.CourseKit.Obstacles;
using Crazyball.CourseKit.Upgrades;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Crazyball.CourseKit.UI
{
    public sealed class CompleteHudPresenter : MonoBehaviour
    {
        [SerializeField] private CompleteGameController gameController;
        [SerializeField] private InGameUpgradeManager upgradeManager;
        [SerializeField] private ObstacleSpawner spawner;
        [SerializeField] private TMP_Text stageLabel;
        [SerializeField] private TMP_Text statusLabel;
        [SerializeField] private TMP_Text upgradeLabel;
        [SerializeField] private TMP_Text progressLabel;
        [SerializeField] private GameObject bossHealthPanel;
        [SerializeField] private TMP_Text bossNameLabel;
        [SerializeField] private TMP_Text bossHealthLabel;
        [SerializeField] private Slider bossHealthSlider;

        private readonly Dictionary<UpgradeDefinition, int> ownedUpgrades = new Dictionary<UpgradeDefinition, int>();

        private void OnEnable()
        {
            if (gameController != null)
            {
                gameController.ProgressChanged += RefreshProgress;
                gameController.StatusChanged += RefreshStatus;
            }
            if (upgradeManager != null)
            {
                upgradeManager.RunStarted += ResetUpgrades;
                upgradeManager.UpgradePurchased += AddUpgrade;
            }
            if (spawner != null)
                spawner.EncounterHealthChanged += RefreshBossHealth;
            if (bossHealthPanel != null)
                bossHealthPanel.SetActive(false);
            ResetUpgrades();
            RefreshProgress(1, CompleteGameController.FinalStage, CompleteGameController.FinalStage);
            RefreshStatus(string.Empty);
        }

        private void OnDisable()
        {
            if (gameController != null)
            {
                gameController.ProgressChanged -= RefreshProgress;
                gameController.StatusChanged -= RefreshStatus;
            }
            if (upgradeManager != null)
            {
                upgradeManager.RunStarted -= ResetUpgrades;
                upgradeManager.UpgradePurchased -= AddUpgrade;
            }
            if (spawner != null)
                spawner.EncounterHealthChanged -= RefreshBossHealth;
        }

        private void RefreshProgress(int stage, int totalStages, int remainingCheckpoints)
        {
            if (stageLabel != null) stageLabel.text = $"STAGE {stage} / {totalStages}";
            if (progressLabel != null)
                progressLabel.text = $"CURRENT STAGE\n{stage} / {totalStages}\n\nCHECKPOINTS LEFT\n{remainingCheckpoints}";
        }

        private void RefreshStatus(string message)
        {
            if (statusLabel != null) statusLabel.text = message;
        }

        private void ResetUpgrades()
        {
            ownedUpgrades.Clear();
            RefreshUpgradeList();
        }

        private void AddUpgrade(UpgradeDefinition definition, int level)
        {
            if (definition == null) return;
            ownedUpgrades[definition] = level;
            RefreshUpgradeList();
        }

        private void RefreshUpgradeList()
        {
            if (upgradeLabel == null) return;
            if (ownedUpgrades.Count == 0)
            {
                upgradeLabel.text = "NO UPGRADES";
                return;
            }

            System.Text.StringBuilder builder = new System.Text.StringBuilder();
            foreach (KeyValuePair<UpgradeDefinition, int> pair in ownedUpgrades)
                builder.AppendLine($"{pair.Key.DisplayName}\n  LV {pair.Value}");
            upgradeLabel.text = builder.ToString().TrimEnd();
        }

        private void RefreshBossHealth(string displayName, int health, int maxHealth)
        {
            bool visible = maxHealth > 0;
            if (bossHealthPanel != null) bossHealthPanel.SetActive(visible);
            if (!visible) return;
            if (bossNameLabel != null) bossNameLabel.text = displayName;
            if (bossHealthLabel != null) bossHealthLabel.text = $"{health} / {maxHealth}";
            if (bossHealthSlider != null)
            {
                bossHealthSlider.maxValue = maxHealth;
                bossHealthSlider.value = health;
            }
        }
    }
}
