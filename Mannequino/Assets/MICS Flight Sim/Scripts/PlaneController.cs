using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports;
using System;
using Microsoft.Win32;

public class PlaneController : MonoBehaviour
{

    [SerializeField]
    private GameObject cube;

    [SerializeField]
    private Rigidbody cube2;


    public Quaternion q;
    public Quaternion mv;

    public bool go;
    
    public float movementSpeed = 20;

    SerialPort sp = new SerialPort(AutodetectArduinoPort(), 115200);

    void Start()
    {
        sp.Open();
        sp.ReadTimeout = 1;

        go = false;
        mv = Quaternion.identity;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            sp.WriteLine("y");
            Debug.Log("start 'em");
        }

        if (sp.IsOpen)
        {
            try
            {
                serialEvent();
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            Go();
        }

        if (go)
        {
            Vector3 movement = cube.transform.localRotation * Vector3.forward;
            cube2.velocity = movement * movementSpeed;
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            Calibrate(q);
        }
    }

    void serialEvent()
    {
        try
        {
            string str = sp.ReadLine().TrimEnd();
            if (str != null && str.StartsWith("["))
            {
          
                string[] thingys = str.Substring(1, str.Length - 2).Split('\t');
                if (thingys.Length == 5)
                {
                    int i;
                    int.TryParse(thingys[0], out i);
                    Quaternion qb = q;
                    q = ReadFrame(thingys);

                    cube.transform.localRotation = mv * q;
                }
            }
        }
        catch (TimeoutException e)
        {

        }
    }


    public void Calibrate(Quaternion sel)
    {
        mv = Quaternion.Inverse(sel);
        Debug.Log("Calibrating " + mv);
    }

    public void Calibrate()
    {
        mv = Quaternion.Inverse(q);
        Debug.Log("Calibrating " + mv);
    }

    public void Go()
    {
        Debug.Log("Go");
        go = true;
    }

    public void buttonPress()
    {
        Debug.Log("Start");
        sp.WriteLine("A");
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
