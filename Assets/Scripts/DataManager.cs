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
}
