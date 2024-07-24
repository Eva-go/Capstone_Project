using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdatableData : ScriptableObject
{
    public event System.Action OnValueUpdated;
    public bool autoUpdate;

    protected virtual void OnValidate()
    {
        #if UNITY_EDITOR
        if (autoUpdate)
        {
            UnityEditor.EditorApplication.update += NotifyOfUpdatedValues;
        }
        #endif
    }

    public void NotifyOfUpdatedValues()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.update -= NotifyOfUpdatedValues;
        if (OnValueUpdated != null)
        {
            OnValueUpdated();
        }
        #endif
    }
}
