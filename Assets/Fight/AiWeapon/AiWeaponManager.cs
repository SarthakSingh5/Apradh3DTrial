using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiWeaponManager : MonoBehaviour
{
    [Header("Fire Rate")]
    [SerializeField] float fireRate;
    float fireRateTimer;
    [SerializeField] bool semiAuto;
    [SerializeField] float minFireRate = 0.2f;
    [SerializeField] float maxFireRate = 1.0f;
    [SerializeField] float maxRange = 50f;

    [Header("Bullet Properties")]
    [SerializeField] GameObject bullet;
    [SerializeField] Transform barrelPos;
    [SerializeField] float bulletVelocity;
    [SerializeField] int bulletsPerShot;
    public float damage = 20;
    AimStateManager aim;

    [SerializeField] AudioClip gunShot;
    [HideInInspector] public AudioSource audioSource;

    Light muzzleFlashLight;
    ParticleSystem muzzleFlashParticles;
    float lightIntensity;
    [SerializeField] float lightReturnSpeed = 20;

    public float enemyKickbackForce = 100;

    public Transform leftHandTarget, leftHandHint;
    WeaponClassManager weaponClass;
    [HideInInspector] public Transform aimTarget;
    [HideInInspector] public bool isFiring;

    void Start()
    {
        muzzleFlashLight = GetComponentInChildren<Light>();
        lightIntensity = muzzleFlashLight.intensity;
        muzzleFlashLight.intensity = 0;
        muzzleFlashParticles = GetComponentInChildren<ParticleSystem>();
        fireRateTimer = fireRate;
    }

    private void OnEnable()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (aimTarget != null)
        {
            float distance = Vector3.Distance(transform.position, aimTarget.position);
            fireRate = Mathf.Lerp(minFireRate, maxFireRate, distance / maxRange);
        }
        
        if (ShouldFire() && isFiring)
        {
            Fire();
        }
        muzzleFlashLight.intensity = Mathf.Lerp(muzzleFlashLight.intensity, 0, lightReturnSpeed * Time.deltaTime);
    }

    bool ShouldFire()
    {
        fireRateTimer += Time.deltaTime;
        return fireRateTimer >= fireRate;
    }

    void Fire()
    {
        fireRateTimer = 0;
        if (aimTarget != null)
        {
            barrelPos.LookAt(aimTarget.position);
        }

        audioSource.PlayOneShot(gunShot);
        TriggerMuzzleFlash();
        for (int i = 0; i < bulletsPerShot; i++)
        {
            GameObject currentBullet = Instantiate(bullet, barrelPos.position, barrelPos.rotation);
            AiBullet bulletScript = currentBullet.GetComponent<AiBullet>();
            bulletScript.weapon = this;
            bulletScript.dir = barrelPos.transform.forward;
            Rigidbody rb = currentBullet.GetComponent<Rigidbody>();
            rb.AddForce(barrelPos.forward * bulletVelocity, ForceMode.Impulse);
        }
    }

    public void SetFiring(bool firing)
    {
        isFiring = firing;
    }

    void TriggerMuzzleFlash()
    {
        muzzleFlashParticles.Play();
        muzzleFlashLight.intensity = lightIntensity;
    }
}
