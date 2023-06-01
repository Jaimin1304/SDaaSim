using UnityEngine;
using System;
using System.IO;

public class DataManager : MonoBehaviour
{
    [Serializable]
    public class LevelData
    {
        public Vector3 playerPosition;
        public Vector3[] floorPositions;

        public static LevelData GetLevelDataFromJsonString(string jsonString)
        {
            return JsonUtility.FromJson<LevelData>(jsonString);
        }

        public string GetJsonString()
        {
            return JsonUtility.ToJson(this);
        }
    }

    public static void SaveJson(string json)
    {
        string fileName = String.Format("{0}.json", DateTime.Now.ToString("yyyyMMddHHmmssffff"));
        string filePath = Path.Combine(Application.streamingAssetsPath, fileName);
        File.WriteAllText(filePath, json);
    }

    public static LevelData LoadJson(string fileName)
    {
        string jsonString = File.ReadAllText(
            String.Format(Application.streamingAssetsPath + "/{0}", fileName)
        );
        return LevelData.GetLevelDataFromJsonString(jsonString);
    }
}
