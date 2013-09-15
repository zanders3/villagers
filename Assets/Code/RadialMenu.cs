using System;
using UnityEngine;

public enum MenuAction
{
    CreatePrefab,
    PackupBuilding,
    Close
}

[Serializable]
public class MenuItem
{
    public string Name;
    public GameObject Prefab;
    public MenuAction Action = MenuAction.CreatePrefab;
    const float Radius = 80.0f, Width = 50.0f;

    public bool DrawButton(Vector2 centre, float angle, Transform parent)
    {
        bool isClicked = GUI.Button(new Rect(
            (Mathf.Sin(Mathf.Deg2Rad * angle) * Radius) + centre.x, 
            (Mathf.Cos(Mathf.Deg2Rad * angle) * Radius) + centre.y,
            Width,
            Width
        ), Name);

        if (isClicked)
        {
            switch (Action)
            {
                case MenuAction.CreatePrefab:
                    GameObject crate = ((GameObject)GameObject.Instantiate(Prefab, parent.transform.position + Vector3.up * 5.0f, Quaternion.identity));
                    Vector3 vel = UnityEngine.Random.onUnitSphere * 5.0f;
                    vel.y = Mathf.Abs(vel.y);
                    crate.rigidbody.velocity = vel;
                    return true;

                default:
                    return true;
            }
        }

        return false;
    }
}

public class RadialMenu : MonoBehaviour
{
    //List of game objects to create (crates, villagers, etc.)
    public MenuItem[] Prefabs;

    private bool showingMenu = false, justShownMenu = false;

    public bool ShowingMenu { get { return showingMenu; } }

    public void OpenMenu()
    {
        Debug.Log("openmenu");
        showingMenu = true;
        justShownMenu = true;
    }

    void Update()
    {
        if (!showingMenu)
            return;

        if (justShownMenu)
            justShownMenu = false;
        else if (Input.GetMouseButtonUp(0))
            showingMenu = false;
    }

    void OnGUI()
    {
        if (!showingMenu)
            return;

        float interval = 360.0f / Prefabs.Length;
        Vector2 centre = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);

        for (int i = 0; i<Prefabs.Length; i++)
            if (Prefabs[i].DrawButton(centre, i * interval, transform))
                showingMenu = false;
    }
}
