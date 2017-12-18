using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceEffectScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}


    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            other.GetComponent<Collider>().material.dynamicFriction = 0;
            other.GetComponent<Rigidbody>().velocity *= 1.1f;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            other.GetComponent<Collider>().material.dynamicFriction = 0.6f;
        }
    }
}
