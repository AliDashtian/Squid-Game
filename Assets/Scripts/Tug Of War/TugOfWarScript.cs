using System.Collections;
using UnityEngine;
using UnityEngine.UI;
//using YsoCorp.GameUtils;

public class TugOfWarScript : MonoBehaviour
{
    public Text timerText;

    public Animator sliderGauge;

    public Transform gaugeArrow, playArea;

    public GameObject losePanel, winPanel, gamePanel;

    public float playAreaMoveAmount = 0.15f;

    public int timerTime;
    public int fallenEnemies, fallenPlayers;

    private bool timeIsUp;

    private void Awake()
    {
        Time.timeScale = 1f;
    }

    private void Start()
    {
        GameStartYSO();

        StartCoroutine(Timer());
    }

    void GameStartYSO()
    {
        int currentLevel = PlayerPrefs.GetInt("currentLevel", 1);
        //YCManager.instance.OnGameStarted(currentLevel);
    }

    public void GameFinish(bool win)
    {
        //YCManager.instance.OnGameFinished(win);
    }

    public void DragTheRopeButton()
    {
        sliderGauge.speed = 0;
        Invoke("ContinueGaugeAnimation", 0.4f);

        if (Mathf.Abs(gaugeArrow.localPosition.x) < 104)
        {
            StartCoroutine(MovePlayAreaRight());
        }
        else
        {
            StartCoroutine(MovePlayAreaLeft());
        }
    }

    void ContinueGaugeAnimation()
    {
        sliderGauge.speed = 1;
    }

    IEnumerator MovePlayAreaRight()
    {
        float x = playArea.position.x;
        float y = x + playAreaMoveAmount;

        while (x <= y)
        {
            playArea.Translate(Vector3.right * Time.deltaTime * 0.5f);
            x = playArea.position.x;

            yield return null;
        }

        GameEndCheck();
    }
    IEnumerator MovePlayAreaLeft()
    {
        float x = playArea.position.x;
        float y = x - playAreaMoveAmount;

        while (x >= y)
        {
            playArea.Translate(Vector3.left * Time.deltaTime * 0.5f);
            x = playArea.position.x;

            yield return null;
        }

        GameEndCheck();
    }
    IEnumerator Timer()
    {
        while (timerTime >= 0)
        {
            var timeSpan = System.TimeSpan.FromSeconds(timerTime);
            timerText.text = timeSpan.Minutes.ToString("00") + ":" + timeSpan.Seconds.ToString("00");

            yield return new WaitForSeconds(1f);

            timerTime--;
        }

        timeIsUp = true;

        Lose();
    }
    
    public void GameEndCheck()
    {
        if (fallenEnemies >= 3)
        {
            Invoke("Win", 0.5f);
        }
        else if (fallenPlayers >= 3) // change later
        {
            Invoke("Lose", 0.5f);
        }
    }
    void UpgradeValueAndSave(string valueKey, int defaultValue, int upgradeAmount)
    {
        int valueToUpgrade = PlayerPrefs.GetInt(valueKey, defaultValue);
        valueToUpgrade += upgradeAmount;
        PlayerPrefs.SetInt(valueKey, valueToUpgrade);
    }
    void Win()
    {
        if (!timeIsUp)
        {
            UpgradeValueAndSave("currentLevel", 1, 1);
            UpgradeValueAndSave("coin", 0, 50);

            GameFinish(true);
            winPanel.SetActive(true);
            gamePanel.SetActive(false);
            Time.timeScale = 0.0f;
        }
    }
    void Lose()
    {
        GameFinish(false);
        losePanel.SetActive(true);
        gamePanel.SetActive(false);
        Time.timeScale = 0.0f;
    }
}
