using System;
using System.Collections.Generic;
using Crazyball.CourseKit.Checkpoints;
using Crazyball.CourseKit.Coins;
using Crazyball.CourseKit.Obstacles;
using Crazyball.CourseKit.Player;
using Crazyball.CourseKit.Upgrades;
using Crazyball.CourseKit.UI;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Crazyball.CourseKit.Core
{
    public sealed class CompleteGameController : MonoBehaviour
    {
        public const int FinalStage = 5;

        [SerializeField] private RunCoinWallet wallet;
        [SerializeField] private ObstacleSpawner spawner;
        [SerializeField] private PlayerBallController playerController;
        [SerializeField] private PlayerSurvival playerSurvival;
        [SerializeField] private InGameUpgradeManager upgrades;
        [SerializeField] private CheckpointUpgradePanel upgradePanel;
        [SerializeField] private GameObject gameOverPanel;
        [SerializeField, Min(5f)] private float stageDuration = 15f;

        private readonly JsonSaveService saveService = new JsonSaveService();
        private PersistentGameData persistentData;
        private PersistentUpgradeService persistentUpgrades;
        private float stageTimer;
        private int stage;
        private GameState state;

        public event Action<int, int, int> ProgressChanged;
        public event Action<string> StatusChanged;

        private void Start()
        {
            persistentData = saveService.Load();
            persistentUpgrades = new PersistentUpgradeService(persistentData, saveService);
            playerSurvival.Defeated += OnDefeated;
            spawner.EncounterEnded += OnEncounterEnded;
            upgrades.UpgradePurchased += OnChoiceResolved;
            upgrades.OfferSkipped += OnChoiceSkipped;
            BeginRun();
        }

        private void OnDestroy()
        {
            if (playerSurvival != null)
                playerSurvival.Defeated -= OnDefeated;
            if (spawner != null)
                spawner.EncounterEnded -= OnEncounterEnded;
            if (upgrades != null)
            {
                upgrades.UpgradePurchased -= OnChoiceResolved;
                upgrades.OfferSkipped -= OnChoiceSkipped;
            }
        }

        private void Update()
        {
            if (state != GameState.Playing)
                return;

            if (spawner.IsEncounterActive)
                return;

            stageTimer -= Time.deltaTime;
            if (stageTimer <= 0f)
                ReachCheckpoint();
        }

        public void BeginRun()
        {
            Time.timeScale = 1f;
            state = GameState.Playing;
            stage = 1;
            spawner.ResetSpawnedObjects();
            wallet.ResetWallet(GetPersistentValue(UpgradeEffectType.StartingCoin));
            playerSurvival.ResetForRun(GetPersistentValue(UpgradeEffectType.Shield));
            upgrades.BeginRun();
            playerController.ResetForRun();
            gameOverPanel.SetActive(false);
            StatusChanged?.Invoke(string.Empty);
            StartStage();
        }

        public void RestartScene()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        private void StartStage()
        {
            state = GameState.Playing;
            stageTimer = stageDuration;
            playerController.SetMovementEnabled(true);
            spawner.Configure(stage);
            spawner.SetSpawning(true);
            ProgressChanged?.Invoke(stage, FinalStage, FinalStage - stage + 1);
            StatusChanged?.Invoke(string.Empty);
        }

        private void ReachCheckpoint()
        {
            state = GameState.Upgrading;
            spawner.SetSpawning(false);
            playerController.SetMovementEnabled(false);
            wallet.Add(3 + stage);

            bool firstClear = !persistentData.HasClearedCheckpoint(stage);
            if (firstClear)
            {
                persistentData.MarkCheckpointCleared(stage);
                persistentData.upgradeCore += 1;
                saveService.Save(persistentData);
            }

            ProgressChanged?.Invoke(stage, FinalStage, FinalStage - stage);
            StatusChanged?.Invoke(firstClear ? "CHECKPOINT  +1 CORE" : "CHECKPOINT");
            upgrades.OpenOffer(persistentUpgrades.IsUnlocked);
            upgradePanel.Show(upgrades.CurrentOffer, upgrades, wallet);
            Time.timeScale = 0f;
        }

        private void OnChoiceResolved(UpgradeDefinition definition, int level)
        {
            ContinueAfterCheckpoint();
        }

        private void OnChoiceSkipped()
        {
            ContinueAfterCheckpoint();
        }

        private void ContinueAfterCheckpoint()
        {
            upgradePanel.Hide();
            Time.timeScale = 1f;
            if (stage >= FinalStage)
            {
                state = GameState.Completed;
                spawner.SetSpawning(false);
                ProgressChanged?.Invoke(FinalStage, FinalStage, 0);
                StatusChanged?.Invoke("ALL STAGES CLEAR");
                gameOverPanel.SetActive(true);
                return;
            }

            stage++;
            StartStage();
        }

        private void OnDefeated()
        {
            if (state != GameState.Playing)
                return;

            state = GameState.GameOver;
            spawner.ResetSpawnedObjects();
            playerController.SetMovementEnabled(false);
            playerController.ResetForRun();
            wallet.ResetWallet(GetPersistentValue(UpgradeEffectType.StartingCoin));
            playerSurvival.ResetForRun(GetPersistentValue(UpgradeEffectType.Shield));
            upgrades.BeginRun();
            ProgressChanged?.Invoke(1, FinalStage, FinalStage);
            StatusChanged?.Invoke("GAME OVER");
            gameOverPanel.SetActive(true);
            Time.timeScale = 0f;
        }

        private void OnEncounterEnded()
        {
            if (state == GameState.Playing)
                ReachCheckpoint();
        }

        private int GetPersistentValue(UpgradeEffectType effectType)
        {
            int value = 0;
            foreach (UpgradeLevelRecord record in persistentData.persistentUpgrades)
            {
                if (record.upgradeId == $"persistent.{effectType.ToString().ToLowerInvariant()}")
                    value += record.level;
            }

            return value;
        }
    }
}
