using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SliderController : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI valueText;

    [SerializeField]
    Slider slider;

    // Define a key for saving and loading the slider value
    [SerializeField]
    string SliderValueKey = "PlayerPrefValue";

    void Start()
    {
        // Load the slider value from PlayerPrefs and set it to the slider
        slider.value = PlayerPrefs.GetFloat(SliderValueKey, slider.value);
        UpdateValueText(slider.value);
    }

    public void OnSliderValueChanged(float value)
    {
        // Save the new slider value to PlayerPrefs when it changes
        PlayerPrefs.SetFloat(SliderValueKey, value);
        PlayerPrefs.Save();
        UpdateValueText(value);
    }

    void UpdateValueText(float value)
    {
        valueText.text = value.ToString("0.00");
    }
}
