
using UnityEngine;
using System.Collections;
using System.Linq;

//The follower script follows the mayor as best as they can.
//They will path towards the mayor if they get too far away.
[RequireComponent(typeof(PathingCharacter))]
public class Follower : MonoBehaviour 
{
	PathingCharacter villager;
	Mayor mayor;
	
	void Start()
	{
		villager = GetComponent<PathingCharacter>();
		mayor = (Mayor)GameObject.FindObjectOfType(typeof(Mayor));
		
		StartCoroutine(Run());
	}
	
	IEnumerator Run()
	{	
		while (true)
		{
			//Villagers try to follow about 1 tile away
			Vector3 direction = (mayor.transform.position - transform.position).normalized;
			villager.FinalTarget = mayor.transform.position - direction;
			
			if (Vector3.Distance(villager.transform.position,mayor.transform.position) > 1.5f)
			{
				yield return StartCoroutine(villager.PathTo(Mathf.RoundToInt(mayor.transform.position.x), Mathf.RoundToInt(mayor.transform.position.z)));
				yield return new WaitForSeconds(0.2f);
			}
			
			yield return new WaitForEndOfFrame();
		}
	}
}
