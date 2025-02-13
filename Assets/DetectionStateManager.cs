using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectionStateManager : MonoBehaviour
{
    [SerializeField] float lookDistance = 30, fov = 120;
    [SerializeField] Transform enemyEyes;
    Transform playerHead;
    GameManager gameManager;

    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        playerHead = gameManager.playerHead; // Using head instead of body
    }

    private void FixedUpdate()
    {
        if (PlayerSeen())
        {
            Debug.Log("GOTCHA");
        }
        else
        {
            Debug.Log("WHERED HE GO");
        }
    }

    public bool PlayerSeen()
    {
        if (Vector3.Distance(enemyEyes.position, playerHead.position) > lookDistance)
            return false;

        Vector3 dirToPlayer = (playerHead.position - enemyEyes.position).normalized;
        float angleToPlayer = Vector3.Angle(enemyEyes.parent.forward, dirToPlayer);
        

        if (angleToPlayer > (fov / 2))
            return false;

        RaycastHit hit;
        if (Physics.Raycast(enemyEyes.position, dirToPlayer, out hit, lookDistance))
        {
            Debug.DrawLine(enemyEyes.position, hit.point, Color.green);

            if (hit.transform == playerHead || hit.transform.root == playerHead.root)
            {
                return true;
            }
        }

        return false;
    }
}
