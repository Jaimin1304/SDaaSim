using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.Events;

public class UIController : MonoBehaviour
{
    public static UIController instance;

    [SerializeField]
    TextMeshProUGUI gameModeText;

    [SerializeField]
    TextMeshProUGUI timerText;

    [SerializeField]
    TextMeshProUGUI playSpeedText;

    [SerializeField]
    TextMeshProUGUI popUpText;

    [SerializeField]
    TextMeshProUGUI objectInfo;

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
    Button popUpConfirmButton;

    [SerializeField]
    Button nodeBtn;

    [SerializeField]
    GameObject popUpPanel;

    [SerializeField]
    GameObject settingsWindow;

    GameObject selectedComponent;
    Simulator.State stateInMemory = Simulator.State.Pause;

    [SerializeField]
    CanvasGroup settingsCanvasGroup;

    [SerializeField]
    CanvasGroup mainCanvasGroup;

    // Define UnityEvents for delete
    public UnityEvent OnDeleteEvent;

    public GameObject SelectedComponent
    {
        get { return selectedComponent; }
        set { selectedComponent = value; }
    }

    public TextMeshProUGUI ObjectInfo
    {
        get { return objectInfo; }
        set { objectInfo = value; }
    }

    void OnDestroy()
    {
        // Unsubscribe from UnityEvents
        Simulator.instance.OnPlayEvent.RemoveListener(HandlePlay);
        Simulator.instance.OnPauseEvent.RemoveListener(HandlePause);
        Simulator.instance.OnFreezeEvent.RemoveListener(HandleFreeze);
        Simulator.instance.OnEditEvent.RemoveListener(HandleEdit);
        Simulator.instance.OnFinishEvent.RemoveListener(HandleFinish);
    }

    void Awake()
    {
        // Initialize the UnityEvents
        OnDeleteEvent = new UnityEvent();
    }

    void Start()
    {
        // Subscribe to UnityEvents
        Simulator.instance.OnPlayEvent.AddListener(HandlePlay);
        Simulator.instance.OnPauseEvent.AddListener(HandlePause);
        Simulator.instance.OnFreezeEvent.AddListener(HandleFreeze);
        Simulator.instance.OnEditEvent.AddListener(HandleEdit);
        Simulator.instance.OnFinishEvent.AddListener(HandleFinish);
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
        settingsButton.enabled = false;
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
        settingsButton.enabled = true;

        playPauseButton.enabled = true;
        speedUpButton.enabled = true;
        slowDownButton.enabled = true;
    }

    void HandleFinish(bool showPopup)
    {
        Debug.Log("Simulation finished");
        playPauseButton.enabled = false;
        speedUpButton.enabled = false;
        slowDownButton.enabled = false;
        gameModeText.text = "System status: Finished";
        if (showPopup)
        {
            ShowPopUp("Simulation finished. Data is saved to 'StreamingAssets/saved simulations'.");
        }
    }

    void Update()
    {
        switch (Simulator.instance.CurrentState)
        {
            case Simulator.State.Play:
                UpdateTimer();
                break;
            case Simulator.State.Pause:
                break;
            case Simulator.State.Edit:
                EditLogic();
                break;
            case Simulator.State.Freeze:
                break;
            case Simulator.State.Finished:
                break;
        }
    }

    void EditPads(bool add, bool rechargeable)
    {
        // Check whether a node is selected
        if (selectedComponent == null)
        {
            return;
        }
        Node nodeComponent = selectedComponent.GetComponent<Node>();
        if (nodeComponent == null)
        {
            return;
        }
        Simulator.instance.EditPad(nodeComponent, add, rechargeable);
    }

    void EditLogic()
    {
        // Handle deletion
        if (Input.GetKeyDown(KeyCode.Delete))
        {
            DeleteSelectedComponent();
        }
        // Handle pad edition
        bool addPad = Input.GetKeyDown(KeyCode.Equals);
        bool removePad = Input.GetKeyDown(KeyCode.KeypadMinus) || Input.GetKeyDown(KeyCode.Minus);
        if (addPad || removePad)
        {
            bool rechargeable = Input.GetKey(KeyCode.R);
            EditPads(addPad, rechargeable);
        }
        // Handle waypoint edition
        if (Input.GetKeyDown(KeyCode.P))
        {
            Edge edgeComponent = selectedComponent.GetComponent<Edge>();
            WayPoint wayPointComponent = selectedComponent.GetComponent<WayPoint>();
            if (edgeComponent != null)
            {
                // add waypoint to an edge with no waypoint, insert at middle
                Simulator.instance.CreateWayPoint(edgeComponent, null);
            }
            else if (wayPointComponent != null)
            {
                // add a new waypoint beside the existing waypoint
                Simulator.instance.CreateWayPoint(wayPointComponent.Edge, wayPointComponent);
            }
        }
        // Handle request startNode/destNode edition
        if (Input.GetKeyDown(KeyCode.H))
        {
            Node nodeComponent = selectedComponent.GetComponent<Node>();
            if (nodeComponent != null)
            {
                // set the selected node to the start node of the request
                Request request = Simulator.instance.Skyway.Requests[0];
                request.StartNode = nodeComponent;
                foreach (SubSwarm subSwarm in request.Swarm.SubSwarms)
                {
                    subSwarm.Node = nodeComponent;
                    subSwarm.Init();
                }
            }
        }
        if (Input.GetKeyDown(KeyCode.T))
        {
            Node nodeComponent = selectedComponent.GetComponent<Node>();
            if (nodeComponent != null)
            {
                // set the selected node to the start node of the request
                Request request = Simulator.instance.Skyway.Requests[0];
                request.DestNode = nodeComponent;
            }
        }
        // Handle payload/drone edition
        if (Input.GetKeyDown(KeyCode.Equals))
        {
            Drone drone = selectedComponent.GetComponent<Drone>();
        }
        if (Input.GetKeyDown(KeyCode.Minus))
        {
            Drone drone = selectedComponent.GetComponent<Drone>();
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
        Drone droneComponent = selectedComponent.GetComponent<Drone>();
        WayPoint wayPointComponent = selectedComponent.GetComponent<WayPoint>();
        selectedComponent = null;
        if (nodeComponent != null)
        {
            Simulator.instance.DeleteNode(nodeComponent);
        }
        else if (edgeComponent != null)
        {
            Simulator.instance.DeleteEdge(edgeComponent);
        }
        else if (wayPointComponent != null)
        {
            Simulator.instance.DeleteWayPoint(wayPointComponent);
        }
        else if (droneComponent != null)
        {
            Payload payload = droneComponent.Payloads[0];
            Request request = droneComponent.SubSwarm.ParentSwarm.Request;
            Simulator.instance.DeletePayload(payload, request);
        }
        else
        {
            Debug.Log("Selected object is neither a Node nor an Edge");
        }
        OnDeleteEvent.Invoke();
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
        Simulator.instance.LoadSkyway();
    }

    public void Reset()
    {
        Debug.Log("reset");
        Simulator.instance.ResetSkyway();
    }

    public void TogglePlayPause()
    {
        Debug.Log("TogglePlayPause");
        if (Simulator.instance.CurrentState == Simulator.State.Play)
        {
            Simulator.instance.ToPause();
        }
        else
        {
            Simulator.instance.ToPlay();
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
        if (Globals.PlaySpeed > 0.125)
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
        timerText.text = Simulator.instance.GetTimeString();
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
        // Block other UI
        settingsCanvasGroup.interactable = false;
        mainCanvasGroup.interactable = false;
        settingsCanvasGroup.alpha = 0.4f;
        mainCanvasGroup.alpha = 0.4f;
    }

    void HidePopUp()
    {
        popUpPanel.SetActive(false);
        // Reactivate other UI
        settingsCanvasGroup.interactable = true;
        mainCanvasGroup.interactable = true;
        settingsCanvasGroup.alpha = 1f;
        mainCanvasGroup.alpha = 1f;
        switch (stateInMemory)
        {
            case Simulator.State.Play:
                Simulator.instance.ToPlay();
                break;
            case Simulator.State.Edit:
                Simulator.instance.ToEdit();
                break;
            case Simulator.State.Pause:
                Simulator.instance.ToPause();
                break;
            case Simulator.State.Freeze:
                Simulator.instance.ToFreeze();
                break;
            case Simulator.State.Finished:
                Simulator.instance.ToFinish(false);
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
        settingsWindow.SetActive(true);
        Simulator.instance.ToFreeze();
    }
}
