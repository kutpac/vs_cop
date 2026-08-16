using Unity.VisualScripting;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] float bulletSpeed = 1f;
    [SerializeField] float lifeTime = 3f;

    private Rigidbody rb;

    private Health health;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        rb.linearVelocity = transform.forward * bulletSpeed;
        Destroy(gameObject, lifeTime);
    }
    void OnCollisionEnter(Collision collider)
    {
        health = collider.gameObject.GetComponent<Health>();
        if (health != null)
        {
        health.TakeDamage(25);
        }
        Destroy(gameObject);
    }
}
