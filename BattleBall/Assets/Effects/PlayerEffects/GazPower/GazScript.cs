using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GazScript : Power {

    private GameObject GazParticles;

    private Collider PlayerCollider;

    // Use this for initialization
    void Start()
    {
        PowerTime = 5f;
        StartCoroutine(PowerDuration(PowerTime));
        powerName = "GAZ";
        GazParticles = Instantiate(Resources.Load<GameObject>("Gaz"));
    }

    void OnDestroy()
    {
        Destroy(GazParticles);
        gameObject.layer = 9;
    }

    // Update is called once per frame
    void Update()
    {
        GazParticles.transform.position = transform.position;
        gameObject.layer = 8;
    }
}
