using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports;
using System;



public class ArduinoController: MonoBehaviour {
    private float amount;
    private float var1;
    private float var2;
    private float var3;
    private float var4;

    [SerializeField]
    private GameObject cube;

    [SerializeField]
    private Rigidbody cube2;


    public Quaternion q;
    public Quaternion mv;
    public Quaternion a = new Quaternion(0, 0, 0, 0);

    public bool go;


    public float movementSpeed = 0.02f;

    SerialPort sp = new SerialPort("COM9", 115200);

    void Start()
    {
        sp.Open();
        sp.ReadTimeout = 1;

        go = false;
        // mv = Quaternion.Euler(new Vector3(0, 0, 0));
       mv =  new Quaternion(0, 0, 0, 0);
        // cube.transform();
    }

    void Update()
    {
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
            Vector3 movement = cube.transform.rotation * Vector3.forward;
            cube2.velocity = movement * movementSpeed;
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

        if (Input.GetKey(KeyCode.C))
        {
            Calibrate();

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
        selectedObject.transform.rotation = Quaternion.Euler(rotationInput);
    }


    void RotateSelected2(Quaternion rotationInput, GameObject selectedObject)
    {
        selectedObject.transform.rotation = rotationInput;
    }



    void serialEvent()
    {
        try
        {
            string str = sp.ReadLine();
            if (str != null)
            {
                str = str.Trim(); 
                string[] inputs = str.Split(' ');

                if (inputs.Length == 4)
                {
                    var1 = float.Parse(inputs[0]);
                    var2 = float.Parse(inputs[1]);
                    var3 = float.Parse(inputs[2]);
                    var4 = float.Parse(inputs[3]);
                    // Debug.Log(var1 + "!!! " + var2);
                }

                //q = new Quaternion(var1,var2, var3, var4);

                q = new Quaternion( var1, var2, var4, var3);

                if (Input.GetKeyDown(KeyCode.C))
                {
                    Debug.Log("Calibrating");

                    //Quaternion s = Quaternion.Inverse(q);
                    mv = Quaternion.Inverse(q);
                    //a = a * s;
                    //RotateSelected2(a, cube);
                }
                //Quaternion.Inverse(q);

               // Vector3 v = new Vector3(var1, var2, var3);
                RotateSelected2(q * mv, cube);
                

            }

            //Debug.Log("Read " + str);
          
        }
        catch (TimeoutException e)
        {
            
        }
    }


    public void Calibrate()
    {
        Debug.Log("Calibrating");
        mv = Quaternion.Inverse(q);
    }
  public void Go()
    {
        go = true;
    }

    public void buttonPress()
    {
        Debug.Log("Start");
        sp.WriteLine("A");
    }
}
