﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceScript : Power {

    public float GenerationSpeed;

    private Vector3 OldGenerationposition;

    private GameObject IceParticles;

    private GameObject IceTrail;

    private Collider PlayerCollider;

    private float Timer = 0;

	// Use this for initialization
	void Start () {
        PowerTime = 5f;
        StartCoroutine(PowerDuration(PowerTime));
        powerName = "ICE";
        GenerationSpeed = 0.1f;
        IceParticles = Instantiate(Resources.Load<GameObject>("Ice"));
        IceParticles.transform.localScale = transform.localScale * 2;
        IceTrail = Resources.Load<GameObject>("IceTrail");
    }

    void OnDestroy()
    {
        Destroy(IceParticles);
    }
	
	// Update is called once per frame
	void Update () {
        Timer += Time.deltaTime;
        IceParticles.transform.position = transform.position;
		if(Timer >= GenerationSpeed && OldGenerationposition != transform.position)
        {
            GameObject iceTrail = Instantiate(IceTrail);
            iceTrail.transform.position = transform.position;
            iceTrail.GetComponent<FieldIceScript>().iceDuration = 1f;
            iceTrail.GetComponent<FieldIceScript>().player = gameObject;
            OldGenerationposition = transform.position;
            Timer = 0;
        }
	}
}