using System;
using UnityEngine;

/// <summary>
/// Controls level loading and setup and character spawning.
/// </summary>
public class LevelLogic : MonoBehaviour
{
    public TextAsset Level;
    private float villagerSpawnTimer = 0.0f;

    void Start()
    {
        Map.Instance.LoadLevel(Level.text);

        //The town hall can house 3 villagers
        GameObject townHall = GameObject.FindGameObjectWithTag("TownHall");
        IsAssigned.RegisterAssignment(townHall, "House", 3);

        //AILoader.CreateCharacter("Gremlin", "Gremlin", townHall.transform.position + new Vector3(5.0f, 0.0f, 0.0f));
        //AILoader.CreateCharacter("Villager", "Soldier", townHall.transform.position);
    }

    void Update()
    {
        //Villager spawning logic
        if (IsAssigned.CanRegister("Villager"))
        {
            if (villagerSpawnTimer <= 0.0f)
            {
                Debug.Log("CreateVillager");
                AILoader.CreateCharacter("Villager", "Villager", GameObject.FindGameObjectWithTag("TownHall").transform.position);
                villagerSpawnTimer = 20.0f;
            }
            else
                villagerSpawnTimer -= Time.deltaTime;
        }
    }
}
