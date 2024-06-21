using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolPositionAdjuster : MonoBehaviour
{
    public Transform Tool; // ���� ������Ʈ
    public Transform cameraTransform; // ī�޶� ������Ʈ
    public float weaponOffset = 0.2f; // ī�޶�κ��� ������ ������

    void Update()
    {
        Vector3 weaponPosition = cameraTransform.position + cameraTransform.forward * weaponOffset;
        Tool.position = weaponPosition;
    }
}
