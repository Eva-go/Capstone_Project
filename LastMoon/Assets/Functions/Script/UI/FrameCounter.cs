using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrameCounter : MonoBehaviour
{
    private float deltaTime = 0f;
    private bool view;

    [SerializeField] private int size = 25;
    [SerializeField] private Color color = Color.red;



    private void Awake()
    {
        var obj = FindObjectsOfType<FrameCounter>();
        if (obj.Length == 1)
        {
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        view = false;
    }

    void Update()
    {
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
        if (Input.GetKeyDown(KeyCode.ScrollLock))
        {
            view = !view;
        }
    }

    private void OnGUI()
    {
        if (view)
        {
            GUIStyle style = new GUIStyle();

            Rect rect = new Rect(30, 30, Screen.width, Screen.height);
            style.alignment = TextAnchor.UpperLeft;
            style.fontSize = size;
            style.normal.textColor = color;

            float ms = deltaTime * 1000f;
            float fps = 1.0f / deltaTime;
            string text = string.Format("{0:0.} FPS ({1:0.0} ms)", fps, ms);

            GUI.Label(rect, text, style);
        }
    }
}