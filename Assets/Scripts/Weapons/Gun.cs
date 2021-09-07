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
    public GameObject bullet;

    //RenderObject
    public RenderObjects gunRenderer;
    
    //bullet force
    public float upwardForce;

    //weapon stats
    public Weapons weapon;
    
    int bulletsLeft, bulletsShot;
    float verticalLookRotation;
    float recoil;

    //Cam
    Camera cam;
    float FOV = 90;
    float fovDistance;
    float currentGunFov;

    //Reference
    public Transform attackPoint;
    public GameObject muzzleFlash;
    public TextMeshProUGUI ammunitionDisplay;

    Transform sightPoint;
    
    PlayerController playerCon;

    //Vector3
    Vector3 directionWithoutSpread;
    Vector3 origin;
    Transform bloom;
    Transform CamHolder;

    //bools
    bool shooting, readyToShoot, reloading;

    public Transform Container;
    public bool allowInvoke = true;

    static int gunCount;
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
        if (photonView.IsMine)
        {   
            //photonView.RPC("setParent", RpcTarget.All);
            
            cam = transform.parent.parent.Find("Cam/Camera").GetComponent<Camera>();

            playerCon = transform.parent.parent.parent.parent.GetComponent<PlayerController>();
            playerCon.canSprint = true;
            ammunitionDisplay.enabled = true;

            SetLayerRecursively(gameObject, 14);

            FOV = cam.fieldOfView;
            currentGunFov = gunRenderer.settings.cameraSettings.cameraFieldOfView;
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
        if (photonView.IsMine)
        {
            playerInput();

            if (Input.GetKeyDown(KeyCode.Mouse0))
                origin = CamHolder.localEulerAngles;
            
            //set ammo display
            if (ammunitionDisplay != null)
                ammunitionDisplay.SetText(bulletsLeft / weapon.bulletAmount + " / " + weapon.rounds / weapon.bulletAmount);
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
        //photonView.RPC("Aim", RpcTarget.All, Input.GetMouseButton(1));

        //Camera Zoom
        if(Input.GetMouseButton(1))
        {
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, FOV / weapon.ZoomFov, Time.deltaTime * weapon.aimSpeed);
            //gunRenderer.settings.cameraSettings.cameraFieldOfView = Mathf.Lerp(gunRenderer.settings.cameraSettings.cameraFieldOfView, fovDistance, Time.deltaTime * weapon.aimSpeed);
        }
        else
        {
            FOV = playerCon.fov;
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, FOV, Time.deltaTime * weapon.aimSpeed);
            //gunRenderer.settings.cameraSettings.cameraFieldOfView = Mathf.Lerp(gunRenderer.settings.cameraSettings.cameraFieldOfView, currentGunFov, Time.deltaTime * weapon.aimSpeed);
        }

        if (weapon.burst == 1) shooting = Input.GetKey(KeyCode.Mouse0);
        else shooting = Input.GetKeyDown(KeyCode.Mouse0);

        //Reloading
        if (Input.GetKeyDown(KeyCode.R) && bulletsLeft < weapon.rounds && !reloading) Reload();
        //Reload automatically when trying to shoot without ammo
        if (readyToShoot && shooting && !reloading && bulletsLeft <= 0) Reload();

        //Shooting
        if(readyToShoot && shooting && !reloading && bulletsLeft > 0 && playerCon.Speed != 7)
        {
            bulletsShot = 0;

            playerCon.isShooting = true;
            playerCon.canSprint = false;

            Shoot();
        }
    }
    //[PunRPC]
    void Aim(bool isAiming)
    {
        Transform anchor = transform.Find("Anchor");
        Transform stateADS = transform.Find("States/ADS");
        Transform stateHip = transform.Find("States/Hip");
        sightPoint = anchor.Find("Design/sightPoint");

        if (photonView.IsMine)
        {
            cam = transform.parent.parent.Find("Cam/Camera").GetComponent<Camera>();

            stateADS.position = cam.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0.6f));
        } 

        Vector3 distanceToSight = anchor.position - sightPoint.position;

        if (isAiming)
        {
            //aim                                                           
            anchor.position = Vector3.Lerp(anchor.position, stateADS.position + distanceToSight, Time.deltaTime * weapon.aimSpeed);
        }
        else
        {
            //hip
            anchor.position = Vector3.Lerp(anchor.position, stateHip.position, Time.deltaTime * weapon.aimSpeed);
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

        //Instantiate bullet
        GameObject currentBullet = Instantiate(bullet, attackPoint.position, Quaternion.identity);

        //Set PhotonView
        currentBullet.GetComponent<Bullet>().view = photonView;

        currentBullet.GetComponent<Bullet>().damage = weapon.damage;
        currentBullet.GetComponent<Bullet>().hitMarker = transform.Find("HitMarker").gameObject;

        
        //Rotate bullet to shoot direction
        currentBullet.transform.forward = -attackPoint.forward;

        //Add force
        currentBullet.GetComponent<Rigidbody>().AddForce(currentBullet.transform.forward * weapon.shootForce, ForceMode.Impulse);

        //Instantiate muzzle flash
        if (muzzleFlash != null)
            Instantiate(muzzleFlash, attackPoint.position, Quaternion.identity);

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
            Invoke("Shoot", weapon.firerate);

        //Recoil
        if(photonView.IsMine)
        {
            if (CamHolder.localRotation.x < 0.5f && CamHolder.localRotation.x > -0.5f)
            {
                float random = Random.Range(-weapon.sideRecoil, weapon.sideRecoil);

                CamHolder.localEulerAngles += new Vector3(-weapon.recoil, random, 0);

                playerCon.canSprint = true;
            }
        }

        //gun fx
        transform.Rotate(-weapon.recoil, 0, 0);
        transform.position -= transform.forward * weapon.kickback;

        Destroy(currentBullet, 5f);
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
        container.localPosition = new Vector3(0.396f, 0.3f, 0.476f);
        prefab.transform.localPosition = Vector3.zero;
        prefab.transform.localRotation = Quaternion.identity;
    }

}
