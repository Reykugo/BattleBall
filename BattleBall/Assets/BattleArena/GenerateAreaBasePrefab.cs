using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateAreaBasePrefab : MonoBehaviour {

    public Vector3 size;

    public GameObject fillBlock;

    // Use this for initialization
    void Start()
    {
        StartCoroutine(GenerateArea());
    }
	
	// Update is called once per frame
	void Update () {
	    
	}

    IEnumerator GenerateArea()
    {
        for (int i = 0; i < size.x; i++)
        {
            for (int j = 0; j < size.y; j++)
            {
                for (int k = 0; k < size.z; k++)
                {
                    var go = Instantiate(fillBlock, transform);
                    go.name = "Cube(" + i + "," + j + "," + k + ")";
                    go.transform.localPosition = new Vector3(i, j, k);
                    yield return new WaitForSeconds(0.01f);
                }
            }
        }
    }
}
