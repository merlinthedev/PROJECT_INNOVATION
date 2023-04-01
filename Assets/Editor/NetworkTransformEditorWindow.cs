using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class NetworkTransformEditorWindow : EditorWindow {
    [MenuItem("Window/NetworkTransform Editor")]
    public static void ShowWindow() {
        GetWindow<NetworkTransformEditorWindow>("NetworkTransform Editor");
    }

    private void OnGUI() {
        //check if the game is running
        if (Application.isPlaying) {
            var transforms = NetworkTransform.Transforms.Values.ToList();

            GUILayout.BeginVertical();
            //make a list of all transforms registered
            foreach (var transform in transforms) {
                GUILayout.BeginVertical();
                DrawTransform(transform);
                GUILayout.EndVertical();
                GUILayout.Space(10);
            }
            GUILayout.EndVertical();
        }
        else {
            GUILayout.Label("Game not running");
        }
    }

    private void DrawTransform(NetworkTransform transform) {
        //reference to the transform the component is on
        EditorGUILayout.ObjectField(transform.transform, typeof(Transform), true);

        //draw the key
        EditorGUILayout.TextArea(transform.key.ToString());

        //draw the position
        EditorGUILayout.TextArea(transform.transform.position.ToString());
        
    }
}
