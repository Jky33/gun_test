using UnityEngine;
using System.Collections.Generic;

public class CSVIMUPoseSource : MonoBehaviour, IGunPoseSource
{
    public TextAsset csvFile;

    private List<Vector3> gyroData = new List<Vector3>();
    private List<Vector3> accData = new List<Vector3>();

    private int index = 0;

    private Quaternion rotation = Quaternion.identity;

    [Header("Settings")]
    public float gyroScale = 1.0f; // deg/s → 1，rad/s → 57.3
    public float alpha = 0.98f;    // gyro權重

    void Start()
    {
        ParseCSV();
    }

    void ParseCSV()
    {
        string[] lines = csvFile.text.Split('\n');

        for (int i = 1; i < lines.Length; i++)
        {
            string line = lines[i];
            if (string.IsNullOrWhiteSpace(line)) continue;

            string[] p = line.Split(',');

            float ax = float.Parse(p[0]);
            float ay = float.Parse(p[1]);
            float az = float.Parse(p[2]);

            float gx = float.Parse(p[3]);
            float gy = float.Parse(p[4]);
            float gz = float.Parse(p[5]);

            accData.Add(new Vector3(ax, ay, az));
            gyroData.Add(new Vector3(gx, gy, gz));
        }
    }

    void Update()
    {
        if (gyroData.Count == 0) return;

        float dt = Time.deltaTime;

        Vector3 gyro = gyroData[index];
        Vector3 acc = accData[index];

        // ===== 1️⃣ Gyro 積分 =====
        Quaternion delta = Quaternion.Euler(
            gyro.x * gyroScale * dt,
            gyro.y * gyroScale * dt,
            gyro.z * gyroScale * dt
        );

        rotation = rotation * delta;

        // ===== 2️⃣ Acc 轉成傾斜角 =====
        Vector3 accNorm = acc.normalized;

        float pitchAcc = Mathf.Atan2(accNorm.y, accNorm.z) * Mathf.Rad2Deg;
        float rollAcc  = Mathf.Atan2(-accNorm.x, accNorm.z) * Mathf.Rad2Deg;

        Quaternion accRotation = Quaternion.Euler(pitchAcc, 0, rollAcc);

        // ===== 3️⃣ 融合 =====
        rotation = Quaternion.Slerp(rotation, accRotation, 1 - alpha);

        index = (index + 1) % gyroData.Count;
    }

    public Quaternion GetRotation()
    {
        return rotation;
    }
}