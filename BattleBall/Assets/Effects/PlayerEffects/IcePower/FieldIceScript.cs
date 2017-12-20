using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldIceScript : MonoBehaviour {


    public GameObject player;

    public float iceDuration;

	// Use this for initialization
	void Start () {
        Destroy(gameObject, iceDuration);
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}


    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player" && other.gameObject.name != player.name)
        {
            other.GetComponent<Collider>().material.dynamicFriction = 0;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player" && other.gameObject != player)
        {
            other.GetComponent<Collider>().material.dynamicFriction = 1f;
        }
    }
}
