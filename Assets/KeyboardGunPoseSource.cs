using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyboardGunPoseSource : MonoBehaviour, IGunPoseSource
{
    public float rotationSpeed = 60f;

    private Quaternion currentRotation = Quaternion.identity;

    void Update()
    {
        float yaw = Input.GetAxis("Horizontal") * rotationSpeed * Time.deltaTime;
        float pitch = -Input.GetAxis("Vertical") * rotationSpeed * Time.deltaTime;

        Quaternion delta =
            Quaternion.Euler(pitch, yaw, 0f);

        currentRotation = currentRotation * delta;
    }

    public GunPose GetPose()
    {
        return new GunPose
        {
            rotation = currentRotation,
            position = Vector3.zero
        };
    }

    public Quaternion GetRotation()
    {
        return currentRotation;
    }
}