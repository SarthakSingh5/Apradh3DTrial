using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Animations.Rigging;


public class AiAgent : MonoBehaviour
{
    public AiStateMachine stateMachine;
    public AiStateId initialState;
    public NavMeshAgent navMeshAgent;
    public AiAgentConfig config;
    public Transform playerTransform;
    [HideInInspector] public AiTargetingSystem targeting;
    [HideInInspector] public AiWeaponManager weaponManager;
    [HideInInspector] public Animator anim;
    [HideInInspector] public AiCoverMovement coverMovement;
    [HideInInspector] public AiSensor sensor;
    public Transform aimTarget;
    public Rig aimRig;


    // Start is called before the first frame update
    void Start()
    {
        
        if (playerTransform == null)
        {
            playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        }
        navMeshAgent = GetComponent<NavMeshAgent>();
        targeting = GetComponent<AiTargetingSystem>();
        weaponManager = GetComponentInChildren<AiWeaponManager>();
        coverMovement = GetComponent<AiCoverMovement>();
        sensor = GetComponent<AiSensor>();
        anim = GetComponent<Animator>();
        stateMachine = new AiStateMachine(this);
        stateMachine.RegisterState(new AiIdleState());
        stateMachine.RegisterState(new AiChasePlayerState());
        stateMachine.RegisterState(new AiFindTargetState());
        stateMachine.RegisterState(new AiAttackTargetState());
        stateMachine.RegisterState(new AiTakeCoverState());
        stateMachine.ChangeState(initialState);
    }

    // Update is called once per frame
    void Update()
    {
        stateMachine.Update();
    }
}
