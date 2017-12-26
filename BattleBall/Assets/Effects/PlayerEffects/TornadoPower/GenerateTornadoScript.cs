using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateTornadoScript : Power {

    public float TornadoTimeToLive = 5f;
    public GameObject TornadoPrefab;
    private DashScript dashScript;
    // Use this for initialization
    void Start() {
        dashScript = player.GetComponent<DashScript>();
        dashScript.OnDashing += GenerateTornado;
        
    }

    void OnDestroy()
    {
        StopCoroutine("PowerDuration");
        dashScript.OnDashing -= GenerateTornado;
    }

    void GenerateTornado(Vector3 movement)
    {
        GameObject tornado = Instantiate(TornadoPrefab, player.transform.position, Quaternion.identity);
        tornado.GetComponent<TornadoScript>().Player = player;
        dashScript.OnDashing -= GenerateTornado;
        Destroy(tornado, TornadoTimeToLive);
        if (gameObject != null)
        {
            Destroy(gameObject);
        }
    }
}
