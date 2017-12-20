using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MetalScript : Power {

    public Material MetalMaterial;
    private Rigidbody PlayerRigidbody;

    private Material baseMaterial;

    // Use this for initialization
    void Start()
    {
        PowerTime = 5f;
        powerName = "METAL";
        baseMaterial = player.GetComponent<MeshRenderer>().material;
        player.GetComponent<MeshRenderer>().material = MetalMaterial;
        PlayerRigidbody = player.GetComponent<Rigidbody>();
    }

    void OnDestroy()
    {
        player.GetComponent<MeshRenderer>().material = baseMaterial;
    }
	
	// Update is called once per frame
	void Update () {
        PlayerRigidbody.velocity = Vector3.zero;
	}
}
