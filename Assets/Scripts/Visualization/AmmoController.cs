using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;

public class AmmoController : MonoBehaviour
{
    public Rigidbody rb;

    public float timeSlowed;
    public float timeNotSlowed;

    private float speed;

    private Vector3 objHitByRaycast;
    public Vector3 direction;
    public GameObject shooter;

    private bool sphere;

    [SerializeField] private GameObject bulletHole;

    // Start is called before the first frame update
    void Start()
    {
        timeSlowed = 0.2f;
        timeNotSlowed = 100f;
        speed = timeNotSlowed;

        CheckForCollisions();
    }

    private void FixedUpdate()
    {
        if (sphere)
        {
            if (Mathf.Abs((direction * speed * Time.deltaTime).magnitude) > Mathf.Abs((transform.position - objHitByRaycast).magnitude))
            {
                CheckForCollisions();
                rb.MovePosition(objHitByRaycast);
            }
            else
            {
                rb.MovePosition(transform.position + direction * speed * Time.deltaTime);
            }
        }
        else
        {
            rb.MovePosition(transform.position + direction * speed * Time.deltaTime);
        }

        speed = timeNotSlowed;
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void CheckForCollisions()
    {
        if (Physics.Raycast(transform.position, direction, out RaycastHit hit, Mathf.Infinity))
        {
            objHitByRaycast = hit.point;
            sphere = true;
        }
        else
        {
            sphere = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerEntity player = other.GetComponent<PlayerEntity>();
            player.Hit(other.gameObject, this.gameObject, shooter);
        }
        else if (!other.CompareTag("Ammo") && !other.CompareTag("TimeSphere"))
        {
            GameObject instantiatedHole = Instantiate(bulletHole, objHitByRaycast, Quaternion.identity);
            Destroy(instantiatedHole, 10);
            Destroy(this.gameObject);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("TimeSphere"))
        {
            speed = timeSlowed;
        }

        sphere = false;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("TimeSphere"))
        {
            speed = timeNotSlowed;

            // BUGIBUGIBUGI t�t� ei ajeta kun pelaajan timesphere deaktivoidaan
            // Ammukset ei pys�hdy seuraavan time spheren reunalle oikein.
            // Fixed ?? kai
            CheckForCollisions();
        }
    }
}