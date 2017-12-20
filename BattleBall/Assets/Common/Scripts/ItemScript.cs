using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemScript : MonoBehaviour {

    private Power.PowerType powerType;


    public PowerReference[] ListOfPower;

    private Dictionary<Power.PowerType, GameObject> powerByType  = new Dictionary<Power.PowerType, GameObject>();
	// Use this for initialization
	void Start () {
        //powerType = Power.PowerType.ICE
        powerType = (Power.PowerType)UnityEngine.Random.Range(0, (int)Power.PowerType.LENGTH);
        foreach(PowerReference powerRef in ListOfPower)
        {
            powerByType[powerRef.power] = powerRef.prefab;
        }
        Debug.Log(powerByType);
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    void AddEffect(GameObject player)
    {
        GameObject power = Instantiate(powerByType[powerType]);
        power.transform.parent = player.transform;
        power.transform.position = player.transform.position;
        power.GetComponent<Power>().StartEffect();
    }

    void OnTriggerEnter(Collider c)
    {
        if(c.tag == "Player")
        {
            AddEffect(c.gameObject);
            Destroy(gameObject);
        }
    }
}

[Serializable]
public class PowerReference
{
    public Power.PowerType power;
    public GameObject prefab;
}
