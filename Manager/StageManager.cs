using UnityEngine;

public class StageManager : MonoBehaviour
{
    public static StageManager Instance;

    public RoundData[][] stages; // stages[0] = Stage1 rounds, stages[1] = Stage2 rounds
    private int currentStage = 0;
    private int currentRound = 0;

    private void Awake() { Instance = this; }

    public void StartStage(int stageIndex)
    {
        currentStage = stageIndex;
        currentRound = 0;
        LoadCurrentRound();
    }

    public void LoadNextRound()
    {
        currentRound++;
        if (currentRound >= stages[currentStage].Length)
        {
            // 스테이지 클리어
            StartNextStage();
        }
        else
        {
            LoadCurrentRound();
        }
    }

    private void LoadCurrentRound()
    {
        RoundManager.Instance.LoadRound(stages[currentStage][currentRound]);
    }

    private void StartNextStage()
    {
        currentStage++;
        if (currentStage >= stages.Length)
        {
            Debug.Log("모든 스테이지 클리어!");
            GameManager.Instance.OnWin();
        }
        else
        {
            currentRound = 0;
            LoadCurrentRound();
        }
    }
}
