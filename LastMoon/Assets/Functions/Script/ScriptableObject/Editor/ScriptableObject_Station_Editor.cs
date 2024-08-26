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
        EditorGUILayout.PropertyField(serializedObject.FindProperty("InputCount"), true);

        Texture texture = null;

        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();

        EditorGUILayout.BeginVertical();
        texture = null;
        if (EditorObject.Input[0] != null)
        {
            texture = EditorObject.Input[0].ItemSprite.texture;
        }
        GUILayout.Box(texture, GUILayout.Width(150), GUILayout.Height(150));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("Input").GetArrayElementAtIndex(0), GUIContent.none, true, GUILayout.Width(150));
        EditorGUILayout.EndVertical();

        EditorGUILayout.BeginVertical();
        texture = null;
        if (EditorObject.Input[1] != null)
        {
            texture = EditorObject.Input[1].ItemSprite.texture;
        }
        GUILayout.Box(texture, GUILayout.Width(150), GUILayout.Height(150));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("Input").GetArrayElementAtIndex(1), GUIContent.none, true, GUILayout.Width(150));
        EditorGUILayout.EndVertical();

        EditorGUILayout.BeginVertical();
        texture = null;
        if (EditorObject.Input[2] != null)
        {
            texture = EditorObject.Input[2].ItemSprite.texture;
        }
        GUILayout.Box(texture, GUILayout.Width(150), GUILayout.Height(150));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("Input").GetArrayElementAtIndex(2), GUIContent.none, true, GUILayout.Width(150));
        EditorGUILayout.EndVertical();

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.Label("Output");
        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.PropertyField(serializedObject.FindProperty("OutputCount"), true);

        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();

        EditorGUILayout.BeginVertical();
        texture = null;
        if (EditorObject.Output[0] != null)
        {
            texture = EditorObject.Output[0].ItemSprite.texture;
        }
        GUILayout.Box(texture, GUILayout.Width(150), GUILayout.Height(150));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("Output").GetArrayElementAtIndex(0), GUIContent.none, true, GUILayout.Width(150));
        EditorGUILayout.EndVertical();

        EditorGUILayout.BeginVertical();
        texture = null;
        if (EditorObject.Output[1] != null)
        {
            texture = EditorObject.Output[1].ItemSprite.texture;
        }
        GUILayout.Box(texture, GUILayout.Width(150), GUILayout.Height(150));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("Output").GetArrayElementAtIndex(1), GUIContent.none, true, GUILayout.Width(150));
        EditorGUILayout.EndVertical();

        EditorGUILayout.BeginVertical();
        texture = null;
        if (EditorObject.Output[2] != null)
        {
            texture = EditorObject.Output[2].ItemSprite.texture;
        }
        GUILayout.Box(texture, GUILayout.Width(150), GUILayout.Height(150));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("Output").GetArrayElementAtIndex(2), GUIContent.none, true, GUILayout.Width(150));
        EditorGUILayout.EndVertical();

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.PropertyField(serializedObject.FindProperty("ProgressTime"), true);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("Temperture"), true);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("Coolent"), true);

        serializedObject.ApplyModifiedProperties();
    }
}
