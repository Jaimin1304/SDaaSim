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
    public const int playSpeedLimit = 64;
    public const float padGap = 4f;
    public const float edgeHeightOffset = 2f;
    public const float edgeNodeGap = 5f;
    public const float g = 9.807f; // Gravitational acceleration

    //public static Vector3 windDirection = Vector3.left;

    static float playSpeed = 1;
    public static float PlaySpeed
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
    const string PadRechargeRateKey = "PadRechargeRate"; // Wh per second

    // Wind direction vector3
    static string WindSpdXKey = "WindSpdX"; // Wind speed x
    static string WindSpdYKey = "WindSpdY"; // Wind speed y
    static string WindSpdZKey = "WindSpdZ"; // Wind speed z

    const string BatChargingEfficKey = "BatChargingEffic"; // Battery charging efficiency
    const string RotorNumKey = "RotorNum"; // Number of rotors
    const string AirDensityKey = "AirDensity"; // Air density
    const string DownwashCoeffKey = "DownwashCoeff"; // Downwash coefficient
    const string PwrXferEfficKey = "PwrXferEffic"; // Power transfer efficiency from battery to propeller
    const string AvionicsPwrKey = "AvionicsPwr"; // Avionics power, power consumption of electronic equipmentF

    // Drone attributes
    const string MaxLiftSpdKey = "MaxLiftSpd"; // Design maximum engine lifting speed in m/s
    const string MaxDescnetSpdKey = "MaxDescnetSpd"; // Design maximum engine descent speed in m/s
    const string MaxHorizontalSpdKey = "MaxHorizontalSpd"; // Design maximum engine horizontal speed in m/s
    const string BodyMassKey = "BodyMass"; // Drone body mass in kg
    const string BatMassKey = "BatMass"; // Drone battery mass in kg
    const string PayloadMassKey = "PayloadMass"; // Drone payload mass in kg
    const string BodyAreaKey = "BodyArea"; // Drone body projected area in m*m
    const string BatAreaKey = "BatArea"; // Drone battery projected area in m*m
    const string PayloadAreaKey = "PayloadArea"; // Drone payload projected area in m*m
    const string BodyDragCoeffKey = "BodyDragCoeff"; // Drone body projected area in m*m
    const string BatDragCoeffKey = "BatDragCoeff"; // Drone battery projected area in m*m
    const string PayloadDragCoeffKey = "PayloadDragCoeff"; // Drone payload projected area in m*m
    const string InducedPowFactorKey = "InducedPowFactor"; // Drone payload projected area in m*m
    const string ProfilePowFactorKey = "ProfilePowFactor"; // Drone payload projected area in m*m
    const string ProfilePowWithSpdFactorKey = "ProfilePowWithSpdFactor"; // Drone payload projected area in m*m

    // Define default values
    const float DefaultCamMovSpeed = 80;
    const float DefaultCamSprintSpeed = 300;
    const float DefaultCamRotateSpeed = 0.15f;
    const float DefaultCamZoomSpeed = 200;
    const float DefaultEdgeThickness = 0.6f;
    const float DefaultPadRechargeRate = 0.05f;
    const float DefaultDroneBatCap = 500;
    static Vector3 DefaultWindSpd = new Vector3(-15, 0, 0);
    const float DefaultBatChargingEffic = 0.9f;
    const float DefaultRotorNum = 4f;
    const float DefaultAirDensity = 1.225f;
    const float DefaultDownwashCoeff = 1f;
    const float DefaultPwrXferEffic = 0.73f;
    const float DefaultAvionicsPwr = 10f;

    // Default drone attributes
    const float DefaultMaxLiftSpd = 5f;
    const float DefaultMaxDescnetSpd = 5f;
    const float DefaultMaxHorizontalSpd = 20f;
    const float DefaultBodyMass = 1.07f;
    const float DefaultBatMass = 1f;
    const float DefaultPayloadMass = 1.5f;
    const float DefaultBodyArea = 0.0599f;
    const float DefaultBatArea = 0.0037f;
    const float DefaultPayloadArea = 0.0135f;
    const float DefaultBodyDragCoeff = 1.49f;
    const float DefaultBatDragCoeff = 1f;
    const float DefaultPayloadDragCoeff = 2.2f;
    const float DefaultInducedPowFactor = 1f;
    const float DefaultProfilePowFactor = 0.790f;
    const float DefaultProfilePowWithSpdFactor = 0.0042f;

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

    public static float PadRechargeRate
    {
        get => PlayerPrefs.GetFloat(PadRechargeRateKey, DefaultPadRechargeRate);
        set
        {
            PlayerPrefs.SetFloat(PadRechargeRateKey, value);
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

    public static Vector3 WindSpd
    {
        get
        {
            float x = PlayerPrefs.GetFloat(WindSpdXKey, DefaultWindSpd.x);
            float y = PlayerPrefs.GetFloat(WindSpdYKey, DefaultWindSpd.y);
            float z = PlayerPrefs.GetFloat(WindSpdZKey, DefaultWindSpd.z);
            return new Vector3(x, y, z);
        }
        set
        {
            PlayerPrefs.SetFloat(WindSpdXKey, value.x);
            PlayerPrefs.SetFloat(WindSpdYKey, value.y);
            PlayerPrefs.SetFloat(WindSpdZKey, value.z);
            PlayerPrefs.Save();
        }
    }

    public static float BatChargingEffic
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

    public static float MaxLiftSpd
    {
        get => PlayerPrefs.GetFloat(MaxLiftSpdKey, DefaultMaxLiftSpd);
        set
        {
            PlayerPrefs.SetFloat(MaxLiftSpdKey, value);
            PlayerPrefs.Save();
        }
    }

    public static float MaxDescnetSpd
    {
        get => PlayerPrefs.GetFloat(MaxDescnetSpdKey, DefaultMaxDescnetSpd);
        set
        {
            PlayerPrefs.SetFloat(MaxDescnetSpdKey, value);
            PlayerPrefs.Save();
        }
    }

    public static float MaxHorizontalSpd
    {
        get => PlayerPrefs.GetFloat(MaxHorizontalSpdKey, DefaultMaxHorizontalSpd);
        set
        {
            PlayerPrefs.SetFloat(MaxHorizontalSpdKey, value);
            PlayerPrefs.Save();
        }
    }

    public static float BodyMass
    {
        get => PlayerPrefs.GetFloat(BodyMassKey, DefaultBodyMass);
        set
        {
            PlayerPrefs.SetFloat(BodyMassKey, value);
            PlayerPrefs.Save();
        }
    }

    public static float BatMass
    {
        get => PlayerPrefs.GetFloat(BatMassKey, DefaultBatMass);
        set
        {
            PlayerPrefs.SetFloat(BatMassKey, value);
            PlayerPrefs.Save();
        }
    }

    public static float PayloadMass
    {
        get => PlayerPrefs.GetFloat(PayloadMassKey, DefaultPayloadMass);
        set
        {
            PlayerPrefs.SetFloat(PayloadMassKey, value);
            PlayerPrefs.Save();
        }
    }

    public static float BodyArea
    {
        get => PlayerPrefs.GetFloat(BodyAreaKey, DefaultBodyArea);
        set
        {
            PlayerPrefs.SetFloat(BodyAreaKey, value);
            PlayerPrefs.Save();
        }
    }

    public static float BatArea
    {
        get => PlayerPrefs.GetFloat(BatAreaKey, DefaultBatArea);
        set
        {
            PlayerPrefs.SetFloat(BatAreaKey, value);
            PlayerPrefs.Save();
        }
    }

    public static float PayloadArea
    {
        get => PlayerPrefs.GetFloat(PayloadAreaKey, DefaultPayloadArea);
        set
        {
            PlayerPrefs.SetFloat(PayloadAreaKey, value);
            PlayerPrefs.Save();
        }
    }

    public static float BodyDragCoeff
    {
        get => PlayerPrefs.GetFloat(BodyDragCoeffKey, DefaultBodyDragCoeff);
        set
        {
            PlayerPrefs.SetFloat(BodyDragCoeffKey, value);
            PlayerPrefs.Save();
        }
    }

    public static float BatDragCoeff
    {
        get => PlayerPrefs.GetFloat(BatDragCoeffKey, DefaultBatDragCoeff);
        set
        {
            PlayerPrefs.SetFloat(BatDragCoeffKey, value);
            PlayerPrefs.Save();
        }
    }

    public static float PayloadDragCoeff
    {
        get => PlayerPrefs.GetFloat(PayloadDragCoeffKey, DefaultPayloadDragCoeff);
        set
        {
            PlayerPrefs.SetFloat(PayloadDragCoeffKey, value);
            PlayerPrefs.Save();
        }
    }

    public static float InducedPowFactor
    {
        get => PlayerPrefs.GetFloat(InducedPowFactorKey, DefaultInducedPowFactor);
        set
        {
            PlayerPrefs.SetFloat(InducedPowFactorKey, value);
            PlayerPrefs.Save();
        }
    }

    public static float ProfilePowFactor
    {
        get => PlayerPrefs.GetFloat(ProfilePowFactorKey, DefaultProfilePowFactor);
        set
        {
            PlayerPrefs.SetFloat(PayloadDragCoeffKey, value);
            PlayerPrefs.Save();
        }
    }

    public static float ProfilePowWithSpdFactor
    {
        get => PlayerPrefs.GetFloat(ProfilePowWithSpdFactorKey, DefaultProfilePowWithSpdFactor);
        set
        {
            PlayerPrefs.SetFloat(ProfilePowWithSpdFactorKey, value);
            PlayerPrefs.Save();
        }
    }
}
