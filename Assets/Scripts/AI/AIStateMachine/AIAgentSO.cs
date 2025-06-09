using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class AIAgentSO : ScriptableObject
{
    public float maxTime;

    public float maxDistance;
    public float minDistance;

    public float reloadingTime;
    public float timeBetweenShoots;    
    public float shootsQuantity;

    public float distanceToFind;
}
