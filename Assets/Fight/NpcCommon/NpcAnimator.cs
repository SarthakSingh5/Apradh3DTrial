using UnityEngine;

public class NpcAnimator : NpcComponent
{
    private float lastYRotation;
    public float targetPosture = 1.0f;
    private float currentPosture = 1.0f;
    public float currentYRotation;
    public float turnSpeed;
    private float postureHoldTimer = 0f;
    private float lockedPosture = 1.0f;

    private float timeAtRest = 0f;

    private void Start()
    {
        lastYRotation = npc.transform.eulerAngles.y;
    }

    private void LateUpdate() 
    {
        // --- 1. MOVEMENT ---
        float forwardSpeed = Vector3.Dot(npc.velocity, npc.transform.forward);
        float rightSpeed = Vector3.Dot(npc.velocity, npc.transform.right);

        npc.anim.SetFloat("Speed", forwardSpeed);
        npc.anim.SetFloat("StrafeSpeed", rightSpeed);

        // --- 2. TURNING ---
        currentYRotation = npc.transform.eulerAngles.y;
        turnSpeed = Mathf.DeltaAngle(lastYRotation, currentYRotation) / Time.deltaTime;
        lastYRotation = currentYRotation;
        
        float turnParam = Mathf.Clamp(turnSpeed / 40f, -0.5f, 0.5f);
        float currentAnimTurn = npc.anim.GetFloat("Turn");
        npc.anim.SetFloat("Turn", Mathf.Lerp(currentAnimTurn, turnParam, Time.deltaTime * 1f));

        // --- 3. POSTURE (Dynamic Human Movement) ---
        targetPosture = 1.0f; 
        
        // 1. Natural human hunch when running or aiming
        if (forwardSpeed > 3f || npc.Aiming)
        {
            targetPosture = 0.85f; 
        }

        // Check if this character is the player
        bool isPlayer = npc.controller is PlayerController;

        // 2. Evaluate Pressure (ONLY FOR AI)
        // The player retains full manual control of their posture.
        if (!isPlayer)
        {
            if (npc.combatPressure > 0.5f)
            {
                lockedPosture = 0.2f; // Hard duck
                postureHoldTimer = 3.0f; // Sustain for 3 seconds
            }
            else if (npc.combatPressure > 0.1f && lockedPosture > 0.2f)
            {
                lockedPosture = 0.5f; // Moderate flinch
                
                if (postureHoldTimer < 1.5f) 
                {
                    postureHoldTimer = 1.5f; 
                }
            }
        }

        // 3. Apply the Sustain Timer (Also bypassed for the player)
        if (postureHoldTimer > 0f && !isPlayer)
        {
            postureHoldTimer -= Time.deltaTime;
            targetPosture = lockedPosture; 
        }
        else
        {
            lockedPosture = 1.0f; 
        }

        // 4. Manual Overrides (Cover / Ducking - Applies to both AI and Player)
        if (npc.inCover || npc.currentAimMode == AimMode.ForceLower)
        {
            targetPosture = 0.0f; 
        }

        // 5. Asymmetric Lerp (Fast drop, smooth rise)
        float postureLerpSpeed = (targetPosture < currentPosture) ? 15f : 4f;

        currentPosture = Mathf.Lerp(currentPosture, targetPosture, Time.deltaTime * postureLerpSpeed);
        npc.anim.SetFloat("Posture", currentPosture);

        // --- 4. RELAXATION ---
        bool isStill = Mathf.Abs(forwardSpeed) < 0.1f && Mathf.Abs(rightSpeed) < 0.1f && turnParam < 0.1f;

        if (isStill && currentPosture == 0f) 
        {
            timeAtRest += Time.deltaTime;
        }
        else
        {
            timeAtRest = 0f; 
        }

        float relaxationParam = Mathf.Clamp01(timeAtRest);
        npc.anim.SetFloat("Relaxation", relaxationParam);


        // --- 5. UPPER BODY ---
        if (npc.carryingGun)
        {
            npc.anim.SetLayerWeight(1, 1f);
        }
        else
        {
            npc.anim.SetLayerWeight(1, 0f);
        }

        npc.anim.SetBool("Aim", npc.Aiming);
    }
}