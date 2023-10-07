using UnityEngine;

public class SettingsController : MonoBehaviour
{
    public GameObject GeneralPanel;
    public GameObject SkywayPanel;
    public GameObject EnvironmentPanel;
    public GameObject DronePanel;

    void Start()
    {
        ShowGeneralPanel(); // Show the General panel by default
    }

    public void ShowGeneralPanel()
    {
        GeneralPanel.SetActive(true);
        SkywayPanel.SetActive(false);
        EnvironmentPanel.SetActive(false);
        DronePanel.SetActive(false);
    }

    public void ShowSkywayPanel()
    {
        GeneralPanel.SetActive(false);
        SkywayPanel.SetActive(true);
        EnvironmentPanel.SetActive(false);
        DronePanel.SetActive(false);
    }

    public void ShowEnvironmentPanel()
    {
        GeneralPanel.SetActive(false);
        SkywayPanel.SetActive(false);
        EnvironmentPanel.SetActive(true);
        DronePanel.SetActive(false);
    }

    public void ShowDronePanel()
    {
        GeneralPanel.SetActive(false);
        SkywayPanel.SetActive(false);
        EnvironmentPanel.SetActive(false);
        DronePanel.SetActive(true);
    }

    public void HideSettings()
    {
        gameObject.SetActive(false);
        Simulator.instance.ToLastState();
    }

    public void SetEdgeThickness(float value)
    {
        Globals.EdgeThickness = value;
        // update thickness for all edges
        foreach (Edge edge in Simulator.instance.Skyway.Edges)
        {
            edge.EdgeView.UpdateEdgeThickness();
        }
    }
}
