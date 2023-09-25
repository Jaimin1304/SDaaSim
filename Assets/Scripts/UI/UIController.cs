using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class UIController : MonoBehaviour
{
    public static UIController instance;

    [SerializeField]
    TextMeshProUGUI gameModeText;

    [SerializeField]
    Button settingsButton;

    [SerializeField]
    Button loadSkywayButton;

    [SerializeField]
    Button saveSkywayButton;

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

    [SerializeField]
    TextMeshProUGUI objectInfo;
    public TextMeshProUGUI ObjectInfo
    {
        get { return objectInfo; }
        set { objectInfo = value; }
    }

    Simulator.State stateInMemory = Simulator.State.Pause;

    [SerializeField]
    Button nodeBtn;

    GameObject selectedComponent;

    public GameObject SelectedComponent
    {
        get { return selectedComponent; }
        set { selectedComponent = value; }
    }

    void OnDestroy()
    {
        // Unsubscribe from UnityEvents
        Simulator.instance.OnPlayEvent.RemoveListener(HandlePlay);
        Simulator.instance.OnPauseEvent.RemoveListener(HandlePause);
        Simulator.instance.OnFreezeEvent.RemoveListener(HandleFreeze);
        Simulator.instance.OnEditEvent.RemoveListener(HandleEdit);
    }

    void Start()
    {
        // Subscribe to UnityEvents
        Simulator.instance.OnPlayEvent.AddListener(HandlePlay);
        Simulator.instance.OnPauseEvent.AddListener(HandlePause);
        Simulator.instance.OnFreezeEvent.AddListener(HandleFreeze);
        Simulator.instance.OnEditEvent.AddListener(HandleEdit);
        // top bar
        saveSkywayButton.onClick.AddListener(SaveSkyway);
        loadSkywayButton.onClick.AddListener(LoadSkyway);
        resetButton.onClick.AddListener(Reset);
        playPauseButton.onClick.AddListener(TogglePlayPause);
        speedUpButton.onClick.AddListener(SpeedUp);
        slowDownButton.onClick.AddListener(SlowDown);
        settingsButton.onClick.AddListener(OpenSettings);
        // pop up panel
        popUpConfirmButton.onClick.AddListener(PopUpConfirm);
        // left bar
        nodeBtn.onClick.AddListener(AddNode);
        // right bar
    }

    void HandlePlay()
    {
        Debug.Log("Simulation started");
        // Handle Play state change
        gameModeText.text = "System status: Playing";
        loadSkywayButton.enabled = false;
        saveSkywayButton.enabled = false;
        nodeBtn.enabled = false;
    }

    void HandlePause()
    {
        Debug.Log("Simulation paused");
        // Handle Pause state change
        gameModeText.text = "System status: Paused";
    }

    void HandleFreeze()
    {
        Debug.Log("Simulation frozen");
        // Handle Freeze state change
        gameModeText.text = "System status: Frozen";
    }

    void HandleEdit()
    {
        Debug.Log("Switched to Edit Mode");
        // Handle Edit state change
        gameModeText.text = "System status: Editing";
        loadSkywayButton.enabled = true;
        saveSkywayButton.enabled = true;
        nodeBtn.enabled = true;
    }

    void Update()
    {
        if (Simulator.instance.CurrentState == Simulator.State.Play)
        {
            UpdateTimer();
        }
        if (Input.GetKeyDown(KeyCode.Delete))
        {
            DeleteSelectedComponent();
        }
        UpdateObjectInfo(selectedComponent);
    }

    public void DeleteSelectedComponent()
    {
        if (selectedComponent == null)
        {
            return;
        }
        Node nodeComponent = selectedComponent.GetComponent<Node>();
        Edge edgeComponent = selectedComponent.GetComponent<Edge>();
        selectedComponent = null;
        if (nodeComponent != null)
        {
            Simulator.instance.DeleteNode(nodeComponent);
        }
        else if (edgeComponent != null)
        {
            Simulator.instance.DeleteEdge(edgeComponent);
        }
        else
        {
            Debug.Log("Selected object is neither a Node nor an Edge");
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
            Simulator.instance.Pause();
        }
        else
        {
            Simulator.instance.Play();
        }
        UpdatePlayPauseUI();
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

    void UpdatePlayPauseUI()
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
        gameModeText.text = "System status: Frozen";
    }

    void HidePopUp()
    {
        popUpPanel.SetActive(false);
        switch (stateInMemory)
        {
            case Simulator.State.Play:
                Simulator.instance.Play();
                break;
            case Simulator.State.Edit:
                Simulator.instance.ToEditMode();
                break;
            case Simulator.State.Pause:
                Simulator.instance.Pause();
                break;
            case Simulator.State.Freeze:
                Simulator.instance.Freeze();
                break;
        }
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

    void UpdateObjectInfo(GameObject gameObject)
    {
        if (gameObject == null)
        {
            objectInfo.text = "No object selected";
            return;
        }
        Vector3 position = gameObject.transform.position;
        objectInfo.text = string.Format(
            "Position: X: {0:F2}, Y: {1:F2}, Z: {2:F2}",
            position.x,
            position.y,
            position.z
        );
    }

    void OpenSettings()
    {
        Debug.Log("open settings");
    }
}
