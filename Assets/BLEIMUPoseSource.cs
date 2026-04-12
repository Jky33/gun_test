using UnityEngine;
using System.Globalization;
using System.Collections.Generic;

public class BLEIMUPoseSource : MonoBehaviour, IGunPoseSource
{
    // ===== 模式 =====
public enum InputMode
{
    BLE,
    CSV,
    Keyboard
}

    public InputMode mode = InputMode.BLE;

    // ===== CSV =====
    [Header("CSV Test")]
    
    public TextAsset csvFile;
    private List<Quaternion> csvRotations = new List<Quaternion>();
    private int csvIndex = 0;

    [Header("Keyboard Control")]
    public float keyboardSpeed = 120f;

    private float yaw = 0f;
    private float pitch = 0f;

    // ===== IMU =====
    public Quaternion imuRotation = Quaternion.identity;
    private bool hasData = false;

    private float timer = 0f;
    public float maxHz = 100f;

    private float csvTimer = 0f;

    private int csvTrigger = 0;

    private List<int> csvTriggers = new List<int>();

    private Quaternion currentCSVRotation = Quaternion.identity;

    private Quaternion latestIMU;
    private bool hasNewData = false;

    // ===== 濾波 =====
    private QuaternionSmoother smoother = new QuaternionSmoother();
    public float smoothing = 0.2f;

    // ===== 靈敏度 =====
    public float sensitivity = 2.0f;

    // ===== 槍方向修正 =====
    public Vector3 gunForwardEuler = Vector3.zero;

    // =========================
    // 初始化
    // =========================
    void Start()
    {
        Debug.Log("Mode: " + mode);

        if (mode == InputMode.CSV)
        {
            ParseCSV();
            hasData = true;
        }
    }

    void Update()
{
    if (mode == InputMode.CSV)
    {
        float interval = 1f / maxHz;
        csvTimer += Time.deltaTime;

        if (csvTimer >= interval)
        {
            csvIndex = (csvIndex + 1) % csvRotations.Count;
            currentCSVRotation = csvRotations[csvIndex];
            triggerValue = csvTriggers[csvIndex];

            csvTimer = 0f;

            Debug.Log("CSV index: " + csvIndex);
        }
    }
}

bool TryParseIMULine(string line,
    out int btn,
    out float gx, out float gy, out float gz)
{
    btn = 0;
    gx = gy = gz = 0;

    if (string.IsNullOrWhiteSpace(line))
        return false;

    if (line.Contains("ax")) // header
        return false;

    string[] p = line.Trim().Split(',');

    if (p.Length < 7)
        return false;

    try
    {
        // BTN
        string[] btnSplit = p[0].Split(':');
        if (btnSplit.Length == 2)
        {
            btn = int.Parse(btnSplit[1]);
        }

        // gyro（你目前用這個）
        gz = float.Parse(p[4], CultureInfo.InvariantCulture);
        gy = float.Parse(p[5], CultureInfo.InvariantCulture);
        gx = float.Parse(p[6], CultureInfo.InvariantCulture);

        return true;
    }
    catch
    {
        Debug.LogWarning("Parse fail: " + line);
        return false;
    }
}

    // =========================
    // BLE 輸入
    // =========================
public int triggerValue = 0;

public void OnBLEData(string data)
{
    if (TryParseIMULine(data, out int btn, out float gx, out float gy, out float gz))
    {
        triggerValue = btn;

        imuRotation = Quaternion.Euler(
            gx * 100f,
            gy * 100f,
            -gz * 100f
        );

        hasData = true;
    }
}

    // =========================
    // CSV 解析
    // =========================
    void ParseCSV()
{
    if (csvFile == null) return;

    string[] lines = csvFile.text.Split('\n');

    foreach (string line in lines)
    {
        if (TryParseIMULine(line, out int btn, out float gx, out float gy, out float gz))
        {
            csvTrigger = btn;
            Quaternion q = Quaternion.Euler(
                gx * 100f,
                gy * 100f,
                -gz * 100f
            );

            csvRotations.Add(q);
            csvTriggers.Add(btn);
        }
    }

    Debug.Log("CSV Loaded: " + csvRotations.Count);
}

    // =========================
    // 主輸出（最重要）
    // =========================
    public Quaternion GetRotation()
{
    if (mode == InputMode.Keyboard)
    {
        float h = Input.GetAxis("Horizontal"); // A/D
        float v = Input.GetAxis("Vertical");   // W/S

        yaw += h * keyboardSpeed * Time.deltaTime;
        pitch -= v * keyboardSpeed * Time.deltaTime;

        Quaternion rot = Quaternion.Euler(pitch, yaw, 0f);
        return rot * Quaternion.Euler(gunForwardEuler);
    }

    if (!hasData)
        return Quaternion.identity;

    Quaternion raw;

    if (mode == InputMode.CSV)
    {
        raw = currentCSVRotation;
    }
    else
    {
        raw = smoother.Update(imuRotation, smoothing);
        raw = ConvertIMUToUnity(raw);
    }

    raw.ToAngleAxis(out float angle, out Vector3 axis);
    float amplifiedAngle = angle * sensitivity;

    Quaternion amplified = Quaternion.AngleAxis(amplifiedAngle, axis);

    return amplified * Quaternion.Euler(gunForwardEuler);
}

    // =========================
    // 座標轉換
    // =========================
    Quaternion ConvertIMUToUnity(Quaternion q)
    {
        return new Quaternion(
            q.y,
            q.z,
            q.x,
            q.w
        );
    }
}