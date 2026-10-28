using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiBullet : MonoBehaviour
{
    [SerializeField] float timeToDestroy = 5f;
    [HideInInspector] public AiWeaponManager weapon;
    [HideInInspector] public Vector3 dir;

    [Header("Tactical Pressure Settings")]
    [SerializeField] private float suppressionRadius = 3.5f; 
    [SerializeField] private LayerMask npcLayerMask;

    void Start()
    {
        Destroy(this.gameObject, timeToDestroy);
    }

    // --- 1. THE WHIZ-BY (ZERO PERFORMANCE COST) ---
    // This fires automatically if the NPC touches the bullet's invisible outer trigger zone
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Npc npc))
        {
            npc.AddPressure(0.15f);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // --- 2. DIRECT HIT ---
        if(collision.gameObject.GetComponentInParent<AiHealth>())
        {
            AiHealth npcHealth = collision.gameObject.GetComponentInParent<AiHealth>();
            npcHealth.TakeDamage(weapon.damage);

            if(npcHealth.health <= 0 && npcHealth.isDead == false)
            {
                Rigidbody rb = collision.gameObject.GetComponentInChildren<Rigidbody>();
                npcHealth.isDead = true;
            }
        }
        else 
        {
            // --- 3. IMPACT PRESSURE ---
            // This is perfectly safe for mobile because it only happens EXACTLY ONCE when the bullet dies, not every frame.
            Collider[] impactSuppression = Physics.OverlapSphere(transform.position, suppressionRadius, npcLayerMask);
            
            foreach (Collider col in impactSuppression)
            {
                if (col.TryGetComponent(out Npc npc)) 
                {
                    float distance = Vector3.Distance(transform.position, npc.transform.position);
                    float intensity = 1f - (distance / suppressionRadius);
                    
                    npc.AddPressure(intensity * 0.4f); 
                }
            }
        }
        
        Destroy(this.gameObject);
    }
}