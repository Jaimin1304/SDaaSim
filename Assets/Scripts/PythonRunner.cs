using System.Collections;
using System.Diagnostics;
using UnityEngine;
using System.IO;

public class PythonRunner : MonoBehaviour
{
    private string pythonPath = @"C:\Program Files\Python310\python.exe";
    private string pythonScriptPath = Path.Combine(Application.streamingAssetsPath, "Server/run_server.py");
    private Process pythonProcess;

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

        pythonProcess = new Process
        {
            StartInfo = startInfo
        };

        pythonProcess.Start();
    }

    // Call the ExecutePythonScript() function, e.g., in the Start() or Update() methods
    private void Start()
    {
        ExecutePythonScript();
    }

    // This method will be called when the application is quitting
    private void OnApplicationQuit()
    {
        if (pythonProcess != null && !pythonProcess.HasExited)
        {
            ClientSocket.communicate("127.0.0.1", 1235, "exit");
            pythonProcess.Kill();
        }
    }
}
