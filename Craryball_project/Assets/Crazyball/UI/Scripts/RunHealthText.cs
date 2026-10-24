using Crazyball.CourseKit.Player;
using TMPro;
using UnityEngine;

namespace Crazyball.CourseKit.UI
{
    public sealed class RunHealthText : MonoBehaviour
    {
        [SerializeField] private PlayerSurvival survival;
        [SerializeField] private TMP_Text label;

        private void OnEnable()
        {
            if (survival == null) return;
            survival.HealthChanged += Refresh;
            Refresh(survival.CurrentHealth, survival.MaxHealth, survival.ShieldCharges);
        }

        private void OnDisable()
        {
            if (survival != null) survival.HealthChanged -= Refresh;
        }

        private void Refresh(int health, int maxHealth, int shield)
        {
            if (label != null)
                label.text = $"HP  {health} / {maxHealth}    SHIELD  {shield}";
        }
    }
}
