using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class AGuidEditorWindow : EditorWindow {
    [MenuItem("Window/AGuidSource Editor")]
    public static void ShowWindow() {
        GetWindow<AGuidEditorWindow>("AGuidSource Editor");
    }

    private void OnGUI() {
        //check if the game is running
        if (Application.isPlaying) {
            GUILayout.Label("Game is running, cant edit GUID's now");
        }
        else {
            //button to assign a new key to all selected objects
            if (GUILayout.Button("Assign New Key")) {
                foreach (var obj in Selection.gameObjects) {
                    var GuidSources = obj.GetComponentsInChildren<AGuidSource>();
                    foreach (var GuidSource in GuidSources) {
                        GuidSource.NewKey();
                    }
                }
            }
            
            GUILayout.BeginVertical();
            //for each selected object that has a NetworkTransform component, draw the edit window
            foreach (var obj in Selection.gameObjects) {
                var transform = obj.GetComponentsInChildren<AGuidSource>();
                foreach (var t in transform) {
                    GUILayout.BeginVertical();
                    DrawEditTransform(t);
                    GUILayout.EndVertical();
                    GUILayout.Space(10);
                }
            }
            GUILayout.EndVertical();
        }
    }

    private void DrawTransform(AGuidSource transform) {
        //reference to the transform the component is on
        EditorGUILayout.ObjectField(transform.transform, typeof(Transform), true);

        //draw the key
        EditorGUILayout.TextArea(transform.key.ToString());

        //draw the position
        EditorGUILayout.TextArea(transform.transform.position.ToString());
        
    }

    private void DrawEditTransform(AGuidSource transform) {
        //reference to the transform the component is on
        EditorGUILayout.ObjectField(transform.transform, typeof(Transform), true);

        //draw the key
        EditorGUILayout.TextArea(transform.key.ToString());

        //draw the position
        EditorGUILayout.TextArea(transform.transform.position.ToString());

        //a button to assign a new GUID to the transform
        if (GUILayout.Button("New GUID")) {
            transform.key = System.Guid.NewGuid();
        }
    }
}
