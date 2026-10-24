using System.Collections.Generic;
using Crazyball.CourseKit.Coins;
using Crazyball.CourseKit.Upgrades;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Crazyball.CourseKit.UI
{
    public sealed class CheckpointUpgradePanel : MonoBehaviour
    {
        [SerializeField] private GameObject panel;
        [SerializeField] private Button[] choiceButtons;
        [SerializeField] private Image[] choiceIcons;
        [SerializeField] private TMP_Text[] choiceLabels;
        [SerializeField] private Button skipButton;

        private InGameUpgradeManager manager;

        private void Awake()
        {
            skipButton.onClick.AddListener(Skip);
        }

        public void Show(IReadOnlyList<UpgradeDefinition> offer, InGameUpgradeManager upgradeManager, RunCoinWallet wallet)
        {
            manager = upgradeManager;
            panel.SetActive(true);

            for (int index = 0; index < choiceButtons.Length; index++)
            {
                Button button = choiceButtons[index];
                button.onClick.RemoveAllListeners();

                if (index >= offer.Count)
                {
                    button.gameObject.SetActive(false);
                    continue;
                }

                UpgradeDefinition definition = offer[index];
                int cost = definition.GetCostForLevel(manager.GetLevel(definition));
                button.gameObject.SetActive(true);
                button.interactable = wallet.CanSpend(cost);
                if (choiceIcons != null && index < choiceIcons.Length && choiceIcons[index] != null)
                {
                    Image icon = choiceIcons[index];
                    icon.sprite = definition.Icon;
                    icon.enabled = definition.Icon != null;
                    icon.preserveAspect = true;
                }
                choiceLabels[index].text = $"{definition.DisplayName}\n{definition.Description}\n{cost} Coin";
                button.onClick.AddListener(() => manager.TryPurchase(definition));
            }
        }

        public void Hide()
        {
            panel.SetActive(false);
        }

        private void Skip()
        {
            manager?.Skip();
        }
    }
}
