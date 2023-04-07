using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEntity : NetworkBehaviour
{
    public GameObject gunRotator;
    public GameObject ammoSpawn;
    public GameObject ammoPrefab;
    public GameObject timeField;
    [SerializeField] private GameObject chronade;

    public float timeSlow;
    public float shootSpeed;
    public float reloadTime;

    public int maxAmmo, ammoLeft;

    private bool reloading;

    [Header("Base setup")]
    public float walkingSpeed = 7.5f;
    public float runningSpeed = 11.5f;
    public float jumpSpeed = 8.0f;
    public float gravity = 20.0f;
    public float lookSpeed = 2.0f;
    public float lookXLimit = 90f;

    CharacterController characterController;
    Vector3 moveDirection = Vector3.zero;
    float rotationX = 0;

    [HideInInspector]
    public bool canMove = true;

    [SerializeField]
    private float cameraYOffset = 0.4f;
    private Camera playerCamera;
    PlayerManager playerManager;

    public override void OnStartClient()
    {
        base.OnStartClient();
        if (base.IsOwner)
        {
            playerManager = PlayerManager.instance;
            Data.Player player = new Data.Player() { health = 100, playerObject = gameObject, connection = GetComponent<NetworkObject>().Owner };
            int id = gameObject.GetInstanceID();
            Debug.Log("Player ID: " + id);

            playerManager.players.Add(id, player);

            playerCamera = Camera.main;
            playerCamera.transform.position = new Vector3(transform.position.x, transform.position.y + cameraYOffset, transform.position.z);
            playerCamera.transform.SetParent(transform);
        }
        else
        {
            gameObject.GetComponent<PlayerEntity>().enabled = false;
        }
    }

    public void Hit(GameObject player, GameObject bullet, GameObject shooter)
    {
        if (!base.IsOwner)
            return;
        Debug.Log("Player ID: " + player.GetInstanceID());
        Debug.Log("Shooter ID: " + shooter.GetInstanceID());
        PlayerManager.instance.DamagePlayer(player.GetInstanceID(), 50, shooter.GetInstanceID());
        Destroy(bullet);
    }

    void Start()
    {
        maxAmmo = 30;
        ammoLeft = maxAmmo;
        shootSpeed = 1;
        reloadTime = 1;

        reloading = false;

        timeSlow = 1;

        timeField.SetActive(false);

        characterController = GetComponent<CharacterController>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void FixedUpdate()
    {
        timeSlow = 1;
    }

    void Update()
    {
        shootSpeed += Time.deltaTime;
        reloadTime += Time.deltaTime;

        if (reloadTime >= 1)
        {
            reloading = false;
        }


        if (!Input.anyKey)
        {
            timeField.SetActive(true);

        }
        else
        {
            timeField.SetActive(false);
        }

        Physics.SyncTransforms();
        Move();

        if (Input.GetKey(KeyCode.Mouse0) && ammoLeft > 0 && shootSpeed >= 0.1f && reloadTime >= 1)
        {
            ShootServer(gameObject);
            shootSpeed = 0;
        }

        if (Input.GetKeyDown(KeyCode.R) && !reloading && ammoLeft != maxAmmo)
        {
            reloading = true;
            Reload();
        }

        if(Input.GetKeyDown(KeyCode.G))
        {
            ThrowGrenadeServer();
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
        reloadTime = 0;
    }

    [ServerRpc]
    public void ShootServer(GameObject shooter)
    {
        Vector3 direction = shooter.GetComponent<PlayerEntity>().ammoSpawn.transform.forward;

        RaycastHit[] allHits;

        allHits = Physics.RaycastAll(shooter.GetComponent<PlayerEntity>().playerCamera.transform.position, shooter.GetComponent<PlayerEntity>().playerCamera.transform.forward, Mathf.Infinity);

        foreach (RaycastHit hit in allHits)
        {
            if (!hit.collider.CompareTag("TimeSphere"))
            {
                direction = hit.point - shooter.GetComponent<PlayerEntity>().ammoSpawn.transform.position;
                break;
            }
        }

        direction = direction.normalized;

        Shoot(shooter, direction);

        shooter.GetComponent<PlayerEntity>().ammoLeft -= 1;
    }

    [ObserversRpc]
    public void Shoot(GameObject shooter, Vector3 direction)
    {
        GameObject ammoInstance = Instantiate(shooter.GetComponent<PlayerEntity>().ammoPrefab, shooter.GetComponent<PlayerEntity>().ammoSpawn.transform.position, Quaternion.identity);
        ammoInstance.GetComponent<AmmoController>().direction = direction;
        ammoInstance.GetComponent<AmmoController>().shooter = shooter;
        Destroy(ammoInstance, 120);
    }

    //private void OnTriggerEnter(Collider other)
    //{
    //    if (other.CompareTag("TimeSphere"))
    //    {
    //        timeSlow = 0.25f;
    //    }
    //}

    [ServerRpc]
    public void ThrowGrenadeServer()
    {
        ThrowGrenade();
    }

    [ObserversRpc]
    public void ThrowGrenade(/*GameObject shooter, Vector3 direction*/)
    {
        GameObject chronadeInstance = Instantiate(chronade, ammoSpawn.transform.position, Quaternion.identity);
        chronadeInstance.GetComponentInChildren<Rigidbody>().AddForce(ammoSpawn.transform.forward * 3, ForceMode.Impulse);
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("TimeSphere"))
        {
            timeSlow = 0.25f;
        }
    }

    private void OnTriggerExit(Collider other)
    {

    }

    public void Aim()
    {

    }
}