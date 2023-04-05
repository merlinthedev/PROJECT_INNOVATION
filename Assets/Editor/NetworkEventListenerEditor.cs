using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(NetworkEventListener)), CanEditMultipleObjects]
public class NetworkEventListenerEditor : Editor
{
    public override void OnInspectorGUI() {
        var listener = (NetworkEventListener)target;
        DrawDefaultInspector();

        //add a dropdown menu to set which event to listen to
        var eventTypes = new List<string>();
        foreach (var eventType in NetworkEventBus.EventTypes) {
            eventTypes.Add(eventType.ToString());
        }

        var previousIndex = listener.networkEventIndex;
        var index = EditorGUILayout.Popup("Event Type", eventTypes.IndexOf(listener.networkEventType.ToString()), eventTypes.ToArray());
        listener.networkEventIndex = index;

        if (index != previousIndex) {
            //mark change dirty
            EditorUtility.SetDirty(listener);
        }
    }
}
