using UnityEngine;
using UnityEngine.UI;

public class RetroRenderSetup : MonoBehaviour
{
    [SerializeField] Camera retroCamera;
    [SerializeField] RawImage displayImage;
    [SerializeField] int targetPixelHeight = 360;

    void Start()
    {
        int scaleFactor = Mathf.Max(1, Mathf.RoundToInt((float)Screen.height / targetPixelHeight));
        int height = Screen.height / scaleFactor;
        int width = Screen.width / scaleFactor;

        RenderTexture rt = new RenderTexture(width, height, 16);
        rt.filterMode = FilterMode.Point;

        retroCamera.targetTexture = rt;
        displayImage.texture = rt;
    }
}
