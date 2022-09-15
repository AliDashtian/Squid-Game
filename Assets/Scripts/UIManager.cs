
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public Text currentLevelText;

    private void Start()
    {
        Application.targetFrameRate = 60;
        currentLevelText.text = "Level " + PlayerPrefs.GetInt("currentLevel", 1);

        print(PlayerPrefs.GetInt("currentLevel", 1));
    }

    public void ShowAdAndLoadHome()
    {
        SceneManager.LoadScene(0);

        //YsoCorp.GameUtils.YCManager.instance.adsManager.ShowInterstitial(() => {
        //    // TODO call the action (eg: play, restart, back, next level, ...)
        //});
    }
    public void ShowAdAndRetryLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

        //YsoCorp.GameUtils.YCManager.instance.adsManager.ShowInterstitial(() => {
        //    // TODO call the action (eg: play, restart, back, next level, ...)
        //});
    }
    public void ShowAdAndLoadNextLevel()
    {
        LoadNextLevel();

        //YsoCorp.GameUtils.YCManager.instance.adsManager.ShowInterstitial(() => {
        //    // TODO call the action (eg: play, restart, back, next level, ...)
        //});
    }

    void LoadNextLevel()
    {
        int currentLevel = PlayerPrefs.GetInt("currentLevel", 1);
        int lastLevelIndex = SceneManager.sceneCountInBuildSettings - 1;

        if (lastLevelIndex >= currentLevel)
        {
            SceneManager.LoadScene(currentLevel);
        }
        else
        {
            if (currentLevel % 3 == 0)
            {
                SceneManager.LoadScene(3);
            }
            else
            {
                SceneManager.LoadScene(currentLevel % 3);
            }
        }
    }
}
