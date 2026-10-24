using UnityEngine;

namespace Crazyball.CourseKit.Obstacles
{
    public sealed class ScrollingObject : MonoBehaviour
    {
        private float speed;
        private float destroyZ;

        public void Initialize(float movementSpeed, float destructionThreshold = -9f)
        {
            speed = movementSpeed;
            destroyZ = destructionThreshold;
        }

        private void Update()
        {
            transform.position += Vector3.back * (speed * Time.deltaTime);
            if (transform.position.z < destroyZ)
                Destroy(gameObject);
        }
    }
}
