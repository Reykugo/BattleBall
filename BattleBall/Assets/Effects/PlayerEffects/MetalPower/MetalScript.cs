using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MetalScript : Power {

    public Material MetalMaterial;

    // Use this for initialization
    void Start()
    {
        Time = 5f;
        name = "METAL";
        GetComponent<MeshRenderer>().material = MetalMaterial;
    }

    void OnDestroy()
    {
        GetComponent<MeshRenderer>().material = baseMaterial;
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
