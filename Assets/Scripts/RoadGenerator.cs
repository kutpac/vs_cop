using System.Collections.Generic;
using UnityEngine;
using Unity.AI.Navigation;

public class RoadGenerator : MonoBehaviour
{
    [SerializeField] int gridWidth = 10;
    [SerializeField] int gridHeight = 10;
    [SerializeField] float tileSize = 3f;
    [SerializeField] int streetSpacing = 3;
    [SerializeField] [Range(0f, 1f)] float streetChance = 0.6f;

    [System.Serializable]
    public struct RoadTile
    {
        public GameObject prefab;
        public bool north, east, south, west;
    }
    [SerializeField] RoadTile[] roadTiles;

    [Header("Buildings")]
    [SerializeField] GameObject[] buildingPrefabs;
    [SerializeField] [Range(0f, 1f)] float alleyChance = 0.15f;

    [Header("Ground")]
    [SerializeField] Material groundMaterial;

    [Header("NavMesh")]
    [SerializeField] NavMeshSurface navMeshSurface;

    [Header("Player")]
    [SerializeField] Transform player;

    private bool[,] isRoad;
    private bool[,] hasBuilding;

    void Start()
    {
        PlaceGround();
        GenerateRoadGrid();
        PlaceBuildings();
        BakeNavMesh();
        SpawnPlayer();
    }

    void BakeNavMesh()
    {
        navMeshSurface.BuildNavMesh();
    }

    void SpawnPlayer()
    {
        List<Vector2Int> availableCells = new List<Vector2Int>();
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                if (isRoad[x, y])
                {
                    availableCells.Add(new Vector2Int(x, y));
                }
            }
        }

        if (availableCells.Count == 0) return;

        Vector2Int chosen = availableCells[Random.Range(0, availableCells.Count)];
        player.position = new Vector3(chosen.x * tileSize, 0f, chosen.y * tileSize);
    }

    void PlaceGround()
    {
        GameObject ground = GameObject.CreatePrimitive(PrimitiveType.Quad);
        ground.transform.SetParent(transform);
        ground.transform.position = new Vector3((gridWidth - 1) * tileSize / 2f, -0.01f, (gridHeight - 1) * tileSize / 2f);
        ground.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
        ground.transform.localScale = new Vector3(gridWidth * tileSize, gridHeight * tileSize, 1f);

        if (groundMaterial != null)
        {
            ground.GetComponent<MeshRenderer>().material = groundMaterial;
        }
    }

    void GenerateRoadGrid()
    {
        streetChance = Random.Range(0.7f,0.8f);
        isRoad = new bool[gridWidth, gridHeight];

        bool[] verticalStreets = new bool[gridWidth];
        bool[] horizontalStreets = new bool[gridHeight];

        for (int x = 0; x < gridWidth; x++)
        {
            verticalStreets[x] = (x % streetSpacing == 0) && Random.value < streetChance;
        }
        for (int y = 0; y < gridHeight; y++)
        {
            horizontalStreets[y] = (y % streetSpacing == 0) && Random.value < streetChance;
        }

        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                isRoad[x, y] = verticalStreets[x] || horizontalStreets[y];
            }
        }

        for (int x = 0; x < gridWidth; x++)
        {
            for(int y = 0; y<gridHeight; y++)
            {
                if(!isRoad[x,y]) continue;

                bool north = y + 1 < gridHeight && isRoad[x, y + 1];
                bool south = y - 1 >= 0 && isRoad[x, y - 1];
                bool east = x + 1 < gridWidth && isRoad[x + 1, y];
                bool west = x - 1 >= 0 && isRoad[x - 1, y];

                GameObject prefab = FindMatchingTile(north, east, south, west, out float rotationY);
                if (prefab != null)
                {
                    Vector3 position = new Vector3(x * tileSize, 0f, y * tileSize);
                    Instantiate(prefab, position, Quaternion.Euler(0f, rotationY, 0f), transform);
                }
            }
        }
    }

    GameObject FindMatchingTile(bool north, bool east, bool south, bool west, out float rotationY)
    {
        foreach (RoadTile tile in roadTiles)
        {
            for (int r = 0; r < 4; r++)
            {
                (bool rn, bool re, bool rs, bool rw) = RotatePattern(tile.north, tile.east, tile.south, tile.west, r);
                if (rn == north && re == east && rs == south && rw == west)
                {
                    rotationY = r * 90f;
                    return tile.prefab;
                }
            }
        }
        rotationY = 0f;
        return null;
    }

    (bool, bool, bool, bool) RotatePattern(bool north, bool east, bool south, bool west, int steps)
    {
        for (int i = 0; i < steps; i++)
        {
            (north, east, south, west) = (west, north, east, south);
        }
        return (north, east, south, west);
    }

    void PlaceBuildings()
    {
        hasBuilding = new bool[gridWidth, gridHeight];

        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                if (isRoad[x, y]) continue;

                bool north = y + 1 < gridHeight && isRoad[x, y + 1];
                bool south = y - 1 >= 0 && isRoad[x, y - 1];
                bool east = x + 1 < gridWidth && isRoad[x + 1, y];
                bool west = x - 1 >= 0 && isRoad[x - 1, y];

                if (!north && !south && !east && !west) continue;
                if (Random.value < alleyChance) continue;

                hasBuilding[x, y] = true;

                GameObject prefab = buildingPrefabs[Random.Range(0, buildingPrefabs.Length)];
                Vector3 position = new Vector3(x * tileSize, 0f, y * tileSize);
                Quaternion rotation = GetFacingRotation(north, east, south, west);

                GameObject building = Instantiate(prefab, position, rotation, transform);
                building.transform.localScale = Vector3.one * 2f;

                Renderer[] renderers = building.GetComponentsInChildren<Renderer>();
                if (renderers.Length > 0)
                {
                    Bounds bounds = renderers[0].bounds;
                    foreach (Renderer r in renderers)
                    {
                        bounds.Encapsulate(r.bounds);
                    }

                    Vector3 offset = bounds.center - building.transform.position;
                    offset.y = 0f;
                    building.transform.position -= offset;
                }
            }
        }
    }

    Quaternion GetFacingRotation(bool north, bool east, bool south, bool west)
    {
        if (north) return Quaternion.Euler(0f, 0f, 0f);
        if (south) return Quaternion.Euler(0f, 180f, 0f);
        if (east) return Quaternion.Euler(0f, 90f, 0f);
        if (west) return Quaternion.Euler(0f, 270f, 0f);
        return Quaternion.identity;
    }
}
