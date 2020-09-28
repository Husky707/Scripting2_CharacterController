using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ShieldImpact : MonoBehaviour
{

    public UnityEvent OnMissileImpact;

    [SerializeField] private ParticleSystem lightning = null;
    [SerializeField] private ParticleSystem hex = null;


    private void OnTriggerEnter(Collider other)
    {

        if(other.gameObject.GetComponent<Missile>() != null)
        {
            ParticleSystem hexes = Instantiate(hex, other.transform.position, Quaternion.identity);
            hexes.transform.forward = -other.transform.forward;
            hexes.Play();

            ParticleSystem zap = Instantiate(lightning, other.transform.position, Quaternion.identity);
            zap.transform.forward = -other.transform.forward;
            zap.Play();

            other.gameObject.SetActive(false);
            OnMissileImpact.Invoke();
        }
    }

}
