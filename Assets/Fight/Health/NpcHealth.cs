using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class NpcHealth : NpcComponent, IDamageable, IHealth
{
	[SerializeField]
	protected float points = 100.0f;

	public float Points
	{
		set
		{
			points = value;

			if (alive && points <= 0f)
			{
				alive = false;

				npc.Died?.Invoke();
			}
			else if (points > 0f && !npc.Alive)
			{
				alive = true;

				npc.Revived?.Invoke();
			}
		}

		get
		{
			return points;
		}
	}

	[SerializeField]
	protected float maxPoints = 100.0f;

	public float MaxPoints
	{
		get { return maxPoints; }
		set { maxPoints = Mathf.Max(value, 0f); }
	}


	protected bool alive = true;

	public bool Alive => alive;


	public override void SetNpc(Npc npc)
	{
		base.SetNpc(npc);

		npc.health = this;
	}


	public void Damage(DamageInfo damageInfo)
	{
		Points -= damageInfo.Damage;
		Debug.Log($"{npc.name} took {damageInfo.Damage} damage, remaining points: {Points}");	}


	#region Testing Methods

	[ContextMenu("Test Kill")]
	void TestKill()
	{
		Points = 0f;
	}

	[ContextMenu("Test Revive")]
	void TestRevive()
	{
		Points = 1f;
	}

	[ContextMenu("Test Damage")]
	void TestDamage()
	{
		Points -= 10f;
	}

	#endregion
}

