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
        StartCoroutine(PowerDuration(PowerTime));
        powerName = "METAL";
        baseMaterial = GetComponent<MeshRenderer>().material;
        MetalMaterial = Resources.Load<Material>("MetalMaterial");
        GetComponent<MeshRenderer>().material = MetalMaterial;
        PlayerRigidbody = GetComponent<Rigidbody>();
    }

    void OnDestroy()
    {
        GetComponent<MeshRenderer>().material = baseMaterial;
    }
	
	// Update is called once per frame
	void Update () {
        PlayerRigidbody.velocity = Vector3.zero;
	}
}
