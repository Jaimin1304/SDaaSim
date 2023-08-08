using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Globals : MonoBehaviour
{
    public const string ip = "127.0.0.1";
    public const int port = 1235;
    public const string quitSignal = "exit";
    public const string endOfFileSignal = "|";
    public const string initSkywayHeader = "initSkyway";
    public const string updateSwarmHeader = "updateSwarm";
    public const string updateSubSwarmHeader = "updateSubSwarm";
    public const string setSubswarmEdge = "set_subswarm_edge";
    public const float camMovSpeed = 160;
    public const float nodeTouchDistance = 1;
    public const float camRotateSpeed = 0.2f;
    public const float droneGapView = 2f;
    public const float droneHeightOffset = 3f;
}
