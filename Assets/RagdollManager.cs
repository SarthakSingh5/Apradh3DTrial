using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RagdollManager : MonoBehaviour
{
    Rigidbody[] rbs;

    // Start is called before the first frame update
    void Start()
    {
        rbs = GetComponentsInChildren<Rigidbody>();
        foreach (Rigidbody rb in rbs)
            rb.isKinematic = true;
    }

    public void TriggerRagdoll()
    {
        // Disable the Animator so it doesn't override the ragdoll physics.
        Animator anim = GetComponent<Animator>();
        if (anim != null)
        {
            anim.enabled = false;
        }

        // Now enable physics on all child rigidbodies.
        foreach (Rigidbody rb in rbs)
        {
            rb.isKinematic = false;
        }
    }

}
