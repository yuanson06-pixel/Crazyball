using System;

namespace Crazyball.CourseKit.Core
{
    public sealed class GameSignals
    {
        public event Action<int> StageStarted;
        public event Action<int> CheckpointReached;
        public event Action<GameState> StateChanged;

        public void RaiseStageStarted(int stageIndex) => StageStarted?.Invoke(stageIndex);
        public void RaiseCheckpointReached(int checkpointId) => CheckpointReached?.Invoke(checkpointId);
        public void RaiseStateChanged(GameState state) => StateChanged?.Invoke(state);
    }
}
