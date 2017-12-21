using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GazScript : Power {

    private GameObject GazParticles;

    private Collider PlayerCollider;

    // Use this for initialization
    void Start()
    {
    }

    void OnDestroy()
    {
        if (activated)
            player.gameObject.layer = 9;
    }

    // Update is called once per frame
    void Update()
    {
        if (activated)
            player.gameObject.layer = 8;
    }
}
