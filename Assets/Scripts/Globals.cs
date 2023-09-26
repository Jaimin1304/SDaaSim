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
    public const int playSpeedLimit = 32;
    public static Vector3 windDirection = Vector3.left;

    static int playSpeed = 1;
    public static int PlaySpeed
    {
        get { return playSpeed; }
        set { playSpeed = value; }
    }

    // Define keys for PlayerPrefs
    private const string CamMovSpeedKey = "CamMovSpeed";
    private const string CamSprintSpeedKey = "CamSprintSpeed";
    private const string CamRotateSpeedKey = "CamRotateSpeed";
    private const string CamZoomSpeedKey = "CamZoomSpeed";
    private const string EdgeThicknessKey = "EdgeThickness";
    private const string DroneBatteryJouleKey = "DroneBatteryJoule";
    private const string WindSpeedKey = "WindSpeed";
    private const string BatChargingEfficKey = "BatChargingEffic";

    // Define default values
    private const float DefaultCamMovSpeed = 80;
    private const float DefaultCamSprintSpeed = 300;
    private const float DefaultCamRotateSpeed = 0.15f;
    private const float DefaultCamZoomSpeed = 200;
    private const float DefaultEdgeThickness = 0.7f;
    private const float DefaultDroneBatteryJoule = 200000;
    private const float DefaultWindSpeed = 5;
    private const float DefaultBatChargingEffic = 0.85f;

    // Properties with PlayerPrefs saving in setters
    public static float CamMovSpeed
    {
        get => PlayerPrefs.GetFloat(CamMovSpeedKey, DefaultCamMovSpeed);
        set => PlayerPrefs.SetFloat(CamMovSpeedKey, value);
    }

    public static float CamSprintSpeed
    {
        get => PlayerPrefs.GetFloat(CamSprintSpeedKey, DefaultCamSprintSpeed);
        set => PlayerPrefs.SetFloat(CamSprintSpeedKey, value);
    }

    public static float CamRotateSpeed
    {
        get => PlayerPrefs.GetFloat(CamRotateSpeedKey, DefaultCamRotateSpeed);
        set => PlayerPrefs.SetFloat(CamRotateSpeedKey, value);
    }

    public static float CamZoomSpeed
    {
        get => PlayerPrefs.GetFloat(CamZoomSpeedKey, DefaultCamZoomSpeed);
        set => PlayerPrefs.SetFloat(CamZoomSpeedKey, value);
    }

    public static float EdgeThickness
    {
        get => PlayerPrefs.GetFloat(EdgeThicknessKey, DefaultEdgeThickness);
        set => PlayerPrefs.SetFloat(EdgeThicknessKey, value);
    }

    public static float DroneBatteryJoule
    {
        get => PlayerPrefs.GetFloat(DroneBatteryJouleKey, DefaultDroneBatteryJoule);
        set => PlayerPrefs.SetFloat(DroneBatteryJouleKey, value);
    }

    public static float WindSpeed
    {
        get => PlayerPrefs.GetFloat(WindSpeedKey, DefaultWindSpeed);
        set => PlayerPrefs.SetFloat(WindSpeedKey, value);
    }

    public static float BatteryChargingEffic
    {
        get => PlayerPrefs.GetFloat(BatChargingEfficKey, DefaultBatChargingEffic);
        set => PlayerPrefs.SetFloat(BatChargingEfficKey, value);
    }
}
