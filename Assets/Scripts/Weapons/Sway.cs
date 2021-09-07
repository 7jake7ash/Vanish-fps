using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sway : MonoBehaviourPun
{
    public float intensity;
    public float smooth;
    public float sprintIntensity;

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

    void Start()
    {
        originRotation = transform.localRotation;
        playerController = transform.parent.parent.parent.parent.GetComponent<PlayerController>();
        weapons = GetComponent<Gun>().weapon;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateSway();
        //Idle
        if(Input.GetAxis("Horizontal") == 0 && Input.GetAxis("Vertical") == 0)
        {
            isSprinting = false;

            UpdateBob(idleCounter, weapons.bobIntensity * 0.5f);
            idleCounter += Time.deltaTime * weapons.bobSpeed;
        }
        //Sprint
        else if (playerController.Speed == 7)
        {
            isSprinting = true;
            
            UpdateBob(idleCounter, weapons.bobIntensity * 2f);
            idleCounter += Time.deltaTime * weapons.bobSpeed * 6f;

            UpdateSprint(sprintCounter, weapons.sprintIntensity);
            sprintCounter += Time.deltaTime * weapons.sprintSpeed;
        }
        //ADS
        else if (playerController.Speed == 3.5f)
        {
            isSprinting = false;

            UpdateBob(idleCounter, weapons.bobIntensity * 0.4f);
            idleCounter += Time.deltaTime * weapons.bobSpeed;
        }
        //Crouching
        else if(playerController.Speed == 2.5f)
        {
            isSprinting = false;

            UpdateBob(idleCounter, weapons.bobIntensity * 0.3f);
            idleCounter += Time.deltaTime * weapons.bobSpeed * 2f;
        }
        //Walk
        else
        {
            isSprinting = false;

            UpdateBob(movementCounter, weapons.bobIntensity);
            movementCounter += Time.deltaTime * weapons.bobSpeed * 4f;
        }

                                //Lerp//
        if (isSprinting)
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, targetPos + new Vector3(-0.7f, 0, 0), Time.deltaTime * 6f);

            transform.Find("Anchor").localEulerAngles = new Vector3(transform.Find("Anchor").localEulerAngles.x, Mathf.LerpAngle(transform.Find("Anchor").localEulerAngles.y, -50, Time.deltaTime * 8f), transform.Find("Anchor").localEulerAngles.z);
        }
        else
        { 
            transform.localPosition = Vector3.Lerp(transform.localPosition, targetPos, Time.deltaTime * 6f);
            transform.Find("Anchor").localEulerAngles = new Vector3(transform.Find("Anchor").localEulerAngles.x, Mathf.LerpAngle(transform.Find("Anchor").localEulerAngles.y, 0, Time.deltaTime * 10f), transform.Find("Anchor").localEulerAngles.z);
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

        transform.localRotation = Quaternion.Lerp(transform.localRotation, targetRotation, Time.fixedDeltaTime * smooth);
    }

    private void UpdateBob(float counter, float intensity)
    {
        if (photonView.IsMine)
        {
            targetPos = originPosition + new Vector3(originPosition.x, Mathf.Sin(counter) * intensity, 0);
            targetPos = targetPos + new Vector3(Mathf.Cos(counter / 2) * intensity * 2,targetPos.y, 0);
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
