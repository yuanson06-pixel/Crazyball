using System;
using System.Collections.Generic;

namespace Crazyball.CourseKit.Upgrades
{
    public sealed class UpgradeState
    {
        private readonly Dictionary<string, int> levels = new Dictionary<string, int>();

        public event Action<UpgradeDefinition, int> LevelChanged;

        public int GetLevel(UpgradeDefinition definition)
        {
            if (definition == null || string.IsNullOrWhiteSpace(definition.UpgradeId))
                return 0;

            return levels.TryGetValue(definition.UpgradeId, out int level) ? level : 0;
        }

        public bool IsMaxLevel(UpgradeDefinition definition)
        {
            return definition != null && GetLevel(definition) >= definition.MaxLevel;
        }

        public bool TryLevelUp(UpgradeDefinition definition)
        {
            if (definition == null || IsMaxLevel(definition))
                return false;

            int nextLevel = GetLevel(definition) + 1;
            levels[definition.UpgradeId] = nextLevel;
            LevelChanged?.Invoke(definition, nextLevel);
            return true;
        }

        public void Clear()
        {
            levels.Clear();
        }
    }
}
