using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : MonoBehaviour
{
    [HideInInspector] public GameObject ownerObject;
    private float timer;
    private int damage;

    public GameObject ammoPrefab;

    private void Start()
    {
        damage = 10;
    }

    private void Update()
    {
        timer += Time.deltaTime;

        if (timer >= 2f)
        {
            if (gameObject.transform.localScale.x <= 0.5f)
            {
                gameObject.transform.localScale += new Vector3(0.1f, 0.1f, 0.1f);
            }
            else
            {
                for (int i = 0; i < 20; i++)
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
}
