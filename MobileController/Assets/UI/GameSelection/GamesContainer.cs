using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class GamesContainer : MonoBehaviour {

    // Use this for initialization
    public GameObject GameButton;
    public Network.NetworkClientScript net;
    private Dictionary<string, GameObject> gamesFound = new Dictionary<string, GameObject>();
	void Start () {
        
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    public void Clear()
    {
        foreach(var g in gamesFound)
        {
            Destroy(g.Value);
        }
        gamesFound.Clear();
    }
    public void Add(string name, string ip, string playerNumber)
    {
        GameObject go;
        if (!gamesFound.ContainsKey(ip))
        {
            go = Instantiate(GameButton, transform);
            gamesFound.Add(ip, go);
            var button = go.GetComponent<Button>();
            button.onClick.AddListener(() => { net.Connect(ip); });
        }
        else
        {
            go = gamesFound[ip];
        }
        Text[] text = go.GetComponentsInChildren<Text>();
        text[0].text = name + ":" + ip;
        text[1].text = playerNumber;
    }

    public int Count()
    {
        return gamesFound.Count;
    }
}
