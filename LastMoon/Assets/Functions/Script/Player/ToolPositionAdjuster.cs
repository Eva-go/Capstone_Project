using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolPositionAdjuster : MonoBehaviour
{
    public Transform Tool; // 무기 오브젝트
    public Transform cameraTransform; // 카메라 오브젝트
    public float weaponOffset = 0.2f; // 카메라로부터 무기의 오프셋

    void Update()
    {
        Vector3 weaponPosition = cameraTransform.position + cameraTransform.forward * weaponOffset;
        Tool.position = weaponPosition;
    }
}
