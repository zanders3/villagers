using UnityEngine;
using System.Linq;

//Contains all of the game logic for the day/night cycle.
//Adjusts lighting, villager AI and gremlin spawning.
public class DayNightCycle : MonoBehaviour
{
	public const float DayInSeconds = 5.0f * 60.0f;//5 minutes per day
	
	private float currentTime = 0.0f;
	private bool wasDaytime = true;
	
	public Gradient SunColor = new Gradient();
	
	public Light Moon;
	public Gradient MoonColor = new Gradient();
	
	private float timeOffset = DayInSeconds * 0.1f;

	public static bool IsDaytime { get; private set; }

    void Start()
    {
        IsDaytime = true;
    }

	void SetDaytime(bool isDaytime)
	{
		IsDaytime = isDaytime;

		foreach (VillagerAIMode aiMode in GameObject.FindObjectsOfType(typeof(VillagerAIMode)).Cast<VillagerAIMode>())
            aiMode.OnStateChange();
	}

	void Update()
	{
		float days = (Time.timeSinceLevelLoad + timeOffset) / DayInSeconds;
		currentTime = days - Mathf.Floor(days);

		transform.rotation = Quaternion.AngleAxis(currentTime * 360.0f, new Vector3(1.0f, 0.0f, 0.0f));
		light.color = SunColor.Evaluate(currentTime);
		Moon.color = MoonColor.Evaluate(currentTime);
		
		bool isDaytime = currentTime <= 0.49f && currentTime >= 0.03f;
		if (wasDaytime != isDaytime)
		{
			wasDaytime = isDaytime;
			light.shadows = isDaytime ? LightShadows.Soft : LightShadows.None;
			Moon.shadows =  isDaytime ? LightShadows.None : LightShadows.Hard;
			SetDaytime(isDaytime);
		}
	}
	
	void OnGUI()
	{
		GUI.Label(new Rect(Screen.width - 80.0f, 10.0f, 80.0f, 20.0f), "Time: " + currentTime.ToString("0.000"));
		if (GUI.Button(new Rect(Screen.width - 80.0f, 30.0f, 80.0f, 20.0f), "Skip"))
		{
			if (currentTime < 0.5f)
				timeOffset += (0.5f - currentTime) * DayInSeconds;
			else
				timeOffset += (1.03f - currentTime) * DayInSeconds;
		}
	}
}

