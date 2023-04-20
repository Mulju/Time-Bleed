using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : MonoBehaviour
{
    [HideInInspector] public GameObject ownerObject;
    private float timer;
    private int damage;

    public GameObject ammoPrefab;


    private void Update()
    {
        timer += Time.deltaTime;

        if (timer >= 2f)
        {
            gameObject.GetComponent<Renderer>().material.color = new Color(gameObject.GetComponent<Renderer>().material.color.r,
                                                               gameObject.GetComponent<Renderer>().material.color.g,
                                                               gameObject.GetComponent<Renderer>().material.color.b, 0.4f);

            if (gameObject.transform.localScale.x <= 0.5f)
            {
                gameObject.transform.localScale += new Vector3(0.1f, 0.1f, 0.1f);
            }
            else
            {
                for (int i = 0; i < 8; i++)
                {
                    GameObject ammoInstance = Instantiate(ownerObject.GetComponent<PlayerEntity>().ammoPrefab, ownerObject.GetComponent<PlayerEntity>().ammoSpawn.transform.position, Quaternion.identity);
                    ammoInstance.GetComponent<AmmoController>().direction = Random.insideUnitSphere;
                    ammoInstance.GetComponent<AmmoController>().shooter = ownerObject;
                    ammoInstance.GetComponent<AmmoController>().damage = damage;
                    Destroy(ammoInstance, 120);
                }

                Destroy(this.gameObject);
            }
        }
    }
}
