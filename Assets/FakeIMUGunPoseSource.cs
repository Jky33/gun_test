using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FakeIMUGunPoseSource : MonoBehaviour, IGunPoseSource
{
    [Header("Fake IMU Settings")]
    public Vector3 angularVelocityDeg; // deg/sec

    private Quaternion currentRotation = Quaternion.identity;

    void Update()
    {
        Vector3 deltaAngle =
            angularVelocityDeg * Time.deltaTime;

        Quaternion delta =
            Quaternion.Euler(deltaAngle);

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
