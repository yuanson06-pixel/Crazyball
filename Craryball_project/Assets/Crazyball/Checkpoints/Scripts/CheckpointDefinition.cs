using UnityEngine;

namespace Crazyball.CourseKit.Checkpoints
{
    [CreateAssetMenu(fileName = "SO_Checkpoint_01", menuName = "Crazyball/Checkpoint Definition")]
    public sealed class CheckpointDefinition : ScriptableObject
    {
        [SerializeField, Min(1)] private int checkpointId = 1;
        [SerializeField, Range(1, 5)] private int stageIndex = 1;
        [SerializeField, Min(0)] private int runCoinReward = 3;
        [SerializeField, Min(0)] private int firstClearCoreReward = 1;

        public int CheckpointId => checkpointId;
        public int StageIndex => stageIndex;
        public int RunCoinReward => runCoinReward;
        public int FirstClearCoreReward => firstClearCoreReward;

        private void OnValidate()
        {
            checkpointId = Mathf.Max(1, checkpointId);
            stageIndex = Mathf.Clamp(stageIndex, 1, 5);
            runCoinReward = Mathf.Max(0, runCoinReward);
            firstClearCoreReward = Mathf.Max(0, firstClearCoreReward);
        }
    }
}
