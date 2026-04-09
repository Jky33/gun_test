using UnityEngine;
using System.IO.Ports;

public class SerialIMUReceiver : MonoBehaviour
{
    public string portName = "COM5";
    public int baudRate = 115200;

    SerialPort serial;

    public Quaternion imuRotation = Quaternion.identity;

    void Start()
    {
        string[] portNames = System.IO.Ports.SerialPort.GetPortNames();
        Debug.Log("Available serial ports: " + portNames.Length);
        foreach (string port in portNames)
        {
            Debug.Log(" - " + port);
        }

        try
        {
            serial = new SerialPort(portName, baudRate);
            serial.ReadTimeout = 1;
            serial.Open();
        }
        catch (System.Exception e)
        {
            Debug.LogWarning("Serial open failed: " + e.Message);
        }
    }

    void Update()
    {
        if (serial == null || !serial.IsOpen)
            return;

        try
        {
            string line = serial.ReadLine();

            string[] data = line.Split(',');

            if (data.Length < 4)
                return;

            float w = float.Parse(data[0]);
            float x = float.Parse(data[1]);
            float y = float.Parse(data[2]);
            float z = float.Parse(data[3]);

            imuRotation = new Quaternion(x, y, z, w);
        }
        catch
        {
            // ignore frame errors
        }
    }

    void OnApplicationQuit()
    {
        if (serial != null && serial.IsOpen)
            serial.Close();
    }
}