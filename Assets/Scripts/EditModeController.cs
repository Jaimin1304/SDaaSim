using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EditModeController : MonoBehaviour
{
    [SerializeField]
    Skyway skyway;

    public void StartSimulation()
    {
        Debug.Log("start simulation");
        //DontDestroyOnLoad(skyway.gameObject);
        SceneManager.LoadScene("OperateScene");
    }

    public void SaveSkyway()
    {
        Debug.Log("SaveSkyway");
    }

    public void LoadSkyway()
    {
        Debug.Log("LoadSkyway");
    }

    public void OpenSettings()
    {
        Debug.Log("OpenSettings");
    }
}
