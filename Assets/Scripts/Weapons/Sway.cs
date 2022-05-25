using FirstGearGames.SmoothCameraShaker;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sway : MonoBehaviourPun
{
    public float intensity;
    public float smooth;
    public float sprintIntensity;
    public Transform anchor;

    //Cam Shake
    public ShakeData shake;

    bool isSprinting;

    private float idleCounter;
    private float movementCounter;
    private float sprintCounter;
    private Vector3 targetPos;
    private Vector3 targetSprintPos;
    private Vector3 originPosition;
    private Vector3 originSprint;
    private Quaternion originRotation;
    private Quaternion targetRotation;
    PlayerController playerController;
    Weapons weapons;
    Gun gun;

    void Start()
    {
        originRotation = transform.localRotation;
        playerController = transform.parent.parent.parent.parent.GetComponent<PlayerController>();
        weapons = GetComponent<Gun>().weapon;
        gun = GetComponent<Gun>();
    }

    // Update is called once per frame
    void Update()
    {
        if(!photonView.IsMine)
        {
            return;
        }

        UpdateSway();

        //In The Air
        if (!playerController.grounded)
        {
            if (playerController.Speed == 8)
                isSprinting = true;
            else
                isSprinting = false;

            targetPos = Vector3.zero;
        }
        //Idle
        else if (Input.GetAxis("Horizontal") == 0 && Input.GetAxis("Vertical") == 0)
        {
            isSprinting = false;

            UpdateBob(idleCounter, weapons.bobIntensity * 0.03f);
            idleCounter += Time.deltaTime * weapons.bobSpeed;
        }
        //Sprint
        else if (playerController.Speed == 8 && !playerController.sliding)
        {
            isSprinting = true;
            
            UpdateBob(idleCounter, weapons.bobIntensity * 2f);
            idleCounter += Time.deltaTime * weapons.bobSpeed * 6f;

            UpdateSprint(sprintCounter, weapons.sprintIntensity);
            sprintCounter += Time.deltaTime * weapons.sprintSpeed;
        }
        //ADS
        else if (playerController.Speed == 5 || playerController.Speed == 5.5f)
        {
            isSprinting = false;

            UpdateBob(idleCounter, weapons.bobIntensity * 0.3f);
            idleCounter += Time.deltaTime * weapons.bobSpeed * 2.5f;
        }
        //Crouching
        else if(playerController.Speed == 3.5f)
        {
            isSprinting = false;

            UpdateBob(idleCounter, weapons.bobIntensity * 0.1f);
            idleCounter += Time.deltaTime * weapons.bobSpeed * 1f;
        }
        //Walk
        else
        {
            isSprinting = false;

            UpdateBob(idleCounter, weapons.bobIntensity);
            idleCounter += Time.deltaTime * weapons.bobSpeed * 4f;
        }

        //Lerp//

        if (isSprinting)
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, targetPos + new Vector3(-0.7f, 0, 0), Time.deltaTime * 6f);

            anchor.localEulerAngles = new Vector3(anchor.localEulerAngles.x, Mathf.LerpAngle(anchor.localEulerAngles.y, -50, Time.deltaTime * 8f), anchor.localEulerAngles.z);
        }
        else
        { 
            transform.localPosition = Vector3.Lerp(transform.localPosition, targetPos, Time.deltaTime * 6f);

            anchor.localEulerAngles = new Vector3(anchor.localEulerAngles.x, Mathf.LerpAngle(anchor.localEulerAngles.y, 0, Time.deltaTime * 8f), anchor.localEulerAngles.z);
        }
    }
    
    private void UpdateSway()
    {
        float xMouse = Input.GetAxis("Mouse X");
        float yMouse = Input.GetAxis("Mouse Y");

        if(!photonView.IsMine)
        {
            yMouse = 0;
            xMouse = 0;
        }

        //rotation
        Quaternion Xadjustment = Quaternion.AngleAxis(-intensity * xMouse, Vector3.up);
        Quaternion Yadjustment = Quaternion.AngleAxis(intensity * yMouse, Vector3.right);
        targetRotation = originRotation * Xadjustment * Yadjustment;

        transform.localEulerAngles = new Vector3(Mathf.LerpAngle(transform.localEulerAngles.x, targetRotation.eulerAngles.x, Time.deltaTime * smooth), Mathf.LerpAngle(transform.localEulerAngles.y, targetRotation.eulerAngles.y, Time.deltaTime * smooth), transform.localEulerAngles.z);
        //transform.localRotation = Quaternion.Lerp(transform.localRotation, targetRotation, Time.time * smooth);
    }

    private void UpdateBob(float counter, float intensity)
    {
        if (photonView.IsMine)
        {
            targetPos = originPosition + new Vector3(originPosition.x, Mathf.Sin(counter) * intensity, 0);
            targetPos = targetPos + new Vector3(Mathf.Cos(counter / 2) * intensity * 2,targetPos.y, 0);

            //Debug.LogWarning(targetPos.y);
            //if (targetPos.y >= 0.001f)
            //{
            //    CameraShakerHandler.Shake(shake);
            //}
        }
    }

    private void UpdateSprint(float counter, float intensity)
    {
        if (photonView.IsMine)
        {
            targetPos = targetPos + new Vector3(Mathf.Sin(counter) * intensity / 2, originPosition.y, 0);
        }  
    }
}
