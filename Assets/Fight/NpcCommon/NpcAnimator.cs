using UnityEngine;

public class NpcAnimator : NpcComponent
{
    private float lastYRotation;
    public float targetPosture = 1.0f;
    private float currentPosture = 1.0f; 
    public float currentYRotation;
    public float turnSpeed;
    
    // NEW: Tracks how long the NPC has been sitting still
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

        // --- 3. POSTURE ---
        // (CRITICAL: You must default to 1.0f every frame so they stand back up when leaving cover!)
        
        if (npc.inCover || npc.currentAimMode == AimMode.ForceLower)
        {
            targetPosture = 0.0f; 
        }

        currentPosture = Mathf.Lerp(currentPosture, targetPosture, Time.deltaTime * 5f);
        npc.anim.SetFloat("Posture", currentPosture);

        // --- 4. RELAXATION (The New Logic) ---
        // Check if the NPC's movement and turning speeds are practically zero
        bool isStill = Mathf.Abs(forwardSpeed) < 0.1f && Mathf.Abs(rightSpeed) < 0.1f && turnParam< 0.1f;

        // If completely still, heavily crouched, and not actively aiming
        if (isStill && currentPosture < 0.5f) 
        {
            // Divide by 4f to make the transition take 2 full seconds of sitting still
            timeAtRest += Time.deltaTime;
        }
        else
        {
            // Instantly snap to high-alert combat ready pose the moment they move or aim
            timeAtRest = 0f; 
        }

        // Clamp between 0 (Combat Ready) and 1 (Fully Relaxed) and send to Blend Tree
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