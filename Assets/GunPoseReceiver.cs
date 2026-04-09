using UnityEngine;

public class GunPoseReceiver : MonoBehaviour
{
    public MonoBehaviour poseSource;

    private IGunPoseSource source;

    void Start()
    {
        source = poseSource as IGunPoseSource;
    }

    void Update()
    {
        if (source == null) return;

        Quaternion rot = source.GetRotation();

        transform.rotation = rot;
    }
}