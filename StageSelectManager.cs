using UnityEngine;
using UnityEngine.SceneManagement;

public class StageSelectManager : MonoBehaviour
{
    public SwipeSnapSimple snap; // Scroll Snap ¿¬°á

    public void StartStage()
    {
        int stage = snap.currentStage;
        string sceneName = "Stage" + stage;
        SceneManager.LoadScene(sceneName);
    }
}