using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionScript : MonoBehaviour {

    public GameObject player;

    public int explosionPower;

	// Use this for initialization
	void Start () {
        Destroy(gameObject, 2);
        transform.position = player.transform.position;
    }

    void OnTriggerEnter(Collider c)
    {
        if (c.tag == "Player" && c.gameObject.name != player.name)
        {
            Debug.Log("Force : " + ((c.transform.position - transform.position) * 1000).magnitude);
            Debug.Log("distance : " + Vector3.Distance(transform.position, c.transform.position));
            c.gameObject.GetComponent<Rigidbody>().AddForce(Vector3.Normalize(c.transform.position - transform.position) * (1 / Vector3.Distance(transform.position, c.transform.position) * explosionPower));
        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
