using UnityEngine;
using UnityEngine.UIElements;

public class EditModeUI : MonoBehaviour
{
    [SerializeField]
    EditModeController editModeController;

    void OnEnable()
    {
        VisualElement root = GetComponent<UIDocument>().rootVisualElement;

        Button startBtn = root.Q<Button>("start");
        Button loadBtn = root.Q<Button>("load");
        Button saveBtn = root.Q<Button>("save");
        Button settingsBtn = root.Q<Button>("settings");

        startBtn.clicked += editModeController.StartSimulation;
        saveBtn.clicked += editModeController.SaveSkyway;
        loadBtn.clicked += editModeController.LoadSkyway;
        settingsBtn.clicked += editModeController.OpenSettings;
    }
}
