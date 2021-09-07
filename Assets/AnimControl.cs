using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimControl : MonoBehaviour
{
    public float speed;

    void Start()
    {
        GetComponent<Animator>().speed = speed;
    }

    void Update()
    {
        
    }
}
