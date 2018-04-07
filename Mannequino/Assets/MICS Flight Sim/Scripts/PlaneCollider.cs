using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneCollider : MonoBehaviour {


    [SerializeField]
    private Transform Spawm;

    public Material defaultMaterialRef;
    public Material newMaterialRef;

    public GameObject[] rings;


    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnCollisionEnter(Collision col)
    {

        if (col.gameObject.tag == "Ground")
        {
            transform.position = Spawm.position;
            rings = GameObject.FindGameObjectsWithTag("Ring");

            for(int i = 0; i < rings.Length; i++)
            {
                Renderer rend = rings[i].GetComponent<Renderer>();
                if (rend != null)
                {
                    rend.material = defaultMaterialRef;
                }

            }

        }

    

    }

    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "Ring")
        {
            Renderer rend = col.GetComponent<Renderer>();
            if (rend != null)
            {
                rend.material = newMaterialRef;
            }

         

        }
    }


}
