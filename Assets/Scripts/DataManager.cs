using UnityEngine;
using System;
using System.IO;

public class DataManager : MonoBehaviour
{
    [SerializeField]
    JsonConverter jsonConverter;

    public string RecordCurrentStateToJson(Skyway skyway)
    {
        return jsonConverter.RecordCurrentStateToJson(skyway);
    }

    public string SwarmToJson(Swarm swarm)
    {
        return jsonConverter.SwarmToJson(swarm);
    }

    public string SubSwarmToJson(SubSwarm subSwarm)
    {
        return jsonConverter.SubSwarmToJson(subSwarm);
    }

    public Skyway LoadSkywayFromJson()
    {
        Skyway skyway = null;
        return skyway;
    }

    public bool SaveSkywayToJson(Skyway skyway)
    {
        try
        {
            string skywayJson = RecordCurrentStateToJson(skyway);
            string fileName =
                "skyway_" + string.Format("{0}.json", DateTime.Now.ToString("yyyyMMddHHmmss"));
            string path = Path.Combine(Application.streamingAssetsPath, "saved skyways", fileName);
            // Check if the directory exists, if not, create it.
            string directory = Path.GetDirectoryName(path);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            Debug.Log(path);
            File.WriteAllText(path, skywayJson);
            return true;
        }
        catch (Exception ex)
        {
            Debug.LogError("Error saving skyway to JSON: " + ex.Message);
            return false;
        }
    }
}
