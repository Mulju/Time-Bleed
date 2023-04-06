using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoController : MonoBehaviour
{
    public Rigidbody rb;

    public float timeSlowed;
    public float timeNotSlowed;

    private float speed;

    private Vector3 timeSphere;
    public Vector3 direction;

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
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void CheckForTimeSpheres()
    {
        if(Physics.Raycast(transform.position, direction, out RaycastHit hit, Mathf.Infinity))
        {
            if(hit.collider.CompareTag("TimeSphere"))
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
        if (other.CompareTag("TimeSphere"))
        {
            speed = timeSlowed;
        }
        else if(!other.CompareTag("Ammo"))
        {
            Destroy(this.gameObject);
        }

        sphere = false;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("TimeSphere"))
        {
            speed = timeNotSlowed;
            CheckForTimeSpheres();
        }
    }
}
