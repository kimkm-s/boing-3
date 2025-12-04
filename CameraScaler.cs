using UnityEngine;

public class CameraScaler : MonoBehaviour
{
    public float baseOrthographicSize = 5f; // 기준값 (16:9 기준)

    void Start()
    {
        float targetAspect = 9f / 16f;
        float currentAspect = (float)Screen.height / Screen.width;

        Camera.main.orthographicSize = baseOrthographicSize * (currentAspect / targetAspect);
    }
}
