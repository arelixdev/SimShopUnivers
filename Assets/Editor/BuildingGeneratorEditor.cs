#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BuildingGenerator))]
public class BuildingGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        BuildingGenerator gen = (BuildingGenerator)target;

        if (GUILayout.Button("Generate Buildings"))
        {
            gen.Generate();
        }

        if (GUILayout.Button("Clear"))
        {
            gen.Clear();
        }
    }
}
#endif
