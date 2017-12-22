using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameUI : MonoBehaviour {

    public GameObject playerUIPrefab;
    private List<GameObject> PlayersUI = new List<GameObject>();

    GameManager gameManager;

    // Use this for initialization
    void Start () {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        int i = 0;
        foreach (var player in gameManager.players)
        {
            var go = Instantiate(playerUIPrefab, transform);
            //go.transform.SetParent(transform, false);
            var rectTransform = go.GetComponent<RectTransform>();
            rectTransform.localPosition = new Vector3(rectTransform.localPosition.x + 290f * i, rectTransform.localPosition.y, rectTransform.localPosition.z);
            var playerUI = go.GetComponent<PlayerUIScript>();
            playerUI.avatar = player.GetComponent<PlayerInfo>().avatar;
            playerUI.Init();
            PlayersUI.Add(go);
            i++;
        }
	}

    void OnDestroy()
    {
        PlayersUI.Clear();
    }
}