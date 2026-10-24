using System;
using Crazyball.CourseKit.Player;
using UnityEngine;

namespace Crazyball.CourseKit.Obstacles
{
    public enum EncounterType { MidBoss, Boss }

    public sealed class EncounterTarget : MonoBehaviour
    {
        private EncounterType encounterType;
        private PlayerBallController player;
        private PlayerSurvival survival;
        private int maxHealth;
        private int health;
        private float moveSpeed;
        private float fireInterval;
        private float fireTimer;
        private Vector3 startPosition;

        public event Action<EncounterTarget, int, int> HealthChanged;
        public event Action<EncounterTarget> Defeated;
        public string DisplayName => encounterType == EncounterType.MidBoss ? "BREAKWALL" : "CORE OVERLORD";

        public void Initialize(EncounterType type, int hitPoints, PlayerBallController targetPlayer, PlayerSurvival targetSurvival)
        {
            encounterType = type;
            maxHealth = Mathf.Max(1, hitPoints);
            health = maxHealth;
            player = targetPlayer;
            survival = targetSurvival;
            startPosition = transform.position;
            moveSpeed = type == EncounterType.MidBoss ? .55f : 0f;
            fireInterval = 1.35f;
            fireTimer = .8f;
            HealthChanged?.Invoke(this, health, maxHealth);
        }

        private void Update()
        {
            if (encounterType == EncounterType.MidBoss)
            {
                transform.position += Vector3.back * (moveSpeed * Time.deltaTime);
                if (transform.position.z <= -4.45f)
                {
                    survival?.TakeHit();
                    transform.position = startPosition;
                    health = maxHealth;
                    HealthChanged?.Invoke(this, health, maxHealth);
                }
                return;
            }

            fireTimer -= Time.deltaTime;
            if (fireTimer <= 0f)
            {
                fireTimer = fireInterval;
                FireAtPlayer();
            }
        }

        public void TakeDamage(int amount)
        {
            if (amount <= 0 || health <= 0) return;
            health = Mathf.Max(0, health - amount);
            HealthChanged?.Invoke(this, health, maxHealth);
            if (health > 0) return;
            Defeated?.Invoke(this);
            Destroy(gameObject);
        }

        private void FireAtPlayer()
        {
            if (player == null) return;
            GameObject projectile = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            projectile.name = "Boss Projectile";
            projectile.transform.position = transform.position + Vector3.back * .8f;
            projectile.transform.localScale = Vector3.one * .34f;
            Collider projectileCollider = projectile.GetComponent<Collider>();
            projectileCollider.isTrigger = true;
            Rigidbody rigidbody = projectile.AddComponent<Rigidbody>();
            rigidbody.useGravity = false;
            rigidbody.isKinematic = true;
            BossProjectile shot = projectile.AddComponent<BossProjectile>();
            shot.Initialize(player.transform, survival);
        }
    }
}
