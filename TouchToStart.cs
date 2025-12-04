using UnityEngine;
using UnityEngine.SceneManagement;

public class TouchToStart : MonoBehaviour
{
    public string stageSelectSceneName = "StageSelect";

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // PC 마우스
        {
            SceneManager.LoadScene(stageSelectSceneName);
        }

        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began) // 모바일 터치
        {
            SceneManager.LoadScene(stageSelectSceneName);
        }
    }
}