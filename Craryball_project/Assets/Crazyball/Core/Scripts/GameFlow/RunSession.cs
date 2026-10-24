using System;

namespace Crazyball.CourseKit.Core
{
    public sealed class RunSession
    {
        public const int FirstStage = 1;
        public const int FinalStage = 5;

        public event Action<int> StageChanged;
        public event Action<int> RunCoinChanged;

        public int CurrentStage { get; private set; } = FirstStage;
        public int RunCoin { get; private set; }

        public void Begin(int startingCoin)
        {
            CurrentStage = FirstStage;
            RunCoin = Math.Max(0, startingCoin);
            StageChanged?.Invoke(CurrentStage);
            RunCoinChanged?.Invoke(RunCoin);
        }

        public void AddCoin(int amount)
        {
            if (amount <= 0)
                return;

            RunCoin += amount;
            RunCoinChanged?.Invoke(RunCoin);
        }

        public bool TrySpendCoin(int amount)
        {
            if (amount < 0 || RunCoin < amount)
                return false;

            RunCoin -= amount;
            RunCoinChanged?.Invoke(RunCoin);
            return true;
        }

        public bool TryAdvanceStage()
        {
            if (CurrentStage >= FinalStage)
                return false;

            CurrentStage++;
            StageChanged?.Invoke(CurrentStage);
            return true;
        }
    }
}
