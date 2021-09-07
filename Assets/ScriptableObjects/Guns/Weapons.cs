using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Gun", menuName = "Gun")]
public class Weapons : ScriptableObject
{
    public string Name;
    public float damage;
    public int burst; // 0 semi / 1 auto
    public float firerate;
    public float shootForce;
    public float bloom;
    public float recoil;                                    
    public float sideRecoil;
    public float kickback;
    public float aimSpeed;
    public float lerpSpeed;
    public float bobIntensity;
    public float bobSpeed;
    public float sprintIntensity;
    public float sprintSpeed;
    public float ZoomFov;
    public int rounds;
    public float bulletAmount;
    public float reloadTime;
    public GameObject prefab;
}