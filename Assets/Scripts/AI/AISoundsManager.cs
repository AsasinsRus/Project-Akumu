using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AISoundsManager : MonoBehaviour
{
    [SerializeField] public AudioClip pew;

    [HideInInspector] public AudioSource audioSource;

    private void Start()
    {
        AudioListener.volume = GeneralSettings.soundsVolume;
        audioSource = GetComponent<AudioSource>();
    }
    public void PewSound()
    {
        audioSource.clip = pew;
        audioSource.Play();
    }
}
