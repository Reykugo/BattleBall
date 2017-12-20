using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConfusionScript : Power {

    // Use this for initialization

    MovingScript movingScript;

	void Start () {
        PowerTime = 5f;
        StartCoroutine(PowerDuration(PowerTime));
        powerName = "CONFUSION";

        movingScript = GetComponentInParent<MovingScript>();
        movingScript.movementModifier = -1f;
		//Change controls.
        //Addd particles.
	}

    void OnDestroy()
    {
        movingScript.movementModifier = 1f;
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
