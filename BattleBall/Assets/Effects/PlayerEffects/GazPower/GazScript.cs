using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GazScript : Power {

    private GameObject GazParticles;

    private Collider PlayerCollider;

    private float PlayerPositionY;

    // Use this for initialization
    void Start()
    {
        PlayerPositionY = player.transform.position.y;
        ParticleSystem particles = GetComponent<ParticleSystem>();
        var main = particles.main;
        main.startColor = player.GetComponent<AvatarScript>().AvatarColor;
    }

    void OnDestroy()
    {
        if (activated)
        {
            player.gameObject.layer = 9;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (activated)
        {
            player.gameObject.layer = 8;
            player.transform.position = new Vector3(player.transform.position.x, PlayerPositionY, player.transform.position.z);
            player.GetComponentInChildren<GroundCheckerScript>().IsGrounded = true;
            Debug.Log(player.GetComponentInChildren<GroundCheckerScript>().IsGrounded);
        }
    }
}
