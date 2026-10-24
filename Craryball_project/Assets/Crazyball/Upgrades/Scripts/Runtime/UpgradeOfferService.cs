using System;
using System.Collections.Generic;

namespace Crazyball.CourseKit.Upgrades
{
    public sealed class UpgradeOfferService
    {
        public const int MaximumOfferCount = 3;

        private readonly Random random;

        public UpgradeOfferService(int? seed = null)
        {
            random = seed.HasValue ? new Random(seed.Value) : new Random();
        }

        public IReadOnlyList<UpgradeDefinition> CreateOffer(
            IReadOnlyList<UpgradeDefinition> catalog,
            UpgradeState runState,
            Func<UpgradeDefinition, bool> isUnlocked)
        {
            var candidates = new List<UpgradeDefinition>();

            if (catalog == null)
                return candidates;

            for (int index = 0; index < catalog.Count; index++)
            {
                UpgradeDefinition definition = catalog[index];
                if (definition == null ||
                    definition.Category != UpgradeCategory.InGame ||
                    runState.IsMaxLevel(definition) ||
                    (isUnlocked != null && !isUnlocked(definition)))
                    continue;

                candidates.Add(definition);
            }

            for (int index = candidates.Count - 1; index > 0; index--)
            {
                int swapIndex = random.Next(index + 1);
                (candidates[index], candidates[swapIndex]) = (candidates[swapIndex], candidates[index]);
            }

            if (candidates.Count > MaximumOfferCount)
                candidates.RemoveRange(MaximumOfferCount, candidates.Count - MaximumOfferCount);

            return candidates;
        }
    }
}
