using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class InGameMenu : MonoBehaviour
{
    [SerializeField] private Slider soundsVolumeSlider;
    [SerializeField] private GameObject menu;
    [SerializeField] private GameObject InGameUI;
    [SerializeField] private GameObject plane;
    [SerializeField] private GameObject Fader;

    [SerializeField] private GameObject continueButton;
    [SerializeField] private GameObject HealthBar;

    private PlayerController playerController;

    private void Start()
    {
        AudioListener.volume = GeneralSettings.soundsVolume;
        menu.SetActive(false);
        plane.SetActive(false);

        playerController = FindFirstObjectByType<PlayerController>();

        Fader.SetActive(true);
        Fader.GetComponent<Animator>().Play("FadeOut");
        Invoke("FaderOff", 0.5f);
    }

    private void FaderOff()
    {
        Fader.SetActive(false);
    }

    public void DieMenu()
    {
        menu.SetActive(true);
        continueButton.SetActive(false);
        HealthBar.SetActive(false);
        plane.SetActive(true);

        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;

        Time.timeScale = 0;
    }

    public void Menu(InputAction.CallbackContext context)
    {
        
        if(context.performed && menu.activeSelf == false)
        {
            menu.SetActive(true);
            InGameUI.SetActive(false);
            plane.SetActive(true);

            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;

            Time.timeScale = 0;
        }

        if(context.performed && menu.activeSelf != true)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            menu.SetActive(false);
            InGameUI.SetActive(true);
            plane.SetActive(false);

            Time.timeScale = 1;
        }
    }

    public void Continue()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        menu.SetActive(false);
        InGameUI.SetActive(true);
        plane.SetActive(false);

        Time.timeScale = 1;
    }

    public void Again()
    {
        Time.timeScale = 1;

        Fader.SetActive(true);
        Fader.GetComponent<Animator>().Play("FadeIn");

        Invoke("LoadCurrentScene", 0.5f);
    }

    public void LoadCurrentScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ToMainMenu()
    {
        Time.timeScale = 1;

        Fader.SetActive(true);
        Fader.GetComponent<Animator>().Play("FadeIn");

        GeneralSettings.firstTime = true;

        Invoke("LoadMainMenuScene", 0.5f);
    }
    public void LoadMainMenuScene()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void Exit()
    {
        Application.Quit();
    }

    public void SetSoundsVolume()
    {
        AudioListener.volume = soundsVolumeSlider.value;
        GeneralSettings.soundsVolume = soundsVolumeSlider.value;
    }
}
