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
}
