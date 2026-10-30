using UnityEngine;

// This forces the script to run after all Animators and IK solvers
[DefaultExecutionOrder(200)]
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
        if (npc == null || spineBone == null || neckBone == null) return;

        float time = Time.time;

        if (time > nextNeckChangeTime && !npc.Aiming)
        {
            // CRANKED TO 90 DEGREES FOR VISIBILITY TEST
            float randomPitch = Random.Range(-20f, 20f); 
            float randomYaw = Random.Range(-90f, 90f);  
            targetNeckOffset = new Vector3(randomPitch, randomYaw, 0f);
            
            nextNeckChangeTime = time + Random.Range(1.5f, 4.0f);
        }
        else if (npc.Aiming)
        {
            targetNeckOffset = Vector3.zero; 
        }

        if (time > nextSpineChangeTime)
        {
            // CRANKED SHIFT FOR VISIBILITY TEST
            float shiftX = Random.Range(-15f, 15f);
            float shiftZ = Random.Range(-15f, 15f);
            targetSpineOffset = new Vector3(shiftX, 0f, shiftZ);

            nextSpineChangeTime = time + Random.Range(2.0f, 5.0f);
        }

        currentNeckOffset = Vector3.Lerp(currentNeckOffset, targetNeckOffset, Time.deltaTime * headMoveSpeed);
        currentSpineOffset = Vector3.Lerp(currentSpineOffset, targetSpineOffset, Time.deltaTime * spineShiftSpeed);

        float crunchMultiplier = npc.combatPressure;
        Vector3 finalSpineOffset = currentSpineOffset + new Vector3(maxSpineCrunch * crunchMultiplier, 0, 0);
        Vector3 finalNeckOffset = currentNeckOffset + new Vector3(maxNeckCrunch * crunchMultiplier, 0, 0);

        spineBone.localRotation *= Quaternion.Euler(finalSpineOffset);
        neckBone.localRotation *= Quaternion.Euler(finalNeckOffset);
    }
} 