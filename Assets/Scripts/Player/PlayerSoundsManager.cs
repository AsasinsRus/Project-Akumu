using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSoundsManager : MonoBehaviour
{
    [SerializeField] public AudioClip[] steps;
    [SerializeField] public AudioClip swift;
    [SerializeField] public AudioClip deflect;

    [HideInInspector] public AudioSource audioSource;

    private void Start()
    {
        AudioListener.volume = GeneralSettings.soundsVolume;
        audioSource = GetComponent<AudioSource>();
    }

    public void StepSound()
    {
        int index = Random.Range(0, steps.Length);

        audioSource.clip = steps[index];
        audioSource.Play();
    }

    public void SwiftSound()
    {
        audioSource.clip = swift;
        audioSource.Play();
    }
}
