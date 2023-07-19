using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Globals : MonoBehaviour
{
    public static string ip = "127.0.0.1";
    public static int port = 1235;
    public static string quitSignal = "exit";
    public static string endOfFileSignal = "|";
    public static string initSkywayHeader = "initSkyway";
    public static float camMovSpeed = 160;
    public static float camRotateSpeed = 0.2f;
}
