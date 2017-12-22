using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemScript : MonoBehaviour {

    public Power.PowerType powerType = Power.PowerType.LENGTH;//Length Mean random.
    private GameManager manager;
    void Start()
    {
        manager = GameObject.Find("GameManager").GetComponent<GameManager>();
        if(powerType == Power.PowerType.LENGTH)
        {
            powerType = (Power.PowerType)UnityEngine.Random.Range(0, (int)Power.PowerType.LENGTH);
        }
    }

    void OnTriggerEnter(Collider c)
    {
        if(c.tag == "Player")
        {
            manager.ActivatePower(c.gameObject, powerType);
            Destroy(gameObject);
        }
    }
}
