using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoController : MonoBehaviour
{
    public Rigidbody rb;

    public float timeSlowed;
    public float timeNotSlowed;

    private float speed;

    private Vector3 objHitByRaycast;
    public Vector3 direction;
    public GameObject shooter;

    private bool collide;

    [SerializeField] private GameObject bulletHole;
    private RaycastHit raycastHit;

    // Start is called before the first frame update
    void Start()
    {
        timeSlowed = 0.2f;
        timeNotSlowed = 100f;

        CheckForCollisions();
    }

    private void FixedUpdate()
    {
        speed = timeNotSlowed;
    }

    // Update is called once per frame
    void Update()
    {
        if (speed == timeNotSlowed)
        {
            if (Physics.Raycast(transform.position, direction, out RaycastHit hit, Mathf.Infinity))
            {
                if (hit.collider.CompareTag("TimeSphere"))
                {
                    transform.position = hit.point;
                }
                else if (hit.collider.CompareTag("Player") && hit.collider.gameObject != shooter.gameObject)
                {
                    hit.collider.GetComponent<PlayerEntity>().AmmoHit(hit.collider.gameObject, shooter);
                    Destroy(this.gameObject);
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




        rb.MovePosition(transform.position + direction * timeSlowed * Time.deltaTime);

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
        if (other.CompareTag("TimeSphere") || other.CompareTag("Ammo") || other.CompareTag("Player"))
        {
        }
        else if (other.gameObject.layer == 6)
        {
            GameObject instantiatedHole = Instantiate(bulletHole, objHitByRaycast + raycastHit.normal * 0.0001f, Quaternion.LookRotation(raycastHit.normal));
            Destroy(instantiatedHole, 10);
            Destroy(this.gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("TimeSphere"))
        {
            speed = timeSlowed;
        }

        collide = false;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("TimeSphere"))
        {
            CheckForCollisions();
            speed = timeNotSlowed;
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