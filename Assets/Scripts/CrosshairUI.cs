using UnityEngine;
using UnityEngine.InputSystem;

public class CrosshairUI : MonoBehaviour
{
    [SerializeField] RectTransform crosshairRect;

    void Start()
    {
        Cursor.visible = false;
    }

    void Update()
    {
        Vector2 mousePos = Mouse.current.position.ReadValue();
        crosshairRect.position = mousePos;
    }
}
