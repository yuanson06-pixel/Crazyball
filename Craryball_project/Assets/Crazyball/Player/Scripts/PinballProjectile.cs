using Crazyball.CourseKit.Obstacles;
using Crazyball.CourseKit.Upgrades;
using UnityEngine;

namespace Crazyball.CourseKit.Player
{
    [RequireComponent(typeof(Rigidbody), typeof(SphereCollider))]
    public sealed class PinballProjectile : MonoBehaviour
    {
        [SerializeField] private PlayerBallController player;
        [SerializeField] private PlayerSurvival survival;
        [SerializeField, Min(1f)] private float launchSpeed = 11f;
        [SerializeField, Min(0f)] private float catchRadius = 1.25f;
        [SerializeField] private float failZ = -7.5f;
        [SerializeField] private Vector3 heldOffset = new Vector3(0f, 0f, .8f);
        [SerializeField, Min(1)] private int damage = 1;
        [SerializeField, Min(0f)] private float catchGraceTime = .2f;
        [SerializeField] private Vector2 horizontalBounds = new Vector2(-3.25f, 3.25f);
        [SerializeField] private float topBoundaryZ = 10.45f;

        private Rigidbody body;
        private Collider ballCollider;
        private bool held = true;
        private float launchedAt;
        private float baseLaunchSpeed;
        private int baseDamage;
        private float baseScale;
        private float sizeMultiplier = 1f;

        public bool IsHeld => held;

        private void Awake()
        {
            body = GetComponent<Rigidbody>();
            ballCollider = GetComponent<Collider>();
            baseLaunchSpeed = launchSpeed;
            baseDamage = damage;
            baseScale = transform.lossyScale.x;
            body.useGravity = false;
            body.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotation;
            body.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            Collider playerCollider = player != null ? player.GetComponent<Collider>() : null;
            if (ballCollider != null && playerCollider != null)
                Physics.IgnoreCollision(ballCollider, playerCollider, true);
            Catch();
        }

        private void FixedUpdate()
        {
            if (held)
            {
                SetWorldScale(baseScale * sizeMultiplier);
                return;
            }

            Vector3 velocity = body.linearVelocity;
            velocity.y = 0f;
            if (velocity.sqrMagnitude < launchSpeed * launchSpeed * .45f && velocity.sqrMagnitude > .01f)
                velocity = velocity.normalized * launchSpeed;
            body.linearVelocity = velocity;

            Vector3 position = body.position;
            if (position.x <= horizontalBounds.x && body.linearVelocity.x < 0f)
            {
                position.x = horizontalBounds.x;
                body.position = position;
                body.linearVelocity = new Vector3(Mathf.Abs(body.linearVelocity.x), 0f, body.linearVelocity.z).normalized * launchSpeed;
            }
            else if (position.x >= horizontalBounds.y && body.linearVelocity.x > 0f)
            {
                position.x = horizontalBounds.y;
                body.position = position;
                body.linearVelocity = new Vector3(-Mathf.Abs(body.linearVelocity.x), 0f, body.linearVelocity.z).normalized * launchSpeed;
            }

            if (position.z >= topBoundaryZ && body.linearVelocity.z > 0f)
            {
                position = body.position;
                position.z = topBoundaryZ;
                body.position = position;
                body.linearVelocity = new Vector3(body.linearVelocity.x, 0f, -Mathf.Abs(body.linearVelocity.z)).normalized * launchSpeed;
            }

            if (transform.position.z < failZ)
            {
                survival.TakeHit();
                Catch();
                return;
            }

            bool catchReady = Time.time >= launchedAt + catchGraceTime;
            if (catchReady && body.linearVelocity.z < 0f &&
                Vector3.Distance(transform.position, player.transform.position) <= catchRadius)
                Catch();
        }

        public void Launch()
        {
            if (!held) return;
            held = false;
            transform.SetParent(null, true);
            body.isKinematic = false;
            launchedAt = Time.time;
            float english = Mathf.Clamp(player.HorizontalVelocity * .08f, -.55f, .55f);
            body.linearVelocity = new Vector3(english, 0f, 1f).normalized * launchSpeed;
        }

        public void ApplyUpgrade(UpgradeEffectType effectType, float value)
        {
            switch (effectType)
            {
                case UpgradeEffectType.PinballDamage:
                    damage = Mathf.Max(baseDamage, Mathf.RoundToInt(value));
                    break;
                case UpgradeEffectType.PinballSpeed:
                    launchSpeed = Mathf.Max(baseLaunchSpeed, value);
                    if (!held && body.linearVelocity.sqrMagnitude > .01f)
                        body.linearVelocity = body.linearVelocity.normalized * launchSpeed;
                    break;
                case UpgradeEffectType.PinballSize:
                    sizeMultiplier = Mathf.Max(1f, value);
                    SetWorldScale(baseScale * sizeMultiplier);
                    break;
            }
        }

        public void ResetUpgrades()
        {
            launchSpeed = baseLaunchSpeed;
            damage = baseDamage;
            sizeMultiplier = 1f;
            SetWorldScale(baseScale);
            Catch();
        }

        public void Catch()
        {
            held = true;
            body.isKinematic = true;
            body.linearVelocity = Vector3.zero;
            body.angularVelocity = Vector3.zero;
            if (player == null) return;

            transform.SetParent(null, true);
            transform.position = player.transform.position + heldOffset;
            transform.SetParent(player.transform, true);
            transform.localRotation = Quaternion.identity;
            SetWorldScale(baseScale * sizeMultiplier);
            body.position = transform.position;
        }

        private void SetWorldScale(float scale)
        {
            if (transform.parent == null)
            {
                transform.localScale = Vector3.one * scale;
                return;
            }

            Vector3 parentScale = transform.parent.lossyScale;
            transform.localScale = new Vector3(
                scale / Mathf.Max(.001f, parentScale.x),
                scale / Mathf.Max(.001f, parentScale.y),
                scale / Mathf.Max(.001f, parentScale.z));
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (held) return;
            if (collision.gameObject.TryGetComponent(out Hazard hazard))
            {
                hazard.TakeDamage(damage);
                float offset = transform.position.x - collision.transform.position.x;
                float english = player.HorizontalVelocity * .08f;
                body.linearVelocity = new Vector3(Mathf.Clamp(offset + english, -.9f, .9f), 0f, -1f).normalized * launchSpeed;
                return;
            }

            if (collision.gameObject.TryGetComponent(out EncounterTarget encounter))
            {
                encounter.TakeDamage(damage);
                float offset = transform.position.x - collision.transform.position.x;
                float english = player.HorizontalVelocity * .08f;
                body.linearVelocity = new Vector3(Mathf.Clamp(offset + english, -.9f, .9f), 0f, -1f).normalized * launchSpeed;
                return;
            }

            ContactPoint contact = collision.GetContact(0);
            Vector3 velocity = body.linearVelocity;
            float movingIntoSurface = Vector3.Dot(velocity, contact.normal);

            // FixedUpdate의 경계 보정 또는 PhysX가 이미 반사한 경우 다시 반사하지 않는다.
            if (movingIntoSurface < -.01f)
                body.linearVelocity = Vector3.Reflect(velocity, contact.normal).normalized * launchSpeed;
        }
    }
}
