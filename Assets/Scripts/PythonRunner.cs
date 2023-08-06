using System.Collections;
using System.Diagnostics;
using UnityEngine;
using System.IO;
using UDebug = UnityEngine.Debug;
using SDebug = System.Diagnostics.Debug;

public class PythonRunner : MonoBehaviour
{
    string pythonPath = @"C:\Program Files\Python310\python.exe";
    string pythonScriptPath = Path.Combine(Application.streamingAssetsPath, "server/run_server.py");
    Process pythonProcess;

    [SerializeField]
    ClientSocket cs;

    public void ExecutePythonScript()
    {
        ProcessStartInfo startInfo = new ProcessStartInfo
        {
            FileName = pythonPath,
            Arguments = $"\"{pythonScriptPath}\"",
            UseShellExecute = false,
            RedirectStandardOutput = false,
            RedirectStandardError = false,
            CreateNoWindow = true,
        };

        pythonProcess = new Process { StartInfo = startInfo };

        pythonProcess.Start();
    }

    void OnApplicationQuit()
    {
        if (pythonProcess != null && !pythonProcess.HasExited)
        {
            cs.Communicate(Globals.ip, Globals.port, Globals.quitSignal);
            UDebug.Log("Quit msg sent");
            pythonProcess.Kill();
        }
        UDebug.Log("Python server terminated");
    }
}
