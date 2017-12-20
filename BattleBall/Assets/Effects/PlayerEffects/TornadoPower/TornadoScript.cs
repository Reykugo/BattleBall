using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TornadoScript : MonoBehaviour {


    public float PullSpeed;

    public GameObject Player;

    private MovingScript playerMove;
	// Use this for initialization
	void Start () {
		
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
	}
}
