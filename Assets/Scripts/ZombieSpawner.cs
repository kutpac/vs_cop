using UnityEngine;
using UnityEngine.AI;

public class ZombieSpawner : MonoBehaviour
{
    [SerializeField] GameObject walkerPrefab;
    [SerializeField] GameObject runnerPrefab;
    [SerializeField] Transform player;
    [SerializeField] Transform enemiesParent;

    [SerializeField] float runnerChance = 0.1f;
    [SerializeField] float minSpawnRadius = 15f;
    [SerializeField] float maxSpawnRadius = 25f;
    [SerializeField] float spawnInterval = 3f;
    [SerializeField] int maxAliveZombies = 30;
    [SerializeField] int maxAttemptsPerSpawn =5;

    Camera mainCamera;
    float timer;

    void Start()
    {
        mainCamera =Camera.main;
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player").transform;
        }
    }

    // Update is called once per frame
    void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            timer = spawnInterval;
            TrySpawnZombie();
        }
    }

    void TrySpawnZombie()
    {
        if(GameObject.FindGameObjectsWithTag("Zombie").Length >= maxAliveZombies) return;

        for (int i = 0 ;i < maxAttemptsPerSpawn; i++)
        {
            float angle = Random.Range(0f, Mathf.PI * 2f);
            float radius = Random.Range(minSpawnRadius, maxSpawnRadius);

            Vector3 candidate = player.position + new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle))*radius;

            if (!NavMesh.SamplePosition(candidate, out NavMeshHit hit, 2f, NavMesh.AllAreas)) continue;
            if (IsVisibleFromCamera(hit.position)) continue;

            GameObject prefabToSpawn = Random.value < runnerChance ? runnerPrefab : walkerPrefab;
            Instantiate(prefabToSpawn, hit.position, Quaternion.identity, enemiesParent);
            return;
        }
    }

    bool IsVisibleFromCamera(Vector3 worldPoint)
    {
        Vector3 vp = mainCamera.WorldToViewportPoint(worldPoint);
        return vp.z > 0f && vp.x >= -0.1f && vp.x <= 1.1f && vp.y >= -0.1f && vp.y <= 1.1f;
    }
}
