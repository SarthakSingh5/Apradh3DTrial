using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcComponent : MonoBehaviour
{
	public Npc npc;
	public Dog dog;

	public Weapon weapon;

	protected virtual void Awake()
	{
		SetNpc(GetComponentInParent<Npc>());
		dog = npc.GetComponentInChildren<Dog>();
		weapon = npc.GetComponentInChildren<Weapon>();
	}

	public virtual void SetNpc(Npc npc)
	{
		this.npc = npc;
	}
}
