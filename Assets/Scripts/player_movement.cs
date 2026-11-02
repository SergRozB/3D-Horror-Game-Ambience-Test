using UnityEngine;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private Transform playerModel;
    [SerializeField] private float playerSpeedWalking = 5;
    [SerializeField] private float playerSpeedRunning = 10;
    [SerializeField] private AudioClip walkingClip;
    [SerializeField] private AudioClip runningClip;
    [SerializeField] private GroundCheckScript groundCheck;
    [SerializeField] private float gravityForce;
    [SerializeField] private bool isOnGround;
    public bool isRunning = false;  
    private float percentageOfAudioClipDone = 0;
    private AudioSource playerAudioSource;
    [SerializeField] private bool isMoving = false;
    private bool isJumping = false;
    private float playerSpeed;
    private PlayerInput playerInput;
    private InputAction movementAction;
    public Vector2 turn; // Stores the x and y rotation of the camera, which is used for rotating the player and camera in the RotateCamera function. X is the horizontal rotation, Y is the vertical rotation
    public float mouseSensitivity = 5;
    [SerializeField] private Camera playerCamera;
    private Rigidbody rb;
    [SerializeField] private float jumpForce = 20;
    private Vector3 newPlayerDirection;
    [SerializeField] private GameObject rotationObject;
    public bool pastOldMusicPoint = false; // Stores if the player has gotten close enough to the new area to start fading the old ambient music out
    [SerializeField] private GameObject newMusicAreaPoint; // The point the player has to get close to for the new ambient music to fade in
    [SerializeField] private GameObject furthestDistanceMusicPoint; 
    public float percentageDistanceTo; // The percentage distance to the new music area point, used for fading music in and out
    [SerializeField] private float cameraRotationSpeed = 1;
    [SerializeField] private GameObject interactableSelectedIcon;
 
    private void Awake()
    {
        rb = GetComponentInChildren<Rigidbody>();
        playerInput = GetComponent<PlayerInput>();
        Cursor.lockState = CursorLockMode.Locked;
        playerSpeed = playerSpeedWalking;
        playerAudioSource = GetComponentInChildren<AudioSource>();
    }

    void Update()
    {
        isOnGround = groundCheck.isOnGround;
        RotateCamera();
        CheckInputs();
        HandleMovement();
        HandlePlayerMovementSound();
        PhaseOutOldAmbient();
        SendRaycast();
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = newPlayerDirection;
        if (!isOnGround)
        {
            rb.AddForce(new Vector3 (0, -gravityForce, 0));
        }

        if(isJumping)
        {
            rb.AddForce(new Vector3(0, jumpForce, 0), ForceMode.Impulse);
        }
    }
    private void RotateCamera()
    {
        turn.x += Input.GetAxis("Mouse X");
        turn.y += Input.GetAxis("Mouse Y");
        float xRotation = Mathf.Clamp(-turn.y * mouseSensitivity, -90, 90);
        float yRotation = turn.x * mouseSensitivity;
        playerCamera.transform.rotation = Quaternion.Euler(xRotation, transform.rotation.eulerAngles.y, 0);
        transform.rotation = Quaternion.Euler(0, yRotation, 0);
        rotationObject.transform.rotation = Quaternion.Euler(0, yRotation, 0);
        //playerModel.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
    }

    private void PhaseOutOldAmbient() 
    {
        float distanceToNewMusicPoint = Vector3.Distance(transform.position, newMusicAreaPoint.transform.position);
        float maxDistanceToNewMusicPoint = Vector3.Distance(newMusicAreaPoint.transform.position, furthestDistanceMusicPoint.transform.position);
        percentageDistanceTo = Mathf.Clamp01(distanceToNewMusicPoint / maxDistanceToNewMusicPoint);
        if (distanceToNewMusicPoint < 3) 
        {
            pastOldMusicPoint = true;
        }
    }

    private void CheckInputs()
    {
        if (Keyboard.current.spaceKey.isPressed && isOnGround)
        {
            isJumping = true;
        }

        else
        {
            isJumping = false;
        }

        if (Keyboard.current.shiftKey.isPressed)
        {
            playerSpeed = playerSpeedRunning;
            isRunning = true;
        }
        else
        {
            playerSpeed = playerSpeedWalking;
            isRunning = false;
        }
    }

    private void HandlePlayerMovementSound()
    {
        if (isMoving && !playerAudioSource.isPlaying && isOnGround)
        {
            if (playerSpeed == playerSpeedWalking)
            {
                playerAudioSource.clip = walkingClip;
            }

            if (playerSpeed == playerSpeedRunning)
            {
                playerAudioSource.clip = runningClip;
            }
            playerAudioSource.time = percentageOfAudioClipDone * playerAudioSource.clip.length;
            playerAudioSource.Play();
        }
        else
        {
            if (playerAudioSource.isPlaying)
            {
                if (isMoving)
                {
                    percentageOfAudioClipDone = playerAudioSource.time / playerAudioSource.clip.length;

                    if (playerAudioSource.clip == walkingClip && playerSpeed == playerSpeedRunning)
                    {
                        playerAudioSource.clip = runningClip;
                        playerAudioSource.time = percentageOfAudioClipDone*runningClip.length;
                    }

                    else if (playerAudioSource.clip == runningClip && playerSpeed == playerSpeedWalking)
                    {
                        playerAudioSource.clip = walkingClip;
                        playerAudioSource.time = percentageOfAudioClipDone*walkingClip.length;
                    }
                }

                else
                    playerAudioSource.Stop();

                if (!isOnGround)
                {
                    playerAudioSource.Stop();
                }
            }
        }
    }

    private void HandleMovement()
    {
        newPlayerDirection = new Vector3(ReturnDirection().x * playerSpeed, rb.linearVelocity.y, ReturnDirection().z * playerSpeed);
        Vector3 lookDir = rotationObject.transform.forward;
        Quaternion lookRotation = Quaternion.LookRotation(lookDir, Vector3.up);
        newPlayerDirection = lookRotation * newPlayerDirection;
    }

    private Vector3 ReturnDirection()
    {
        Vector3 direction = Vector3.zero;
        if (Keyboard.current.wKey.isPressed)
        {
            direction.z = 1;
        }
        if (Keyboard.current.aKey.isPressed)
        {
            direction.x = -1;
        }
        if (Keyboard.current.dKey.isPressed)
        {
            direction.x = 1;
        }
        if (Keyboard.current.sKey.isPressed)
        {
            direction.z = -1;
        }

        if (direction.magnitude > 0)
        {
            direction = direction.normalized;
            isMoving = true;
        }
        else
        {
            isMoving = false;
        }

            return direction;
    }

    private void SendRaycast()
    {
        RaycastHit hit;
        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit))
        {
            if (hit.transform.gameObject.CompareTag("Door"))
            {
                float distanceToDoor = Vector3.Distance(transform.position, hit.transform.position);
                if (distanceToDoor < 2.5f)
                {
                    interactableSelectedIcon.SetActive(true);

                    if (Keyboard.current.eKey.isPressed)
                    {
                        DoorScript doorScript = hit.transform.gameObject.GetComponent<DoorScript>();
                        doorScript.swing = true;
                    }
                }
            }

            else 
            {
                interactableSelectedIcon.SetActive(false);
            }
        }
    }
}