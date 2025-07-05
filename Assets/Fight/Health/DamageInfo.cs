using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public struct DamageInfo
{
	public float Damage;

	public static implicit operator DamageInfo(float damage)
	{
		return new DamageInfo { Damage = damage };
	}
}
