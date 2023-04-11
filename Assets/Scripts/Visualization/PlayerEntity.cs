using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using TMPro;
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
    private bool timeFieldIsActive;

    private Vector3 timeFieldOriginalScale;

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

    private Data.Player player;
    [SerializeField] public TextMeshPro tmpPlayerName;

    public override void OnStartClient()
    {
        base.OnStartClient();
        if (base.IsServer)
        {
            //player = GameObject.FindGameObjectWithTag("ClientGameManager").GetComponent<ClientGameManager>().playerData;
            //player.health = 100;
            //player.playerObject = gameObject;
            //player.connection = GetComponent<NetworkObject>().Owner;

            //tmpPlayerName.text = player.name;

            //playerManager = PlayerManager.instance;
            //int id = gameObject.GetInstanceID();
            //Debug.Log("Player ID: " + id);

            //playerManager.players.Add(id, player);
        }
        if (base.IsOwner)
        {
            // Sis�lt��k� "player" nyt kopion "playerData"sta, vai onko se referenssi t�h�n? Vanha syntaksi alla
            //Data.Player player = new Data.Player() { health = 100, playerObject = gameObject, connection = GetComponent<NetworkObject>().Owner };

            playerCamera = Camera.main;
            playerCamera.transform.position = new Vector3(transform.position.x, transform.position.y + cameraYOffset, transform.position.z);
            playerCamera.transform.SetParent(transform);
        }
        else
        {
            //gameObject.GetComponent<PlayerEntity>().enabled = false;
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void Hit(GameObject hitPlayer, GameObject shooter)
    {
        if (!base.IsOwner)
            return;
        Debug.Log("Player ID: " + hitPlayer.GetInstanceID());
        Debug.Log("Shooter ID: " + shooter.GetInstanceID());
        PlayerManager.instance.DamagePlayer(hitPlayer.GetInstanceID(), 50, shooter.GetInstanceID());
    }


    void Start()
    {
        maxAmmo = 30;
        ammoLeft = maxAmmo;
        shootSpeed = 1;
        reloadTime = 1;

        reloading = false;

        timeFieldOriginalScale = timeField.transform.localScale;
        timeSlow = 1;
        timeFieldIsActive = true;

        characterController = GetComponent<CharacterController>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void FixedUpdate()
    {
        if (!base.IsOwner)
        {
            return;
        }
        timeSlow = 1;
    }

    void Update()
    {
        if (!base.IsOwner)
        {
            return;
        }

        if (shootSpeed < 1)
        {
            shootSpeed += Time.deltaTime;
        }
        if (reloadTime < 2)
        {
            reloadTime += Time.deltaTime;
        }

        if (reloadTime >= 1)
        {
            reloading = false;
        }

        Physics.SyncTransforms();
        Move();

        if (Input.GetKey(KeyCode.Mouse0) && ammoLeft > 0 && shootSpeed >= 0.1f && reloadTime >= 1)
        {
            Vector3 direction;
            if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out RaycastHit hit, Mathf.Infinity) && Mathf.Abs((ammoSpawn.transform.position - hit.point).magnitude) >= 2f)
            {
                direction = hit.point - ammoSpawn.transform.position;
            }
            else
            {
                direction = ammoSpawn.transform.forward;
            }

            direction = direction.normalized;

            ShootServer(gameObject, direction);
            shootSpeed = 0;
            ammoLeft -= 1;
        }

        if (Input.GetKeyDown(KeyCode.R) && !reloading && ammoLeft != maxAmmo)
        {
            reloading = true;
            Reload();
        }


        if (!Input.GetKey(KeyCode.Mouse0) && !IsMoving() && !timeFieldIsActive)
        {
            TimeFieldServerActivate(gameObject);
            timeFieldIsActive = true;
        }
        else if (timeFieldIsActive && (IsMoving() || Input.GetKey(KeyCode.Mouse0)))
        {
            TimeFieldServerDeactivate(gameObject);
            timeFieldIsActive = false;
        }

        if (Input.GetKeyDown(KeyCode.G))
        {
            ThrowGrenadeServer();
        }
    }

    public bool IsMoving()
    {
        // Reload? other actions?
        if (Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D) || Input.GetButton("Jump") || !characterController.isGrounded || Input.GetKey(KeyCode.G))
        {
            return true;
        }

        return false;
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
    public void TimeFieldServerDeactivate(GameObject timeField)
    {
        TimeFieldDeactivate(timeField);
    }

    [ObserversRpc]
    public void TimeFieldDeactivate(GameObject timeField)
    {
        timeField.GetComponent<PlayerEntity>().timeField.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
    }

    [ServerRpc]
    public void TimeFieldServerActivate(GameObject timeField)
    {
        TimeFieldActivate(timeField);
    }

    [ObserversRpc]
    public void TimeFieldActivate(GameObject timeField)
    {
        timeField.GetComponent<PlayerEntity>().timeField.transform.localScale = timeFieldOriginalScale;
    }


    [ServerRpc]
    public void ShootServer(GameObject shooter, Vector3 direction)
    {
        Shoot(shooter, direction);
    }

    [ObserversRpc]
    public void Shoot(GameObject shooter, Vector3 direction)
    {
        GameObject ammoInstance = Instantiate(shooter.GetComponent<PlayerEntity>().ammoPrefab, shooter.GetComponent<PlayerEntity>().ammoSpawn.transform.position, Quaternion.identity);
        ammoInstance.GetComponent<AmmoController>().direction = direction;
        ammoInstance.GetComponent<AmmoController>().shooter = shooter;
        Destroy(ammoInstance, 120);
    }

    [ServerRpc]
    public void ThrowGrenadeServer()
    {
        ThrowGrenade();
    }

    [ObserversRpc]
    public void ThrowGrenade(/*GameObject shooter, Vector3 direction*/)
    {
        GameObject chronadeInstance = Instantiate(chronade, ammoSpawn.transform.position, Quaternion.identity);
        chronadeInstance.GetComponent<ChronoGrenade>().ownerObject = gameObject;
        chronadeInstance.GetComponentInChildren<Rigidbody>().AddForce(ammoSpawn.transform.forward * 3, ForceMode.Impulse);
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("TimeSphere") && !other.transform.parent.CompareTag("Player"))
        {
            timeSlow = 0.25f;
        }
    }

    public void Aim()
    {

    }
}