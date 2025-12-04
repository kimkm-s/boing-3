using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void RoundWin()
    {
        Debug.Log("라운드 클리어!");
        RoundManager.Instance.LoadNextRound();

    }

    public void OnWin()
    {
        Debug.Log("게임 클리어!");
        // TODO: UI, 다음 스테이지 등 처리
    }

    public void OnLose()
    {
        Debug.Log("게임 오버!");
        // TODO: 재시작 버튼 등 처리
    }
}
