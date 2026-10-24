using UnityEngine;
using Crazyball.CourseKit.Player;

namespace Crazyball.CourseKit.Obstacles
{
    public sealed class Hazard : MonoBehaviour
    {
        [SerializeField, Min(1)] private int maxHealth = 1;

        private int health;
        private bool damagedPlayer;
        private BoxCollider playerDamageTrigger;
        private readonly Collider[] overlapResults = new Collider[8];

        public int Health => health;

        private void Awake()
        {
            health = maxHealth;
            CreatePlayerDamageTrigger();
        }

        public void TakeDamage(int amount)
        {
            if (amount <= 0 || health <= 0) return;
            health -= amount;
            if (health <= 0)
                Destroy(gameObject);
        }

        private void OnCollisionEnter(Collision collision)
        {
            DamagePlayer(collision.gameObject);
        }

        private void OnTriggerEnter(Collider other)
        {
            DamagePlayer(other.gameObject);
        }

        public void DamagePlayer(GameObject target)
        {
            if (damagedPlayer) return;
            PlayerSurvival survival = target.GetComponentInParent<PlayerSurvival>();
            if (survival == null) return;
            damagedPlayer = true;
            survival.TakeHit();
            Destroy(gameObject);
        }

        private void FixedUpdate()
        {
            if (damagedPlayer || playerDamageTrigger == null) return;

            Bounds bounds = playerDamageTrigger.bounds;
            int count = Physics.OverlapBoxNonAlloc(
                bounds.center,
                bounds.extents,
                overlapResults,
                playerDamageTrigger.transform.rotation,
                ~0,
                QueryTriggerInteraction.Collide);

            for (int index = 0; index < count; index++)
            {
                Collider overlap = overlapResults[index];
                if (overlap == null || overlap.transform.IsChildOf(transform)) continue;
                PlayerSurvival survival = overlap.GetComponentInParent<PlayerSurvival>();
                if (survival == null) continue;
                DamagePlayer(survival.gameObject);
                break;
            }
        }

        private void CreatePlayerDamageTrigger()
        {
            Transform existing = transform.Find("Player Damage Trigger");
            if (existing != null)
            {
                playerDamageTrigger = existing.GetComponent<BoxCollider>();
                HazardDamageTrigger existingTrigger = existing.GetComponent<HazardDamageTrigger>();
                existingTrigger?.Initialize(this);
                return;
            }

            BoxCollider source = GetComponent<BoxCollider>();
            if (source == null) return;

            GameObject triggerObject = new GameObject("Player Damage Trigger");
            triggerObject.transform.SetParent(transform, false);
            triggerObject.layer = gameObject.layer;

            BoxCollider trigger = triggerObject.AddComponent<BoxCollider>();
            trigger.isTrigger = true;
            trigger.center = source.center;
            trigger.size = source.size;
            playerDamageTrigger = trigger;

            HazardDamageTrigger damageTrigger = triggerObject.AddComponent<HazardDamageTrigger>();
            damageTrigger.Initialize(this);
        }
    }
}
