using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


[CustomEditor (typeof (ScriptableObject_Station))]
public class ScriptableObject_Station_Editor : Editor
{

    public override void OnInspectorGUI()
    {
        //base.OnInspectorGUI();
        serializedObject.Update();

        ScriptableObject_Station EditorObject = (ScriptableObject_Station)target;

        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.Label("Input");
        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();

        Texture texture = null;

        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();

        EditorGUILayout.BeginVertical();
        texture = null;
        if (EditorObject.Input001 != null)
        {
            texture = EditorObject.Input001.ItemSprite.texture;
        }
        GUILayout.Box(texture, GUILayout.Width(150), GUILayout.Height(150));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("Input001"), GUIContent.none, true, GUILayout.Width(150));
        EditorGUILayout.EndVertical();

        EditorGUILayout.BeginVertical();
        texture = null;
        if (EditorObject.Input002 != null)
        {
            texture = EditorObject.Input002.ItemSprite.texture;
        }
        GUILayout.Box(texture, GUILayout.Width(150), GUILayout.Height(150));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("Input002"), GUIContent.none, true, GUILayout.Width(150));
        EditorGUILayout.EndVertical();

        EditorGUILayout.BeginVertical();
        texture = null;
        if (EditorObject.Input003 != null)
        {
            texture = EditorObject.Input003.ItemSprite.texture;
        }
        GUILayout.Box(texture, GUILayout.Width(150), GUILayout.Height(150));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("Input003"), GUIContent.none, true, GUILayout.Width(150));
        EditorGUILayout.EndVertical();

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.Label("Output");
        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();

        EditorGUILayout.BeginVertical();
        texture = null;
        if (EditorObject.Output001 != null)
        {
            texture = EditorObject.Output001.ItemSprite.texture;
        }
        GUILayout.Box(texture, GUILayout.Width(150), GUILayout.Height(150));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("Output001"), GUIContent.none, true, GUILayout.Width(150));
        EditorGUILayout.EndVertical();

        EditorGUILayout.BeginVertical();
        texture = null;
        if (EditorObject.Output002 != null)
        {
            texture = EditorObject.Output002.ItemSprite.texture;
        }
        GUILayout.Box(texture, GUILayout.Width(150), GUILayout.Height(150));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("Output002"), GUIContent.none, true, GUILayout.Width(150));
        EditorGUILayout.EndVertical();

        EditorGUILayout.BeginVertical();
        texture = null;
        if (EditorObject.Output003 != null)
        {
            texture = EditorObject.Output003.ItemSprite.texture;
        }
        GUILayout.Box(texture, GUILayout.Width(150), GUILayout.Height(150));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("Output003"), GUIContent.none, true, GUILayout.Width(150));
        EditorGUILayout.EndVertical();

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.PropertyField(serializedObject.FindProperty("ProgressTime"), true);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("Temperture"), true);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("Coolent"), true);

        serializedObject.ApplyModifiedProperties();
    }
}
