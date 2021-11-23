using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(LSystemVariation))]
public class LSystemEditorVariation : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        LSystemVariation lsystem = (LSystemVariation)target;
        if(GUILayout.Button("Generate"))
        {
            lsystem.GenerateSystem();
        }
    }
}