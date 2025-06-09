using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ragdoll : MonoBehaviour
{
    private Rigidbody[] rigidbodies; 
    private Animator animator;
    void Start()
    {
        rigidbodies = GetComponentsInChildren<Rigidbody>();
        animator = GetComponent<Animator>();

        DeactivateRigidbody();
    }

    public void AddForce(Vector3 direction, ForceMode forceMode)
    {
        foreach (var rigidbody in rigidbodies)
        {
            rigidbody.AddForce(direction, forceMode);
        }
    }

    public void ActivateRigidbody()
    {
        animator.enabled = false;

        foreach(var rigidbody in rigidbodies)
        {
            rigidbody.isKinematic = false;
        }
    }
    public void DeactivateRigidbody()
    {
        animator.enabled = true;

        foreach (var rigidbody in rigidbodies)
        {
            rigidbody.isKinematic = true;
        }
    }
}
