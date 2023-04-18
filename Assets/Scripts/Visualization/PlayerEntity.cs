using FishNet.Connection;
using FishNet.Managing;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerEntity : NetworkBehaviour
{
    public GameObject gunRotator;
    public GameObject ammoSpawn;
    public GameObject ammoPrefab;
    public GameObject timeField;
    public GameObject bulletHole;
    public GameObject timeBindSkill;
    [SerializeField] private GameObject chronade;

    private GameObject reloadBar, reloadBackground, reloadParent;
    private GameObject damageIndicatorParent;

    public float timeSlow;
    public float shootSpeed;
    public float reloadTime;
    public float timeBindTimer, timeBindCooldown;
    public float chronadeTimer, chronadeCooldown;
    public float recoil;

    public int maxAmmo, ammoLeft;

    private bool reloading;
    private bool timeFieldIsActive;

    [SyncVar] public float timeSpeed;
    private float mouseScroll;

    [HideInInspector]
    public float headDamage = 2f, torsoDamage = 1f, legsDamage = 0.7f;

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

    [SyncVar] public string playerName;
    public TextMeshPro tmpPlayerName;
    [SerializeField] private TextMeshPro debugConsole;
    public string newPlayersName;

    [HideInInspector] public MenuControl menuControl;

    // For health and ammo UI
    public TextMeshProUGUI healthTMP, ammoTMP;

    [SerializeField] private SoundControl soundControl;
    [SerializeField] private GameObject playerHitEffect;

    private Slider speedSlider = null;

    [SerializeField] private Material redTeamMaterial;
    [SerializeField] private Material greenTeamMaterial;
    [SerializeField] private GameObject body;
    [HideInInspector] public int ownTeamTag;

    [SerializeField] private GameObject rayCastVisual;
    [SerializeField] private GameObject spawnForRayVisual;
    private MatchManager mManager;

    public override void OnStartClient()
    {
        // This function is run on all player entities in the scene. Depending on is the user the owner of that object or the server,
        // different behaviours are done.
        base.OnStartClient();
        playerManager = PlayerManager.instance;

        // Only run if you are the owner of this object. Skip for all other player entities in the scene.
        if (base.IsOwner)
        {
            // Sis�lt��k� "player" nyt kopion "playerData"sta, vai onko se referenssi t�h�n? Vanha syntaksi alla
            //Data.Player player = new Data.Player() { health = 100, playerObject = gameObject, connection = GetComponent<NetworkObject>().Owner };

            playerCamera = Camera.main;
            playerCamera.transform.position = new Vector3(transform.position.x, transform.position.y + cameraYOffset, transform.position.z);
            playerCamera.transform.SetParent(transform);

            speedSlider = GameObject.FindGameObjectWithTag("SpeedSlider").GetComponent<Slider>();


            TimeSpeedSlider(speedSlider.value);
        }

        // This part is run for all the entities in the scene if you are the server.
        if (base.IsServer)
        {
            Data.Player player = new Data.Player() { health = 100, playerObject = gameObject, connection = GetComponent<NetworkObject>().Owner };
            int id = gameObject.GetInstanceID();
            Debug.Log("Player ID: " + id);
            debugConsole.text = "Player ID: " + id;

            playerManager.AddPlayer(id, player);

            // Change the match state to waiting for players
            mManager.currentMatchState = MatchManager.MatchState.WAITING_FOR_PLAYERS;
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void Hit(GameObject hitPlayer, GameObject shooter, float damageMultiplier)
    {
        if (hitPlayer.GetComponent<PlayerEntity>().ownTeamTag == shooter.GetComponent<PlayerEntity>().ownTeamTag)
        {
            // If the shooter is in the same team as you, don't take damage
            return;
        }

        int damageAmount = Mathf.FloorToInt(20 * damageMultiplier);
        PlayerManager.instance.DamagePlayer(hitPlayer.GetInstanceID(), damageAmount, shooter.GetInstanceID());
        Debug.Log("Player ID: " + hitPlayer.GetInstanceID());
        Debug.Log("Shooter ID: " + shooter.GetInstanceID());
    }

    public void ShowDamageDirection(GameObject player, Vector3 direction)
    {
        damageIndicatorParent.GetComponent<DmgIndicatorSystem>().AddDamageIndicator(player, direction);
    }

    public void AmmoHit(GameObject hitPlayer, GameObject shooter, float damageMultiplier)
    {
        if (base.IsServer)
        {
            Hit(hitPlayer, shooter, damageMultiplier);
        }
    }

    private void OnDisable()
    {
        //gameObject.GetComponent<NetworkObject>().Owner.Disconnect(false);
        playerManager.RemovePlayer(gameObject.GetComponent<NetworkObject>().Owner);
    }

    void Start()
    {
        mManager = MatchManager.matchManager;

        maxAmmo = 30;
        ammoLeft = maxAmmo;
        shootSpeed = 1;
        reloadTime = 1;

        recoil = 0.3f;

        timeBindCooldown = 10f;
        timeBindTimer = timeBindCooldown;



        chronadeCooldown = 5f;
        chronadeTimer = chronadeCooldown;

        reloading = false;

        timeFieldOriginalScale = timeField.transform.localScale;
        timeSlow = 1;
        timeFieldIsActive = true;

        timeSpeed = 1f;
        mouseScroll = 0f;

        timeField.GetComponent<TimeSphere>().isTimeField = true;

        characterController = GetComponent<CharacterController>();

        menuControl = GameObject.FindGameObjectWithTag("MenuControl").GetComponent<MenuControl>();

        healthTMP = GameObject.FindGameObjectWithTag("UIHealth").GetComponent<TextMeshProUGUI>();
        ammoTMP = GameObject.FindGameObjectWithTag("UIAmmo").GetComponent<TextMeshProUGUI>();

        reloadParent = GameObject.FindGameObjectWithTag("ReloadParent");

        foreach (Transform child in reloadParent.transform)
        {
            child?.gameObject.SetActive(true);
        }

        reloadBackground = GameObject.FindGameObjectWithTag("ReloadBackground");
        reloadBar = GameObject.FindGameObjectWithTag("ReloadBar");
        damageIndicatorParent = GameObject.Find("DmgIndicatorHolder");

        reloadBackground.SetActive(false);
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
        if(mManager.currentMatchState == MatchManager.MatchState.MATCH_ENDED)
        {
            // Match ended
            return;
        }

        if (!base.IsOwner)
        {
            return;
        }

        if (timeBindTimer < timeBindCooldown)
        {
            timeBindTimer += Time.deltaTime;
        }

        if (chronadeTimer < chronadeCooldown)
        {
            chronadeTimer += Time.deltaTime;
        }

        if (shootSpeed < 1)
        {
            shootSpeed += Time.deltaTime;
        }

        if (reloadTime < 1)
        {
            reloadTime += Time.deltaTime;

            // reload bar animation
            reloadBar.GetComponent<RectTransform>().localScale = new Vector3(reloadTime, reloadBar.GetComponent<RectTransform>().localScale.y, reloadBar.GetComponent<RectTransform>().localScale.z);
        }

        if (reloadTime >= 1 && reloading)
        {
            reloading = false;
            reloadBackground.gameObject.SetActive(false);
        }

        Physics.SyncTransforms();
        Move();

        if (Input.GetKey(KeyCode.Mouse0) && ammoLeft > 0 && shootSpeed >= 0.1f && reloadTime >= 1)
        {
            //Vector3 direction;
            //if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out RaycastHit hit, Mathf.Infinity) && Mathf.Abs((ammoSpawn.transform.position - hit.point).magnitude) >= 0.6f)
            //{
            //    direction = hit.point - ammoSpawn.transform.position;
            //}
            //else
            //{
            //    direction = ammoSpawn.transform.forward;
            //}
            //direction = direction.normalized;

            //direction = (playerCamera.transform.forward);

            ShootServer(gameObject);
            shootSpeed = 0;
            ammoLeft -= 1;

            rotationX -= recoil;
        }

        if ((Input.GetKeyDown(KeyCode.R) || ammoLeft == 0) && !reloading && ammoLeft != maxAmmo)
        {
            reloading = true;
            Reload();
        }

        if (base.IsOwner)
        {
            // If you own this player entity, change the ammo in the UI
            ammoTMP.text = "Ammo - " + ammoLeft;
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

        if (Input.GetKeyDown(KeyCode.G) && chronadeTimer >= chronadeCooldown)
        {
            ThrowGrenadeServer();
            chronadeTimer = 0;
        }

        if (Input.GetKeyDown(KeyCode.F) && timeBindTimer >= timeBindCooldown)
        {
            timeBindTimer = 0;
            TimeBindServer();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // Press Esc for pause screen and to lock/unlock cursor
            playerManager.ChangeCursorLock();
            menuControl.OpenCloseMenu();
        }

        if (Input.mouseScrollDelta.y != 0)
        {
            mouseScroll = Input.mouseScrollDelta.y;

            // change slider value 
            if (speedSlider != null)
            {
                speedSlider.value += mouseScroll * 0.05f;
            }

            TimeSpeedSlider(mouseScroll * 0.05f);
        }
    }

    private void OnApplicationQuit()
    {
        // LocalConnection
        playerManager.RemovePlayer(gameObject.GetComponent<NetworkObject>().Owner);
    }

    public void ChangeTeam(int teamTag)
    {
        ownTeamTag = teamTag;

        if (teamTag == 0)
        {
            body.GetComponent<Renderer>().material = redTeamMaterial;
        }
        else
        {
            body.GetComponent<Renderer>().material = greenTeamMaterial;
        }
    }

    [ServerRpc]
    public void RespawnServer()
    {
        Respawn();
    }

    [ObserversRpc]
    public void Respawn()
    {
        timeField.GetComponent<TimeSphere>().IncreaseCircumference();
    }

    [ServerRpc]
    public void TimeSpeedSlider(float sliderValue)
    {
        timeSpeed += sliderValue;
        if (timeSpeed < 0.1)
        {
            timeSpeed = 0.1f;
        }
        else if (timeSpeed > 1f)
        {
            timeSpeed = 1f;
        }
        UpdateTimeSpeed();
    }

    [ObserversRpc]
    public void UpdateTimeSpeed()
    {
        timeField.GetComponent<TimeSphere>().timeSpeed = timeSpeed;
        timeField.GetComponent<TimeSphere>().ChangeAlpha(timeSpeed);
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
        // isRunning = Input.GetKey(KeyCode.LeftShift);

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

        if (!characterController.isGrounded && (timeSlow < 1f || timeSpeed < 1f))
        {
            moveDirection.y -= gravity * Time.deltaTime * timeSlow * timeSpeed;
        }
        else if(!characterController.isGrounded)
        {
            moveDirection.y -= gravity * Time.deltaTime;
        }

        // Move the controller
        characterController.Move(moveDirection * Time.deltaTime * timeSlow * timeSpeed * 0.8f);

        // Player and Camera rotation
        if (canMove && playerCamera != null && Cursor.lockState == CursorLockMode.Locked)
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

        reloadBackground.gameObject.SetActive(true);
    }

    [ServerRpc]
    public void TimeFieldServerDeactivate(GameObject timeField)
    {
        TimeFieldDeactivate(timeField);
    }

    [ObserversRpc]
    public void TimeFieldDeactivate(GameObject timeField)
    {
        //timeField.GetComponent<PlayerEntity>().timeField.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);

        //timeField.GetComponent<TimeSphere>().ReduceCircumference();
    }

    [ServerRpc]
    public void TimeFieldServerActivate(GameObject timeField)
    {
        TimeFieldActivate(timeField);
    }

    [ObserversRpc]
    public void TimeFieldActivate(GameObject timeField)
    {
        //timeField.GetComponent<PlayerEntity>().timeField.transform.localScale = timeFieldOriginalScale;
    }


    [ServerRpc]
    public void ShootServer(GameObject shooter)
    {
        Shoot(shooter);
    }

    [ObserversRpc]
    public void Shoot(GameObject shooter)
    {
        Vector3 startPos = shooter.transform.position + new Vector3(0, cameraYOffset, 0);
        Vector3 direction = shooter.GetComponent<PlayerEntity>().gunRotator.transform.forward;
        soundControl.PlayShootSound();

        if (Physics.Raycast(startPos + direction, direction, out RaycastHit hit, Mathf.Infinity))
        {
            if (!ammoSpawn.GetComponent<AmmoSpawn>().isSlowed)
            {
                // Line visual for the shot, only if not in a timesphere
                LineRenderer instantiatedVisual = Instantiate(rayCastVisual).GetComponent<LineRenderer>();
                instantiatedVisual.SetPosition(0, spawnForRayVisual.transform.position);
                instantiatedVisual.SetPosition(1, hit.point);
                Destroy(instantiatedVisual, 2);
            }

            if (ammoSpawn.GetComponent<AmmoSpawn>().isSlowed)
            {
                GameObject ammoInstance = Instantiate(shooter.GetComponent<PlayerEntity>().ammoPrefab, shooter.GetComponent<PlayerEntity>().ammoSpawn.transform.position, Quaternion.identity);
                ammoInstance.GetComponent<AmmoController>().direction = direction;
                ammoInstance.GetComponent<AmmoController>().shooter = shooter;
                Destroy(ammoInstance, 120);
            }
            if (ammoSpawn.GetComponent<AmmoSpawn>().isInsideTerrain)
            {
                if (Physics.Raycast(startPos, direction, out RaycastHit bulletHit, Mathf.Infinity))
                {
                    GameObject instantiatedHole = Instantiate(bulletHole, bulletHit.point + bulletHit.normal * 0.0001f, Quaternion.LookRotation(bulletHit.normal));
                    Destroy(instantiatedHole, 10);
                }
            }
            else if (hit.collider.CompareTag("TimeSphere"))
            {
                GameObject ammoInstance = Instantiate(shooter.GetComponent<PlayerEntity>().ammoPrefab, hit.point, Quaternion.identity);
                ammoInstance.GetComponent<AmmoController>().direction = direction;
                ammoInstance.GetComponent<AmmoController>().shooter = shooter;
                Destroy(ammoInstance, 120);
            }
            else if (hit.collider.CompareTag("PlayerHead") && hit.collider.GetComponent<PlayerHead>().player.gameObject != this.gameObject)
            {
                Instantiate(playerHitEffect, hit.point, Quaternion.LookRotation(hit.normal));
                if (base.IsServer)
                {
                    Hit(hit.collider.GetComponent<PlayerHead>().player.gameObject, this.gameObject, headDamage);
                }
                hit.collider.GetComponent<PlayerHead>().player.GetComponent<PlayerEntity>().ShowDamageDirection(hit.collider.GetComponent<PlayerHead>().player.gameObject, direction);
            }
            else if (hit.collider.CompareTag("PlayerTorso") && hit.collider.GetComponent<PlayerTorso>().player.gameObject != this.gameObject)
            {
                Instantiate(playerHitEffect, hit.point, Quaternion.LookRotation(hit.normal));
                if (base.IsServer)
                {
                    Hit(hit.collider.GetComponent<PlayerTorso>().player.gameObject, this.gameObject, torsoDamage);
                }
                hit.collider.GetComponent<PlayerTorso>().player.GetComponent<PlayerEntity>().ShowDamageDirection(hit.collider.GetComponent<PlayerTorso>().player.gameObject, direction);
            }
            else if (hit.collider.CompareTag("PlayerLegs") && hit.collider.GetComponent<PlayerLegs>().player.gameObject != this.gameObject)
            {
                Instantiate(playerHitEffect, hit.point, Quaternion.LookRotation(hit.normal));
                if (base.IsServer)
                {
                    Hit(hit.collider.GetComponent<PlayerLegs>().player.gameObject, this.gameObject, legsDamage);
                }
                hit.collider.GetComponent<PlayerLegs>().player.GetComponent<PlayerEntity>().ShowDamageDirection(hit.collider.GetComponent<PlayerLegs>().player.gameObject, direction);
            }
            else if (!hit.collider.gameObject.CompareTag("Player"))
            {
                GameObject instantiatedHole = Instantiate(bulletHole, hit.point + hit.normal * 0.0001f, Quaternion.LookRotation(hit.normal));
                Destroy(instantiatedHole, 10);
            }
        }

        //GameObject ammoInstance = Instantiate(shooter.GetComponent<PlayerEntity>().ammoPrefab, shooter.GetComponent<PlayerEntity>().ammoSpawn.transform.position, Quaternion.identity);
        //ammoInstance.GetComponent<AmmoController>().direction = direction;
        //ammoInstance.GetComponent<AmmoController>().shooter = shooter;
        //Destroy(ammoInstance, 120);
    }

    [ServerRpc]
    public void TimeBindServer()
    {
        TimeBind();
    }

    [ObserversRpc]
    public void TimeBind()
    {
        GameObject timeBindInstance = Instantiate(timeBindSkill, ammoSpawn.transform.position, Quaternion.identity);
        timeBindInstance.GetComponentInChildren<Rigidbody>().AddForce(new Vector3(ammoSpawn.transform.forward.x, ammoSpawn.transform.forward.y + 0.2f, ammoSpawn.transform.forward.z) * 4, ForceMode.Impulse);
        Destroy(timeBindInstance, 25);
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
        chronadeInstance.GetComponentInChildren<Rigidbody>().AddForce(new Vector3(ammoSpawn.transform.forward.x, ammoSpawn.transform.forward.y + 0.2f, ammoSpawn.transform.forward.z) * 4, ForceMode.Impulse);
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("TimeSphere") && (other.transform.parent == null || !other.transform.parent.CompareTag("Player")))
        {
            timeSlow = 0.25f;
        }
    }

    public void Aim()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        //if (other.CompareTag("Ammo") && other.TryGetComponent<AmmoController>(out AmmoController ammo) && other.GetComponent<AmmoController>().shooter != this.gameObject)
        //{
        //    PlayerEntity player = gameObject.GetComponent<PlayerEntity>();

        //    if (base.IsOwner)
        //    {
        //        player.Hit(gameObject, ammo.shooter);
        //    }

        //    Destroy(other.gameObject);
        //}
    }
}