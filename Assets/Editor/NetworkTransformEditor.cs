using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(NetworkTransform))]
public class NetworkTransformEditor : Editor
{
    public override void OnInspectorGUI() {
        var networkTarget = (NetworkTransform)target;
        DrawDefaultInspector();
        EditorGUILayout.TextArea(networkTarget.key.ToString());
    }
}
