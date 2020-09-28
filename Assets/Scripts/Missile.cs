using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Missile : MonoBehaviour, Damager
{
    public int damageAmount
    {
        get { return damage; }
        set { damage = value; }
    }
    [SerializeField] private int damage = 10;
    Rigidbody rb = null;
    Transform trans = null;
    Vector3 forward;
    [SerializeField] private float speed = 5f;
    [SerializeField] private bool moves = true;
    [SerializeField] private float lifeTime = 5f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        trans = GetComponent<Transform>();
    }

    private void Start()
    {
        forward = trans.forward;
    }

    float lifeTic = 0f;
    private void Update()
    {
        if(lifeTime >= 0)
        {
            lifeTic += Time.deltaTime;
            if(lifeTic >= lifeTime)
            {
                Destroy(this.gameObject);
            }
        }
    }
    private void FixedUpdate()
    {
        if(moves)
            rb.AddForce(forward * speed);
    }

    private void OnCollisionEnter(Collision collision)
    {
        Health target = collision.gameObject.GetComponent<Health>();
        if ( target != null)
        {
            target.IncrementHealth(-damageAmount);
        }

        gameObject.SetActive(false);
    }
}
