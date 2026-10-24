namespace Crazyball.CourseKit.Upgrades
{
    public interface IUpgradeEffectReceiver
    {
        UpgradeEffectType EffectType { get; }
        void Apply(float value, int level);
    }
}
