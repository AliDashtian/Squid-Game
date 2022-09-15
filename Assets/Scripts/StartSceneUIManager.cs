using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartSceneUIManager : MonoBehaviour
{
    public int rewardedVideoCoinAmount;
    public GameObject soundImage, rewardedAdPanel;
    public Text currentLevelText, tapToPlayText, coinText, shopCoinText;

    private void Start()
    {
        if (AudioListener.volume == 1)
        {
            soundImage.SetActive(true);
        }
        else
        {
            soundImage.SetActive(false);
        }

        SetCoinAmount();
        currentLevelText.text = "Level " + PlayerPrefs.GetInt("currentLevel", 1);
    }

    public void SetCoinAmount()
    {
        coinText.text = PlayerPrefs.GetInt("coin", 0).ToString();
    }

    public void SoundButton()
    {
        if (AudioListener.volume == 1)
        {
            AudioListener.volume = 0;
            soundImage.SetActive(false);
        }
        else
        {
            AudioListener.volume = 1;
            soundImage.SetActive(true);
        }
    }

    //public void ShowRewardedAd()
    //{
    //    YsoCorp.GameUtils.YCManager.instance.adsManager.ShowRewarded((bool ok) => {
    //        if (ok)
    //        {
    //            int coin = PlayerPrefs.GetInt("coin", 0);
    //            coin += rewardedVideoCoinAmount;
    //            PlayerPrefs.SetInt("coin", coin);

    //            coinText.text = coin.ToString();

    //            rewardedAdPanel.SetActive(true);
    //        }
    //    });
    //}
    //public void ShowRewardedAdShop()
    //{
    //    YsoCorp.GameUtils.YCManager.instance.adsManager.ShowRewarded((bool ok) => {
    //        if (ok)
    //        {
    //            int coin = PlayerPrefs.GetInt("coin", 0);
    //            coin += rewardedVideoCoinAmount;
    //            PlayerPrefs.SetInt("coin", coin);

    //            shopCoinText.text = coin.ToString(); ;

    //            rewardedAdPanel.SetActive(true);
    //        }
    //    });
    //}

    public void ShowAdAndPlay()
    {
        PlayButton();

        //YsoCorp.GameUtils.YCManager.instance.adsManager.ShowInterstitial(() => {
        //    // TODO call the action (eg: play, restart, back, next level, ...)
        //});
    }

    void PlayButton()
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
                SceneManager.LoadScene(currentLevel / 3);
            }
            else
            {
                SceneManager.LoadScene(currentLevel % 3);
            }
        }

        tapToPlayText.text = "Loading...";
    }
}
