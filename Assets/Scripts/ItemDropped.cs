using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDropped : MonoBehaviour
{
    private bool ItemOnGround = false;
    [SerializeField] private float DespawnTimer = 10;


    void Start()
    {
        ItemOnGround = true;
    }

    void Update()
    {
        if(ItemOnGround)
        {
            if(DespawnTimer > 0)
            {
                DespawnTimer -= Time.deltaTime;
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}
