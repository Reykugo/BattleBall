using UnityEngine;
using System.Collections;

public class BilboardScript : MonoBehaviour
{
    private void Start()
    {
        
    }
    // Update is called once per frame
    void Update()
    {
        transform.LookAt(Camera.main.transform);
    }
}
