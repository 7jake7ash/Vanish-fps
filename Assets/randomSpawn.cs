using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class randomSpawn : MonoBehaviour
{
    [Header("PickUp")]
    public GameObject[] StartGuns;

    public int minTime;
    public int maxTime;
    public int maxSpawns;
    public int maxSpawnsPerDrop;

    static int amount = 0;
    // Start is called before the first frame update
    void Start()
    {
        amount = 0;

        InvokeRepeating("Spawn", Random.Range(minTime, maxTime), Random.Range(minTime, maxTime));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Spawn()
    {
        if(amount >= maxSpawns)
        {
            return;
        }
        int spawnsThisDrop = 0;
        for (Vector3 i = new Vector3(Random.Range(-100, 100), 100, Random.Range(-100, 100)); spawnsThisDrop <= maxSpawnsPerDrop; i = new Vector3(Random.Range(-100, 100), 100, Random.Range(-100, 100)))
        {
            if(Physics.Raycast(i, Vector3.down, 10000))
            {
                Debug.Log("spawn");
                PhotonNetwork.InstantiateRoomObject(StartGuns[Random.Range(0, StartGuns.Length)].name, i, Quaternion.identity);
                spawnsThisDrop++;
                amount++;
            }
        }
    }
}
