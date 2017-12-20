using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightningScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        this.transform.eulerAngles = Vector3.zero;
	}

    void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player" && other.gameObject != this.transform.parent.gameObject)
        {
            Debug.Log(other.gameObject.name);
            other.GetComponent<AvatarScript>().StunPlayer();
        }
    }


}
