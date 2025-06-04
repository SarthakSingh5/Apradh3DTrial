using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Animations.Rigging;
using UnityEngine.Events;


public class Npc : MonoBehaviour
{
    [HideInInspector] public Animator anim;

    public float WalkSpeed = 3.5f;
	public float RunSpeed = 7f;

    public Vector3 velocity = Vector3.zero;
    public Vector3 direction = Vector3.zero;
    public bool canTurn = true;
    public bool canMove = false;

    public UnityAction<Vector3> SetDestination;
    public UnityAction<float> SetMaxSpeed;

    public UnityAction<Vector3> LookAt;
    public UnityAction LookForward;
    public System.Func<Vector3, bool> IsLookingAt;

    public bool carryingGun = true;


    private bool aiming = false;
    public bool inCover = false;
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

    public Vector3 SensorPosition
    {
        get
        {
            return transform.position + Vector3.up * 1.5f;
        }
    }

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
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, Mathf.Infinity))
        {
            transform.position = hit.point; // Snap NPC to ground
        }
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
