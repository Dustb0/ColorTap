using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(ColorTapButton))]
public class ChalleneButtonEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        // Additional code for the derived class...
    }
}
