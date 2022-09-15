using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopScript : MonoBehaviour
{
    public int price;

    public Text moneyText;

    public SkinnedMeshRenderer playerMeshRenderer;

    public GameObject[] skinButtonLock;

    public Material[] skins;

    private void Start()
    {
        moneyText.text = PlayerPrefs.GetInt("coin", 0).ToString();

        playerMeshRenderer.material = skins[PlayerPrefs.GetInt("currentSkin", 0)];

        for (int i = 0; i < skinButtonLock.Length; i++)
        {
            if (PlayerPrefs.GetInt("skin" + i, 0) == 1)
            {
                skinButtonLock[i].GetComponentInParent<Button>().interactable = true;
                skinButtonLock[i].SetActive(false);
            }
            else
            {
                skinButtonLock[i].SetActive(true);
                skinButtonLock[i].GetComponentInParent<Button>().interactable = false;
            }
        }
    }

    public void SkinButton(int index)
    {
        PlayerPrefs.SetInt("currentSkin", index);

        playerMeshRenderer.material = skins[index];
    }

    public void RandomUnlockButton()
    {
        int money = PlayerPrefs.GetInt("coin", 0);

        if (money >= price && !AreAllButtonsNull())
        {
            int rand = GetRandomNumber();

            skinButtonLock[rand].SetActive(false);
            skinButtonLock[rand].GetComponentInParent<Button>().interactable = true;

            PlayerPrefs.SetInt("skin" + rand, 1);

            money -= price;
            PlayerPrefs.SetInt("coin", money);
            moneyText.text = money.ToString();
        }

        if (AreAllButtonsNull())
        {
            gameObject.SetActive(false); // later
        }
    }

    int GetRandomNumber()
    {
        List<int> availableButtons = new List<int>();

        for (int i = 0; i < skinButtonLock.Length; i++) //-------------------
        {
            if (skinButtonLock[i].activeInHierarchy)
            {
                availableButtons.Add(i);
            }
        }

        int rand = Random.Range(0, availableButtons.Count);   //------------------

        return availableButtons[rand];
    }

    bool AreAllButtonsNull()
    {
        int c = 0;

        for (int i = 0; i < skinButtonLock.Length; i++)  //--------------
        {
            if (!skinButtonLock[i].activeInHierarchy)
            {
                c++;
            }
        }

        if (c == skinButtonLock.Length)    //-----------------------
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
