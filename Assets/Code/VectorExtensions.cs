using UnityEngine;

public static class VectorExtensions
{
	public static Vector2 ToVector2(this Vector3 vector)
	{
		return new Vector2(vector.x, vector.z);
	}
	
	public static Vector3 ToVector3(this Vector2 vector)
	{
		return new Vector3(vector.x, 0.0f, vector.y);
	}
}
