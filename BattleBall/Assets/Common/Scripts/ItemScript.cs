using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemScript : MonoBehaviour {

    private Power.PowerType powerType;

	// Use this for initialization
	void Start () {
        //powerType = Power.PowerType.ICE;
        powerType = (Power.PowerType)Random.Range(0, (int)Power.PowerType.LENGTH);
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
                    Debug.Log("Power : GAZ started");
                    break;

                case Power.PowerType.METAL:
                    c.gameObject.AddComponent<MetalScript>();
                    Debug.Log("Power : METAL started");
                    break;

                case Power.PowerType.ICE:
                    c.gameObject.AddComponent<IceScript>();
                    Debug.Log("Power : ICE started");
                    break;

                default:
                    break;
            }
            Destroy(gameObject);
        }
    }
}
