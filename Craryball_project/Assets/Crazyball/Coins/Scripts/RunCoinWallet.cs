using System;
using Crazyball.CourseKit.Core;
using UnityEngine;

namespace Crazyball.CourseKit.Coins
{
    public sealed class RunCoinWallet : MonoBehaviour, ICurrencyWallet
    {
        public event Action<int> BalanceChanged;

        public int Balance { get; private set; }

        public void ResetWallet(int startingBalance = 0)
        {
            Balance = Mathf.Max(0, startingBalance);
            BalanceChanged?.Invoke(Balance);
        }

        public bool CanSpend(int amount)
        {
            return amount >= 0 && Balance >= amount;
        }

        public bool TrySpend(int amount)
        {
            if (!CanSpend(amount))
                return false;

            Balance -= amount;
            BalanceChanged?.Invoke(Balance);
            return true;
        }

        public void Add(int amount)
        {
            if (amount <= 0)
                return;

            Balance += amount;
            BalanceChanged?.Invoke(Balance);
        }
    }
}
