using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SwordDirection : MonoBehaviour
{
    public GameObject sword;
    public float offsetXPos;
    public float offsetXNeg;

    private float direction = 1;

    private void Update()
    {
        float desiredDirection = GetComponentInParent<PlayerController>().inputVector.x;

        if(desiredDirection != 0 && desiredDirection != direction)
        {
            direction = desiredDirection;
        }

        transform.rotation = sword.transform.rotation;
        transform.position = new Vector3(transform.position.x, sword.transform.position.y, sword.transform.position.z);
        
    }

    public void SetXonSword()
    {
        

        if (direction > 0)
        {
            transform.localPosition = new Vector3(sword.transform.localPosition.x, offsetXPos, transform.localPosition.z);
        }
        else if (direction < 0)
        {
            transform.localPosition = new Vector3(sword.transform.localPosition.x, offsetXNeg, transform.localPosition.z);
        }
    }
}
