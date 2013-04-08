using UnityEngine;
using System.Collections;

public class Idle : Coward
{
	protected override IEnumerator RunDaytime()
	{
		while (true)
			yield return StartCoroutine(WaitByCampfire("Idle"));
	}
}
