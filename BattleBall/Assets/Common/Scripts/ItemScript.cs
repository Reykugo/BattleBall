using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemScript : MonoBehaviour {

    public GameObject[] Powers;

    private GameObject Power;

	// Use this for initialization
	void Start () {
        Power = Powers[Random.Range(0, Powers.Length)];
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnTriggerEnter(Collider c)
    {
        if(c.tag == "Player")
        {
            Power = Instantiate(Power);
            Power.GetComponent<PowerScript>().AssignToPlayer(c.gameObject);
            Destroy(gameObject);
        }
    }
}
