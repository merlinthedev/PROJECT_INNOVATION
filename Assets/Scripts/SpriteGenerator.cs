using System;
using UnityEditor;
using UnityEngine;

public class SpriteGenerator : MonoBehaviour {
    public Camera camera;
    public Vector2Int imageSize;
    public string spriteName;
    
    public void RenderSprite() {
        //save what the camera sees to a sprite asset in the root of our assets folder
        Texture2D texture = new Texture2D(imageSize.x, imageSize.y, TextureFormat.RGB24, false);
        Rect rect = new Rect(0, 0, imageSize.x, imageSize.y);
        texture.ReadPixels(rect, 0, 0);
        texture.Apply();
        byte[] bytes = texture.EncodeToPNG();
        string path = Application.dataPath + "/" + spriteName + ".png";
        System.IO.File.WriteAllBytes(path, bytes);
        
#if UNITY_EDITOR
        AssetDatabase.Refresh();
#endif
        
        Destroy(texture);
    }
}