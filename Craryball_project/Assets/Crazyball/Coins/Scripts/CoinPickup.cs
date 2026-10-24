using UnityEngine;

namespace Crazyball.CourseKit.Coins
{
    [RequireComponent(typeof(Collider))]
    public sealed class CoinPickup : MonoBehaviour
    {
        [SerializeField, Min(1)] private int value = 1;
        [SerializeField] private AudioClip pickupSound;

        private bool collected;
        private Transform magnetTarget;
        private float magnetRadius;
        private float magnetSpeed;

        public void SetMagnet(Transform target, float radius, float speed = 8f)
        {
            magnetTarget = target;
            magnetRadius = Mathf.Max(0f, radius);
            magnetSpeed = Mathf.Max(0f, speed);
        }

        private void Update()
        {
            if (magnetTarget == null || magnetRadius <= 0f)
                return;

            if ((magnetTarget.position - transform.position).sqrMagnitude <= magnetRadius * magnetRadius)
                transform.position = Vector3.MoveTowards(transform.position, magnetTarget.position, magnetSpeed * Time.deltaTime);
        }

        private void Reset()
        {
            Collider pickupCollider = GetComponent<Collider>();
            pickupCollider.isTrigger = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (collected || !other.TryGetComponent(out RunCoinCollector collector))
                return;

            collected = true;
            collector.Collect(value);

            if (pickupSound != null)
                AudioSource.PlayClipAtPoint(pickupSound, transform.position);

            Destroy(gameObject);
        }
    }
}
