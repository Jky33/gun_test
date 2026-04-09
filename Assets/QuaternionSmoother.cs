using UnityEngine;

public class QuaternionSmoother
{
    private Quaternion current = Quaternion.identity;
    private bool initialized = false;

    public Quaternion Update(Quaternion target, float alpha)
    {
        if (!initialized)
        {
            current = target;
            initialized = true;
            return current;
        }

        current = Quaternion.Slerp(current, target, alpha);
        return current;
    }
}
