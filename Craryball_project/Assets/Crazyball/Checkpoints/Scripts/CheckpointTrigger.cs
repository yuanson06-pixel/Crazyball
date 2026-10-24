using System;
using UnityEngine;

namespace Crazyball.CourseKit.Checkpoints
{
    [RequireComponent(typeof(Collider))]
    public sealed class CheckpointTrigger : MonoBehaviour
    {
        [SerializeField] private CheckpointDefinition definition;

        public event Action<CheckpointDefinition> Reached;

        private bool consumed;

        private void Reset()
        {
            Collider trigger = GetComponent<Collider>();
            trigger.isTrigger = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (consumed || definition == null || !other.TryGetComponent<Player.PlayerBallController>(out _))
                return;

            consumed = true;
            Reached?.Invoke(definition);
        }
    }
}
