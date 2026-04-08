using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Animations.Rigging;
using UnityEngine.Events;

[SelectionBase]
[RequireComponent(typeof(Animator))]
public class Npc : MonoBehaviour
{
    [HideInInspector] public Animator anim;

    public float WalkSpeed = 3.5f;
    public float RunSpeed = 7f;

    public Vector3 velocity = Vector3.zero;
    public Vector3 direction = Vector3.zero;
    public bool canTurn = true;
    public bool canMove = false;
    public bool inCover = false;

    public Vector3 SensorPosition
    {
        get
        {
            return transform.position + Vector3.up * 1.5f;
        }
    }


    #region Locomotion Delegates

    public UnityAction<Vector3> SetDestination;

    // after setting destination, this returns whether there is a path or not.
    public System.Func<bool> HasPathToDestination;

    public UnityAction<float> SetMaxSpeed;


    /// <summary>
    /// Look at a point in world space.
    /// </summary>
    public UnityAction<Vector3> LookAt;

    public UnityAction LookForward;

    /// <summary>
    /// Returns true if looking at point in world space.
    /// </summary>
    public System.Func<Vector3, bool> IsLookingAt;



    #endregion


    #region Health

    public IHealth health;

    public bool Alive
    {
        get
        {
            if (health == null)
            {
                return true;
            }

            return health.Alive;
        }
    }

    public UnityAction Died;
    public UnityAction Revived;

    #endregion


    #region Gun Properties and Delegates

    public Shooter shooter;

    public bool carryingGun = true;


    private bool aiming = false;

    public void SetAim(bool aim)
    {
        if (carryingGun)
        {
            this.aiming = aim;
        }
        else
        {
            this.aiming = false;
        }
    }

    public bool Aiming => aiming;


    public UnityAction TryShoot;
    public UnityAction NotShoot;

    #endregion


    public UnityAction OnTaskComplete;

    public bool canShoot = false;


    private Vector3 lastVelocity;
    public float currentBloom;
    private float recoilBloom;


    public bool isPanicking = false;

    [Header("Weapon System")]
    public Weapon weapon; // Centralized reference




    protected void Awake()
    {
        if (anim == null)
        {
            anim = GetComponent<Animator>();
        }

        if (anim == null)
        {
            Debug.LogError($"Animator not found on '{gameObject.name}'");
        }

        if (weapon == null)
        {
            weapon = GetComponentInChildren<Weapon>();
        }
    }

    private void Update()
    {
        CalculateDynamicBloom();
    }


    private void LateUpdate()
    {
        Vector3 horizontalVelocity = new Vector3(velocity.x, 0f, velocity.z);

        if (horizontalVelocity.sqrMagnitude > 3f)
        {
            direction = horizontalVelocity.normalized;
        }
    }

    public void AddRecoilBloom()
    {
        if (weapon != null)
        {
            // Pulling stats directly from the weapon (Shooter)
            recoilBloom += weapon.shooter.bloomPerShot;
        }
    }

    private void CalculateDynamicBloom()
    {
        // 0. Safety Check for Weapon Stats
        float recoveryRate = (weapon != null) ? weapon.shooter.bloomRecoveryRate : 5f;

        // 1. Instability (Speed)
        float speed = new Vector3(velocity.x, 0, velocity.z).magnitude;
        float velocityBloom = speed * 0.1f;

        // 2. Jerk (Acceleration)
        Vector3 acceleration = (velocity - lastVelocity) / Time.deltaTime;
        float jerkBloom = acceleration.magnitude * 0.05f;

        // 3. RECOIL BLOOM LOGIC
        // This moves the heat back to 0 based on the weapon's recovery rate
        recoilBloom = Mathf.MoveTowards(recoilBloom, 0f, Time.deltaTime * recoveryRate);

        // 4. Update the state
        // We add jerk and recoil heat to the existing bloom
        currentBloom += (jerkBloom); 

        // 5. Recovery & Combination
        // We Lerp the total bloom toward the current velocity-based baseline + heat
        float targetBloom = velocityBloom + recoilBloom;
        currentBloom = Mathf.Lerp(currentBloom, targetBloom, Time.deltaTime * 5f);

        lastVelocity = velocity;

        // 6. Clamp it!
        // Increased max clamp to 3.0 to allow for heavy recoil sprays
        currentBloom = Mathf.Clamp(currentBloom, 0f, 3.0f);
    }

}
