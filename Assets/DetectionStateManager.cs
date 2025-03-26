using UnityEngine;
using UnityEngine.Animations.Rigging;

public class DetectionStateManager : MonoBehaviour
{
    [SerializeField] float lookDistance = 30f, fov = 120f;
    [SerializeField] Transform npcEyes;
    [SerializeField] LayerMask detectionMask;
    [SerializeField] bool isEnemy;
    public Transform aimTarget;
    [SerializeField] Rig aimingRig;
    [SerializeField] float aimSmoothTime = 5f;


    private Transform currentTarget;
    private NPCHealth targetHealth;
    private bool hasTarget;


    void Update()
    {
        if (IsEnemyDetected(out Transform detectedTarget))
        {
            targetHealth = detectedTarget.GetComponent<NPCHealth>();

            if (targetHealth != null && targetHealth.isDead) // Skip dead targets
            {
                hasTarget = false;
                currentTarget = null;
            }
            else
            {
                Debug.Log(detectedTarget.name);
                currentTarget = detectedTarget;
                hasTarget = true;
            }
        }
        else
        {
            hasTarget = false;
        }

        UpdateAiming();
    }

    public bool IsEnemyDetected(out Transform detectedTarget)
    {
        detectedTarget = null;
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("NPC");

        foreach (GameObject enemy in enemies)
        {
            if (enemy.transform == transform) continue;

            DetectionStateManager enemyScript = enemy.GetComponent<DetectionStateManager>();
            if (enemyScript && enemyScript.isEnemy != isEnemy)
            {
                NPCHealth enemyHealth = enemy.GetComponent<NPCHealth>();
                if (enemyHealth != null && enemyHealth.isDead) continue; // Ignore dead NPCs

                Transform targetPoint = enemy.transform;

                if (Vector3.Distance(npcEyes.position, targetPoint.position) > lookDistance)
                    continue;

                Vector3 dirToTarget = (targetPoint.position - npcEyes.position).normalized;
                float angleToTarget = Vector3.Angle(npcEyes.forward, dirToTarget);

                if (angleToTarget > (fov / 2))
                    continue;

                RaycastHit hit;
                if (Physics.Raycast(npcEyes.position, dirToTarget, out hit, lookDistance, detectionMask))
                {
                    if (hit.transform == targetPoint)
                    {
                        detectedTarget = targetPoint;
                        return true;
                    }
                }
            }
        }
        return false;
    }

    void UpdateAiming()
    {
        if (hasTarget && currentTarget != null)
        {
            aimTarget.position = currentTarget.position;
            aimingRig.weight = Mathf.Lerp(aimingRig.weight, 1f, Time.deltaTime * aimSmoothTime);
        }
        else
        {
            aimingRig.weight = Mathf.Lerp(aimingRig.weight, 0f, Time.deltaTime * aimSmoothTime);
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Sound"))
        {
            Debug.Log(gameObject.name + " heard a sound from " + other.gameObject.name);
            // NPC reacts to the sound
        }
    }

}