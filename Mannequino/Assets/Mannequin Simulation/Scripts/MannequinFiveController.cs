using System.Collections;
using System.Collections.Generic;
using System.IO.Ports;
using System;
using UnityEngine;
using Microsoft.Win32;

public class MannequinFiveController : MonoBehaviour
{

    public Transform[] limbs;
    private SerialPort serial;
    private bool isReading;

    public Quaternion q;
    public Quaternion[] mv;

    public bool[] isCalibrated;

    void Start()
    {
        serial = new SerialPort(AutodetectArduinoPort(), 115200);
        serial.ReadTimeout = 100;
        serial.Open();
        isReading = false;
        mv = new Quaternion[limbs.Length];
        for (int i = 0; i < mv.Length; i++)
        {
            //mv[i] = new Quaternion(0, 0, 0, 0);
            mv[i] = limbs[i].localRotation;
        }

        isCalibrated = new bool[limbs.Length];
        for (int i = 0; i < isCalibrated.Length; i++)
        {
            isCalibrated[i] = false;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            serial.WriteLine("y");
            Debug.Log("start 'em");
            isReading = true;

        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            
            Debug.Log("Calibrate");
            Calibrate();

        }


        if (isReading)
        {
            try
            {
                string line = serial.ReadLine().TrimEnd();
                if (line.StartsWith("[") && line.EndsWith("]"))
                {
                    string[] thingys = line.Substring(1, line.Length - 2).Split('\t');
                    if (thingys.Length == 5)
                    {
                        int i;
                        int.TryParse(thingys[0], out i);


                        q = ReadFrame(thingys);
                        

                        //q = Quaternion.Lerp(q, q, .35f);
                        limbs[i].localRotation = mv[i] * q;

                       

                        if (Input.GetKeyDown(KeyCode.C))
                        {
                            Calibrate();
                        }

                    }
                }
                else
                {
                    Debug.Log(line);
                }
            }
            catch (TimeoutException)
            {
            }
        }
    }

    public void StartReading()
    {
        serial.WriteLine("y");
        Debug.Log("start 'em");
        isReading = true;
    }

    public void Calibrate()
    {
        Debug.Log("Calibrating");
        for (int j = 0; j < isCalibrated.Length; j++)
        {
            isCalibrated[j] = false;
        }
        for (int i = 0; i < isCalibrated.Length; i++)
        {
            while (!isCalibrated[i])
            {
                string line = serial.ReadLine().TrimEnd();
                if (line.StartsWith("[") && line.EndsWith("]"))
                {
                    string[] thingys = line.Substring(1, line.Length - 2).Split('\t');
                    if (thingys.Length == 5)
                    {
                        int j;
                        int.TryParse(thingys[0], out j);
                        Quaternion q = ReadFrame(thingys);
                        mv[j] = Quaternion.Inverse(q);
                        isCalibrated[i] = true;
                    }
                }
            }

        }
    }

    Quaternion ReadFrame(String[] thingys)
    {
        Quaternion q = new Quaternion();
        float.TryParse(thingys[1], out q.w);
        float.TryParse(thingys[2], out q.y);
        float.TryParse(thingys[3], out q.z);
        float.TryParse(thingys[4], out q.x);
        q.x *= -1;
        q.y *= -1;
        return q;
    }


    public static string AutodetectArduinoPort()
    {
        List<string> comports = new List<string>();
        RegistryKey rk1 = Registry.LocalMachine;
        RegistryKey rk2 = rk1.OpenSubKey("SYSTEM\\CurrentControlSet\\Enum");
        string temp;
        foreach (string s3 in rk2.GetSubKeyNames())
        {
            RegistryKey rk3 = rk2.OpenSubKey(s3);
            foreach (string s in rk3.GetSubKeyNames())
            {
                if (s.Contains("VID") && s.Contains("PID"))
                {
                    RegistryKey rk4 = rk3.OpenSubKey(s);
                    foreach (string s2 in rk4.GetSubKeyNames())
                    {
                        RegistryKey rk5 = rk4.OpenSubKey(s2);
                        if ((temp = (string)rk5.GetValue("FriendlyName")) != null && temp.Contains("Arduino"))
                        {
                            RegistryKey rk6 = rk5.OpenSubKey("Device Parameters");
                            if (rk6 != null && (temp = (string)rk6.GetValue("PortName")) != null)
                            {
                                comports.Add(temp);
                            }
                        }
                    }
                }
            }
        }

        if (comports.Count > 0)
        {
            foreach (string s in SerialPort.GetPortNames())
            {
                if (comports.Contains(s))
                    return s;
            }
        }

        return "COM9";
    }



}



