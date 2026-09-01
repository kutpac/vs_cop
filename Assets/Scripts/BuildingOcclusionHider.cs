using System.Collections.Generic;
using UnityEngine;

public class BuildingOcclusionHider : MonoBehaviour
{
    [SerializeField] Transform player;
    [SerializeField] LayerMask buildingLayer;
    [SerializeField] float cameraOverlapRadius = 0.5f;
    [SerializeField] float checkInterval = 0.1f;

    private RaycastHit[] raycastBuffer = new RaycastHit[16];
    private Collider[] overlapBuffer = new Collider[16];

    private HashSet<Renderer> currentlyBlocking = new HashSet<Renderer>();
    private HashSet<Renderer> previouslyBlocking = new HashSet<Renderer>();

    private float checkTimer;

    void Update()
    {
        checkTimer -= Time.deltaTime;
        if (checkTimer > 0f) return;
        checkTimer = checkInterval;

        currentlyBlocking.Clear();

        Vector3 direction = player.position - transform.position;
        float distance = direction.magnitude;

        int hitCount = Physics.RaycastNonAlloc(transform.position, direction, raycastBuffer, distance, buildingLayer);
        for (int i = 0; i < hitCount; i++)
        {
            Renderer hitRenderer = raycastBuffer[i].collider.GetComponent<Renderer>();
            if (hitRenderer == null) continue;
            currentlyBlocking.Add(hitRenderer);
        }

        int overlapCount = Physics.OverlapSphereNonAlloc(transform.position, cameraOverlapRadius, overlapBuffer, buildingLayer);
        for (int i = 0; i < overlapCount; i++)
        {
            Renderer overlapRenderer = overlapBuffer[i].GetComponent<Renderer>();
            if (overlapRenderer == null) continue;
            currentlyBlocking.Add(overlapRenderer);
        }

        foreach (Renderer renderer in currentlyBlocking)
        {
            renderer.enabled = false;
        }

        foreach (Renderer renderer in previouslyBlocking)
        {
            if (!currentlyBlocking.Contains(renderer))
            {
                renderer.enabled = true;
            }
        }

        (previouslyBlocking, currentlyBlocking) = (currentlyBlocking, previouslyBlocking);
    }
}
