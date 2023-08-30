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
    public const string updateDronesHeader = "updateDrones";
    public const string setSubswarmEdge = "set_subswarm_edge";
    public const string subswarmLand = "subswarm_land";
    public const string splitSubswarm = "split_subswarm";
    public const string mergeTwoSubswarms = "merge_two_subswarms";
    public const string proceed = "proceed";
    public const float camMovSpeed = 80;
    public const float camSprintSpeed = 300;
    public const float camRotateSpeed = 0.2f;
    public const float camZoomSpeed = 200;
    public const float camMaxZoomDistance = 200;
    public const float camMinZoomDistance = 3;
    public const float nodeTouchDistance = 1;
    public const float droneGapView = 2f;
    public const float droneHeightOffset = 3f;
    public const int playSpeedLimit = 32;

    static int playSpeed = 1;
    public static int PlaySpeed
    {
        get { return playSpeed; }
        set { playSpeed = value; }
    }
}
