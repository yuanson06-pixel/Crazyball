namespace Crazyball.CourseKit.Core
{
    public interface ISaveService
    {
        PersistentGameData Load();
        void Save(PersistentGameData data);
    }
}
