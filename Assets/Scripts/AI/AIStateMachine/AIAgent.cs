using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum UnitType
{ 
    MeleeWalkable, ShootingWalkable, ShootingFlying
}

public class AIAgent : MonoBehaviour
{
    public AIStateMachine stateMachine;
    public AIStateID initialState;
    public AIStateID currentState;
    public UnitType type;

    [HideInInspector] public Transform target;
    [HideInInspector] public NavMeshAgent navMeshAgent;

    public AIAgentSO config;

    [HideInInspector] public WeaponIK weaponIK;

    [HideInInspector] public float minDistance = 0.0f;
    [HideInInspector] public float maxDistance = 0.0f;

    [HideInInspector] public ShootProjectTiles shootProjectTiles;

    // Start is called before the first frame update
    void Start()
    {
        stateMachine = new AIStateMachine(this);

        stateMachine.RegisterState(new AIChasePlayerState());
        stateMachine.RegisterState(new AIDeathState());
        stateMachine.RegisterState(new AIIdleState());
        stateMachine.RegisterState(new AIAttackPlayerState());

        if (target == null)
            target = GameObject.FindGameObjectWithTag("Player").transform;

        weaponIK = GetComponent<WeaponIK>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        shootProjectTiles = GetComponent<ShootProjectTiles>();

        SetDistance(type);

        stateMachine.ChangeState(initialState);
    }

    // Update is called once per frame
    void Update()
    {
        stateMachine.Update();
        currentState = stateMachine.currentState;
    }

    private void SetDistance(UnitType type)
    {
        maxDistance = config.maxDistance;
        minDistance = config.minDistance;
    }
}
