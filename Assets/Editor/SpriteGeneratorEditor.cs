using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SpriteGenerator))]
public class SpriteGeneratorEditor : Editor
{
    public override void OnInspectorGUI() {
        DrawDefaultInspector();

        SpriteGenerator myScript = (SpriteGenerator)target;
        if (GUILayout.Button("Render Sprite")) {
            myScript.RenderSprite();
        }
    }
}
