using UnityEngine;
using System.Collections.Generic;

public class DetectionStateManager : MonoBehaviour
{
    [SerializeField] float lookDistance = 30f, fov = 120f;
    [SerializeField] Transform enemyEyes;
    [SerializeField] LayerMask detectionMask; // Layer mask for detecting NPCs and players
    [SerializeField] bool isEnemy; // True for enemy NPCs, false for user team NPCs

    public List<Transform> targets = new List<Transform>();

    void Start()
    {
        FindTargets();
    }

    private void FixedUpdate()
    {
        if (IsEnemyDetected())
        {
            Debug.Log("Enemy Spotted!"+ targets[0].name);
        }
    }

    void FindTargets()
    {
        targets.Clear();
        GameObject[] allNPCs = GameObject.FindGameObjectsWithTag("NPC");
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        foreach (GameObject npc in allNPCs)
        {
            if (npc.transform != transform) // Ignore self
            {
                DetectionStateManager npcScript = npc.GetComponent<DetectionStateManager>();
                if (npcScript && npcScript.isEnemy != isEnemy) // Detect only opposite teams
                {
                    targets.Add(npc.transform);
                }
            }
        }

        if (player != null && !isEnemy) // User NPCs detect player
        {
            targets.Add(player.transform);
        }
    }

    public bool IsEnemyDetected()
    {
        foreach (Transform target in targets)
        {
            if (Vector3.Distance(enemyEyes.position, target.position) > lookDistance)
                return false;

            Vector3 dirToTarget = (target.position - enemyEyes.position).normalized;
            float angleToTarget = Vector3.Angle(enemyEyes.forward, dirToTarget);

            if (angleToTarget > (fov / 2))
                return false;

            RaycastHit hit;
            if (Physics.Raycast(enemyEyes.position, dirToTarget, out hit, lookDistance, detectionMask))
            {
                Debug.DrawLine(enemyEyes.position, hit.point, Color.green);

                if (hit.transform == target)
                {
                    return true;
                }
            }
        }
    return false;

    }
}
