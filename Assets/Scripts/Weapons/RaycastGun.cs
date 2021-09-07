
using Photon.Pun;
using UnityEngine;

public class RaycastGun : MonoBehaviourPun
{
    public Weapons weapon;

    private HealthBar health;

    private EnemyHealth enemyHealth;

    private float currentCooldown;
    public GameObject bullethitPrefab;
    public LayerMask canBeShot;

    void Update()
    {
        Aim(Input.GetMouseButton(1));

        if (weapon.burst != 1)
        {
            //Semi
            if (photonView.IsMine && Input.GetMouseButtonDown(0) && currentCooldown <= 0)
            {
                photonView.RPC("Shoot", RpcTarget.All);
            }
        }
        else
        {
            //Auto
            if (photonView.IsMine && Input.GetMouseButton(0) && currentCooldown <= 0)
            {
                photonView.RPC("Shoot", RpcTarget.All);
            }
        }

        //Weapon position elasticity
        transform.localPosition = Vector3.Lerp(transform.localPosition, Vector3.zero, Time.deltaTime * 4f);

        //cooldown
        if (currentCooldown > 0) currentCooldown -= Time.deltaTime;
    }

    void Aim(bool isAiming)
    {
        if (photonView.IsMine)
        {
            Transform anchor = transform.Find("Anchor");
            Transform stateADS = transform.Find("States/ADS");
            Transform stateHip = transform.Find("States/Hip");

            if (isAiming)
            {
                //aim
                anchor.position = Vector3.Lerp(anchor.position, stateADS.position, Time.deltaTime * weapon.aimSpeed);
            }
            else
            {
                //hip
                anchor.position = Vector3.Lerp(anchor.position, stateHip.position, Time.deltaTime * weapon.aimSpeed);
            }
        }
    }
    [PunRPC]
    public void Shoot()
    {
        Transform spawn = transform.parent.parent.Find("Cam").transform;

        //Bloom
        Vector3 bloom = spawn.position + spawn.forward * 1000f;
        bloom += Random.Range(-weapon.bloom, weapon.bloom) * spawn.up;
        bloom += Random.Range(-weapon.bloom, weapon.bloom) * spawn.right;
        bloom -= spawn.position;
        bloom.Normalize();

        //Raycast
        RaycastHit hit = new RaycastHit();
        if(Physics.Raycast(spawn.position, bloom, out hit, 1000f, canBeShot))
        {
            
            Material Mat = hit.collider.GetComponent<Renderer>().material;

            GameObject impact = PhotonNetwork.Instantiate(bullethitPrefab.name, hit.point + hit.normal * 0.001f, Quaternion.identity);

            impact.GetComponent<Renderer>().material = Mat;

            impact.transform.LookAt(hit.point + hit.normal);

            if (photonView.IsMine)
            {
                //Body Shot
                if (hit.collider.gameObject.CompareTag("Body"))
                {
                    health = hit.transform.parent.parent.GetComponent<HealthBar>();

                    Debug.Log(health);
                    health.Shot(weapon.damage);
                }
                //Head Shot
                if (hit.collider.gameObject.CompareTag("Head"))
                {
                    health = hit.transform.parent.parent.GetComponent<HealthBar>();

                    health.Shot(weapon.damage * 1.5f);
                }
                //Enemy
                if (hit.collider.gameObject.CompareTag("Enemy"))
                {
                    enemyHealth = hit.collider.transform.parent.GetComponent<EnemyHealth>();
                    
                    enemyHealth.Shot(weapon.damage);
                }
            }

            Destroy(impact, 1f);
        }

        //gun fx
        transform.Rotate(-weapon.recoil, 0, 0);
        transform.position -= transform.forward * weapon.kickback;

        //cooldown
        currentCooldown = weapon.firerate;
    }
}
