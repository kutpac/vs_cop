using UnityEngine;

public class RainSplashSpawner : MonoBehaviour
{
    [SerializeField] GameObject[] splashPrefabs;
    [SerializeField] int poolSize = 20;
    [SerializeField] float splashChance = 0.05f;

    private ParticleSystem rainSystem;
    private ParticleCollisionEvent[] collisionEvents;
    private GameObject[] pool;
    private int poolIndex;

    void Start()
    {
        rainSystem = GetComponent<ParticleSystem>();
        collisionEvents = new ParticleCollisionEvent[16];

        pool = new GameObject[poolSize];
        for (int i = 0; i < poolSize; i++)
        {
            GameObject prefab = splashPrefabs[Random.Range(0, splashPrefabs.Length)];
            pool[i] = Instantiate(prefab, Vector3.zero, Quaternion.Euler(90f, 0f, 0f));
            pool[i].SetActive(false);
        }
    }

    void OnParticleCollision(GameObject other)
    {
        int count = rainSystem.GetCollisionEvents(other, collisionEvents);

        for (int i = 0; i < count; i++)
        {
            if (Random.value > splashChance) continue;
            SpawnSplash(collisionEvents[i].intersection);
        }
    }

    private void SpawnSplash(Vector3 position)
    {
        GameObject splash = pool[poolIndex];
        poolIndex = (poolIndex + 1) % pool.Length;

        splash.transform.position = position;
        splash.transform.rotation = Quaternion.Euler(90f, Random.Range(0f, 360f), 0f);
        splash.SetActive(true);
    }
}
