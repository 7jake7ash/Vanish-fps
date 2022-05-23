using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public Rigidbody rb;
    public GameObject bulletHitPrefab;
    public GameObject hitMarker;
    public AudioSource audioSource;

    private EnemyHealth enemyHealth;
    private HealthBar health;
    private TargetHealth targetHealth;

    //Stats
    public float damage;

    public PhotonView view;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnCollisionEnter(Collision collision)
    {
        if (view.IsMine)
        {
            //Body Shot
            if (collision.collider.gameObject.CompareTag("Body") && !collision.collider.GetComponent<Outline>().playerController.GetComponent<PhotonView>().IsMine)
            {
                //health = collision.transform.parent.parent.parent.GetComponent<HealthBar>();
                health = collision.collider.GetComponent<Outline>().playerController.GetComponent<HealthBar>();
                //health.hitMarker = hitMarker;
                //hitMarker.SetActive(true);

                audioSource.Play();

                Debug.Log(health);
                health.Shot(damage);
                //health.HitMarker(0.5f);
            }
            //Head Shot
            if (collision.collider.gameObject.CompareTag("Head") && !collision.collider.GetComponent<Outline>().playerController.GetComponent<PhotonView>().IsMine)
            {
                health = collision.transform.parent.parent.parent.GetComponent<HealthBar>();
                //health.hitMarker = hitMarker;
                //hitMarker.SetActive(true);

                audioSource.Play();

                health.Shot(damage * 1.5f);
                //health.HitMarker(0.5f);
            }
            //Enemy
            if (collision.collider.gameObject.CompareTag("Enemy"))
            {
                enemyHealth = collision.collider.transform.parent.GetComponent<EnemyHealth>();
                enemyHealth.hitMarker = hitMarker;
                hitMarker.SetActive(true);
                //enemyHealth.HitMarker(0.5f);

                enemyHealth.Shot(damage);
            }
            //Target
            if (collision.collider.gameObject.CompareTag("Target"))
            {
                targetHealth = collision.collider.transform.GetComponent<TargetHealth>();
                targetHealth.hitMarker = hitMarker;

                audioSource.Play();

                targetHealth.Shot(damage);
                //targetHealth.HitMarker(0.5f);
            }
        }
        Destroy(gameObject);
    }

    private void OnCollisionStay(Collision collision)
    {
        Destroy(gameObject);
    }
}
