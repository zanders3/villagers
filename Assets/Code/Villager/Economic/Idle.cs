using UnityEngine;
using System.Collections;

public class Idle : Coward
{
	protected override IEnumerator RunDaytime()
	{
		yield return StartCoroutine(WaitByCampfire("Idle"));
	}
}
