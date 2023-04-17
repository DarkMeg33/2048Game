using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public GameObject MenuPanel;
    public GameObject MainPanel;
    public GameObject SettingsPanel;
    public GameObject HowToPlay;

    public GameObject SoundsOn;
    public GameObject SoundsOff;

    public AudioSource audioSource;
    public AudioClip clickButton;

    private bool isSound = true;
    
    public void Play()
    {
        MenuPanel.SetActive(false);
        MainPanel.SetActive(true);
        PlaySound();
    }

    public void OpenPanelHowToPlay()
    {
        HowToPlay.SetActive(true);
        PlaySound();
    }
    public void ClosePanelHowToPlay()
    {
        HowToPlay.SetActive(false);
        PlaySound();
    }

    public void Home()
    {
        MainPanel.SetActive(false);
        MenuPanel.SetActive(true);
        PlaySound();
    }

    public void OpenPanelSettings()
    {
        SettingsPanel.SetActive(true);
        PlaySound();
    }
    public void ClosePanelSettings()
    {
        SettingsPanel.SetActive(false);
        PlaySound();
    }
    public void SoundOn()
    {
        SoundsOn.SetActive(true);
        SoundsOff.SetActive(false);
        isSound = true;
        PlaySound();
    }
    public void SoundOff()
    {
        SoundsOn.SetActive(false);
        SoundsOff.SetActive(true);
        isSound = false;
    }

    public void Continue()
    {
        Application.LoadLevel(0);
        PlaySound();
    }

    void PlaySound()
    {
        if (isSound)
        {
            audioSource.PlayOneShot(clickButton);
        }
    }

}
