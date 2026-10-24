using UnityEngine;
using Crazyball.CourseKit.Player;

namespace Crazyball.CourseKit.Obstacles
{
    [RequireComponent(typeof(Collider))]
    public sealed class HazardDamageTrigger : MonoBehaviour
    {
        private Hazard owner;

        public void Initialize(Hazard hazard)
        {
            owner = hazard;
        }

        private void OnTriggerEnter(Collider other)
        {
            PlayerSurvival survival = other.GetComponentInParent<PlayerSurvival>();
            if (survival != null)
                owner?.DamagePlayer(survival.gameObject);
        }
    }
}
