using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChronoGrenade : MonoBehaviour
{
    [SerializeField] private ParticleSystem chronadeEffect;
    private float animationLength = 1;
    public GameObject ownerObject;
    public int updateID;

    private void OnTriggerEnter(Collider col)
    {
        if(col.CompareTag("TimeSphere"))
        {
            //Debug.Log("OwnerID: " + ownerID + "\nColliderID: " + col.transform.parent.GetInstanceID() + "\nUpdateID: " + updateID);
            if(ownerObject == col.transform.parent?.gameObject)
            {
                // Did we hit the players own timesphere?
                return;
            }

            // Reduce the timesphere when hit with Chronade
            col.GetComponent<TimeSphere>().ReduceCircumference();

            // Tee joku hieno animaatio t‰ss‰
            ParticleSystem instantiatedEffect = Instantiate(chronadeEffect, transform.position, Quaternion.identity);
            instantiatedEffect.Play();
            Destroy(instantiatedEffect, animationLength);
            Destroy(gameObject);
        }

        if(col.CompareTag("Clock"))
        {
            // V‰hennet‰‰n aikaa jos on vihollisen kello
        }
    }
}
