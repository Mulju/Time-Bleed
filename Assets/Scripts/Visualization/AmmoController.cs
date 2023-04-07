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

    private Vector3 timeSphere;
    public Vector3 direction;
    public GameObject shooter;

    private bool sphere;

    // Start is called before the first frame update
    void Start()
    {
        timeSlowed = 0.15f;
        timeNotSlowed = 100f;
        speed = timeNotSlowed;

        CheckForTimeSpheres();
    }

    private void FixedUpdate()
    {
        if (sphere)
        {
            if (Mathf.Abs((direction * speed * Time.deltaTime).magnitude) > Mathf.Abs((transform.position - timeSphere).magnitude))
            {
                rb.MovePosition(timeSphere);
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

    private void CheckForTimeSpheres()
    {
        if (Physics.Raycast(transform.position, direction, out RaycastHit hit, Mathf.Infinity))
        {
            if (hit.collider.CompareTag("TimeSphere"))
            {
                timeSphere = hit.point;
                sphere = true;
            }
            else
            {
                sphere = false;
            }
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
            CheckForTimeSpheres();
        }
    }
}