using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(LSystem))]
public class LSystemEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        LSystem lsystem = (LSystem)target;
        if(GUILayout.Button("Generate"))
        {
            lsystem.GenerateSystem();
        }
    }
}