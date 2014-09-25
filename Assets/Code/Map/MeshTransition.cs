
using UnityEngine;
using UnityEditor;

public class MeshTransition : ScriptableObject
{
    [MenuItem("Assets/Create Mesh Transition")]
    public static void CreateTransition()
    {
        AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<MeshTransition>(), "Assets/MeshTransition.asset");
        AssetDatabase.Refresh();
    }
    
    public MeshFilter[] Fill, Side, Corner, InsetCorner;
}
