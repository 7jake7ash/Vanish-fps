using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Enemy", menuName = "Enemy")]
public class Enemys : ScriptableObject
{
    public string Name;
    public float damage;
    public float health;
    public float moveSpeed;
    public GameObject prefab;
}
