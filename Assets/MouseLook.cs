using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLook : MonoBehaviour
{
    public float sensitivity = 2.5f;
    float yaw;
    float pitch;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        yaw += Input.GetAxis("Mouse X") * sensitivity;
        pitch -= Input.GetAxis("Mouse Y") * sensitivity;
        pitch = Mathf.Clamp(pitch, -85f, 85f);

        transform.rotation = Quaternion.Euler(pitch, yaw, 0f);
    }
}
