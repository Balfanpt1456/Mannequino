using System.Collections;
using System.Collections.Generic;
using System.IO.Ports;
using System;
using UnityEngine;

public class SixGameController : MonoBehaviour
{
    public Transform left;
    public Transform right;

    public Transform[] cubes;
    private SerialPort serial;
    private bool isReading;

    void Start()
    {
        serial = new SerialPort("COM9", 115200);
        serial.ReadTimeout = 100;
        serial.Open();
        isReading = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            serial.WriteLine("y");
            Debug.Log("start 'em");
            isReading = true;
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
                        Quaternion q = new Quaternion();
                        float.TryParse(thingys[1], out q.w);
                        float.TryParse(thingys[2], out q.y);
                        float.TryParse(thingys[3], out q.z);
                        float.TryParse(thingys[4], out q.x);
                        q.x *= -1;
                        q.y *= -1;

                        cubes[i].localRotation = q;
                        
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
}