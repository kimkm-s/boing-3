using UnityEngine;
using UnityEngine.SceneManagement;

public class StartButton : MonoBehaviour
{
    // 이동할 씬 이름
    public string nextSceneName = "Test";

    // 버튼 클릭 시 호출
    public void OnStartButtonClicked()
    {
        SceneManager.LoadScene(nextSceneName);
    }
}
