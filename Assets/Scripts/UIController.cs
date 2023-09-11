using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class UIController : MonoBehaviour
{
    [SerializeField]
    Button saveSkywayButton;

    [SerializeField]
    Button loadSkywayButton;

    [SerializeField]
    Button resetButton;

    [SerializeField]
    Button playPauseButton;

    [SerializeField]
    Button speedUpButton;

    [SerializeField]
    Button slowDownButton;

    [SerializeField]
    TextMeshProUGUI timerText;

    [SerializeField]
    TextMeshProUGUI playSpeedText;

    [SerializeField]
    Button popUpConfirmButton;

    [SerializeField]
    GameObject popUpPanel;

    [SerializeField]
    TextMeshProUGUI popUpText;

    Simulator.State stateInMemory = Simulator.State.Pause;

    [SerializeField]
    Button nodeBtn;

    [SerializeField]
    Button wayPointBtn;

    void Start()
    {
        // top bar
        saveSkywayButton.onClick.AddListener(SaveSkyway);
        loadSkywayButton.onClick.AddListener(LoadSkyway);
        resetButton.onClick.AddListener(Reset);
        playPauseButton.onClick.AddListener(TogglePlayPause);
        speedUpButton.onClick.AddListener(SpeedUp);
        slowDownButton.onClick.AddListener(SlowDown);
        // pop up panel
        popUpConfirmButton.onClick.AddListener(PopUpConfirm);
        // left bar
        nodeBtn.onClick.AddListener(AddNode);
        wayPointBtn.onClick.AddListener(AddWayPoint);
        // right bar
    }

    void Update()
    {
        if (Simulator.instance.CurrentState == Simulator.State.Play)
        {
            UpdateTimer();
        }
    }

    public void SaveSkyway()
    {
        Debug.Log("SaveSkyway");
        Simulator.instance.SaveSkyway();
        ShowPopUp("Current skyway is saved to 'StreamingAssets/saved skyways'.");
    }

    public void LoadSkyway()
    {
        Debug.Log("LoadSkyway");
    }

    public void Reset()
    {
        Debug.Log("reset");
    }

    public void TogglePlayPause()
    {
        Debug.Log("TogglePlayPause");
        if (Simulator.instance.CurrentState == Simulator.State.Play)
        {
            Simulator.instance.CurrentState = Simulator.State.Pause;
        }
        else if (Simulator.instance.CurrentState == Simulator.State.Edit)
        {
            //Simulator.instance.InitSimulation();
            Simulator.instance.CurrentState = Simulator.State.Play;
        }
        else
        {
            Simulator.instance.CurrentState = Simulator.State.Play;
        }
        UpdateUI();
    }

    public void SpeedUp()
    {
        if (Globals.PlaySpeed < Globals.playSpeedLimit)
        {
            Globals.PlaySpeed *= 2;
        }
        playSpeedText.text = string.Format("x{0}", Globals.PlaySpeed);
    }

    public void SlowDown()
    {
        if (Globals.PlaySpeed > 1)
        {
            Globals.PlaySpeed /= 2;
        }
        playSpeedText.text = string.Format("x{0}", Globals.PlaySpeed);
    }

    void UpdateUI()
    {
        playPauseButton.GetComponentInChildren<TextMeshProUGUI>().text =
            Simulator.instance.CurrentState == Simulator.State.Play ? "Pause" : "Play";
        Debug.Log(Simulator.instance.CurrentState);
    }

    void UpdateTimer()
    {
        int hours = Mathf.FloorToInt(Simulator.instance.ElapsedTime / 3600f);
        int hourInSec = hours * 3600;
        int minutes = Mathf.FloorToInt((Simulator.instance.ElapsedTime - hourInSec) / 60f);
        int seconds = Mathf.FloorToInt(Simulator.instance.ElapsedTime - hourInSec - minutes * 60);
        timerText.text = string.Format("{0:00}:{1:00}:{2:00}", hours, minutes, seconds);
    }

    void PopUpConfirm()
    {
        Debug.Log("ok");
        HidePopUp();
    }

    void ShowPopUp(string message)
    {
        stateInMemory = Simulator.instance.CurrentState;
        Simulator.instance.CurrentState = Simulator.State.Freeze;
        popUpText.text = message;
        popUpPanel.SetActive(true);
    }

    void HidePopUp()
    {
        popUpPanel.SetActive(false);
        Simulator.instance.CurrentState = stateInMemory;
    }

    void AddNode()
    {
        Debug.Log("add node");
        Simulator.instance.CreateNode();
    }

    void AddWayPoint()
    {
        Debug.Log("add waypoint");
    }

    public void DisplayDetails(GameObject gameObject)
    {
        Debug.Log("DisplayDetails for " + gameObject.name);
    }
}
