﻿using System;
using UnityEditor;
using UnityEngine;

public class SpriteGenerator : MonoBehaviour {
    public Camera camera;
    public Vector2Int imageSize;
    public string spriteName;
    
    public void RenderSprite() {
        RenderTexture renderTexture = new RenderTexture(imageSize.x, imageSize.y, 24);
        camera.targetTexture = renderTexture;
        Texture2D texture = new Texture2D(imageSize.x, imageSize.y, TextureFormat.RGBA32, false);
        Rect rect = new Rect(0, 0, imageSize.x, imageSize.y);

        camera.Render();
        var activeTexture = RenderTexture.active;
        RenderTexture.active = renderTexture;
        texture.ReadPixels(rect, 0, 0);
        texture.Apply();

        camera.targetTexture = null;
        RenderTexture.active = activeTexture;
        Destroy(renderTexture);

        var sprite = Sprite.Create(texture, rect, new Vector2(0.5f, 0.5f));

        var path = EditorUtility.SaveFilePanelInProject("Save Sprite", spriteName, "png", "Save Sprite", "Assets");
        if (path.Length != 0) {
            var bytes = texture.EncodeToPNG();
            System.IO.File.WriteAllBytes(path, bytes);
            AssetDatabase.Refresh();
        }

        DestroyImmediate(texture);
    }
}