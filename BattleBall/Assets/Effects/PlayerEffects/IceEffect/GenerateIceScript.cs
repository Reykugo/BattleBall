using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateIceScript : MonoBehaviour {

    public GameObject IcePrefab;
    public float GenerationSpeed;
    public float IceDuration;

    private float timer = 0;
    private Vector3 oldGenerationposition;
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
        timer += Time.deltaTime;
		if(timer >= GenerationSpeed && oldGenerationposition != this.transform.position)
        {
            float playerSize = this.transform.parent.transform.localScale.y; 
            GameObject ice = Instantiate<GameObject>(IcePrefab, new Vector3(this.transform.position.x ,
                this.transform.position.y  - (playerSize/2), this.transform.position.z), Quaternion.identity);
            oldGenerationposition = this.transform.position;
            Destroy(ice, IceDuration);
            timer = 0;
        }
	}
}
