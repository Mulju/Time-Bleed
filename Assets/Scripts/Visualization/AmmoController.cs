using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;

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

    private bool collide;
    private bool isInsdeTimeField;

    [SerializeField] private GameObject bulletHole;
    private RaycastHit raycastHit;

    [SerializeField] private GameObject playerHitEffect;

    // Start is called before the first frame update
    void Start()
    {
        timeSlowed = 0.2f;
        timeNotSlowed = 100f;

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
        if (ammoSpeed == timeNotSlowed)
        {
            if (Physics.Raycast(transform.position, direction, out RaycastHit hit, Mathf.Infinity))
            {
                if (hit.collider.CompareTag("TimeSphere"))
                {
                    transform.position = hit.point;
                }
                else if (hit.collider.CompareTag("PlayerHead"))
                {
                    if (hit.collider.GetComponent<PlayerHead>().player.gameObject != shooter.gameObject)
                    {
                        hit.collider.GetComponent<PlayerHead>().player.GetComponent<PlayerEntity>().AmmoHit(hit.collider.GetComponent<PlayerHead>().player, shooter, shooter.GetComponent<PlayerEntity>().headDamage);
                    }

                    // Instantiate "blood" effect
                    //Instantiate(playerHitEffect, hit.point, Quaternion.LookRotation(hit.normal));
                    Destroy(this.gameObject);
                }
                else if (hit.collider.CompareTag("PlayerTorso"))
                {
                    if (hit.collider.GetComponent<PlayerTorso>().player.gameObject != shooter.gameObject)
                    {
                        hit.collider.GetComponent<PlayerTorso>().player.GetComponent<PlayerEntity>().AmmoHit(hit.collider.GetComponent<PlayerTorso>().player, shooter, shooter.GetComponent<PlayerEntity>().torsoDamage);
                    }

                    Destroy(this.gameObject);
                }
                else if (hit.collider.CompareTag("PlayerLegs"))
                {
                    if (hit.collider.GetComponent<PlayerLegs>().player.gameObject != shooter.gameObject)
                    {
                        hit.collider.GetComponent<PlayerLegs>().player.GetComponent<PlayerEntity>().AmmoHit(hit.collider.GetComponent<PlayerLegs>().player, shooter, shooter.GetComponent<PlayerEntity>().legsDamage);
                    }

                    Destroy(this.gameObject);
                }
                else if (hit.collider.CompareTag("Player"))
                {

                }
                else
                {
                    GameObject instantiatedHole = Instantiate(bulletHole, hit.point + hit.normal * 0.0001f, Quaternion.LookRotation(hit.normal));
                    Destroy(instantiatedHole, 10);
                    Destroy(this.gameObject);
                }
            }
            else
            {
                Destroy(this.gameObject);
            }
        }


        if(isInsdeTimeField)
        {
            rb.MovePosition(transform.position + direction * Mathf.Pow(timeSpeed, 2.3f) * 20 * Time.deltaTime);

            
        }
        else
        {
            rb.MovePosition(transform.position + direction * timeSlowed * Time.deltaTime);
        }



        //if (collide)
        //{
        //    if (Mathf.Abs((direction * timeNotSlowed * Time.deltaTime).magnitude) > Mathf.Abs((transform.position - objHitByRaycast).magnitude))
        //    {
        //        rb.MovePosition(objHitByRaycast);
        //        CheckForCollisions();
        //    }
        //    else
        //    {
        //        rb.MovePosition(transform.position + direction * speed * Time.deltaTime);
        //    }
        //}
        //else
        //{
        //    rb.MovePosition(transform.position + direction * speed * Time.deltaTime);
        //    CheckForCollisions();
        //}
    }

    private void CheckForCollisions()
    {
        if (Physics.Raycast(transform.position, direction, out RaycastHit hit, Mathf.Infinity))
        {
            raycastHit = hit;
            objHitByRaycast = hit.point;
            collide = true;
        }
        else
        {
            collide = false;
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("TimeSphere") || other.CompareTag("Ammo") || other.CompareTag("Player") || other.CompareTag("ChronoGrenade") || other.CompareTag("TimeBind"))
        {
            if (other.CompareTag("Player") && other.gameObject != shooter.gameObject)
            {
                Instantiate(playerHitEffect, gameObject.transform.position, Quaternion.LookRotation(new Vector3(0, 0, gameObject.transform.rotation.z * -1)));
            }
        }
        else if (other.gameObject.layer == 6)
        {
            GameObject instantiatedHole = Instantiate(bulletHole, objHitByRaycast + raycastHit.normal * 0.0001f, Quaternion.LookRotation(raycastHit.normal));
            Destroy(instantiatedHole, 10);
            Destroy(this.gameObject);
        }
        else
        {
            Debug.Log(other.name);
            Destroy(gameObject);
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

        
        collide = false;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("TimeSphere"))
        {
            CheckForCollisions();
            ammoSpeed = timeNotSlowed;
        }
    }




    public void Shoot(GameObject shooter, Vector3 direction, Vector3 startPos)
    {
        if (Physics.Raycast(startPos, direction, out RaycastHit hit, Mathf.Infinity))
        {
            if (hit.collider.CompareTag("TimeSphere"))
            {
                GameObject ammoInstance = Instantiate(shooter.GetComponent<PlayerEntity>().ammoPrefab, hit.point, Quaternion.identity);
                ammoInstance.GetComponent<AmmoController>().direction = direction;
                ammoInstance.GetComponent<AmmoController>().shooter = shooter;
                Destroy(ammoInstance, 120);
            }
            else
            {
                // bullet holet, hit() jne...
                GameObject ammoInstance = Instantiate(shooter.GetComponent<PlayerEntity>().ammoPrefab, hit.point, Quaternion.identity);
                ammoInstance.GetComponent<AmmoController>().direction = direction;
                ammoInstance.GetComponent<AmmoController>().shooter = shooter;
                Destroy(ammoInstance, 2);
            }

        }
    }
}