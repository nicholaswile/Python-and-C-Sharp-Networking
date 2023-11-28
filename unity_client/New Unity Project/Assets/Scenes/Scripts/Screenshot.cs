// 2023-11-27 by NW

// Take a screenshot and save it to an image file

using UnityEngine;
using System;

// UI
using TMPro;
using UnityEngine.UI;

public class Screenshot : MonoBehaviour
{
    [SerializeField] private TMP_Text display_text;
    [SerializeField] private Button button;
    [SerializeField] private GameObject client;

    string path = "Assets/Screenshots/screenshot";

    private void TakeScreenshot()
    {
        var time = DateTime.Now;
        string formatted_time = time.ToString("yyyy-MM-dd-hhmmss");
        ScreenCapture.CaptureScreenshot(path + "-" + formatted_time + ".png");
        display_text.text = "Took Screenshot at " + formatted_time + "!";
        display_text.text += "\nSaving screenshot to screenshots folder...";

        client.SetActive(true);
    }

    private void Start()
    {
        button.onClick.AddListener(TakeScreenshot);
    }
}