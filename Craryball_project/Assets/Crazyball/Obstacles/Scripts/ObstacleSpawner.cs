using Crazyball.CourseKit.Coins;
using Crazyball.CourseKit.Player;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Crazyball.CourseKit.Obstacles
{
    public sealed class ObstacleSpawner : MonoBehaviour
    {
        [SerializeField] private GameObject obstacleBlockPrefab;
        [SerializeField] private GameObject coinPrefab;
        [SerializeField] private float spawnZ = 8f;
        [SerializeField] private float laneWidth = 6.2f;
        [SerializeField] private PlayerBallController player;
        [SerializeField] private PlayerSurvival survival;

        private float timer;
        private float interval = 1.4f;
        private float scrollSpeed = 2f;
        private float coinChance = 0.45f;
        private bool spawning;
        private readonly List<GameObject> spawnedObjects = new List<GameObject>();
        private EncounterTarget activeEncounter;

        public event Action<string, int, int> EncounterHealthChanged;
        public event Action EncounterEnded;
        public bool IsEncounterActive => activeEncounter != null;

        private void Update()
        {
            if (!spawning || obstacleBlockPrefab == null)
                return;

            timer -= Time.deltaTime;
            if (timer > 0f)
                return;

            timer = interval;
            SpawnRow();
        }

        public void Configure(int stage, float worldSlowMultiplier = 1f, float coinBonus = 0f)
        {
            ResetSpawnedObjects();
            interval = Mathf.Max(0.45f, 1.55f - stage * 0.16f);
            scrollSpeed = (1.8f + stage * 0.45f) * Mathf.Clamp(worldSlowMultiplier, 0.4f, 1f);
            coinChance = Mathf.Clamp01(0.35f + coinBonus);
            timer = 0.2f;

            if (stage == 3)
                SpawnEncounter(EncounterType.MidBoss, 14);
            else if (stage == 5)
                SpawnEncounter(EncounterType.Boss, 28);
        }

        public void SetSpawning(bool enabled)
        {
            spawning = enabled && !IsEncounterActive;
        }

        public void ResetSpawnedObjects()
        {
            spawning = false;
            timer = 0f;
            for (int index = spawnedObjects.Count - 1; index >= 0; index--)
            {
                if (spawnedObjects[index] != null)
                    Destroy(spawnedObjects[index]);
            }
            spawnedObjects.Clear();

            // [수정] FindObjectsByType 호출 시 FindObjectsSortMode.None 추가
            foreach (BossProjectile projectile in FindObjectsByType<BossProjectile>(FindObjectsSortMode.None))
            {
                if (projectile != null) Destroy(projectile.gameObject);
            }

            activeEncounter = null;
            EncounterHealthChanged?.Invoke(string.Empty, 0, 0);
        }

        private void SpawnEncounter(EncounterType type, int health)
        {
            GameObject target = GameObject.CreatePrimitive(PrimitiveType.Cube);
            target.name = type == EncounterType.MidBoss ? "Mid Boss - Breakwall" : "Boss - Core Overlord";
            target.transform.position = new Vector3(0f, .55f, type == EncounterType.MidBoss ? 8f : 8.6f);
            target.transform.localScale = type == EncounterType.MidBoss
                ? new Vector3(laneWidth, 1.1f, .7f)
                : new Vector3(2.3f, 1.4f, 1f);
            activeEncounter = target.AddComponent<EncounterTarget>();
            activeEncounter.HealthChanged += OnEncounterHealthChanged;
            activeEncounter.Defeated += OnEncounterDefeated;
            activeEncounter.Initialize(type, health, player, survival);
            spawnedObjects.Add(target);
            EncounterHealthChanged?.Invoke(activeEncounter.DisplayName, health, health);
        }

        private void OnEncounterHealthChanged(EncounterTarget target, int health, int maxHealth)
        {
            EncounterHealthChanged?.Invoke(target.DisplayName, health, maxHealth);
        }

        private void OnEncounterDefeated(EncounterTarget target)
        {
            target.HealthChanged -= OnEncounterHealthChanged;
            target.Defeated -= OnEncounterDefeated;
            activeEncounter = null;
            EncounterHealthChanged?.Invoke(string.Empty, 0, 0);
            EncounterEnded?.Invoke();
        }

        private void SpawnRow()
        {
            float gapWidth = UnityEngine.Random.Range(1.5f, 2.25f);
            float gapCenter = UnityEngine.Random.Range(-1.7f, 1.7f);
            float leftWidth = Mathf.Max(0.35f, gapCenter + laneWidth * 0.5f - gapWidth * 0.5f);
            float rightWidth = Mathf.Max(0.35f, laneWidth - leftWidth - gapWidth);

            SpawnBlock(-laneWidth * 0.5f + leftWidth * 0.5f, leftWidth);
            SpawnBlock(laneWidth * 0.5f - rightWidth * 0.5f, rightWidth);

            if (coinPrefab != null && UnityEngine.Random.value <= coinChance)
            {
                GameObject coin = Instantiate(coinPrefab, new Vector3(gapCenter, 0.55f, spawnZ + 0.5f), Quaternion.identity);
                coin.AddComponent<ScrollingObject>().Initialize(scrollSpeed);
                spawnedObjects.Add(coin);
            }
        }

        private void SpawnBlock(float x, float width)
        {
            GameObject block = Instantiate(obstacleBlockPrefab, new Vector3(x, 0.5f, spawnZ), Quaternion.identity);
            Vector3 scale = block.transform.localScale;
            block.transform.localScale = new Vector3(width, scale.y, scale.z);
            block.AddComponent<ScrollingObject>().Initialize(scrollSpeed);
            spawnedObjects.Add(block);
        }
    }
}