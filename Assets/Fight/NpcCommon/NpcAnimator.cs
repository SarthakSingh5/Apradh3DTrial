using UnityEngine;

public class NpcAnimator : NpcComponent
{
    private float lastYRotation;
    public float targetPosture = 1.0f;
    private float currentPosture = 1.0f; 
    public float currentYRotation;
    public float turnSpeed;


    private void Start()
    {
        lastYRotation = npc.transform.eulerAngles.y;
    }

    // CHANGE UPDATE TO LATEUPDATE
    private void LateUpdate() 
    {
        // --- 1. MOVEMENT ---
        float forwardSpeed = Vector3.Dot(npc.velocity, npc.transform.forward);
        float rightSpeed = Vector3.Dot(npc.velocity, npc.transform.right);

        npc.anim.SetFloat("Speed", forwardSpeed);
        npc.anim.SetFloat("StrafeSpeed", rightSpeed);

        // --- 2. TURNING (The Robot Fix) ---
        currentYRotation = npc.transform.eulerAngles.y;
        turnSpeed = Mathf.DeltaAngle(lastYRotation, currentYRotation) / Time.deltaTime;
        lastYRotation = currentYRotation;
        
        // Divide by 40f instead of 100f. This makes the legs hyper-sensitive to camera movement.
        // A turn speed of just 20 degrees per second will now hit your 0.5 threshold perfectly.
        float turnParam = Mathf.Clamp(turnSpeed / 40f, -0.5f, 0.5f);
        
        // Increased Lerp speed to 15f so the legs react to the mouse instantly
        float currentAnimTurn = npc.anim.GetFloat("Turn");
        npc.anim.SetFloat("Turn", Mathf.Lerp(currentAnimTurn, turnParam, Time.deltaTime * 1f));


        // --- 3. POSTURE ---
        targetPosture = 1.0f;
        if (npc.inCover || npc.currentAimMode == AimMode.ForceLower)
        {
            targetPosture = 0.0f; 
        }

        currentPosture = Mathf.Lerp(currentPosture, targetPosture, Time.deltaTime * 5f);
        npc.anim.SetFloat("Posture", currentPosture);


        // --- 4. UPPER BODY ---
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