using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] float timeToDestroy;
    [HideInInspector] public WeaponManager weapon;
    [HideInInspector] public Vector3 dir;


    // Start is called before the first frame update
    void Start()
    {
        Destroy(this.gameObject, timeToDestroy);
    }

    // Update is called once per frame


    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.GetComponentInParent<AiHealth>())
        {
            AiHealth npcHealth = collision.gameObject.GetComponentInParent<AiHealth>();
            npcHealth.TakeDamage(weapon.damage);

            if(npcHealth.health <= 0 && npcHealth.isDead == false)
            {
                Rigidbody rb = collision.gameObject.GetComponentInChildren<Rigidbody>();
                rb.AddForce(dir * weapon.enemyKickbackForce, ForceMode.Impulse);
                npcHealth.isDead = true;
            }
        }
        Destroy(this.gameObject);
    }
}
