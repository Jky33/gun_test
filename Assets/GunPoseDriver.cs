using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunPoseDriver : MonoBehaviour
{
    [Header("Debug Input")]
    public Vector3 debugEuler;   // 用 Inspector 或鍵盤控制
    
    //void Update()
    //{
    //    transform.localRotation = Quaternion.Euler(debugEuler);
    //} //實體輸入
    void Update()
{
    float yaw = Input.GetAxis("Horizontal") * 60f * Time.deltaTime;
    float pitch = -Input.GetAxis("Vertical") * 60f * Time.deltaTime;

    transform.Rotate(pitch, yaw, 0f, Space.Self);
}   //鍵盤輸入
}
