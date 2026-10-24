using Crazyball.CourseKit.Player;
using UnityEngine;

namespace Crazyball.CourseKit.Obstacles
{
    public sealed class BossProjectile : MonoBehaviour
    {
        private PlayerSurvival survival;
        private Vector3 direction;
        private float speed = 6.5f;

        public void Initialize(Transform target, PlayerSurvival targetSurvival)
        {
            survival = targetSurvival;
            direction = (target.position - transform.position).normalized;
            direction.y = 0f;
        }

        private void Update()
        {
            transform.position += direction * (speed * Time.deltaTime);
            if (transform.position.z < -8f || Mathf.Abs(transform.position.x) > 6f)
                Destroy(gameObject);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.TryGetComponent(out PlayerBallController _)) return;
            survival?.TakeHit();
            Destroy(gameObject);
        }
    }
}
