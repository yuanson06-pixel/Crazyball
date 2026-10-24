using System;

namespace Crazyball.CourseKit.Core
{
    public interface ICurrencyWallet
    {
        event Action<int> BalanceChanged;

        int Balance { get; }
        bool CanSpend(int amount);
        bool TrySpend(int amount);
        void Add(int amount);
    }
}
