using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TornadoScript : Power {


    public float PullSpeed;

    public GameObject Player;

    private MovingScript playerMove;
	// Use this for initialization
	void Start () {
        playerMove = player.GetComponent<MovingScript>();
	}
	
    void OnTriggerStay(Collider other)
    {
        if(other.tag == "Player" && other.gameObject != Player)
        {
            other.transform.position = Vector3.MoveTowards(other.transform.position, this.transform.position, PullSpeed * Time.deltaTime);
        }
    }
	// Update is called once per frame
	void Update () {
        this.transform.position += playerMove.currentDirection * playerMove.speed;
        this.transform.localEulerAngles = Vector3.zero;
	}
}
