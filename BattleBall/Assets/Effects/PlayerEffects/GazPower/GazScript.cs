using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GazScript : Power {

    public GameObject GazParticles;

    private GameObject gazParticles;

    // Use this for initialization
    void Start()
    {
        Time = 5f;
        name = "GAZ";
        GazParticles = Resources.Load<GameObject>("GazParticle");
        gazParticles = Instantiate(GazParticles);
    }

    // Update is called once per frame
    void Update()
    {
        gazParticles.transform.position = transform.position;
    }
}
