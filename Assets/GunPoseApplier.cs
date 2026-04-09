using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GunPoseApplier : MonoBehaviour
{
    public MonoBehaviour poseSourceComponent;

    private IGunPoseSource poseSource;

    void Start()
    {
        poseSource = poseSourceComponent as IGunPoseSource;
        if (poseSource == null)
        {
            Debug.LogError("PoseSource does not implement IGunPoseSource");
        }
    }

    void Update()
    {
        if (poseSource == null) return;

        Quaternion rotation = poseSource.GetRotation();
        transform.localRotation = rotation;
    }
}
