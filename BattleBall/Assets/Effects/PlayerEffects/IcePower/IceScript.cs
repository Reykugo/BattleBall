using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceScript : Power {

    public float GenerationSpeed;


    private Vector3 oldGenerationposition;

    private GameObject iceParticles;

    private GameObject iceTrail;

    private Collider playerCollider;

    private float timer = 0;

	// Use this for initialization
	void Start () {
        this.transform.localScale = player.transform.localScale * 2;
        iceTrail = Resources.Load<GameObject>("IceTrail");
    }


	
	// Update is called once per frame
	void Update () {
        timer += Time.deltaTime;
		if(timer >= GenerationSpeed && oldGenerationposition != player.transform.position)
        {
            GameObject iceT = Instantiate(iceTrail);
            iceT.transform.position = player.transform.position;
            iceT.GetComponent<FieldIceScript>().player = player;
            oldGenerationposition = player.transform.position;
            timer = 0;
        }
	}
}
