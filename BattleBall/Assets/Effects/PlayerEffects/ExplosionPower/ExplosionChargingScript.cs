using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionChargingScript : Power {

    public GameObject ExplosionParticlesCharging;
    public GameObject ExplosionParticles;

    private Material baseMaterial;

    // Use this for initialization
    void Start()
    {

    }

    void OnDestroy()
    {
        GameObject explosion = Instantiate<GameObject>(ExplosionParticles);
        explosion.GetComponent<ExplosionScript>().player = player;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
