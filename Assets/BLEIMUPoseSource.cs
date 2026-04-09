using UnityEngine;
using System.Globalization;
using System.Collections.Generic;

public class BLEIMUPoseSource : MonoBehaviour, IGunPoseSource
{

    [Header("CSV Test")]
public TextAsset csvFile;

private List<Quaternion> csvRotations = new List<Quaternion>();

private int csvIndex = 0;
    public Quaternion imuRotation = Quaternion.identity;

    private QuaternionSmoother smoother = new QuaternionSmoother();
    public float smoothing = 0.2f;

    // ===== 狀態 =====
    private bool hasData = false;
    private bool calibrated = false;

    private Quaternion initialOffset = Quaternion.identity;

    public Vector3 gunForwardEuler = Vector3.zero;

    public enum InputMode
{
    BLE,
    CSV
}

public InputMode mode = InputMode.BLE;

    // ===== BLE 輸入 =====
    public void OnBLEData(string data)
    {
        ParseQuaternion(data);
    }

   void Start()
{
    Debug.Log("Mode: " + mode);

    if (mode == InputMode.CSV)
    {
        ParseCSV();
        hasData = true;
    }
}

    void ParseCSV()
{
    if (csvFile == null) return;

    string[] lines = csvFile.text.Split('\n');

    for (int i = 1; i < lines.Length; i++)
    {
        string line = lines[i];
        if (string.IsNullOrWhiteSpace(line)) continue;

        string[] p = line.Split(',');

        float gx = float.Parse(p[3]);
        float gy = float.Parse(p[4]);
        float gz = float.Parse(p[5]);

        // 👉 簡單轉成旋轉（測試用）
        Quaternion q = Quaternion.Euler(gx, gy, gz);

        csvRotations.Add(q);
    }

    Debug.Log("CSV Loaded: " + csvRotations.Count);
}

    void ParseQuaternion(string text)
    {
        string[] p = text.Split(',');

        if (p.Length < 4) return;

        try
        {
            float w = float.Parse(p[0], CultureInfo.InvariantCulture);
            float x = float.Parse(p[1], CultureInfo.InvariantCulture);
            float y = float.Parse(p[2], CultureInfo.InvariantCulture);
            float z = float.Parse(p[3], CultureInfo.InvariantCulture);

            imuRotation = new Quaternion(x, y, z, w);
            hasData = true;
        }
        catch { }
    }

    // ===== 主輸出 =====
    public Quaternion GetRotation()
    
{
    Quaternion AmplifyRotation(Quaternion q, float gain)
{
    // 取得旋轉軸 + 角度
    q.ToAngleAxis(out float angle, out Vector3 axis);

    // 放大角度
    float amplifiedAngle = angle * gain;

    return Quaternion.AngleAxis(amplifiedAngle, axis);
}
    if (mode == InputMode.CSV)
    {
        return GetCSVRotation();
    }

    if (!hasData)
        return Quaternion.identity;

    Quaternion q = smoother.Update(imuRotation, smoothing);

Quaternion converted = ConvertIMUToUnity(q);

// 👉 放大旋轉（真正有效）
Quaternion amplified = AmplifyRotation(converted, 3.0f);

return initialOffset * amplified * Quaternion.Euler(gunForwardEuler);
}
    Quaternion GetCSVRotation()
{
    if (csvRotations.Count == 0)
        return Quaternion.identity;

    Quaternion q = csvRotations[csvIndex];

    csvIndex = (csvIndex + 1) % csvRotations.Count;

    return q;
}

    // ===== 座標轉換 =====
    Quaternion ConvertIMUToUnity(Quaternion q)
    {
        // IMU: (X前, Y右, Z上)
        // Unity: (X右, Y上, Z前)

        return new Quaternion(
            q.y,   // Unity X ← IMU Y
            q.z,   // Unity Y ← IMU Z
            q.x,   // Unity Z ← IMU X
            q.w
        );
    }

    // ===== 自動校正（第一幀）=====
    void Update()
    {
        if (!calibrated && hasData)
        {
            Quaternion converted = ConvertIMUToUnity(imuRotation);

            initialOffset = Quaternion.Inverse(converted);
            calibrated = true;

            Debug.Log("IMU Calibrated");
        }
    }
}