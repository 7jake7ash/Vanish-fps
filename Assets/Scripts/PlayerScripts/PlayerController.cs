using FirstGearGames.SmoothCameraShaker;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviourPun
{
    public static PlayerController Instance;
    public PlayerManager plrManager;
    public TeamColor teamColor;
    public Camera cam;
    private Sway sway;
    public AudioSource walkSound;
    public AudioSource humSound;
    public GameObject SlidingBall;

    //Cam Shake
    public ShakeData shake;

    [SerializeField] GameObject cameraHolder;

    [SerializeField] public float hSensitivity, vSensitivity, aimSensitivity, Speed, jumpSpeed, gravity, jumpHeight, crouchSpeed;

    Quaternion verticalLookRotation;

    public Transform groundCheck;
    public float groundDistance = 0.75f;
    public LayerMask groundMask;
    public float fov = 60f;
    public float audioSpeed = 10;
    public bool grounded;
    bool canJump;
    bool canDoubleJump;
    bool crouching;
    bool sliding;
    bool forwardJump;
    bool justLeftGround;
    bool soundReady = true;
    public bool isShooting;
    public bool canSprint = true;

    public CharacterController controller;

    Vector3 velocity;
    Vector3 move;
    Vector3 jumpMove;
    Vector3 slideMove;

    string jumpKey = "w";

    public PhotonView PV;

    public Transform recoil;
    float xRotation;

    Transform menuTransform;

    public Material[] mats;
    
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

            //if(plrManager.team != )
            //teamColor.outline.OutlineMode = Outline.Mode.OutlineVisible;
            //transform.Find("Model/VanishPlayer/Cube").GetComponent<MeshRenderer>().material = mats[plrManager.team];
        }
        else
        {
            transform.Find("Model/VanishPlayer").gameObject.SetActive(false);
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
        //if (!menuTransform.GetComponent<Inventory>().InventoryOpen && !menuTransform.GetComponent<EscapeMenu>().EscapeOpen)
        if(Cursor.visible == false)
        {
            Look();
        }
        if(cameraHolder.transform.Find("itemContainer").childCount == 0)
        {
            cam.fieldOfView = fov;
        }
    }
    void StopSlide()
    {
        sliding = false;
    }
    void Move()
    {
        grounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        //Crouch Toggle / Slide Toggle
        if (Input.GetKeyDown(KeyCode.C) && Input.GetKey(KeyCode.LeftShift))
        {
            sliding = true;
            slideMove = move;
            Invoke("StopSlide", 3);
        }
        else if(Input.GetKeyDown(KeyCode.C))
        {
            sliding = false;
            //crouching = false;
        }
        if(Input.GetKeyDown(KeyCode.C))
        {
            crouching = !crouching;
        }
        Debug.LogError(sliding);
        if (grounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }
        //Speed
        if(Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.S))
        {
            if (grounded)
            {
                if (canSprint && Input.GetKey(KeyCode.LeftShift) && !Input.GetMouseButton(1))
                    Speed = 8;
                else
                    Speed = 5.5f;
            }
            else
            {
                Speed = 2.5f;
            }
        } else
        {
            if(grounded)
                Speed = 5f;
            else
                Speed = 2.5f;
        }
        
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        //Sliding
        if(sliding)
        {
            controller.height = 1f;

            Speed = 5f;
        }

        //Crouching
        if (crouching)
        {
            controller.height = 1f;

            Speed = 3.5f;

            if(x != 0 || z != 0)
            {
                if(soundReady)
                {
                    photonView.RPC("playAudio", RpcTarget.All, Speed);
                    soundReady = false;
                    Invoke("AudioUpdate", 0.7f);
                }
            }
        }

        //Reset Height
        if (!crouching || !sliding)
        {
            controller.height = Mathf.Lerp(controller.height, 2f, crouchSpeed);
        }
        
        //Audio
        if(x != 0 || z != 0)
        {
            if(soundReady && grounded)
            {
                photonView.RPC("playAudio", RpcTarget.All, Speed);
                soundReady = false;
                Invoke("AudioUpdate", Speed * audioSpeed + 1f);
            }
        }

        if(grounded && !sliding)
        {
            move = transform.right * x + transform.forward * z;

            move = Vector3.ClampMagnitude(move, 1);

            //

            controller.Move(Speed * move * Time.deltaTime);
        }
        else if(!grounded)
        {
            Vector3 m = jumpMove + transform.right * x + transform.forward * z;

            m = Vector3.ClampMagnitude(m, 1);

            controller.Move(jumpSpeed * m * Time.deltaTime);
        }
        else if(sliding)
        {
            move = Vector3.ClampMagnitude(move, 1);

            controller.Move(Speed * slideMove * Time.deltaTime);
        }

        //Gravity
        velocity.y += gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);
    }
    void AudioUpdate()
    {
        soundReady = true;
    }
    [PunRPC]
    void playAudio(float speed)
    {
        walkSound.volume = speed * 0.10f;
        walkSound.pitch = Random.Range(0.9f, 1.1f);
        walkSound.Play();
    }
    void Jump()
    {
        //Jump
        if (Input.GetKeyDown(KeyCode.Space) && grounded && canJump)
        {
            jumpSpeed = Speed;

            jumpMove = move;

            canJump = false;

            Invoke("JumpCooldown", 0.5f);

            velocity.y += Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        if(Input.GetKeyDown(KeyCode.Space) && !grounded && canDoubleJump)
        {
            velocity.y = -2f;

            velocity.y += Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
    }

    private void JumpCooldown()
    {
        canJump = true;
    }

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

    public void playSound()
    {
        photonView.RPC("PlayAudio", RpcTarget.All);
    }

    [PunRPC]
    void PlayAudio()
    {
        humSound.Play();
        Invoke("stopSound", GameObject.Find("Countdown").GetComponent<Photon.Pun.UtilityScripts.Timer>().Countdown);
    }

    void stopSound()
    {
        humSound.Stop();
    }
}
