using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Text;
using Newtonsoft.Json;

[Serializable]
public class SerializableSkyway
{
    public List<SerializableNode> nodes;
    public List<SerializableEdge> edges;
    public List<SerializableRequest> requests;
    public List<SerializableSwarm> swarms;
}

[Serializable]
public class SerializableNode
{
    public string id;
    public Vector3 position;
    public List<string> pads;
    public List<string> drones;
    public List<string> edges;
}

[Serializable]
public class SerializableEdge
{
    public string id;
    public string leftNode;
    public string rightNode;
}

[Serializable]
public class SerializablePad
{
    public string id;
    public string node;
    public bool isAvailable;
}

[Serializable]
public class SerializableRequest
{
    public string id;
    public string startNode;
    public string destNode;
    public List<SerializablePayload> payloads;
}

[Serializable]
public class SerializablePayload
{
    public string id;
    public float weight;
}

[Serializable]
public class SerializableSwarm
{
    public string id;
    public string request;
    public List<SerializableSubSwarm> subSwarms;
}

[Serializable]
public class SerializableSubSwarm
{
    public string id;
    public Vector3 position;
    public List<SerializableDrone> drones;
    public string targetNode;
}

[Serializable]
public class SerializableDrone
{
    public string id;
    public float selfWeight;
    public float speed;
    public float maxPayloadWeight;
    public float batteryStatus;
    public List<string> payloads;
}

public class JsonConverter : MonoBehaviour
{
    public string RecordCurrentStateToJson(Skyway skyway)
    {
        SerializableSkyway currState = skyway.ToSerializableSkyway();
        string skywayJson = JsonUtility.ToJson(currState);
        return skywayJson;
    }
}
