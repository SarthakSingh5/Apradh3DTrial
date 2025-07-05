using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcRagdoll : NpcComponent
{
	[SerializeField]
	[Tooltip("The Root of the ragdoll, used when scanning.")]
	GameObject RagdollRoot;

	[Header("Components (use context menu to auto set)")]
	[SerializeField]
	List<Collider> RagdollColliders;

	[SerializeField]
	List<Rigidbody> RagdollBodies;


	public override void SetNpc(Npc npc)
	{
		base.SetNpc(npc);

		npc.Revived += OnRevived;
		npc.Died += OnDied;
	}


	void OnDied()
	{
		SetRagdollActive(true);


		Vector3 velocity = npc.velocity;

		foreach (Rigidbody rigidbody in RagdollBodies)
		{
			rigidbody.linearVelocity = velocity;
		}

	}

	void OnRevived()
	{
		SetRagdollActive(false);
	}


	private void Start()
	{
		SetRagdollActive(false);
	}


	[ContextMenu("Scan Ragdoll Components")]
	void ScanRagdollComponents()
	{
		if (RagdollRoot == null)
		{
			Debug.LogError("RagdollRoot is null, please set it before scanning.");
		}

		RagdollColliders = new List<Collider>(RagdollRoot.GetComponentsInChildren<Collider>());
		RagdollBodies = new List<Rigidbody>(RagdollRoot.GetComponentsInChildren<Rigidbody>());
	}


	void SetCharacterControllerEnabled(bool enabled)
	{
		var cc = npc.GetComponent<CharacterController>();

		if (cc == null)
		{
			return;
		}

		cc.enabled = enabled;
	}


	public void SetRagdollActive(bool active)
	{
		// toggling the character controller
		SetCharacterControllerEnabled(!active);

		// toggling the animator
		npc.anim.enabled = !active;

		// we don't need to disable colliders on the ragdoll
		// in fact, these colliders are useful for limb-based damage
		//
		//foreach (var c in RagdollColliders)
		//{
		//	c.enabled = active;
		//}

		// toggling the kinematic state of rigidbodies
		foreach (var b in RagdollBodies)
		{
			b.isKinematic = active ? false : true;
		}
	}

}
