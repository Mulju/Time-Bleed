using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.UIElements;

public class AmmoController : MonoBehaviour
{
    public Rigidbody rb;

    public float timeSlowed;
    public float timeNotSlowed;

    private float ammoSpeed;
    private float timeSpeed;

    private Vector3 objHitByRaycast;
    public Vector3 direction;
    public GameObject shooter;
    public int damage;

    private bool isInsdeTimeField;

    [SerializeField] private GameObject bulletHole;
    private RaycastHit raycastHit;

    [SerializeField] private GameObject playerHitEffect;

    [SerializeField] private GameObject rayCastVisual;
    private LayerMask layerMask;

    [HideInInspector] public bool isGrenadeShot = false;

    [SerializeField] private GameObject bulletTrail;

    private bool hitFlag = false;

    // Start is called before the first frame update
    void Start()
    {
        timeSlowed = 0.2f;
        timeNotSlowed = 100f;
        layerMask = LayerMask.GetMask("Player", "Terrain", "Water", "Default");

        CheckForCollisions();

    }

    private void FixedUpdate()
    {
        isInsdeTimeField = true;
        ammoSpeed = timeNotSlowed;
    }

    // Update is called once per frame
    void Update()
    {
        if (hitFlag) return;

        if (ammoSpeed == timeNotSlowed)
        {
            if (Physics.Raycast(transform.position, direction, out RaycastHit hit, Mathf.Infinity, layerMask))
            {
                /*
                // Line visual for the shot
                LineRenderer instantiatedVisual = Instantiate(rayCastVisual).GetComponent<LineRenderer>();
                instantiatedVisual.SetPosition(0, transform.position);
                instantiatedVisual.SetPosition(1, hit.point);
                Destroy(instantiatedVisual.gameObject, 2);
                */
                bool hitTimeSphere = false;
                Vector3 ogPos = transform.position;


                if (hit.collider.CompareTag("TimeSphere"))
                {
                    transform.position = hit.point;
                    ammoSpeed = timeSlowed;
                    hitTimeSphere = true;
                }
                else if (hit.collider.CompareTag("PlayerHead"))
                {
                    if (hit.collider.GetComponent<PlayerHead>().player.gameObject != shooter.gameObject)
                    {
                        hit.collider.GetComponent<PlayerHead>().player.GetComponent<PlayerEntity>().AmmoHit(hit.collider.GetComponent<PlayerHead>().player, shooter, shooter.GetComponent<PlayerEntity>().headDamage, damage);
                        hit.collider.GetComponent<PlayerHead>().player.GetComponent<PlayerEntity>().ShowDamageDirection(hit.collider.GetComponent<PlayerHead>().player.gameObject, direction);

                        Instantiate(playerHitEffect, hit.point, Quaternion.LookRotation(hit.normal));
                    }

                    // Instantiate "blood" effect
                    //Instantiate(playerHitEffect, hit.point, Quaternion.LookRotation(hit.normal));
                    Destroy(this.gameObject);
                }
                else if (hit.collider.CompareTag("PlayerTorso"))
                {
                    if (hit.collider.GetComponent<PlayerTorso>().player.gameObject != shooter.gameObject)
                    {
                        hit.collider.GetComponent<PlayerTorso>().player.GetComponent<PlayerEntity>().AmmoHit(hit.collider.GetComponent<PlayerTorso>().player, shooter, shooter.GetComponent<PlayerEntity>().torsoDamage, damage);
                        hit.collider.GetComponent<PlayerTorso>().player.GetComponent<PlayerEntity>().ShowDamageDirection(hit.collider.GetComponent<PlayerTorso>().player.gameObject, direction);

                        Instantiate(playerHitEffect, hit.point, Quaternion.LookRotation(hit.normal));
                    }

                    Destroy(this.gameObject);
                }
                else if (hit.collider.CompareTag("PlayerLegs"))
                {
                    if (hit.collider.GetComponent<PlayerLegs>().player.gameObject != shooter.gameObject)
                    {
                        hit.collider.GetComponent<PlayerLegs>().player.GetComponent<PlayerEntity>().AmmoHit(hit.collider.GetComponent<PlayerLegs>().player, shooter, shooter.GetComponent<PlayerEntity>().legsDamage, damage);
                        hit.collider.GetComponent<PlayerLegs>().player.GetComponent<PlayerEntity>().ShowDamageDirection(hit.collider.GetComponent<PlayerLegs>().player.gameObject, direction);

                        Instantiate(playerHitEffect, hit.point, Quaternion.LookRotation(hit.normal));
                    }

                    Destroy(this.gameObject);
                }
                else if (!hit.collider.CompareTag("Player"))
                {
                    GameObject instantiatedHole = Instantiate(bulletHole, hit.point + hit.normal * 0.0001f, Quaternion.LookRotation(hit.normal));
                    if(isGrenadeShot)
                    {
                        // If this bullet was created by grenade, decrease the sound
                        instantiatedHole.GetComponent<AudioSource>().volume = 0.01f;
                    }
                    Destroy(instantiatedHole, 10);
                    Destroy(this.gameObject, 4);

                    this.GetComponent<MeshRenderer>().enabled = false;
                    this.GetComponent<Collider>().enabled = false;

                    hitFlag = true;
                }

                Debug.Log("Visual hit: " + hit.collider.name);
                if(!hitTimeSphere)
                {
                    StartCoroutine(BulletTrail(ogPos, hit.point, hitTimeSphere));
                }
            }
            else
            {
                Destroy(this.gameObject);
            }
        }
        else
        {
            if (isInsdeTimeField)
            {
                rb.MovePosition(transform.position + direction * Mathf.Pow(timeSpeed, 2.3f) * 20 * Time.deltaTime);
            }
            else
            {
                rb.MovePosition(transform.position + direction * timeSlowed * Time.deltaTime);
            }
        }
    }

    private void CheckForCollisions()
    {
        if (Physics.Raycast(transform.position, direction, out RaycastHit hit, Mathf.Infinity, layerMask))
        {
            raycastHit = hit;
            objHitByRaycast = hit.point;
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("TimeSphere") || other.CompareTag("Ammo") || other.CompareTag("Player") || other.CompareTag("ChronoGrenade") || other.CompareTag("TimeBind"))
        {
            if (other.CompareTag("Player") && other.gameObject != shooter.gameObject)
            {
                //Instantiate(playerHitEffect, gameObject.transform.position, Quaternion.LookRotation(new Vector3(0, 0, gameObject.transform.rotation.z * -1)));
            }
        }
        else if (other.gameObject.layer == 6)
        {
            GameObject instantiatedHole = Instantiate(bulletHole, objHitByRaycast + raycastHit.normal * 0.0001f, Quaternion.LookRotation(raycastHit.normal));
            if (isGrenadeShot)
            {
                // If this bullet was created by grenade, decrease the sound
                instantiatedHole.GetComponent<AudioSource>().volume = 0.01f;
            }
            Destroy(instantiatedHole, 10);
            Destroy(this.gameObject);
        }
        else if (!other.CompareTag("PlayerHead") && !other.CompareTag("PlayerTorso") && !other.CompareTag("PlayerLegs"))
        {
            //Destroy(gameObject);
        }

        if (other.CompareTag("TimeSphere"))
        {
            ammoSpeed = timeSlowed;
            timeSpeed = other.GetComponent<TimeSphere>().timeSpeed;

            if (!other.gameObject.GetComponent<TimeSphere>().isTimeField)
            {
                isInsdeTimeField = false;
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("TimeSphere"))
        {
            ammoSpeed = timeSlowed;
            timeSpeed = other.GetComponent<TimeSphere>().timeSpeed;

            if (!other.gameObject.GetComponent<TimeSphere>().isTimeField)
            {
                isInsdeTimeField = false;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("TimeSphere"))
        {
            CheckForCollisions();
            ammoSpeed = timeNotSlowed;
        }
    }

    IEnumerator BulletTrail(Vector3 startPos, Vector3 endPos, bool hitTimeSphere)
    {
        Vector3 direction = (endPos - startPos).normalized;
        GameObject instansiatedTrail = Instantiate(bulletTrail, startPos, Quaternion.LookRotation(direction));
        Destroy(instansiatedTrail, 1.6f);

        float distance = Vector3.Distance(hitTimeSphere ? endPos + new Vector3(0, 0, 0.1f) : endPos, startPos) / 5;
        for (int i = 0; i < 5; i++)
        {
            instansiatedTrail.transform.position += direction * distance;
            yield return null;
        }

        instansiatedTrail.GetComponent<TrailRenderer>().emitting = false;
        instansiatedTrail.GetComponentInChildren<TrailRenderer>().emitting = false;
    }
}