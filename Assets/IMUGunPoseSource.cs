using UnityEngine;

public class IMUGunPoseSource : MonoBehaviour, IGunPoseSource
{
    public SerialIMUReceiver imuReceiver;

    public Quaternion GetRotation()
    {
        if (imuReceiver == null)
            return Quaternion.identity;

        return imuReceiver.imuRotation;
    }
}