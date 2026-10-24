using Crazyball.CourseKit.Coins;
using Crazyball.CourseKit.Core;

namespace Crazyball.CourseKit.Checkpoints
{
    public sealed class CheckpointRewardService
    {
        private readonly RunCoinWallet runWallet;
        private readonly PersistentGameData persistentData;
        private readonly ISaveService saveService;

        public CheckpointRewardService(
            RunCoinWallet runWallet,
            PersistentGameData persistentData,
            ISaveService saveService)
        {
            this.runWallet = runWallet;
            this.persistentData = persistentData;
            this.saveService = saveService;
        }

        public bool Grant(CheckpointDefinition checkpoint)
        {
            runWallet.Add(checkpoint.RunCoinReward);

            if (persistentData.HasClearedCheckpoint(checkpoint.CheckpointId))
                return false;

            persistentData.MarkCheckpointCleared(checkpoint.CheckpointId);
            persistentData.upgradeCore += checkpoint.FirstClearCoreReward;
            saveService.Save(persistentData);
            return true;
        }
    }
}
