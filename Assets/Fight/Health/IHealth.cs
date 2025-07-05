using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public interface IHealth
{
	float Points { get; set; }

	float MaxPoints { get; set; }

	bool Alive { get; }
}
