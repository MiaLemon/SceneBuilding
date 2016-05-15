using UnityEngine;
using System.Collections;
using System.IO;
using UnityEditor;

public class SceneBuilderEditor : EditorWindow
{
    [MenuItem("Window/Serialize Selected Prefab")]
    static void SerializePrefab()
    {
        string json = SceneBuilder.PrefabSerializer.Save(Selection.activeGameObject);
        File.WriteAllText(Application.dataPath + "/SaveFiles/Prefab1.txt", json);
    }

    [MenuItem("Window/Load Prefab")]
    static void LoadPrefab()
    {
        string file = EditorUtility.OpenFilePanel("Load Prefab", Application.dataPath, "");
        string json = File.ReadAllText(file);
        SceneBuilder.PrefabSerializer.Load(json);
    }
}
