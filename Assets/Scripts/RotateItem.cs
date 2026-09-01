using UnityEngine;

public class RotateItem : MonoBehaviour
{
    [SerializeField] float rotationSpeed = 90f;

    void Update()
    {
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
    }
}
