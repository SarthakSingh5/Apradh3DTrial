using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] float timeToDestroy;
    [HideInInspector] public ProjectileShooter shooter;
    [HideInInspector] public Vector3 dir;
    
    // NEW: Tracks the physical gun or NPC that fired this bullet
    [HideInInspector] public Transform ownerTransform; 

    [Header("Tactical Pressure Settings")]
    [SerializeField] private float suppressionRadius = 3.5f; 
    [SerializeField] private LayerMask npcLayerMask;

    private Vector3 lastPosition;
    private List<Npc> suppressedNpcs = new List<Npc>();

    void Start()
    {
        lastPosition = transform.position;
        Destroy(this.gameObject, timeToDestroy);
    }

    void Update()
    {
        Collider[] nearMisses = Physics.OverlapCapsule(lastPosition, transform.position, suppressionRadius, npcLayerMask);
        
        foreach (Collider col in nearMisses)
        {
            if (col.TryGetComponent(out Npc npc))
            {
                // FIXED: We check against the new ownerTransform instead of the ScriptableObject
                if (ownerTransform != null && ownerTransform.IsChildOf(npc.transform)) continue;

                if (!suppressedNpcs.Contains(npc))
                {
                    npc.AddPressure(0.15f);
                    suppressedNpcs.Add(npc);
                }
            }
        }

        lastPosition = transform.position;
    }

    private void OnCollisionEnter(Collision collision)
    {
        IDamageable damageable = collision.collider.GetComponentInParent<IDamageable>();
        
        if (damageable != null)
        {
            float damage = shooter.Damage + Random.Range(0f, shooter.DamageRandom);
            damageable.Damage(damage);
        }
        else 
        {
            Collider[] impactSuppression = Physics.OverlapSphere(transform.position, suppressionRadius, npcLayerMask);
            
            foreach (Collider col in impactSuppression)
            {
                if (col.TryGetComponent(out Npc npc)) 
                {
                    // FIXED: We check against the new ownerTransform
                    if (ownerTransform != null && ownerTransform.IsChildOf(npc.transform)) continue;

                    float distance = Vector3.Distance(transform.position, npc.transform.position);
                    float intensity = 1f - (distance / suppressionRadius);
                    
                    npc.AddPressure(intensity * 0.4f); 
                }
            }
        }
        
        Destroy(this.gameObject);
    }
}