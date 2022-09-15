using UnityEngine;

public class FinishLineScript : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (!GameManager.timeIsUp)
            {
                GameManager.won = true;
                GameManager.isFinished = true;

                UpgradeValueAndSave("currentLevel", 1, 1);
                UpgradeValueAndSave("coin", 0, 50);
            }
        }

        if (other.CompareTag("Other Players"))
        {
            other.GetComponent<OtherPlayersScript>().Win();
        }
    }

    void UpgradeValueAndSave(string valueKey, int defaultValue, int upgradeAmount)
    {
        int valueToUpgrade = PlayerPrefs.GetInt(valueKey, defaultValue);
        valueToUpgrade += upgradeAmount;
        PlayerPrefs.SetInt(valueKey, valueToUpgrade);
    }
}
