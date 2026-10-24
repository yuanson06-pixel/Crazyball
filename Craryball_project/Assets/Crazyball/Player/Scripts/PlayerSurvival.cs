using System;
using Crazyball.CourseKit.Obstacles;
using Crazyball.CourseKit.Upgrades;
using UnityEngine;

namespace Crazyball.CourseKit.Player
{
    public sealed class PlayerSurvival : MonoBehaviour, IUpgradeEffectReceiver
    {
        [SerializeField, Min(1)] private int maxHealth = 3;
        [SerializeField] private int shieldCharges;

        public event Action Defeated;
        public event Action<int, int, int> HealthChanged;

        public UpgradeEffectType EffectType => UpgradeEffectType.Shield;
        public int CurrentHealth { get; private set; }
        public int MaxHealth => maxHealth;
        public int ShieldCharges => shieldCharges;

        public void Apply(float value, int level)
        {
            shieldCharges += Mathf.Max(1, Mathf.RoundToInt(value));
            NotifyChanged();
        }

        public void ResetForRun(int startingShield)
        {
            CurrentHealth = maxHealth;
            shieldCharges = Mathf.Max(0, startingShield);
            NotifyChanged();
        }

        public void TakeHit(int damage = 1)
        {
            if (damage <= 0) return;
            if (shieldCharges > 0)
                shieldCharges--;
            else
                CurrentHealth = Mathf.Max(0, CurrentHealth - damage);

            NotifyChanged();
            if (CurrentHealth <= 0)
                Defeated?.Invoke();
        }

        private void NotifyChanged()
        {
            HealthChanged?.Invoke(CurrentHealth, maxHealth, shieldCharges);
        }
    }
}
