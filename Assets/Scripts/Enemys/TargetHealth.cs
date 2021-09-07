using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetHealth : MonoBehaviour
{
    public float Health = 100;
    public GameObject hitMarker;
    void Start()
    {
        
    }

    void Update()
    {
        
    }
    public void Shot(float damage)
    {
        Health -= damage;
        Debug.Log(Health);

        if (Health <= 0)
        {
            Debug.Log("Target Dead");
            GetComponent<MeshRenderer>().enabled = false;
            Invoke("Respawn", 5);
        }
    }
    public void HitMarker(float time)
    {
        hitMarker.SetActive(true);
        Invoke("Wait", time);
    }

    void Wait(float time)
    {
        hitMarker.SetActive(false);
    }

    void Respawn()
    {
        GetComponent<MeshRenderer>().enabled = true;
        Health = 100;
    }
}
