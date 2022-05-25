using FirstGearGames.SmoothCameraShaker;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class Gun : MonoBehaviourPun
{
    //PickUp
    public GameObject pickupPrefab;

    //bullet
    GameObject bulletUsed;

    public GameObject bullet;
    public GameObject bullet1;
    public GameObject bullet2;

    //gun shot sound
    public AudioSource gunShot;
    public AudioSource hit;

    //RenderObject
    public RenderObjects gunRenderer;
    
    //bullet force
    public float upwardForce;

    //weapon stats
    public Weapons weapon;
    
    int bulletsLeft, bulletsShot;
    float verticalLookRotation;
    float recoil;
    float bloom;
    public float adsZ;

    //Cam
    Camera cam;
    float FOV = 90;
    float fovDistance;
    float currentGunFov;

    //Cam Shake
    public ShakeData shake;

    //Reference
    public Transform barrelPoint;
    public Transform attackPoint;
    public GameObject muzzleFlash;
    public TextMeshProUGUI ammunitionDisplay;
    
    public Transform sightPoint;
    
    PlayerController playerCon;

    //Vector3
    Vector3 directionWithoutSpread;
    Vector3 origin;
    Transform CamHolder;

    Transform anchor;
    Transform stateADS;
    Transform stateHip;
    Transform hipOffset;
    Vector3 originADS = new Vector3(0,0,0);

    //bools
    bool shooting, readyToShoot, reloading, playSound;

    public Transform Container;
    public bool allowInvoke = true;

    public LayerMask groundMask;

    static int gunCount;

    float gunYdrop = 0;
    float aimTime;

    //Aim Lerp
    IEnumerator SimpleLerp()
    {
        float x = weapon.aimSpeed;  // time frame

        for (float f = 0; f <= x; f += Time.deltaTime)
        {
            //anchor.position = Vector3.Lerp(start, end, x);

            aimTime = x;

            yield return null;
        }
    }

    void Awake()
    {
        bulletsLeft = weapon.rounds;
        readyToShoot = true;
    }
    [PunRPC]
    void setParent()
    {
        Debug.LogError(Container);
        transform.SetParent(Container);
    }
    void Start()
    {
        //photonView.RPC("bulletType", RpcTarget.AllBuffered);
        playerCon = transform.parent.parent.parent.parent.GetComponent<PlayerController>();
        if (photonView.IsMine)
        {
            //photonView.RPC("setParent", RpcTarget.All);

            playSound = true;
            gunShot.spatialBlend = 0;

            cam = transform.parent.parent.Find("Cam/Camera").GetComponent<Camera>();

            //playerCon = transform.parent.parent.parent.parent.GetComponent<PlayerController>();
            playerCon.canSprint = true;
            ammunitionDisplay.enabled = true;

            SetLayerRecursively(gameObject, 14);

            FOV = cam.fieldOfView;
            currentGunFov = gunRenderer.settings.cameraSettings.cameraFieldOfView;

            //States

            anchor = transform.Find("Anchor");
            stateADS = transform.Find("States/ADS");
            stateHip = transform.Find("States/Hip");
            sightPoint = anchor.Find("Design/sightPoint");

            stateADS.position = cam.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, adsZ));

            hipOffset = stateHip;
        }
    }

    [PunRPC]
    void bulletType()
    {
        playerCon = transform.parent.parent.parent.parent.GetComponent<PlayerController>();

        if (playerCon.plrManager.team == 0)
        {
            bulletUsed = bullet;
        }
        else if (playerCon.plrManager.team == 1)
        {
            bulletUsed = bullet1;
        }
        else
        {
            bulletUsed = bullet2;
        }
    }

    void SetLayerRecursively(GameObject obj, int newLayer)
    {
        if (null == obj)
        {
            return;
        }

        obj.layer = newLayer;

        foreach (Transform child in obj.transform)
        {
            if (null == child)
            {
                continue;
            }
            SetLayerRecursively(child.gameObject, newLayer);
        }
    }

    void Update()
    {
        if (photonView.IsMine && Cursor.visible == false)
        {
            playerInput();

            if (Input.GetKeyDown(KeyCode.Mouse0))
                origin = CamHolder.localEulerAngles;
            
            //set ammo display
            if (ammunitionDisplay != null)
                ammunitionDisplay.SetText(bulletsLeft / weapon.bulletAmount + " / " + weapon.rounds / weapon.bulletAmount);
        }

        if (photonView.IsMine)
        {
            if (!playerCon.grounded)
            {
                gunYdrop = Mathf.Lerp(gunYdrop, -0.015f, Time.deltaTime * 2);

                stateHip.localPosition = Vector3.Lerp(stateHip.localPosition, new Vector3(0, -0.12f, 0), Time.deltaTime * 2);
            }
            else
            {
                gunYdrop = Mathf.Lerp(gunYdrop, 0, Time.deltaTime * 8);

                stateHip.localPosition = Vector3.Lerp(stateHip.localPosition, Vector3.zero, Time.deltaTime * 8);
            }
        }

        //Weapon position elasticity
        transform.localPosition = Vector3.Lerp(transform.localPosition, Vector3.zero, Time.deltaTime * 4f);
        //Recoil elasticity
        if (!shooting)
        {
            if (photonView.IsMine)
            {
                playerCon = transform.parent.parent.parent.parent.GetComponent<PlayerController>();
                playerCon.isShooting = false;
            }

            CamHolder = transform.parent.parent;
            CamHolder.localRotation = Quaternion.Lerp(CamHolder.localRotation, Quaternion.Euler(Vector3.zero), weapon.lerpSpeed);
        }
    }

    private void playerInput()
    {
        //Aim //should not send RPC every frame
        Aim(Input.GetMouseButton(1));

        if (weapon.burst == 1) shooting = Input.GetKey(KeyCode.Mouse0);
        else shooting = Input.GetKeyDown(KeyCode.Mouse0);

        //Reloading
        if (Input.GetKeyDown(KeyCode.R) && bulletsLeft < weapon.rounds && !reloading) Reload();

        //Reload automatically when trying to shoot without ammo
        if (readyToShoot && shooting && !reloading && bulletsLeft <= 0) Reload();

        //Shooting
        if(readyToShoot && shooting && !reloading && bulletsLeft > 0 && playerCon.Speed != 7)
        {
            RaycastHit hit;
            
            if (Physics.Raycast(cam.transform.position, cam.transform.TransformDirection(Vector3.forward), out hit,Vector3.Distance(sightPoint.position, cam.transform.position), groundMask))
            {
                return;
            }

            bulletsShot = 0;

            playerCon.isShooting = true;
            playerCon.canSprint = false;

            Shoot();
        }

        if (Input.GetKeyDown(KeyCode.G))
        {
            DropGun();
            DestroyGun();
        }
    }
    //[PunRPC]
    void Aim(bool isAiming)
    {
        if (photonView.IsMine)
        {
            cam = transform.parent.parent.Find("Cam/Camera").GetComponent<Camera>();
        } 

        if (isAiming)
        {
            Vector3 distanceToSight = anchor.position - sightPoint.position + new Vector3(0, gunYdrop, 0);

            //aim
            anchor.position = Vector3.Lerp(anchor.position, stateADS.position + distanceToSight, Time.deltaTime * weapon.aimSpeed);
            //StartCoroutine(SimpleLerp());

            bloom = weapon.aimBloom;

            //Camera Zoom
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, FOV / weapon.ZoomFov, Time.deltaTime * weapon.zoomSpeed);
        }
        else
        {
            //hip
            anchor.position = Vector3.Lerp(anchor.position, stateHip.position, Time.deltaTime * weapon.aimSpeed);
            //StartCoroutine(SimpleLerp());

            bloom = weapon.bloom;

            //Camera Zoom
            FOV = playerCon.fov;
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, FOV, Time.deltaTime * weapon.zoomSpeed);
        }
    }

    private void Shoot()
    {
        photonView.RPC("ShootBullet", RpcTarget.All);
    }

    private void ResetShot()
    {
        readyToShoot = true;
        allowInvoke = true;
    }

    private void Reload()
    {
        playerCon.canSprint = false;
        reloading = true;
        Animator animator = transform.Find("Anchor/Design/Arms/VanishArmLeft").GetComponent<Animator>();
        animator.speed = 1 / weapon.reloadTime;
        animator.Play("Reload");
        Invoke("ReloadFinished", weapon.reloadTime);
    }

    private void ReloadFinished()
    {
        playerCon.canSprint = true;
        bulletsLeft = weapon.rounds;
        reloading = false;
    }

    [PunRPC]
    public void ShootBullet()
    {   
        readyToShoot = false;

        //Sound
        if (playSound)
        {
            gunShot.pitch = Random.Range(0.9f, 1.1f);
            gunShot.PlayOneShot(gunShot.clip);
        }

        //Instantiate bullet
        GameObject currentBullet = Instantiate(bullet, attackPoint.position, Quaternion.identity);

        //Set PhotonView
        currentBullet.GetComponent<Bullet>().view = photonView;

        currentBullet.GetComponent<Bullet>().audioSource = hit;
        currentBullet.GetComponent<Bullet>().damage = weapon.damage;
        currentBullet.GetComponent<Bullet>().hitMarker = transform.Find("HitMarker").gameObject;

        //Rotate bullet to shoot direction
        attackPoint.Rotate(Random.Range(-bloom, bloom), Random.Range(-bloom, bloom), 0);
        currentBullet.transform.forward = -attackPoint.forward;

        //Add force
        currentBullet.GetComponent<Rigidbody>().AddForce(currentBullet.transform.forward * weapon.shootForce, ForceMode.Impulse);

        //Reset rotation
        attackPoint.localEulerAngles = new Vector3(0,180,0);

        //Instantiate muzzle flash
        if (muzzleFlash != null)
        {
            muzzleFlash.SetActive(true);
            Invoke("Flash", 0.03f);
            
            //GameObject flash = Instantiate(muzzleFlash, barrelPoint.position + new Vector3(Random.Range(-.01f,.01f), Random.Range(-.01f, .01f), Random.Range(-.01f, .01f)), Quaternion.identity);
            //flash.transform.forward = -attackPoint.forward;
            //flash.transform.Rotate(Random.Range(-.01f, .01f), Random.Range(-.01f, .01f), Random.Range(-.01f, .01f));
            //Destroy(flash, 0.03f);
        }

        bulletsLeft--;
        bulletsShot++;

        //Invoke resetShot
        if (allowInvoke)
        {
            Invoke("ResetShot", weapon.firerate);
            allowInvoke = false;
        }

        //if Shotgun
        if (bulletsShot < weapon.bulletAmount && bulletsLeft > 0)
        {
            playSound = false;
            Invoke("Shoot", 0);
        } else
        {
            playSound = true;
        }

        //Recoil
        if(photonView.IsMine)
        {
            stateADS.position = cam.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, adsZ));

            CameraShakerHandler.Shake(GetComponent<Sway>().shake);

            if (CamHolder.localRotation.x < 0.5f && CamHolder.localRotation.x > -0.5f)
            {
                //Cam
                float camRandom = Random.Range(-weapon.sideRecoil, weapon.sideRecoil);

                CamHolder.localEulerAngles += new Vector3(-weapon.recoil, camRandom, 0);

                //gun fx
                transform.Rotate(-weapon.modelRecoil, camRandom * 2, 0);
                transform.position -= transform.forward * weapon.kickback;

                playerCon.canSprint = true;
            }
        }

        Destroy(currentBullet, 5f);
    }

    void Flash()
    {
        muzzleFlash.SetActive(false);
    }

    public void DropGun()
    {
        photonView.RPC("Drop", RpcTarget.MasterClient);
    }
    [PunRPC]
    void Drop()
    {
        GameObject player = transform.parent.parent.parent.parent.gameObject;
        GameObject drop = PhotonNetwork.InstantiateRoomObject(pickupPrefab.name, player.transform.position + player.transform.forward * 1.1f, Quaternion.Euler(0, player.transform.rotation.eulerAngles.y, 0));
        Rigidbody Rb = drop.GetComponent<Rigidbody>();
        Rb.AddForce(player.transform.forward * 5, ForceMode.Impulse);
        Rb.AddTorque(player.transform.forward + new Vector3(Random.Range(-10, 10), Random.Range(-10, 10), Random.Range(-10, 10)), ForceMode.Impulse);
    }
    public void DestroyGun()
    {
        photonView.RPC("Destroy", RpcTarget.All);
    }
    [PunRPC]
    public void Destroy()
    {
        if (photonView.IsMine)
        {
            PhotonNetwork.Destroy(gameObject);
        }
    }

    public void Parent(int viewId, int plrId)
    {
        photonView.RPC("SetParent", RpcTarget.AllBuffered, viewId, plrId);
    }
    [PunRPC]
    void SetParent(int viewId, int plrId)
    {
        GameObject prefab = PhotonNetwork.GetPhotonView(viewId).gameObject;
        Transform container = PhotonNetwork.GetPhotonView(plrId).transform.Find("Recoil/CameraHolder/itemContainer");
        prefab.gameObject.GetComponent<Gun>().Container = container;
        //prefab.transform.Find("Anchor/Design").gameObject.SetActive(!photonView);
        Debug.Log(prefab);
        Debug.Log(container);
        prefab.transform.SetParent(container);
        container.localPosition = new Vector3(0.396f, 0.7f, 0.476f);
        prefab.transform.localPosition = Vector3.zero;
        prefab.transform.localRotation = Quaternion.identity;
    }

}
