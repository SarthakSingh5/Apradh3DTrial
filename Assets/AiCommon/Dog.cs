using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Animations.Rigging;
using UnityEngine.Events;

public class Dog : AiController
{
    public AiStateMachine stateMachine;
    public AiStateId initialState;
    public AiAgentConfig config;
    public Transform playerTransform;
    [HideInInspector] public AiTargetingSystem targeting;
    [HideInInspector] public AiWeaponManager weaponManager;
    [HideInInspector] public AiCoverMovement coverMovement;
    [HideInInspector] public AiSensor sensor;
    [HideInInspector] public WorldBounds worldBounds;
    public Transform aimTarget;
    public Rig aimRig;


    void Start()
    {
        targeting = GetComponent<AiTargetingSystem>();
        weaponManager = GetComponentInChildren<AiWeaponManager>();
        coverMovement = GetComponent<AiCoverMovement>();
        worldBounds = GetComponent<WorldBounds>();
        sensor = GetComponent<AiSensor>();
        stateMachine = new AiStateMachine(this);
        stateMachine.RegisterState(new AiIdleState());
        stateMachine.RegisterState(new AiFindTargetState());
        stateMachine.RegisterState(new AiAttackTargetState());
        stateMachine.RegisterState(new AiEngageTargetState());
        stateMachine.RegisterState(new AiCoverState());
        stateMachine.ChangeState(initialState);
    }

    protected override void Update()
    {
        base.Update();
        stateMachine.Update();
        
    }
}
