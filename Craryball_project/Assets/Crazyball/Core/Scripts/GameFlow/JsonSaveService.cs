using System;
using System.IO;
using UnityEngine;

namespace Crazyball.CourseKit.Core
{
    public sealed class JsonSaveService : ISaveService
    {
        private const string FileName = "crazyball-save.json";

        private string SavePath => Path.Combine(Application.persistentDataPath, FileName);

        public PersistentGameData Load()
        {
            try
            {
                if (!File.Exists(SavePath))
                    return new PersistentGameData();

                string json = File.ReadAllText(SavePath);
                PersistentGameData data = JsonUtility.FromJson<PersistentGameData>(json);
                return data ?? new PersistentGameData();
            }
            catch (Exception exception)
            {
                Debug.LogWarning($"Could not load Crazyball save data. A new save will be used.\n{exception.Message}");
                return new PersistentGameData();
            }
        }

        public void Save(PersistentGameData data)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            string json = JsonUtility.ToJson(data, true);
            File.WriteAllText(SavePath, json);
        }
    }
}
