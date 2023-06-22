using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Globals : MonoBehaviour
{
    public static string ip;
    public static int port = 1235;
    public static string quitSignal;

    void Awake()
    {
        ip = "127.0.0.1";
        port = 1235;
        quitSignal = "exit";
    }
}
