using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Animations.Rigging;
using UnityEngine.Events;


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
    [HideInInspector] public WorldBounds worldBounds;
    public Transform aimTarget;
    public Rig aimRig;

    public Vector3 velocity = Vector3.zero;
    public bool canTurn = true;

    public UnityAction<Vector3> LookAt;
    public System.Func<Vector3, bool> IsLookingAt;

    #region Gun Properties and Delegates
    public Gun gun;
    public bool carryingGun = true;


    private bool aiming = false;
    public void SetAim(bool aim)
    {
        if (carryingGun)
        {
            this.aiming = aim;
        }
        else
        {
            this.aiming = false;
        }
    }
    public bool Aiming => aiming;


    public UnityAction TryShoot;

    #endregion

    public Vector3 SensorPosition
    {
        get
        {
            return transform.position + Vector3.up * 1.5f;
        }
    }




    // Start is called before the first frame update
    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        targeting = GetComponent<AiTargetingSystem>();
        weaponManager = GetComponentInChildren<AiWeaponManager>();
        coverMovement = GetComponent<AiCoverMovement>();
        worldBounds = GetComponent<WorldBounds>();
        sensor = GetComponent<AiSensor>();
        anim = GetComponent<Animator>();
        gun = GetComponentInChildren<Gun>();
        stateMachine = new AiStateMachine(this);
        stateMachine.RegisterState(new AiIdleState());
        stateMachine.RegisterState(new AiChasePlayerState());
        stateMachine.RegisterState(new AiFindTargetState());
        stateMachine.RegisterState(new AiAttackTargetState());
        stateMachine.RegisterState(new AiEngageTargetState());
        stateMachine.ChangeState(initialState);
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, Mathf.Infinity))
        {
            transform.position = hit.point; // Snap NPC to ground
        }
        stateMachine.Update();

        velocity = navMeshAgent.velocity;
        navMeshAgent.updateRotation = canTurn; // Enable or disable rotation based on canTurn
    }
}
