using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviourPun
{
    public static PlayerController Instance;
    public Camera cam;
    private Sway sway;

    [SerializeField] GameObject cameraHolder;

    [SerializeField] public float hSensitivity, vSensitivity, aimSensitivity, Speed, gravity, jumpHeight, crouchSpeed, jumpSpeed;

    Quaternion verticalLookRotation;

    public Transform groundCheck;
    public float groundDistance = 0.5f;
    public LayerMask groundMask;
    public float fov = 60f;
    bool grounded;
    bool canJump;
    bool canDoubleJump;
    bool crouching;
    bool forwardJump;
    public bool isShooting;
    public bool canSprint = true;

    public CharacterController controller;

    Vector3 velocity;

    public PhotonView PV;

    public Transform recoil;
    float xRotation;

    Transform menuTransform;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        PV = GetComponent<PhotonView>();
        fov = PlayerPrefs.GetFloat("FOV", 60);
    }

    void Start()
    {
        if(!PV.IsMine)
        {
            Destroy(transform.Find("Recoil/CameraHolder/Cam/Camera").gameObject);

            transform.Find("Recoil/CameraHolder/itemContainer").localPosition = new Vector3(0.396f, 0.3f, 0.476f);
        }

        recoil = transform.Find("Recoil");
        menuTransform = transform.Find("UI/Menu");

        canSprint = true;
        forwardJump = false;
        canJump = true;
        //canDoubleJump = false;

        Instance = this;
    }

    void Update()
    {
        if (!PV.IsMine)
            return;
        
        Move();
        Jump();
        if (!menuTransform.GetComponent<Inventory>().InventoryOpen && !menuTransform.GetComponent<EscapeMenu>().EscapeOpen)
        {
            Look();
        }
        if(cameraHolder.transform.Find("itemContainer").childCount == 0)
        {
            cam.fieldOfView = fov;
        }
    }
    void Move()
    {
        grounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        
        if(grounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        //Speed
        if(Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.S))
        {
            if(Input.GetKey(KeyCode.LeftShift) && canSprint)
            {
                Speed = 7;
            }
            else
            {
                Speed = 5;
            }
        } else
        {
            if(grounded)
            {
                Speed = 4;
            }
            else
            {
                Speed = 3.5f;
            }
        }
        if (Input.GetMouseButton(1))
        {
            Speed = 3.5f;
        }

        //if(Input.GetKeyUp(KeyCode.W) && !grounded)
        //{
        //    forwardJump = false;
        //}

        //Crouching
        if (Input.GetKey(KeyCode.LeftControl))
        {
            crouching = true;

            controller.height = 1f;

            Speed = 2.5f;
        } 
        else
        {
            crouching = false;

            controller.height = Mathf.Lerp(controller.height, 2f, crouchSpeed);
        }

        ////Speed
        //if (Input.GetKey(KeyCode.LeftShift) && !crouching && !forwardJump)
        //{
        //    //Aim
        //    if (Input.GetMouseButton(1) || !grounded)
        //    {
        //        Speed = 2.5f;
        //    }
        //    //Sprint
        //    else
        //    {
        //        Speed = 7;
        //    }
        //}
        //else
        //{
        //    //Aim
        //    if (Input.GetMouseButton(1) || !grounded)
        //    {
        //        Speed = 2.5f;
        //    }
        //    //Walking
        //    else
        //    {
        //        Speed = 5;
        //    }
        //}
    
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;

        move = Vector3.ClampMagnitude(move, 1);

        controller.Move(Speed * move * Time.deltaTime);

        //Jump Momentum

        //GetComponent<Rigidbody>().AddForce(transform.forward * 5, ForceMode.Impulse);

        //Gravity
        velocity.y += gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);
    }

    void Jump()
    {
        //Jump
        if (Input.GetKeyDown(KeyCode.Space) && grounded && canJump)
        {
            if(Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.W))
            {
                forwardJump = true;
                Speed = 7;
            }
            
            canJump = false;

            velocity.y += Mathf.Sqrt(jumpHeight * -2f * gravity);

            Invoke("JumpCooldown", 1f);

            Invoke("DoubleJumpCooldown", 0.2f);
        }
        //Jetpack
        //if(grounded)
        //{
            //canDoubleJump = false;
        //}

        if(Input.GetKeyDown(KeyCode.Space) && !grounded && canDoubleJump)
        {
            velocity.y = -2f;

            //canDoubleJump = false;

            velocity.y += Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
    }

    private void JumpCooldown()
    {
        canJump = true;
    }

    //private void DoubleJumpCooldown()
    //{
        //canDoubleJump = true;
    //}

    void Look()
    {
        float hSens;
        float vSens;
        if (Input.GetMouseButton(1))
        {
            hSens = hSensitivity * aimSensitivity;
            vSens = vSensitivity * aimSensitivity;
        }else
        {
            hSens = hSensitivity;
            vSens = vSensitivity;
        }
        
        if (!isShooting)
        {
            transform.Rotate(Vector3.up * Input.GetAxisRaw("Mouse X") * hSens);

            xRotation += Input.GetAxisRaw("Mouse Y") * vSens * Time.fixedDeltaTime;
            xRotation = Mathf.Clamp(xRotation, -75f, 85f);

            recoil.localRotation = Quaternion.Euler(-xRotation, 0, 0);
            
        } else
        {
            transform.Rotate(Vector3.up * Input.GetAxisRaw("Mouse X") * hSens);
            
            if (cameraHolder.transform.localRotation.x < 0.5f && cameraHolder.transform.localRotation.x > -0.5f)
            {
                cameraHolder.transform.Rotate(Vector3.left * Input.GetAxisRaw("Mouse Y") * vSens * Time.fixedDeltaTime);
            }
        }
    }
}
