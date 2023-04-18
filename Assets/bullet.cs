using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bullet : MonoBehaviour
{
    private Rigidbody bulletRigidbody;
    void Awake()
    {
        bulletRigidbody = GetComponent<Rigidbody>();
    }
    void Start()
    {
        float speed = 50.0f;
        bulletRigidbody.velocity = transform.forward * speed * Time.deltaTime;
    }

    void OnTriggerEnter(Collider other)
    {
        Destroy(gameObject);
    }
}
