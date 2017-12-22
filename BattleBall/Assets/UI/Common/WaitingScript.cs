using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WaitingScript : MonoBehaviour {

    public string textBase = "no Text";
    private Text t;
	// Use this for initialization
    void Awake()
    {
        t = GetComponent<Text>();
    }

    void OnEnable()
    {
        StartCoroutine(dotWaitingCoroutine());
    }

    void OnDisable()
    {
        StopCoroutine("dotWaitingCoroutine");
    }

    IEnumerator dotWaitingCoroutine()
    {
        int timer = 0;
        while (enabled)
        {
            t.text = textBase;
            for (int i = 0; i < timer; i++)
            {
                t.text += ".";
            }
            timer++;
            timer %= 4;
            yield return new WaitForSeconds(1f);
        }

    }
}
