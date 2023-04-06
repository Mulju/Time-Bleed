using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEntity : NetworkBehaviour
{
    public int connectionID;
    public Logic.CombatResolver resolver;

    public GameObject gunRotator;
    public GameObject ammoSpawn;
    public GameObject ammoPrefab;

    public float timeSlow;

    public int maxAmmo, ammoLeft;

    [Header("Base setup")]
    public float walkingSpeed = 7.5f;
    public float runningSpeed = 11.5f;
    public float jumpSpeed = 8.0f;
    public float gravity = 20.0f;
    public float lookSpeed = 2.0f;
    public float lookXLimit = 45.0f;

    CharacterController characterController;
    Vector3 moveDirection = Vector3.zero;
    float rotationX = 0;

    [HideInInspector]
    public bool canMove = true;

    [SerializeField]
    private float cameraYOffset = 0.4f;
    private Camera playerCamera;


    public override void OnStartClient()
    {
        base.OnStartClient();
        if (base.IsOwner)
        {
            playerCamera = Camera.main;
            playerCamera.transform.position = new Vector3(transform.position.x, transform.position.y + cameraYOffset, transform.position.z);
            playerCamera.transform.SetParent(transform);
        }
        else
        {
            gameObject.GetComponent<PlayerEntity>().enabled = false;
        }
    }

    void Start()
    {
        maxAmmo = 30;
        ammoLeft = maxAmmo;

        timeSlow = 1f;

        characterController = GetComponent<CharacterController>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        Move();

        if (Input.GetKeyDown(KeyCode.Mouse0) && ammoLeft > 0)
        {
            ShootServer(gameObject);
        }

        if(Input.GetKeyDown(KeyCode.R))
        {
            Reload();
        }
    }

    public void Move()
    {
        bool isRunning = false;

        // Press Left Shift to run
        isRunning = Input.GetKey(KeyCode.LeftShift);

        // We are grounded, so recalculate move direction based on axis
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);

        float curSpeedX = canMove ? (isRunning ? runningSpeed : walkingSpeed) * Input.GetAxis("Vertical") : 0;
        float curSpeedY = canMove ? (isRunning ? runningSpeed : walkingSpeed) * Input.GetAxis("Horizontal") : 0;
        float movementDirectionY = moveDirection.y;
        moveDirection = (forward * curSpeedX) + (right * curSpeedY);

        if (Input.GetButton("Jump") && canMove && characterController.isGrounded)
        {
            moveDirection.y = jumpSpeed;
        }
        else
        {
            moveDirection.y = movementDirectionY;
        }

        if (!characterController.isGrounded)
        {
            moveDirection.y -= gravity * Time.deltaTime;
        }

        // Move the controller
        characterController.Move(moveDirection * Time.deltaTime * timeSlow);

        // Player and Camera rotation
        if (canMove && playerCamera != null)
        {
            rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
            rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
            playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
            transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);

            gunRotator.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
        }
    }

    public void Reload()
    {
        ammoLeft = maxAmmo;
    }

    [ServerRpc]
    public void ShootServer(GameObject shooter)
    {
        Vector3 direction;

        if (Physics.Raycast(shooter.GetComponent<PlayerEntity>().playerCamera.transform.position, shooter.GetComponent<PlayerEntity>().playerCamera.transform.forward, out RaycastHit hit, Mathf.Infinity))
        {
            direction = hit.point - shooter.GetComponent<PlayerEntity>().ammoSpawn.transform.position;
        }
        else
        {
            direction = shooter.GetComponent<PlayerEntity>().ammoSpawn.transform.forward;
        }

        direction = direction.normalized;

        Shoot(shooter, direction);

        shooter.GetComponent<PlayerEntity>().ammoLeft -= 1;
    }
     
    [ObserversRpc]
    public void Shoot(GameObject shooter, Vector3 direction)
    {
        GameObject ammoInstance = Instantiate(shooter.GetComponent<PlayerEntity>().ammoPrefab, shooter.GetComponent<PlayerEntity>().ammoSpawn.transform.position, Quaternion.identity);
        ammoInstance.GetComponent<Rigidbody>().velocity = direction * 15;
        Destroy(ammoInstance, 2);
    }


    public void Aim()
    {

    }
}
