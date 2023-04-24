using FishNet.Connection;
using FishNet.Managing;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;


public class PlayerEntity : NetworkBehaviour
{
    public GameObject gunRotator;
    public GameObject ammoSpawn;
    public GameObject ammoPrefab;
    public GameObject timeField;
    public GameObject bulletHole;
    public GameObject timeBindSkill;

    [SerializeField] private GameObject chronade;
    [SerializeField] private GameObject sniperScope;
    [SerializeField] private GameObject aimingPosition;
    [SerializeField] private GameObject grenadePrefab;

    [SerializeField] private GameObject riflePrefab;
    [SerializeField] private GameObject sniperPrefab;
    [SerializeField] private GameObject shotgunPrefab;
    private GameObject currentWeaponPrefab;

    public Image timeBindUI;
    public Image GrenadeUI;

    public GameObject gunPosition;

    private Coroutine aimDownSightInstance;

    private Vector3 gunOriginalPosition;

    private GameObject damageIndicatorParent;
    public Animator animator;

    private WeaponDictionary weaponDictionary;
    public Data.Weapon currentWeapon;

    public float timeSlow;
    public float shootTimer;
    public float timeBindTimer, timeBindCooldown;
    public float chronadeTimer, chronadeCooldown;
    public float grenadeTimer, grenadeCooldown;
    public float recoil;

    private bool isCooking;
    private float cookTimer;
    private float deployTimer;
    private bool isScoped;

    private bool reloading;
    private Coroutine reloadCoroutine;

    [SyncVar] public float timeSpeed;
    private float mouseScroll;

    [HideInInspector]
    public float headDamage = 2f, torsoDamage = 1f, legsDamage = 0.7f;

    [Header("Base setup")]
    public float walkingSpeed = 8.5f;
    public float runningSpeed = 11.5f;
    public float jumpSpeed = 8.0f;
    public float gravity = 20.0f;
    public float lookSpeed = 2.0f;
    public float lookXLimit = 90f;

    public float sensitivity = 1f;

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

    public int amountOfChronades = 1;

    private LayerMask layerMask;

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

            weaponDictionary = new WeaponDictionary();
            currentWeapon = weaponDictionary.weapons["rifle"];

            shootTimer = 3;

            timeBindUI = GameObject.FindGameObjectWithTag("TimeBindCooldown").GetComponent<Image>();
            GrenadeUI = GameObject.FindGameObjectWithTag("GrenadeCooldown").GetComponent<Image>();
        }

        // This part is run for all the entities in the scene if you are the server.
        if (base.IsServer)
        {
            Data.Player player = new Data.Player() { health = 100, playerObject = gameObject, connection = GetComponent<NetworkObject>().Owner };
            int id = gameObject.GetInstanceID();

            playerManager.AddPlayer(id, player);

            // Change the match state to waiting for players
            mManager.currentMatchState = MatchManager.MatchState.WAITING_FOR_PLAYERS;

            timeSpeed = 1f;
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void Hit(GameObject hitPlayer, GameObject shooter, float damageMultiplier, int damage)
    {
        if (hitPlayer.GetComponent<PlayerEntity>().ownTeamTag == shooter.GetComponent<PlayerEntity>().ownTeamTag)
        {
            // If the shooter is in the same team as you, don't take damage
            return;
        }

        int damageAmount = Mathf.FloorToInt(damage * damageMultiplier);
        PlayerManager.instance.DamagePlayer(hitPlayer.GetInstanceID(), damageAmount, shooter.GetInstanceID());
    }

    public void ShowDamageDirection(GameObject player, Vector3 direction)
    {
        damageIndicatorParent.GetComponent<DmgIndicatorSystem>().AddDamageIndicator(player, direction);
    }

    public void AmmoHit(GameObject hitPlayer, GameObject shooter, float damageMultiplier, int damage)
    {
        if (base.IsServer)
        {
            Hit(hitPlayer, shooter, damageMultiplier, damage);
        }
    }

    void Start()
    {
        gunOriginalPosition = gunPosition.transform.localPosition;

        mManager = MatchManager.matchManager;

        currentWeaponPrefab = riflePrefab;

        recoil = 0.3f;

        isScoped = false;

        deployTimer = 4f;
        cookTimer = 0;
        isCooking = false;

        timeBindCooldown = 10f;
        timeBindTimer = timeBindCooldown;

        grenadeCooldown = 10f;
        grenadeTimer = grenadeCooldown;

        reloading = false;

        timeSlow = 1;

        mouseScroll = 0f;

        timeField.GetComponent<TimeSphere>().isTimeField = true;

        characterController = GetComponent<CharacterController>();

        menuControl = GameObject.FindGameObjectWithTag("MenuControl").GetComponent<MenuControl>();

        healthTMP = GameObject.FindGameObjectWithTag("UIHealth").GetComponent<TextMeshProUGUI>();
        ammoTMP = GameObject.FindGameObjectWithTag("UIAmmo").GetComponent<TextMeshProUGUI>();

        damageIndicatorParent = GameObject.Find("DmgIndicatorHolder");

        layerMask = LayerMask.GetMask("Player", "Terrain", "Water", "Default");
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
        if (mManager.currentMatchState == MatchManager.MatchState.MATCH_ENDED)
        {
            // Match ended
            //menuControl.OpenEndMatchScoreboard(MatchManager.matchManager.currentVictoryState);
            return;
        }

        if (!base.IsOwner)
        {
            return;
        }

        if (deployTimer < 4f)
        {
            deployTimer += Time.deltaTime;
        }

        if (timeBindTimer < timeBindCooldown)
        {
            timeBindTimer += Time.deltaTime;
            timeBindUI.fillAmount -= Time.deltaTime / 10f;
        }

        if (chronadeTimer < chronadeCooldown)
        {
            chronadeTimer += Time.deltaTime;
        }

        if (grenadeTimer < grenadeCooldown)
        {
            grenadeTimer += Time.deltaTime;
            GrenadeUI.fillAmount -= Time.deltaTime / 10f;
        }

        if (shootTimer < 2)
        {
            shootTimer += Time.deltaTime;
        }

        Physics.SyncTransforms();
        Move();

        if (Input.GetKey(KeyCode.Alpha1) && currentWeapon != weaponDictionary.weapons["rifle"])
        {
            ChangeWeapon(0);
        }
        else if (Input.GetKey(KeyCode.Alpha2) && currentWeapon != weaponDictionary.weapons["sniper"])
        {
            ChangeWeapon(1);
        }
        else if (Input.GetKey(KeyCode.Alpha3) && currentWeapon != weaponDictionary.weapons["shotgun"])
        {
            ChangeWeapon(2);
        }

        if (deployTimer >= currentWeapon.deployTime && !reloading)
        {
            animator.SetBool("Reloading", false);
        }

        if (Input.GetKeyDown(KeyCode.Mouse1) && currentWeapon != weaponDictionary.weapons["shotgun"])
        {
            if (currentWeapon == weaponDictionary.weapons["sniper"])
            {
                Aim(true);
            }
            else
            {
                Aim(false);
            }
        }

        if (((currentWeapon.holdToShoot && Input.GetKey(KeyCode.Mouse0)) || (!currentWeapon.holdToShoot && Input.GetKeyDown(KeyCode.Mouse0))) && currentWeapon.ammoLeft > 0 && shootTimer >= (60f / currentWeapon.fireRate) && reloading == false && deployTimer >= currentWeapon.deployTime)
        {
            if (currentWeapon.bulletsPerShot == 1)
            {
                ShootServer(gameObject, currentWeapon.damage, false, true, isScoped, currentWeapon.hipFireAccuracy);
            }
            else
            {
                for (int i = 0; i < currentWeapon.bulletsPerShot - 1; i++)
                {
                    ShootServer(gameObject, currentWeapon.damage, true, false, isScoped, currentWeapon.hipFireAccuracy);
                }
                ShootServer(gameObject, currentWeapon.damage, true, true, isScoped, currentWeapon.hipFireAccuracy);
            }

            shootTimer = 0;
            currentWeapon.ammoLeft -= 1;
        }

        if ((Input.GetKeyDown(KeyCode.R) || currentWeapon.ammoLeft == 0) && !reloading && currentWeapon.ammoLeft != currentWeapon.magSize)
        {
            reloadCoroutine = StartCoroutine(Reload());
        }

        if (base.IsOwner)
        {
            // If you own this player entity, change the ammo in the UI
            ammoTMP.text = "Ammo - " + currentWeapon.ammoLeft;
        }

        if (Input.GetKeyDown(KeyCode.E) && amountOfChronades > 0)
        {
            ThrowGrenadeServer();
            amountOfChronades--;
            menuControl.chronadeImages[amountOfChronades].GetComponentInChildren<Text>().enabled = false;
            menuControl.chronadeImages[amountOfChronades].enabled = false;
        }

        // Refresh the number of shown chronades in the UI
        for(int i = 0; i < amountOfChronades; i++)
        {
            menuControl.chronadeImages[i].enabled = true;
            menuControl.chronadeImages[i].GetComponentInChildren<Text>().enabled = true;
        }

        if (Input.GetKeyDown(KeyCode.Q) && timeBindTimer >= timeBindCooldown)
        {
            timeBindTimer = 0;
            TimeBindServer();
            timeBindUI.fillAmount = 1;
        }

        if (Input.GetKey(KeyCode.G) && cookTimer <= 2.5f && grenadeTimer >= grenadeCooldown)
        {
            cookTimer += Time.deltaTime;
            isCooking = true;
        }
        else if((Input.GetKeyUp(KeyCode.G) || cookTimer >= 2.5f) && isCooking)
        {
            ThrowFragGrenadeServer(cookTimer);
            cookTimer = 0;
            isCooking = false;
            grenadeTimer = 0;
            GrenadeUI.fillAmount = 1;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // Press Esc for pause screen and to lock/unlock cursor
            playerManager.ChangeCursorLock();
            menuControl.OpenCloseMenu();
        }

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            // Press Tab for scoreboard
            menuControl.OpenCloseScoreboard();
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

    public void ChangeWeapon(int weaponIndex)
    {
        ChangeWeaponPrefabServer(weaponIndex);

        if (isScoped && currentWeapon == weaponDictionary.weapons["rifle"])
        {
            StopCoroutine(aimDownSightInstance);
            gunPosition.transform.localPosition = gunOriginalPosition;
        }

        currentWeapon = weaponDictionary.weapons.ElementAt(weaponIndex).Value;

        deployTimer = 0;

        if (reloadCoroutine != null)
            StopCoroutine(reloadCoroutine);
        reloading = false;

        playerCamera.fieldOfView = 60f;
        sensitivity = 1f;
        isScoped = false;
        sniperScope.SetActive(false);
    }

    [ServerRpc]
    public void ChangeWeaponPrefabServer(int weaponIndex)
    {
        ChangeWeaponPrefab(weaponIndex);
    }

    [ObserversRpc]
    public void ChangeWeaponPrefab(int weaponIndex)
    {
        currentWeaponPrefab.SetActive(false);

        if (weaponIndex == 0)
        {
            currentWeaponPrefab = riflePrefab;
        }
        else if (weaponIndex == 1)
        {
            currentWeaponPrefab = sniperPrefab;
        }
        else if (weaponIndex == 2)
        {
            currentWeaponPrefab = shotgunPrefab;
        }
        currentWeaponPrefab.SetActive(true);


        if(base.IsOwner)
        {
            animator = currentWeaponPrefab.GetComponent<Animator>();
            animator.SetBool("Reloading", true);
        }
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

        if (base.IsOwner)
        {
            weaponDictionary.Respawn();
        }
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

        if (!isScoped)
        {
            walkingSpeed = 8.5f * currentWeapon.movementSpeedMultiplier;
        }
        else if (isScoped)
        {
            walkingSpeed = 8.5f * currentWeapon.movementSpeedMultiplierScoped;
        }

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
        else if (!characterController.isGrounded)
        {
            moveDirection.y -= gravity * Time.deltaTime;
        }

        // Move the controller
        characterController.Move(moveDirection * Time.deltaTime * timeSlow * timeSpeed * 0.8f);

        // Player and Camera rotation
        if (canMove && playerCamera != null && Cursor.lockState == CursorLockMode.Locked)
        {
            rotationX += -Input.GetAxis("Mouse Y") * lookSpeed * sensitivity;
            rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
            playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
            transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed * sensitivity, 0);

            gunRotator.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
        }
    }

    IEnumerator Reload()
    {
        reloading = true;

        animator.SetBool("Reloading", true);
        yield return new WaitForSeconds(currentWeapon.reloadTime - 0.25f);

        animator.SetBool("Reloading", false);
        yield return new WaitForSeconds(0.25f);

        currentWeapon.ammoLeft = currentWeapon.magSize;
        reloading = false;
    }

    [ServerRpc]
    public void ShootServer(GameObject shooter, int damage, bool isShotgun, bool isLastShot, bool scoped, float accuracy)
    {
        Shoot(shooter, damage, isShotgun, isLastShot, scoped, accuracy);
    }

    [ObserversRpc]
    public void Shoot(GameObject shooter, int damage, bool isShotgun, bool isLastShot, bool scoped, float accuracy)
    {
        Vector3 startPos = shooter.transform.position + new Vector3(0, cameraYOffset, 0);
        Vector3 direction = shooter.GetComponent<PlayerEntity>().gunRotator.transform.forward;

        if (isShotgun)
        {
            Vector3 random = Random.insideUnitSphere * 0.1f;
            float x = random.x;
            float y = random.y;
            float z = random.z;

            direction = new Vector3(shooter.GetComponent<PlayerEntity>().gunRotator.transform.forward.x + x, shooter.GetComponent<PlayerEntity>().gunRotator.transform.forward.y + y, shooter.GetComponent<PlayerEntity>().gunRotator.transform.forward.z + z).normalized;
        }
        else if (!scoped)
        {
            Vector3 random = Random.insideUnitSphere * 0.02f * accuracy;
            float x = random.x;
            float y = random.y;
            float z = random.z;

            direction = new Vector3(shooter.GetComponent<PlayerEntity>().gunRotator.transform.forward.x + x, shooter.GetComponent<PlayerEntity>().gunRotator.transform.forward.y + y, shooter.GetComponent<PlayerEntity>().gunRotator.transform.forward.z + z).normalized;
        }

        if (Physics.Raycast(startPos + direction, direction, out RaycastHit hit, Mathf.Infinity, layerMask))
        {
            if (!ammoSpawn.GetComponent<AmmoSpawn>().isSlowed)
            {
                Vector3 visualSpawn;
                if (base.IsOwner && isScoped && currentWeapon == weaponDictionary.weapons["sniper"])
                {
                    visualSpawn = ammoSpawn.transform.position;
                }
                else
                {
                    visualSpawn = spawnForRayVisual.transform.position;
                }
                // Line visual for the shot, only if not in a timesphere
                LineRenderer instantiatedVisual = Instantiate(rayCastVisual).GetComponent<LineRenderer>();
                instantiatedVisual.SetPosition(0, visualSpawn);
                instantiatedVisual.SetPosition(1, hit.point);
                Destroy(instantiatedVisual.gameObject, 2);
            }

            if (ammoSpawn.GetComponent<AmmoSpawn>().isSlowed)
            {
                GameObject ammoInstance = Instantiate(shooter.GetComponent<PlayerEntity>().ammoPrefab, shooter.GetComponent<PlayerEntity>().ammoSpawn.transform.position, Quaternion.identity);
                ammoInstance.GetComponent<AmmoController>().direction = direction;
                ammoInstance.GetComponent<AmmoController>().shooter = shooter;
                ammoInstance.GetComponent<AmmoController>().damage = damage;
                Destroy(ammoInstance, 120);
            }
            if (ammoSpawn.GetComponent<AmmoSpawn>().isInsideTerrain)
            {
                if (Physics.Raycast(startPos, direction, out RaycastHit bulletHit, Mathf.Infinity, layerMask))
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
                ammoInstance.GetComponent<AmmoController>().damage = damage;
                Destroy(ammoInstance, 120);
            }
            else if (hit.collider.CompareTag("PlayerHead") && hit.collider.GetComponent<PlayerHead>().player.gameObject != this.gameObject)
            {
                Instantiate(playerHitEffect, hit.point, Quaternion.LookRotation(hit.normal));
                if (base.IsServer)
                {
                    Hit(hit.collider.GetComponent<PlayerHead>().player.gameObject, this.gameObject, headDamage, damage);
                }
                hit.collider.GetComponent<PlayerHead>().player.GetComponent<PlayerEntity>().ShowDamageDirection(hit.collider.GetComponent<PlayerHead>().player.gameObject, direction);
            }
            else if (hit.collider.CompareTag("PlayerTorso") && hit.collider.GetComponent<PlayerTorso>().player.gameObject != this.gameObject)
            {
                Instantiate(playerHitEffect, hit.point, Quaternion.LookRotation(hit.normal));
                if (base.IsServer)
                {
                    Hit(hit.collider.GetComponent<PlayerTorso>().player.gameObject, this.gameObject, torsoDamage, damage);
                }
                hit.collider.GetComponent<PlayerTorso>().player.GetComponent<PlayerEntity>().ShowDamageDirection(hit.collider.GetComponent<PlayerTorso>().player.gameObject, direction);
            }
            else if (hit.collider.CompareTag("PlayerLegs") && hit.collider.GetComponent<PlayerLegs>().player.gameObject != this.gameObject)
            {
                Instantiate(playerHitEffect, hit.point, Quaternion.LookRotation(hit.normal));
                if (base.IsServer)
                {
                    Hit(hit.collider.GetComponent<PlayerLegs>().player.gameObject, this.gameObject, legsDamage, damage);
                }
                hit.collider.GetComponent<PlayerLegs>().player.GetComponent<PlayerEntity>().ShowDamageDirection(hit.collider.GetComponent<PlayerLegs>().player.gameObject, direction);
            }
            else if (!hit.collider.gameObject.CompareTag("Player"))
            {
                GameObject instantiatedHole = Instantiate(bulletHole, hit.point + hit.normal * 0.0001f, Quaternion.LookRotation(hit.normal));
                Destroy(instantiatedHole, 10);
            }
        }

        if (isLastShot)
        {
            soundControl.PlayShootSound();
            // recoil
            if (base.IsOwner)
            {
                if (!isScoped)
                {
                    float random = Random.Range(-recoil, recoil);
                    rotationX -= recoil * currentWeapon.recoilMultiplier;
                    transform.rotation *= Quaternion.Euler(0, random * currentWeapon.recoilMultiplier, 0);
                }
                else if (isScoped)
                {
                    float random = Random.Range(-recoil, recoil);
                    rotationX -= recoil * currentWeapon.recoilMultiplierScoped;
                    transform.rotation *= Quaternion.Euler(0, random * currentWeapon.recoilMultiplierScoped, 0);
                }
            }
        }
    }

    [ObserversRpc]
    public void PlayerHitEffect(Vector3 position, Vector3 direction)
    {
        Instantiate(playerHitEffect, position, Quaternion.LookRotation(direction));
    }

    [ServerRpc]
    public void ThrowFragGrenadeServer(float cookTime)
    {
        ThrowFragGrenade(cookTime);
    }

    [ObserversRpc]
    public void ThrowFragGrenade(float cookTime)
    {
        GameObject grenade = Instantiate(grenadePrefab, ammoSpawn.transform.position + ammoSpawn.transform.forward, Quaternion.identity);
        grenade.GetComponentInChildren<Rigidbody>().AddForce(new Vector3(ammoSpawn.transform.forward.x, ammoSpawn.transform.forward.y + 0.2f, ammoSpawn.transform.forward.z) * 20, ForceMode.Impulse);
        grenade.GetComponent<Grenade>().ownerObject = gameObject;
        grenade.GetComponent<Grenade>().cookTime = cookTime;
        Destroy(grenade, 10);
    }

    [ServerRpc]
    public void TimeBindServer()
    {
        TimeBind();
    }

    [ObserversRpc]
    public void TimeBind()
    {
        GameObject timeBindInstance = Instantiate(timeBindSkill, ammoSpawn.transform.position + ammoSpawn.transform.forward, Quaternion.identity);
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
        GameObject chronadeInstance = Instantiate(chronade, ammoSpawn.transform.position + ammoSpawn.transform.forward, Quaternion.identity);
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

    public void Aim(bool isSniper)
    {
        isScoped = !isScoped;

        if (isSniper)
        {
            playerCamera.fieldOfView = playerCamera.fieldOfView == 60f ? 25f : 60f;
            sensitivity = playerCamera.fieldOfView / 60f;
            sniperScope.SetActive(!sniperScope.activeSelf);
        }
        else
        {
            playerCamera.fieldOfView = playerCamera.fieldOfView == 60f ? 40f : 60f;
            sensitivity = playerCamera.fieldOfView / 60f;
        }

        if (currentWeapon == weaponDictionary.weapons["rifle"])
        {
            if (aimDownSightInstance != null)
            {
                StopCoroutine(aimDownSightInstance);
            }

            aimDownSightInstance = StartCoroutine(AimDownSight());
        }

        if (isScoped)
        {
            sensitivity *= 0.6f;
        }
    }

    IEnumerator AimDownSight()
    {
        float remainingTime = 1;

        if (isScoped)
        {
            while (remainingTime > 0)
            {
                float step = Time.deltaTime * 2;
                gunPosition.transform.localPosition = Vector3.MoveTowards(gunPosition.transform.localPosition, aimingPosition.transform.localPosition, step);

                remainingTime -= Time.deltaTime;
                yield return null;
            }
        }
        else if (!isScoped)
        {
            while (remainingTime > 0)
            {
                float step = Time.deltaTime * 2;
                gunPosition.transform.localPosition = Vector3.MoveTowards(gunPosition.transform.localPosition, gunOriginalPosition, step);

                remainingTime -= Time.deltaTime;
                yield return null;
            }
        }

    }
}