using UnityEngine;

  public class RainPuddleSpawner : MonoBehaviour
  {
      [SerializeField] Transform anchor;
      [SerializeField] GameObject[] puddlePrefabs;
      [SerializeField] Transform decalParent;
      [SerializeField] int puddleCount = 15;
      [SerializeField] float spawnRadius = 15f;
      [SerializeField] float maxDistance = 20f;
      [SerializeField] float groundHeight = 0f;
      [SerializeField] float checkInterval = 2f;

      private GameObject[] activePuddles;
      private float checkTimer;
      private int recycleIndex;

      void Start()
      {
          activePuddles = new GameObject[puddleCount];
          for (int i = 0; i < puddleCount; i++)
          {
              activePuddles[i] = SpawnPuddle();
          }
      }

      void Update()
      {
          checkTimer -= Time.deltaTime;
          if (checkTimer <= 0f)
          {
              checkTimer = checkInterval/puddleCount;
              CheckOnePuddle();
          }
      }
      void RecyclePuddles()
      {
          for (int i = 0; i < activePuddles.Length; i++)
          {
              Vector3 flatAnchor = new Vector3(anchor.position.x, 0f, anchor.position.z);
              Vector3 flatPuddle = new Vector3(activePuddles[i].transform.position.x, 0f, activePuddles[i].transform.position.z);

              if (Vector3.Distance(flatAnchor, flatPuddle) > maxDistance)
              {
                  RepositionPuddle(activePuddles[i]);
              }
          }
      }
      
      GameObject SpawnPuddle()
      {
          GameObject prefab = puddlePrefabs[Random.Range(0, puddlePrefabs.Length)];
          Vector3 position = RandomPositionAroundAnchor();
          return Instantiate(prefab, position, Quaternion.Euler(90f, 0f, 0f), DecalParent.Instance);
      }

      void RepositionPuddle(GameObject puddle)
      {
          puddle.transform.position = RandomPositionAroundAnchor();
          puddle.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
      }

      Vector3 RandomPositionAroundAnchor()
      {
          Vector2 offset = Random.insideUnitCircle * spawnRadius;
          return new Vector3(anchor.position.x + offset.x, groundHeight, anchor.position.z + offset.y);
      }
      void CheckOnePuddle()
    {
      Vector3 flatAnchor = new Vector3(anchor.position.x, 0f, anchor.position.z);
      GameObject puddle = activePuddles[recycleIndex];
      Vector3 flatPuddle = new Vector3(puddle.transform.position.x, 0f, puddle.transform.position.z);

      if (Vector3.Distance(flatAnchor, flatPuddle) > maxDistance)
      {
        RepositionPuddle(puddle);
      }
      recycleIndex = (recycleIndex + 1) % activePuddles.Length;
    }
  }