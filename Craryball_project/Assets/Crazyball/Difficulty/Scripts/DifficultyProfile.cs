using UnityEngine;

namespace Crazyball.CourseKit.Difficulty
{
    [CreateAssetMenu(fileName = "SO_Difficulty_Stage01", menuName = "Crazyball/Difficulty Profile")]
    public sealed class DifficultyProfile : ScriptableObject
    {
        [SerializeField, Range(1, 5)] private int stageIndex = 1;
        [SerializeField, Min(0.1f)] private float scrollSpeed = 1.2f;
        [SerializeField, Min(0.1f)] private float spawnInterval = 1f;
        [SerializeField, Min(0)] private int movingObstacleWeight;
        [SerializeField, Min(0)] private int enemyUnitWeight;

        public int StageIndex => stageIndex;
        public float ScrollSpeed => scrollSpeed;
        public float SpawnInterval => spawnInterval;
        public int MovingObstacleWeight => movingObstacleWeight;
        public int EnemyUnitWeight => enemyUnitWeight;
    }
}
