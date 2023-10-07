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
    public const float padGap = 4f;
    public const float edgeHeightOffset = 2f;
    public const float edgeNodeGap = 5f;

    //public static Vector3 windDirection = Vector3.left;

    static int playSpeed = 1;
    public static int PlaySpeed
    {
        get { return playSpeed; }
        set { playSpeed = value; }
    }

    // Define keys for PlayerPrefs
    const string CamMovSpeedKey = "CamMovSpeed";
    const string CamSprintSpeedKey = "CamSprintSpeed";
    const string CamRotateSpeedKey = "CamRotateSpeed";
    const string CamZoomSpeedKey = "CamZoomSpeed";
    const string EdgeThicknessKey = "EdgeThickness";
    const string DroneBatCapKey = "DroneBatCap"; // Drone battery capacity in Wh
    const string WindSpeedKey = "WindSpeed";

    // Wind direction vector3
    static string WindDirectionXKey = "WindDirectionX";
    static string WindDirectionYKey = "WindDirectionY";
    static string WindDirectionZKey = "WindDirectionZ";

    const string BatChargingEfficKey = "BatChargingEffic"; // Battery charging efficiency
    const string RotorNumKey = "RotorNum"; // Number of rotors
    const string AirDensityKey = "AirDensity"; // Air density
    const string DownwashCoeffKey = "DownwashCoeff"; // Downwash coefficient
    const string PwrXferEfficKey = "PwrXferEffic"; // Power transfer efficiency from battery to propeller
    const string AvionicsPwrKey = "AvionicsPwr"; // Avionics power, power consumption of electronic equipmentF

    // Define default values
    const float DefaultCamMovSpeed = 80;
    const float DefaultCamSprintSpeed = 300;
    const float DefaultCamRotateSpeed = 0.15f;
    const float DefaultCamZoomSpeed = 200;
    const float DefaultEdgeThickness = 0.6f;
    const float DefaultDroneBatCap = 200000;
    const float DefaultWindSpeed = 5;
    static Vector3 DefaultWindDirection = Vector3.left;
    const float DefaultBatChargingEffic = 0.85f;
    const float DefaultRotorNum = 4f;
    const float DefaultAirDensity = 1.225f;
    const float DefaultDownwashCoeff = 1f;
    const float DefaultPwrXferEffic = 0.73f;
    const float DefaultAvionicsPwr = 10f;

    // Properties with PlayerPrefs saving in setters
    public static float CamMovSpeed
    {
        get => PlayerPrefs.GetFloat(CamMovSpeedKey, DefaultCamMovSpeed);
        set
        {
            PlayerPrefs.SetFloat(CamMovSpeedKey, value);
            PlayerPrefs.Save();
        }
    }

    public static float CamSprintSpeed
    {
        get => PlayerPrefs.GetFloat(CamSprintSpeedKey, DefaultCamSprintSpeed);
        set
        {
            PlayerPrefs.SetFloat(CamSprintSpeedKey, value);
            PlayerPrefs.Save();
        }
    }

    public static float CamRotateSpeed
    {
        get => PlayerPrefs.GetFloat(CamRotateSpeedKey, DefaultCamRotateSpeed);
        set
        {
            PlayerPrefs.SetFloat(CamRotateSpeedKey, value);
            PlayerPrefs.Save();
        }
    }

    public static float CamZoomSpeed
    {
        get => PlayerPrefs.GetFloat(CamZoomSpeedKey, DefaultCamZoomSpeed);
        set
        {
            PlayerPrefs.SetFloat(CamZoomSpeedKey, value);
            PlayerPrefs.Save();
        }
    }

    public static float EdgeThickness
    {
        get => PlayerPrefs.GetFloat(EdgeThicknessKey, DefaultEdgeThickness);
        set
        {
            PlayerPrefs.SetFloat(EdgeThicknessKey, value);
            PlayerPrefs.Save();
        }
    }

    public static float DroneBatCap
    {
        get => PlayerPrefs.GetFloat(DroneBatCapKey, DefaultDroneBatCap);
        set
        {
            PlayerPrefs.SetFloat(DroneBatCapKey, value);
            PlayerPrefs.Save();
        }
    }

    public static float WindSpeed
    {
        get => PlayerPrefs.GetFloat(WindSpeedKey, DefaultWindSpeed);
        set
        {
            PlayerPrefs.SetFloat(WindSpeedKey, value);
            PlayerPrefs.Save();
        }
    }

    public static Vector3 WindDirection
    {
        get
        {
            float x = PlayerPrefs.GetFloat(WindDirectionXKey, DefaultWindDirection.x);
            float y = PlayerPrefs.GetFloat(WindDirectionYKey, DefaultWindDirection.y);
            float z = PlayerPrefs.GetFloat(WindDirectionZKey, DefaultWindDirection.z);
            return new Vector3(x, y, z);
        }
        set
        {
            PlayerPrefs.SetFloat(WindDirectionXKey, value.x);
            PlayerPrefs.SetFloat(WindDirectionYKey, value.y);
            PlayerPrefs.SetFloat(WindDirectionZKey, value.z);
            PlayerPrefs.Save();
        }
    }

    public static float BatteryChargingEffic
    {
        get => PlayerPrefs.GetFloat(BatChargingEfficKey, DefaultBatChargingEffic);
        set
        {
            PlayerPrefs.SetFloat(BatChargingEfficKey, value);
            PlayerPrefs.Save();
        }
    }

    public static float RotorNum
    {
        get => PlayerPrefs.GetFloat(RotorNumKey, DefaultRotorNum);
        set
        {
            PlayerPrefs.SetFloat(RotorNumKey, value);
            PlayerPrefs.Save();
        }
    }

    public static float AirDensity
    {
        get => PlayerPrefs.GetFloat(AirDensityKey, DefaultAirDensity);
        set
        {
            PlayerPrefs.SetFloat(AirDensityKey, value);
            PlayerPrefs.Save();
        }
    }

    public static float DownwashCoeff
    {
        get => PlayerPrefs.GetFloat(DownwashCoeffKey, DefaultDownwashCoeff);
        set
        {
            PlayerPrefs.SetFloat(DownwashCoeffKey, value);
            PlayerPrefs.Save();
        }
    }

    public static float PwrXferEffic
    {
        get => PlayerPrefs.GetFloat(PwrXferEfficKey, DefaultPwrXferEffic);
        set
        {
            PlayerPrefs.SetFloat(PwrXferEfficKey, value);
            PlayerPrefs.Save();
        }
    }

    public static float AvionicsPwr
    {
        get => PlayerPrefs.GetFloat(AvionicsPwrKey, DefaultAvionicsPwr);
        set
        {
            PlayerPrefs.SetFloat(AvionicsPwrKey, value);
            PlayerPrefs.Save();
        }
    }
}
