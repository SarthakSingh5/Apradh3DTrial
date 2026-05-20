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

    private float totalRecoilAppliedX = 0f; // Tracks horizontal shifting so we can recover it
    [SerializeField] private float horizontalRecoverySpeed = 10f; // Speed at which the view centers back up

    private float currentCameraRoll = 0f; // Tracks current Z-axis tilt
    [SerializeField] private float rollReturnSpeed = 20f; // How fast the screen flattens back out

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

        // Pass -LookAngle.y (Pitch), LookAngle.x (Yaw), and currentCameraRoll (Roll)
        Quaternion targetRotation = Quaternion.Euler(-LookAngle.y, LookAngle.x, currentCameraRoll);

        CameraTarget.rotation = Quaternion.Lerp(CameraTarget.rotation, targetRotation, LookSmoothing * Time.deltaTime);
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
        if (inputEnabled == false) return;

        // 1. Standard Touch/Mouse Input
        LookAngle.x += LookInput.x * LookSensitivity.x;
        LookAngle.y += LookInput.y * LookSensitivity.y;

        // Break memories if the player manually fights the camera adjustment
        if (Mathf.Abs(LookInput.y) > 0.01f) totalRecoilAppliedY = 0f;
        if (Mathf.Abs(LookInput.x) > 0.01f) totalRecoilAppliedX = 0f;

        // 2. The Shared Cooldown Window Threshold Check
        if (Time.time > lastShotTime + recoveryDelay)
        {
            // Recover Vertical (Y)
            if (totalRecoilAppliedY > 0f && Mathf.Abs(LookInput.y) < 0.01f)
            {
                float recoveryAmountY = recoilRecoverySpeed * Time.deltaTime;
                recoveryAmountY = Mathf.Min(recoveryAmountY, totalRecoilAppliedY);
                LookAngle.y -= recoveryAmountY;
                totalRecoilAppliedY -= recoveryAmountY;
            }

            // Recover Horizontal Centering (X)
            if (totalRecoilAppliedX != 0f && Mathf.Abs(LookInput.x) < 0.01f)
            {
                // MoveTowards brings the tracking state smoothly closer to 0
                float targetXMemory = Mathf.MoveTowards(totalRecoilAppliedX, 0f, horizontalRecoverySpeed * Time.deltaTime);

                // Calculate the exact change applied this frame and apply it to our real angle
                float deltaX = totalRecoilAppliedX - targetXMemory;
                LookAngle.x -= deltaX;
                totalRecoilAppliedX = targetXMemory;
            }
        }
        // Smoothly flatten Camera Roll (Z-axis) back to 0 constantly every frame
        currentCameraRoll = Mathf.MoveTowards(currentCameraRoll, 0f, rollReturnSpeed * Time.deltaTime);

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

    public void ApplyCameraKick(float verticalKick, float horizontalKick, float rollKick)
    {
        if (!npc.Alive || !inputEnabled) return;

        // 1. Vertical & Horizontal Forces
        LookAngle.y += verticalKick;
        totalRecoilAppliedY += verticalKick;

        float randomHoriz = Random.Range(-horizontalKick, horizontalKick);
        LookAngle.x += randomHoriz;
        totalRecoilAppliedX += randomHoriz;

        // 2. Camera Roll (Randomly twist left or right per bullet)
        currentCameraRoll += Random.Range(-rollKick, rollKick);

        // 3. Housekeeping Clamps and Timers
        LookAngle.y = Mathf.Clamp(LookAngle.y, -70f, 70f);
        lastShotTime = Time.time;
    }

}
