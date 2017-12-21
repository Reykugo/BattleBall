using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSceneOnLoadingEnd : MonoBehaviour {

    public int SceneIndex;
	// Use this for initialization
	void Start () {
        SceneManager.LoadScene(SceneIndex);
    }
}
