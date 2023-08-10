using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class UIController : MonoBehaviour
{
    [SerializeField]
    Button playPauseButton;

    [SerializeField]
    TextMeshProUGUI timerText;

    private bool isPlaying = false;
    private float elapsedTime = 0f;

    private void Start()
    {
        playPauseButton.onClick.AddListener(TogglePlayPause);
        UpdateUI();
    }

    private void Update()
    {
        if (isPlaying)
        {
            elapsedTime += Time.deltaTime;
            UpdateTimer();
        }
    }

    private void TogglePlayPause()
    {
        if (isPlaying)
        {
            SkywaySimulator.instance.CurrentState = SkywaySimulator.State.Pause;
        }
        else
        {
            SkywaySimulator.instance.CurrentState = SkywaySimulator.State.Play;
        }
        isPlaying = !isPlaying;
        UpdateUI();
    }

    private void UpdateUI()
    {
        playPauseButton.GetComponentInChildren<TextMeshProUGUI>().text = isPlaying
            ? "Pause"
            : "Play";
    }

    private void UpdateTimer()
    {
        int hours = Mathf.FloorToInt(elapsedTime / 3600f);
        int hourInSec = hours * 3600;
        int minutes = Mathf.FloorToInt((elapsedTime - hourInSec) / 60f);
        int seconds = Mathf.FloorToInt(elapsedTime - hourInSec - minutes * 60);
        timerText.text = string.Format("{0:00}:{1:00}:{2:00}", hours, minutes, seconds);
    }
}
