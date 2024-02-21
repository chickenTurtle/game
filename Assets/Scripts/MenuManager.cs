using UnityEngine;
using UnityEngine.Audio;
using System;
using System.Linq;
using TMPro;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    public static MenuManager instance;
    public GameObject MainMenu;
    public GameObject SettingsMenu;
    public AudioMixer audioMixer;
    public TMP_Dropdown resolutionDropdown;
    public Toggle fullscreen;
    public Int32 resultionIndex;

    Resolution[] resolutions;
    public void Start()
    {
        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();

        int currResIndex = 0;

        for (int i = 0; i < resolutions.Length; i++)
        {
            if (resolutions[i].width == Screen.width && resolutions[i].height == Screen.height)
                currResIndex = i;
        }

        resolutionDropdown.AddOptions(resolutions.Select(x => x.width + " x " + x.height).ToList<string>());
        resolutionDropdown.value = currResIndex;
        resolutionDropdown.RefreshShownValue();
        fullscreen.isOn = Screen.fullScreen;
    }
    public void Awake()
    {
        if (instance == null)
            instance = this;

    }

    public void OpenSettings()
    {
        SettingsMenu.SetActive(true);
        MainMenu.SetActive(false);
    }

    public void OpenMainMenu()
    {
        MainMenu.SetActive(true);
        SettingsMenu.SetActive(false);
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void ChangeVolume(float volume)
    {
        audioMixer.SetFloat("volume", volume);
    }

    public void SetResolution(Int32 index)
    {
        Screen.SetResolution(Screen.resolutions[index].width, Screen.resolutions[index].height, Screen.fullScreen);
    }

    public void SetFullscreen(bool value)
    {
        Screen.fullScreen = value;
    }

}