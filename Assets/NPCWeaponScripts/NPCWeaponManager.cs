using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCWeaponManager : MonoBehaviour
{
    [Header("Fire Rate")]
    [SerializeField] float fireRate;
    float fireRateTimer;

    [Header("Bullet Properties")]
    [SerializeField] GameObject bullet;
    [SerializeField] Transform barrelPos;
    [SerializeField] float bulletVelocity;
    [SerializeField] int bulletsPerShot;
    public float damage = 20;


    [SerializeField] AudioClip gunShot;
    [HideInInspector] public AudioSource audioSource;

    DetectionStateManager detectionManager;

    void Start()
    {
        fireRateTimer = fireRate;
        detectionManager=GetComponentInParent<DetectionStateManager>();
    }

    private void OnEnable()
    {
        audioSource = GetComponent<AudioSource>();
    }



    // Update is called once per frame
    void Update()
    {
        if (ShouldFire()) Fire();
    }

    bool ShouldFire()
    {
        fireRateTimer += Time.deltaTime;
        if (fireRateTimer < fireRate) return false;
        if (Input.GetKey(KeyCode.Mouse0)) return true;

        return false;
    }


    public void Fire()
    {
        fireRateTimer = 0;
        barrelPos.LookAt(detectionManager.targets[0]);

        audioSource.PlayOneShot(gunShot);

        for (int i = 0; i < bulletsPerShot; i++)
        {
            GameObject currentBullet = Instantiate(bullet, barrelPos.position, barrelPos.rotation);

            NPCBullet bulletScript = currentBullet.GetComponent<NPCBullet>();
            bulletScript.weapon = this;

            bulletScript.dir = barrelPos.transform.forward;

            Rigidbody rb = currentBullet.GetComponent<Rigidbody>();
            rb.AddForce(barrelPos.forward * bulletVelocity, ForceMode.Impulse);
        }

    }


}
