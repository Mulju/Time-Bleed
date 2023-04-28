using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChronoGrenade : MonoBehaviour
{
    [SerializeField] private ParticleSystem chronadeEffect;
    private float animationLength = 1;
    public GameObject ownerObject;
    public int updateID;

    private float timer;

    private void Update()
    {
        gameObject.transform.Rotate(2f, 2f, 2f);

        timer += Time.deltaTime;

        if (timer >= 2f)
        {
            

            gameObject.GetComponent<Renderer>().material.color = new Color(gameObject.GetComponent<Renderer>().material.color.r,
                                                                           gameObject.GetComponent<Renderer>().material.color.g,
                                                                           gameObject.GetComponent<Renderer>().material.color.b, 0.4f);

            if (gameObject.transform.localScale.x <= 5f)
            {
                gameObject.transform.localScale += new Vector3(0.1f, 0.1f, 0.1f);
            }
            else
            {
                Destroy(this.gameObject);
            }
            

            // gameObject.GetComponent<SphereCollider>().radius += 0.1f;
        }
    }

    private void OnTriggerEnter(Collider col)
    {
        if(col.CompareTag("TimeSphere"))
        {
            //Debug.Log("OwnerID: " + ownerID + "\nColliderID: " + col.transform.parent.GetInstanceID() + "\nUpdateID: " + updateID);
            if(ownerObject == col.transform.parent?.gameObject || 
                ownerObject.GetComponent<PlayerEntity>().ownTeamTag == col.transform.parent?.GetComponent<PlayerEntity>()?.ownTeamTag)
            {
                // Did we hit the players own or a teammates timesphere?
                return;
            }

            if(col.GetComponent<TimeSphere>().isTimeField)
            {
                // Is a players time field, reduce the time resource to 0
                col.transform.parent.GetComponent<PlayerEntity>().StartReducingResource();
            }
            else
            {
                // Reduce the timesphere when hit with Chronade
                col.GetComponent<TimeSphere>().ReduceCircumference();
            }


            // Tee joku hieno animaatio tässä
            ParticleSystem instantiatedEffect = Instantiate(chronadeEffect, transform.position, Quaternion.identity);
            instantiatedEffect.Play();
            Destroy(instantiatedEffect, animationLength);
            Destroy(gameObject);
        }
    }
}