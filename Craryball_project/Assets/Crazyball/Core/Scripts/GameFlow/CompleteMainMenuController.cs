using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Crazyball.CourseKit.Core
{
    public sealed class CompleteMainMenuController : MonoBehaviour
    {
        [SerializeField] private TMP_Text progressText;
        [SerializeField] private string courseSceneName = "Crazyball_Course";

        private void Start()
        {
            PersistentGameData data = new JsonSaveService().Load();
            if (progressText != null)
                progressText.text = $"UPGRADE CORES  {data.upgradeCore}\nBEST CHECKPOINT  {data.highestCheckpoint} / 5";
        }

        public void Play()
        {
            SceneManager.LoadScene(courseSceneName);
        }

        public void Quit()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}
