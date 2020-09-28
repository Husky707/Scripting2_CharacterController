using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CharacterJumper : MonoBehaviour
{
    public event Action Event_HitGround = delegate { };
    public event Action Event_IsAirborn = delegate { };
    [SerializeField] CharacterController player = null;

    //[SerializeField] Collider groundCollider = null;
    public bool isGrounded = true;

    bool colExit = false;
    bool colEnter = false;
    public bool justExited = false;

    private void Update()
    {
        if(justExited)
        {
            if(!isGrounded)
            {
                Event_IsAirborn.Invoke();
                justExited = false;
            }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (isGrounded)
            return;

        if(other.transform.tag == "Ground")
        {
            isGrounded = true;
            player.OnLanded();
            Event_HitGround.Invoke();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.transform.tag == "Ground")
        {
            isGrounded = false;
            justExited = true;

        }
    }

    /*
    private void Update()
    {
        if (colExit)
        {
            colExit = false;
            if (colEnter)
            {
                isGrounded = true;
            }
            else
            {
                Event_IsAirborn.Invoke();
                isGrounded = false;
            }
        }
        colEnter = false;
    }

    private void OnCollisionEnter(Collision collision)
    {

        if(collision.transform.tag == "Ground")
        {
            colEnter = true;
            if (!isGrounded)
            {
                isGrounded = true;
                Event_HitGround.Invoke();
            }
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if(collision.transform.tag == "Ground")
        {
            colEnter = true;
        }

    }
    private void OnCollisionExit(Collision collision)
    {
        if(collision.transform.tag == "Ground")
            colExit = true;
        

    }
    */
}
