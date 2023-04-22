using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : MonoBehaviour
{
    [HideInInspector] public GameObject ownerObject;
    private float timer;
    private int damage;
    private int spheresCount;

    public GameObject ammoPrefab;
    public float cookTime;

    private void Awake()
    {
        spheresCount = 0;
    }

    private void Start()
    {
        damage = 10;
        timer = cookTime;
    }

    private void FixedUpdate()
    {
        if (spheresCount > 0)
        {
            gameObject.GetComponent<Rigidbody>().AddForce(-Physics.gravity * 0.95f);
        }
    }

    private void Update()
    {
        gameObject.transform.Rotate(2f, 2f, 2f);

        if (spheresCount == 0)
        {
            timer += Time.deltaTime;
        }
        else
        {
            timer += Time.deltaTime * 0.5f;
        }

        if (timer >= 2.5f)
        {
            if (gameObject.transform.localScale.x <= 0.5f)
            {
                gameObject.transform.localScale += new Vector3(0.1f, 0.1f, 0.1f);
            }
            else
            {
                for (int i = 0; i < 80; i++)
                {
                    GameObject ammoInstance = Instantiate(ammoPrefab, transform.position, Quaternion.identity);
                    ammoInstance.GetComponent<AmmoController>().direction = (Random.insideUnitSphere).normalized;
                    ammoInstance.GetComponent<AmmoController>().shooter = ownerObject;
                    ammoInstance.GetComponent<AmmoController>().damage = damage;
                    Destroy(ammoInstance, 120);
                }

                Destroy(this.gameObject);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("TimeSphere") && ownerObject != other.transform.parent?.gameObject)
        {
            if (spheresCount == 0)
            {
                gameObject.GetComponent<Rigidbody>().velocity = new Vector3(gameObject.GetComponent<Rigidbody>().velocity.x * 0.05f, gameObject.GetComponent<Rigidbody>().velocity.y * 0.05f, gameObject.GetComponent<Rigidbody>().velocity.z * 0.05f);
            }

            spheresCount++;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("TimeSphere") && ownerObject != other.transform.parent?.gameObject )
        {
            spheresCount--;

            if(spheresCount == 0)
            {
                gameObject.GetComponent<Rigidbody>().velocity = new Vector3(gameObject.GetComponent<Rigidbody>().velocity.x * 20f, gameObject.GetComponent<Rigidbody>().velocity.y * 10f, gameObject.GetComponent<Rigidbody>().velocity.z * 20f);
            }
        }
    }
}
