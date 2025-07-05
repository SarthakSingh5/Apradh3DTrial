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
    }

    private void Update()
    {
        
    }


    private void LateUpdate()
    {
        Vector3 horizontalVelocity = new Vector3(velocity.x, 0f, velocity.z);

        if (horizontalVelocity.sqrMagnitude > 3f)
        {
            direction = horizontalVelocity.normalized;
        }
    }
}
