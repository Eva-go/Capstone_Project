using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


//[CustomEditor (typeof (ScriptableObject_Item))]
public class ScriptableObject_Item_Editor : Editor
{
    ScriptableObject_Item EditorObject = null;

    private void OnEnable()
    {
        EditorObject = (ScriptableObject_Item)target;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();


        EditorGUILayout.BeginVertical();


        EditorObject.ItemSprite = (Sprite)EditorGUILayout.ObjectField("Sprite",
            EditorObject.ItemSprite, typeof(Sprite), true);

        EditorObject.ItemLU = (Texture)EditorGUILayout.ObjectField("Look-Up Texture",
            EditorObject.ItemLU, typeof(Texture), true);

        EditorObject.ItemLUM = (Material)EditorGUILayout.ObjectField("Look-Up Material",
            EditorObject.ItemLUM, typeof(Material), true);

        EditorGUILayout.EndVertical();
            
    }
}
