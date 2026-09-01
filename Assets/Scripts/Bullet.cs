using Unity.VisualScripting;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] float bulletSpeed = 1f;
    [SerializeField] float lifeTime = 3f;

    [SerializeField] GameObject bloodSprayPrefab;
    [SerializeField] GameObject[] bloodPuddlePrefabs;
    [SerializeField] Transform decalParent;

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
        ZombieController zombie = collider.gameObject.GetComponent<ZombieController>();
        health = collider.gameObject.GetComponent<Health>();
        if (health != null)
        {
            health.TakeDamage(25);
            if (zombie != null)
            {
                ContactPoint contact = collider.GetContact(0);
                Transform sprayPoint = zombie.GetBloodSprayPoint(contact.point);
                Instantiate(bloodSprayPrefab, sprayPoint.position, Quaternion.LookRotation(contact.normal),sprayPoint);
                
                if (bloodPuddlePrefabs.Length > 0)
                {
                    Vector3 puddlePosition = new Vector3(contact.point.x, 0.8f, contact.point.z);
                    GameObject puddlePrefab = bloodPuddlePrefabs[Random.Range(0,bloodPuddlePrefabs.Length)];
                    GameObject puddle = Instantiate(puddlePrefab,puddlePosition,Quaternion.Euler(90f, Random.Range(0,360f),0f),DecalParent.Instance);
                    Destroy(puddle,45f);
                }

            }
        }
        Destroy(gameObject);
    }
}
