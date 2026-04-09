using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct GunPose
{
    public Quaternion rotation;
    public Vector3 position;

    public static GunPose Identity()
    {
        return new GunPose
        {
            rotation = Quaternion.identity,
            position = Vector3.zero
        };
    }
}