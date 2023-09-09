using UnityEngine;
using UnityEngine.UIElements;

public class OperateUI : MonoBehaviour
{
    [SerializeField]
    UIController uIController;

    void OnEnable()
    {
        VisualElement root = GetComponent<UIDocument>().rootVisualElement;

        Button playBtn = root.Q<Button>("playBtn");
        Button speedUpBtn = root.Q<Button>("speedUpBtn");
        Button slowDownBtn = root.Q<Button>("slowDownBtn");
        Button saveBtn = root.Q<Button>("saveBtn");
        Button loadBtn = root.Q<Button>("loadBtn");
        Button resetBtn = root.Q<Button>("resetBtn");
        Button settingsBtn = root.Q<Button>("settingsBtn");

        Label timeLabel = root.Q<Label>("timeLabel");

        playBtn.clicked += uIController.TogglePlayPause;
        speedUpBtn.clicked += uIController.SpeedUp;
        slowDownBtn.clicked += uIController.SlowDown;
        saveBtn.clicked += uIController.SaveSkyway;
        loadBtn.clicked += uIController.LoadSkyway;
        resetBtn.clicked += uIController.Reset;
    }
}
