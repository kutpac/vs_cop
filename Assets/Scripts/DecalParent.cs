using UnityEngine;

public class DecalParent : MonoBehaviour
{
    public static Transform Instance;

    void Awake()
    {
        Instance = transform;
    }
}
