using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateTornadoScript : Power {

    public GameObject TornadoPrefab;
    // Use this for initialization
    void Start() {
        GameObject tornado = Instantiate(TornadoPrefab, player.transform.position, Quaternion.identity);
        tornado.GetComponent<TornadoScript>().Player = player;
        Destroy(tornado, 10);
    }
	
    // Update is called once per frame
    void Update () {
		
	}
}
