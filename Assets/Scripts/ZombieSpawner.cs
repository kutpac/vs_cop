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
    [SerializeField] int maxAttemptsPerSpawn =5;
    [SerializeField] LayerMask noSpawnLayer;

    [SerializeField] float initialSpawnInterval = 3f;
    [SerializeField] float minSpawnInterval = 0.75f;
    [SerializeField] int initialMaxAliveZombies = 30;
    [SerializeField] int maxAliveZombiesCap = 100;
    [SerializeField] float difficultyRampDuration = 300f;

    Camera mainCamera;
    float timer;
    float elapsedTime;
    float currentSpawnInterval;
    int currentMaxAliveZombies;

    void Start()
    {
        mainCamera =Camera.main;
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player").transform;
        }

        currentSpawnInterval = initialSpawnInterval;
        currentMaxAliveZombies = initialMaxAliveZombies;
    }

    // Update is called once per frame
    void Update()
    {
        elapsedTime += Time.deltaTime;
        UpdateDifficulty();

        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            timer = currentSpawnInterval;
            TrySpawnZombie();
        }
    }

    void UpdateDifficulty()
    {
        float t = Mathf.Clamp01(elapsedTime / difficultyRampDuration);
        currentSpawnInterval = Mathf.Lerp(initialSpawnInterval, minSpawnInterval, t);
        currentMaxAliveZombies = Mathf.RoundToInt(Mathf.Lerp(initialMaxAliveZombies, maxAliveZombiesCap, t));
    }

    void TrySpawnZombie()
    {
        if(GameObject.FindGameObjectsWithTag("Zombie").Length >= currentMaxAliveZombies) return;

        for (int i = 0 ;i < maxAttemptsPerSpawn; i++)
        {
            float angle = Random.Range(0f, Mathf.PI * 2f);
            float radius = Random.Range(minSpawnRadius, maxSpawnRadius);

            Vector3 candidate = player.position + new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle))*radius;

            if (!NavMesh.SamplePosition(candidate, out NavMeshHit hit, 2f, NavMesh.AllAreas)) continue;
            if (IsVisibleFromCamera(hit.position)) continue;
            if (Physics.CheckSphere(hit.position, 1f, noSpawnLayer)) continue;

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
