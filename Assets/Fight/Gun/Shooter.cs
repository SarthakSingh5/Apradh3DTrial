using UnityEngine;


public class Shooter : ScriptableObject
{
    [Header("Firing")]
    [SerializeField]
    protected float MaxRange = 30f;

    [Header("Physics")]
    [SerializeField]
    protected float HitImpulse = 200f;

    [Header("Damage")]
    [SerializeField]
    public float Damage = 10.0f;

    [SerializeField]
    public float DamageRandom = 5f;

    [Header("Accuracy")]
    protected float MaxSpreadAngle = 5f;


    [Header("Components")]
    [SerializeField]
    public Transform muzzle;

    [SerializeField]
    protected ParticleSystem MuzzleFlashFX;

    [SerializeField]
    protected GameObject bullet;

    [SerializeField]
    protected float bulletVelocity;

    [SerializeField]
    protected int bulletsPerShot;

    [SerializeField]
    protected LayerMask HitMask = Physics.DefaultRaycastLayers;


    #region Tracer

    [SerializeField]
    protected LineRenderer tracerPrefab;

    [SerializeField]
    protected float tracerSpeed = 100f;

    [SerializeField]
    protected float tracerLifetime = 0.1f;

    #endregion

    public virtual void Shoot(Transform muzzle)
    {
        

    }

    void PlayMuzzleFlashFX()
    {
        if (MuzzleFlashFX == null)
        {
            return;
        }

        if (MuzzleFlashFX.isPlaying)
        {
            MuzzleFlashFX.Stop();
        }

        MuzzleFlashFX.Play();
    }
}
