using System;
using System.Collections.Generic;

namespace Crazyball.CourseKit.Core
{
    [Serializable]
    public sealed class PersistentGameData
    {
        public const int CurrentVersion = 1;

        public int version = CurrentVersion;
        public int bestScore;
        public int highestCheckpoint;
        public int upgradeCore;
        public List<int> clearedCheckpointIds = new List<int>();
        public List<UpgradeLevelRecord> persistentUpgrades = new List<UpgradeLevelRecord>();

        public bool HasClearedCheckpoint(int checkpointId)
        {
            return clearedCheckpointIds.Contains(checkpointId);
        }

        public void MarkCheckpointCleared(int checkpointId)
        {
            if (!clearedCheckpointIds.Contains(checkpointId))
                clearedCheckpointIds.Add(checkpointId);

            if (checkpointId > highestCheckpoint)
                highestCheckpoint = checkpointId;
        }
    }

    [Serializable]
    public sealed class UpgradeLevelRecord
    {
        public string upgradeId;
        public int level;
    }
}
