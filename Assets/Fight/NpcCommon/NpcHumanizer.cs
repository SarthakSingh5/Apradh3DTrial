using UnityEngine;

public class NpcHumanizer : MonoBehaviour
{
    private Npc npc;

    [Header("Bone References")]
    public Transform spineBone;
    public Transform neckBone;

    [Header("Suppression (Player Driven)")]
    public float maxSpineCrunch = 15f; 
    public float maxNeckCrunch = 20f;  

    [Header("Human Idle (Bystander Movement)")]
    public float headMoveSpeed = 3f;
    public float spineShiftSpeed = 1.5f;

    private Vector3 targetNeckOffset;
    private Vector3 currentNeckOffset;
    private float nextNeckChangeTime;

    private Vector3 targetSpineOffset;
    private Vector3 currentSpineOffset;
    private float nextSpineChangeTime;

    private void Awake()
    {
        npc = GetComponent<Npc>();
    }

    private void LateUpdate()
    {
        if (npc == null) return;

        float time = Time.time;

        // --- 1. INTENT-DRIVEN HEAD MOVEMENT ---
        // Humans don't pan like security cameras. They look at a spot, hold, and shift.
        if (time > nextNeckChangeTime && !npc.Aiming)
        {
            // Pick a new subtle angle to look at
            float randomPitch = Random.Range(-5f, 12f); // Look slightly up or down
            float randomYaw = Random.Range(-30f, 30f);  // Look left or right
            targetNeckOffset = new Vector3(randomPitch, randomYaw, 0f);
            
            // Hold this gaze for 1.5 to 4 seconds
            nextNeckChangeTime = time + Random.Range(1.5f, 4.0f);
        }
        else if (npc.Aiming)
        {
            // Lock the head straight forward when aiming down sights
            targetNeckOffset = Vector3.zero; 
        }

        // --- 2. INTENT-DRIVEN WEIGHT SHIFTING (SPINE) ---
        if (time > nextSpineChangeTime)
        {
            // Subtly shift weight (lean slightly forward/back, or side to side)
            float shiftX = Random.Range(-3f, 5f);
            float shiftZ = Random.Range(-3f, 3f);
            targetSpineOffset = new Vector3(shiftX, 0f, shiftZ);

            // Hold this posture for 2 to 5 seconds
            nextSpineChangeTime = time + Random.Range(2.0f, 5.0f);
        }

        // Smoothly ease into the new targets (Lerp completely prevents robotic vibration)
        currentNeckOffset = Vector3.Lerp(currentNeckOffset, targetNeckOffset, Time.deltaTime * headMoveSpeed);
        currentSpineOffset = Vector3.Lerp(currentSpineOffset, targetSpineOffset, Time.deltaTime * spineShiftSpeed);

        // --- 3. COMBINE WITH SUPPRESSION CRUNCH ---
        float crunchMultiplier = npc.combatPressure;
        Vector3 finalSpineOffset = currentSpineOffset + new Vector3(maxSpineCrunch * crunchMultiplier, 0, 0);
        Vector3 finalNeckOffset = currentNeckOffset + new Vector3(maxNeckCrunch * crunchMultiplier, 0, 0);

        // --- 4. APPLY TO BONES ---
        if (spineBone != null)
        {
            spineBone.localRotation *= Quaternion.Euler(finalSpineOffset);
        }

        if (neckBone != null)
        {
            neckBone.localRotation *= Quaternion.Euler(finalNeckOffset);
        }
    }
} 