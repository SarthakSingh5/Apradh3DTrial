using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;


public partial class PlayerController : NpcController
{
    [SerializeField]
    CharacterController characterController;

    private Vector2 LookInput;
    private Vector2 MoveInput;
    private bool RunInput = false;
    private Vector2 LookAngle = Vector2.zero;

    [SerializeField]
    Vector3 CameraOffset = new Vector3(0f, 1.2f, 0f);

    [SerializeField]
    Transform CameraTarget;

    [SerializeField]
    private float LookSmoothing = 5f;

    [SerializeField]
    private Vector2 LookSensitivity = Vector2.one * 0.4f;

    [SerializeField]
    float MoveSmoothing = 5f;


    NavMeshAgent agent;

    bool attacking = false;

    bool inputEnabled = true;

    #region Recoil Variables
    
    private float totalRecoilAppliedY = 0f;
    [SerializeField] private float recoilRecoverySpeed = 15f; // Increased for a snappier mobile feel
    [SerializeField] private float recoveryDelay = 0.1f;
    private float lastShotTime;

    #endregion


    public void SetInputEnabled(bool inputEnabled)
    {
        this.inputEnabled = inputEnabled;
    }



    #region Input Callbacks

    void OnTestKill(InputValue input)
    {
        npc.health.Points = 0f;
    }


    void OnTestRevive(InputValue input)
    {
        npc.health.Points = 1f;
    }

    void OnMove(InputValue input)
    {
        MoveInput = input.Get<Vector2>();
    }

    void OnAim(InputValue input)
    {
        aiming = !aiming;

        npc.SetAim(aiming);
    }


    void OnAttack(InputValue input)
    {
        attacking = input.isPressed;

        if (npc.Alive)
        {
            if (attacking)
            {
                npc.TryShoot?.Invoke();
            }
            else
            {
                npc.NotShoot?.Invoke();
            }
        }
    }

    void OnLook(InputValue input)
    {
        LookInput = input.Get<Vector2>();
    }

    void OnRun(InputValue input)
    {
        RunInput = input.isPressed;
    }

    void OnReload(InputValue input)
    {
        if (npc.Alive)
        {
            npc.TryReload?.Invoke();
        }
    }

    #endregion


    #region Base Class Overrides

    public override void SetNpc(Npc npc)
    {
        base.SetNpc(npc);


        if (npc != null)
        {
            if (characterController == null)
            {
                characterController = npc.GetComponentInParent<CharacterController>();
            }

            if (characterController == null)
            {
                Debug.LogError($"Character Controller not found on {npc.gameObject.name}");
            }


            agent = npc.GetComponentInParent<NavMeshAgent>();

            if (agent != null)
            {
                // no need to disable the agent
                agent.enabled = false;
            }
        }
    }

    #endregion


    #region Unity Callbacks

    void Start()
    {
        if (CameraTarget == null)
        {
            try
            {
                CameraTarget = FindObjectOfType<ThirdPersonCameraTarget>().transform;
            }
            catch { }
        }
    }


    void Update()
    {
        UpdateLookAngle();
        UpdateMovement();
        ApplyGravity();

        if (npc.Alive)
        {

            UpdateAiming();

            characterController.Move(npc.velocity * Time.deltaTime);
        }

        // if (agent != null && npc.Alive)
        // 	{
        // 		agent.isStopped = true;
        // 		agent.Warp(transform.position);
        // 	}
    }


    void LateUpdate()
    {
        CameraTarget.position = npc.transform.position + CameraOffset;
        CameraTarget.rotation = Quaternion.Lerp(CameraTarget.rotation, Quaternion.Euler(-LookAngle.y, LookAngle.x, 0f), LookSmoothing * Time.deltaTime);
    }

    #endregion


    #region Movement and Looking Method


    void ApplyGravity()
    {
        if (characterController.isGrounded == false)
        {
            npc.velocity.y += Physics.gravity.y * Time.deltaTime;
        }
        else
        {
            npc.velocity.y = -3f;
        }
    }



    void UpdateLookAngle()
    {
        if (inputEnabled == false)
        {
            return;
        }

        // 1. Process regular touch/joystick input
        LookAngle.x += LookInput.x * LookSensitivity.x;
        LookAngle.y += LookInput.y * LookSensitivity.y;

        // 2. BREAK RECOVERY: If player intentionally fights/moves looking inputs,
        // stop forcing historical recoil correction.
        if (Mathf.Abs(LookInput.y) > 0.01f)
        {
            totalRecoilAppliedY = 0f;
        }

        // 3. RECOIL RECOVERY PROCEDURAL LOOP
        // If there's recoil to compensate, no heavy stick input, and cooldown has passed
        if (totalRecoilAppliedY > 0f && Mathf.Abs(LookInput.y) < 0.01f && Time.time > lastShotTime + recoveryDelay)
        {
            float recoveryAmount = recoilRecoverySpeed * Time.deltaTime;

            // Protect from over-snapping past the origin target point
            recoveryAmount = Mathf.Min(recoveryAmount, totalRecoilAppliedY);

            LookAngle.y -= recoveryAmount;
            totalRecoilAppliedY -= recoveryAmount;
        }

        LookAngle.y = Mathf.Clamp(LookAngle.y, -70f, 70f);
    }


    void UpdateMovement()
    {
        if (inputEnabled == false || !npc.Alive)
        {
            return;
        }

        Vector3 forward = CameraTarget.forward;
        forward.y = 0f;
        forward.Normalize();


        Vector3 motion = forward * MoveInput.y + CameraTarget.right * MoveInput.x;
        motion.y = 0f;
        motion.Normalize();


        float targetSpeed = RunInput ? npc.RunSpeed : npc.WalkSpeed;

        float velocityX = Mathf.Lerp(npc.velocity.x, motion.x * targetSpeed, Time.deltaTime * MoveSmoothing);
        float velocityZ = Mathf.Lerp(npc.velocity.z, motion.z * targetSpeed, Time.deltaTime * MoveSmoothing);

        npc.velocity = new Vector3(velocityX, npc.velocity.y, velocityZ);

        if (motion.sqrMagnitude > 0.01f && !aiming)
        {
            // 
            //Quaternion targetRotation = Quaternion.Euler(0f, LookAngle.x, 0f);

            Quaternion targetRotation = Quaternion.LookRotation(motion);

            npc.transform.rotation = Quaternion.Lerp(npc.transform.rotation, targetRotation, MoveSmoothing * Time.deltaTime);
        }

    }

    #endregion

    public void ApplyCameraKick(float verticalKick, float horizontalKick)
    {
        if (!npc.Alive || !inputEnabled) return;

        // 1. Instantly punch tracking coordinates up/sway
        LookAngle.y += verticalKick;
        LookAngle.x += Random.Range(-horizontalKick, horizontalKick);

        // 2. Track total distance to climb back down later
        totalRecoilAppliedY += verticalKick;

        // 3. Log timestamp to dictate recovery delay threshold
        lastShotTime = Time.time;

        // Safety clamp boundary check
        LookAngle.y = Mathf.Clamp(LookAngle.y, -70f, 70f);
    }

}
