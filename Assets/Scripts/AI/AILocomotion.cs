using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class AILocomotion : MonoBehaviour
{

    [SerializeField] private float maxSpeed;

    private NavMeshAgent agent;
    private Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        DissolveSoundPlay();
    }

    private void DissolveSoundPlay()
    {
        var sound = FindFirstObjectByType<GeneralSoundsManager>().dissole;
        var audioSource = GetComponent<AudioSource>();

        audioSource.clip = sound;
        audioSource.Play();
    }

    void Update()
    {
        animator.SetFloat("velocityX", Mathf.Abs(agent.velocity.x / maxSpeed));
    }
}
