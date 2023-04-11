using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;

public class AmmoController : NetworkBehaviour
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
        if (collide)
        {
            if (Mathf.Abs((direction * timeNotSlowed * Time.deltaTime).magnitude) > Mathf.Abs((transform.position - objHitByRaycast).magnitude))
            {
                rb.MovePosition(objHitByRaycast);
                CheckForCollisions();
            }
            else
            {
                rb.MovePosition(transform.position + direction * speed * Time.deltaTime);
            }
        }
        else
        {
            rb.MovePosition(transform.position + direction * speed * Time.deltaTime);
            CheckForCollisions();
        }
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
            speed = timeNotSlowed;
            CheckForCollisions();
        }
    }
}