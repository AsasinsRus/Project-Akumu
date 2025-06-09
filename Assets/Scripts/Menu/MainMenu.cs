using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private Slider soundsVolumeSlider;

    private Animator animator;
    private int wantToCloseHash;

    [SerializeField] private Button[] mainMenuButtons;

    private void Start()
    {
        animator = GetComponent<Animator>();

        wantToCloseHash = Animator.StringToHash("wantToClose");
    }

    public void mainMenuButtonsInteractable(bool interactable)
    {
        foreach(var button in mainMenuButtons)
        {
            button.interactable = interactable;
        }
    }
    public void SimulationButton()
    {
        mainMenuButtonsInteractable(false);

        animator.SetBool(wantToCloseHash, false);
        animator.Play("SimulationMenuOn");
    }

    public void SimulationAIButton()
    {
        animator.Play("FadeIn");
    }

    public void LoadSimulation()
    {
        GeneralSettings.firstTime = true;
        SceneManager.LoadScene("Simulation");
    }

    public void SettingsButton()
    {
        mainMenuButtonsInteractable(false);
        animator.SetBool(wantToCloseHash, false);
        animator.Play("SettingsMenuOn");
    }

    public void SetSoundsVolume()
    {
        AudioListener.volume = soundsVolumeSlider.value;
        GeneralSettings.soundsVolume = soundsVolumeSlider.value;
    }
    public void Back()
    {
        mainMenuButtonsInteractable(true);
        animator.SetBool(wantToCloseHash, true);
    }
    public void Exit()
    {
        Application.Quit();
    }
}
