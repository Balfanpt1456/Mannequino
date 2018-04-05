using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports;
using System;



public class PlaneController : MonoBehaviour
{
    private float amount;
    private float var1;
    private float var2;
    private float var3;
    private float var4;

    [SerializeField]
    private Transform parent;

    [SerializeField]
    private GameObject cube;


    [SerializeField]
    private Rigidbody cube2;


    public Quaternion q;
    public Quaternion mv;
    public Quaternion a = new Quaternion(0, 0, 0, 0);

    public bool go;



    public float movementSpeed =50;

    SerialPort sp = new SerialPort("COM8", 115200);

    void Start()
    {
        sp.Open();
        sp.ReadTimeout = 1;

        go = false;
        // mv = Quaternion.Euler(new Vector3(0, 0, 0));
         mv = cube.transform.localRotation;
        // cube.transform();
    }

    void Update()
    {

        //parent.position = cube.transform.position;
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



        if (go)
        {
            Vector3 movement = cube.transform.localRotation * Vector3.forward;
            cube2.velocity = movement * movementSpeed;
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            Calibrate(q);
        }


        if (Input.GetKey(KeyCode.W))
        {


            //Vector3 movement = cube.transform.rotation * Vector3.forward;
            // cube2.velocity = movement * movementSpeed;


            //cube.transform.TransformDirection(Vector3.forward);
            //cube.transform.position += transform.TransformDirection(Vector3.right) * movementSpeed;
            //cube.transform.position += Vector3.forward * Time.deltaTime * movementSpeed;
            //cube.transform.position += transform.forward * Time.deltaTime * movementSpeed;
            //cube2.velocity = transform.forward * movementSpeed;

        }

    


    }

    void FixedUpdate()
    {

    }



    void RotateSelected(Vector3 rotationInput, GameObject selectedObject)
    {
        selectedObject.transform.localRotation = Quaternion.Euler(rotationInput);
    }


    void RotateSelected2(Quaternion rotationInput, GameObject selectedObject)
    {
        selectedObject.transform.rotation = rotationInput;
    }



    void serialEvent()
    {
        try
        {
            string str = sp.ReadLine().TrimEnd();
            if (str != null)
            {
          
                string[] thingys = str.Substring(1, str.Length - 2).Split('\t');
                if (thingys.Length == 5)
                {
                    int i;
                    int.TryParse(thingys[0], out i);
                    q = ReadFrame(thingys);

                    Debug.Log(q);
                    //q = Quaternion.Lerp(q, q, .35f);
                   


                    Debug.Log(q + "" + mv);

                    RotateSelected2(q * mv, cube); 
                 

                    

                }

            }

            //Debug.Log("Read " + str);

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


}
