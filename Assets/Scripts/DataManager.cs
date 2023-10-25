using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using SFB;

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

    public string DronesToJson(SubSwarm subSwarm)
    {
        return jsonConverter.DronesToJson(subSwarm);
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

    public void SaveAllDroneDataToCSV(List<Drone> allDrones)
    {
        Debug.Log("SaveAllDroneDataToCSV");
        // Define the directory to save all CSV files for this simulation session
        string directoryName = "simulation_" + DateTime.Now.ToString("yyyyMMddHHmmss");
        string directoryPath = Path.Combine(
            Application.streamingAssetsPath,
            "saved simulations",
            directoryName
        );
        // Check if the directory exists, if not, create it.
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }
        // Loop through each drone and save its data to an individual CSV file
        foreach (Drone drone in allDrones)
        {
            // Get the data collection from the drone
            List<DroneData> dataCollection = drone.DataCollection;
            // Initialize CSV string with Header row
            string csvData = "TimeString,X,Y,Z,Speed,BatteryStatus,Node,Edge\n";
            // Append each data row to the CSV string
            foreach (var data in dataCollection)
            {
                csvData += data.ToCSV() + "\n";
            }
            // Define the file name and path using the drone's GameObject name
            string fileName = drone.gameObject.name + ".csv";
            string filePath = Path.Combine(directoryPath, fileName);
            // Write the CSV data to the file
            File.WriteAllText(filePath, csvData);
        }
    }

    public String FetchSkywayJsonFromFile()
    {
        string[] paths = StandaloneFileBrowser.OpenFilePanel(
            "Select a skyway JSON",
            Application.streamingAssetsPath + "/saved skyways",
            "json",
            false
        );
        if (paths.Length == 0)
        { // User delete file selection
            return "";
        }
        string path = paths[0];
        try
        {
            string skywayJson = File.ReadAllText(path);
            return skywayJson;
        }
        catch (Exception ex)
        {
            Debug.LogError("Error loading skyway from JSON: " + ex.Message);
        }
        return "";
    }

    public Skyway LoadSkywayFromJson(String skywayJson)
    {
        return jsonConverter.JsonToSkyway(skywayJson);
    }
}
