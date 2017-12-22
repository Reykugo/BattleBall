using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TornadoScript : MonoBehaviour {


    public float PullSpeed;

    public float Duration;
    public float Speed;

    public GameObject Player;

    private MovingScript playerMove;

    public  Vector3 finalDestination;
	// Use this for initialization
	void Start () {
        playerMove = Player.GetComponent<MovingScript>();
        finalDestination = this.transform.position + (playerMove.currentDirection * (Duration * Speed));//We can do much better.
	}
	
    void OnTriggerStay(Collider other)
    {
        if(other.tag == "Player" && other.gameObject != Player) { 
        
            //other.transform.position = Vector3.MoveTowards(other.transform.position, this.transform.position, PullSpeed * Time.deltaTime);
            Vector3 dir = Vector3.Normalize(other.transform.position - this.transform.position);
            other.GetComponent<Rigidbody>().AddForce(-dir * PullSpeed); 
        }
    }
    // Update is called once per frame
    void Update () {
        this.transform.position = Vector3.MoveTowards(this.transform.position, finalDestination, Speed*Time.deltaTime);
    }
}
