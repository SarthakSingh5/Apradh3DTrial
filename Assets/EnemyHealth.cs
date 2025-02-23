using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCHealth : MonoBehaviour
{
    [SerializeField] public float health;
    RagdollManager ragdollManager;
    [HideInInspector] public bool isDead;

    public void Start()
    {
        ragdollManager = GetComponent<RagdollManager>();
    }

    public void TakeDamage(float damage)
    {
        if (health > 0)
        {
            health -= damage;
            Debug.Log("Hit");
            if (health <= 0)
                EnemyDeath();
        }
    }

    void EnemyDeath()
    {
        ragdollManager.TriggerRagdoll();
        isDead=true;
        Debug.Log("Death");
    }
}
