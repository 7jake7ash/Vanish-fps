using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Spawner : MonoBehaviour
{
    float Timer;
    float random;

    static int EnemyCount;
    public int EnemyLimit;
    public GameObject[] enemys;
    void Start()
    {
        random = Random.Range(10f, 30f);
    }

    
    void Update()
    {
        if(EnemyCount <= EnemyLimit)
        {
            Timer += Time.deltaTime;
            if(Timer >= random)
            {
                spawnEnemy();
            }
        } 
    }

    void spawnEnemy()
    {
        Debug.LogWarning("Spawn");
        
        Timer = 0f;

        EnemyCount++;

        random = Random.Range(10f, 30f);

        PhotonNetwork.Instantiate(enemys[Random.Range(0, enemys.Length)].name, transform.position, transform.rotation);
    }
}
