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
    public const float camRotateSpeed = 0.15f;
    public const float camZoomSpeed = 200;
    public const float camMaxZoomDistance = 150;
    public const float camMaxPitch = 80;
    public const float camMinZoomDistance = 6;
    public const float camDefaultZoomDistance = 50;
    public const float nodeTouchDistance = 1;
    public const float droneGapView = 2f;
    public const float droneHeightOffset = 3f;
    public const float textScaleValue = 0.013f;
    public const float arrow3DScaleValue = 0.02f;
    public const float doubleClickGap = 0.25f;
    public const float objectCreationDistance = 15f;
    public const float editModeDragMultiplier = 0.2f;
    public const float edgeLineWidth = 0.7f;
    public const int playSpeedLimit = 32;

    static int playSpeed = 1;
    public static int PlaySpeed
    {
        get { return playSpeed; }
        set { playSpeed = value; }
    }
}
