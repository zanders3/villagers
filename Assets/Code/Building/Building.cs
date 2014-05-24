using UnityEngine;
using System.Collections;

public class Building : MonoBehaviour 
{
	public Tile BuildingType;
	
	public int TotalHealth = 5;
	public int Width = 1, Height = 1;
	
	private int health = 0;
	
	public int Health
	{
		get { return health; }
	}
	
	public static Building TownHall
	{
		get;
		private set;
	}
	
	public int Tx
	{
		get { return Mathf.RoundToInt(transform.position.x); }
	}
	
	public int Ty
	{
		get { return Mathf.RoundToInt(transform.position.z); }
	}
	
	public bool IsBuilt
	{
		get;
		private set;
	}
	
	void Start()
	{
		Map.Instance.PlaceBuilding(BuildingType, Tx, Ty, Width, Height);
		
		//Town halls start off already built
		/*if (BuildingType == Tile.TownHall)
		{
			health = TotalHealth;
			IsBuilt = true;
			TownHall = this;
		}
		else*/
		{
			health = 0;
			IsBuilt = false;
		}
	}
	
	public void Damage(int amount)
	{
		health -= amount;
		if (health <= 0)
		{
			health = 0;
			Destroy(gameObject);
		}
	}
	
	public void Build()
	{
		if (health < TotalHealth)
		{
			health++;
			
			if (health >= TotalHealth)
				IsBuilt = true;
		}
	}
	
	void Update()
	{
		if (!IsBuilt || transform.position.y != 0.0f)
		{
			float percentBuilt = (float)health / (float)TotalHealth;
			transform.position = new Vector3(transform.position.x, Mathf.Lerp(transform.position.y, (percentBuilt - 1.0f) * 3.0f, Time.deltaTime), transform.position.z);
		}
	}
	
	void OnDrawGizmos()
	{
		float offsetX = (Width - 1) * 0.5f, offsetZ = (Height - 1) * 0.5f;
		
		if (health < TotalHealth)
		{
			Gizmos.color = Color.red;
			Gizmos.DrawCube(new Vector3(transform.position.x + offsetX, 2.5f, transform.position.z + offsetZ), new Vector3(health * 0.1f, 0.1f, 0.1f));
			
			Gizmos.color = Color.black;
			Gizmos.DrawWireCube(new Vector3(transform.position.x + offsetX, 2.5f, transform.position.z + offsetZ), new Vector3(TotalHealth * 0.1f, 0.1f, 0.1f));
		}
		
		if (!IsBuilt)
		{
			Gizmos.color = Color.blue;
			Gizmos.DrawCube(new Vector3(transform.position.x, 0.0f, transform.position.z), new Vector3(1.0f, 0.01f, 1.0f));
		}
	}
}
