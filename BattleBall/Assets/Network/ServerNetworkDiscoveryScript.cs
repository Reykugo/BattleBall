﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


public class ServerNetworkDiscoveryScript : NetworkDiscovery
{
    // Use this for initialization
    void Start()
    {
        //Initialize broadcast discovery
        bool complete = Initialize();
        if (!complete)
        {
            Debug.Log("port not available");
        }
        //Start sending broadcast.
        StartAsServer();
    }
}
