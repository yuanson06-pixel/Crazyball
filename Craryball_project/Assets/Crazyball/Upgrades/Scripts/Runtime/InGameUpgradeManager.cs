using System;
using System.Collections.Generic;
using Crazyball.CourseKit.Coins;
using UnityEngine;

namespace Crazyball.CourseKit.Upgrades
{
    public sealed class InGameUpgradeManager : MonoBehaviour
    {
        [SerializeField] private RunCoinWallet wallet;
        [SerializeField] private UpgradeDefinition[] catalog;

        private readonly UpgradeState runState = new UpgradeState();
        private UpgradeOfferService offerService;
        private IReadOnlyList<UpgradeDefinition> currentOffer = Array.Empty<UpgradeDefinition>();
        private bool choiceResolved;

        public event Action<IReadOnlyList<UpgradeDefinition>> OfferCreated;
        public event Action RunStarted;
        public event Action<UpgradeDefinition, int> UpgradePurchased;
        public event Action OfferSkipped;

        public IReadOnlyList<UpgradeDefinition> CurrentOffer => currentOffer;

        private void Awake()
        {
            offerService = new UpgradeOfferService();
        }

        public void BeginRun()
        {
            runState.Clear();
            currentOffer = Array.Empty<UpgradeDefinition>();
            choiceResolved = true;
            RunStarted?.Invoke();
        }

        public void OpenOffer(Func<UpgradeDefinition, bool> isUnlocked)
        {
            currentOffer = offerService.CreateOffer(catalog, runState, isUnlocked);
            choiceResolved = false;
            OfferCreated?.Invoke(currentOffer);
        }

        public bool TryPurchase(UpgradeDefinition definition)
        {
            if (choiceResolved || definition == null || !ContainsCurrentOffer(definition) || wallet == null)
                return false;

            int currentLevel = runState.GetLevel(definition);
            int cost = definition.GetCostForLevel(currentLevel);
            if (!wallet.TrySpend(cost) || !runState.TryLevelUp(definition))
                return false;

            choiceResolved = true;
            UpgradePurchased?.Invoke(definition, currentLevel + 1);
            return true;
        }

        public void Skip()
        {
            if (choiceResolved)
                return;

            choiceResolved = true;
            OfferSkipped?.Invoke();
        }

        public int GetLevel(UpgradeDefinition definition) => runState.GetLevel(definition);

        private bool ContainsCurrentOffer(UpgradeDefinition definition)
        {
            for (int index = 0; index < currentOffer.Count; index++)
            {
                if (currentOffer[index] == definition)
                    return true;
            }

            return false;
        }
    }
}
