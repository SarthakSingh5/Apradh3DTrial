using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcComponent : MonoBehaviour
{
	public Npc npc;
	public Dog dog;

	protected virtual void Awake()
	{
		SetNpc(GetComponentInParent<Npc>());
		dog = GetComponentInChildren<Dog>();
	}

	public virtual void SetNpc(Npc npc)
	{
		this.npc = npc;
	}
}
