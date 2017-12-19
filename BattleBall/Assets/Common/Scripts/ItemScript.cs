using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemScript : MonoBehaviour {

    private Power.PowerType powerType;

	// Use this for initialization
	void Start () {
        powerType = (Power.PowerType)Random.Range(0, 1);
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnTriggerEnter(Collider c)
    {
        if(c.tag == "Player")
        {
            switch (powerType)
            {
                case Power.PowerType.GAZ:
                    c.gameObject.AddComponent<GazScript>();
                    break;

                case Power.PowerType.METAL:
                    c.gameObject.AddComponent<MetalScript>();
                    break;

                default:
                    break;
            }
            Destroy(gameObject);
        }
    }
}
