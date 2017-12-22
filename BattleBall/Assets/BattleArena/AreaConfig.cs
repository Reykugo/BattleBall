using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaConfig : MonoBehaviour
{
    public List<Transform> ItemSpawns;
    public List<Transform> spawners;
    public Transform RespawnPoint;

    public GameObject Item;

    void Start()
    {
        StartCoroutine(SpawnItems());
    }


    IEnumerator SpawnItems()
    {
        float delay = Random.Range(3, 10);
        yield return new WaitForSeconds(delay);
        Transform spawn = ChooseItemSpawn(ItemSpawns);
        SpawnItem(spawn);
        StartCoroutine(SpawnItems());
    }

    public Transform ChooseItemSpawn(List<Transform> Spawns)
    {
        int index = Random.Range(0, Spawns.Count);
        Transform spawn = Spawns[index];
        return spawn;
    }

    public void SpawnItem(Transform spawn)
    {
        GameObject item = Instantiate<GameObject>(Item);
        item.transform.position = spawn.transform.position;
    }
}
