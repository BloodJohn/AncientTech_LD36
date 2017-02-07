using SmartLocalization;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MerchantController : MonoBehaviour
{
    public const string sceneName = "Merchant";
    public Text description;
    public Text item200;
    public Text item500;
    public Text item1k;
    public Text item10k;

    public Image icon200;
    public Image icon500;
    public Image icon1k;
    public Image icon10k;
    public Button summerButton;

    /// <summary>не хватает сукна</summary>
    public Color redColor;
    /// <summary>можно купить</summary>
    public Color greenColor;
    /// <summary>уже купил!</summary>
    public Color goldColor;

    void Awake()
    {
        description.text = string.Format(LanguageManager.Instance.GetTextValue("merchant_description"), CoreGame.Instance.FeltedCount);
        item200.text = LanguageManager.Instance.GetTextValue("merchant_item200");
        item500.text = LanguageManager.Instance.GetTextValue("merchant_item500");
        item1k.text = LanguageManager.Instance.GetTextValue("merchant_item1k");
        item10k.text = LanguageManager.Instance.GetTextValue("merchant_item10k");

        summerButton.GetComponentInChildren<Text>().text = LanguageManager.Instance.GetTextValue("summer_button");

        if (CoreGame.Instance.ScytheCount > 0)
        {
            icon200.color = goldColor;
        }
        else
        {
            if (CoreGame.Instance.FeltedCount >= CoreGame.PriceScythe)
            {
                icon200.color = greenColor;
            }
            else
            {
                icon200.color = redColor;
            }
        }

        if (CoreGame.Instance.HayforkCount > 0)
        {
            icon500.color = goldColor;
        }
        else
        {
            if (CoreGame.Instance.FeltedCount >= CoreGame.PriceHayfork)
            {
                icon500.color = greenColor;
            }
            else
            {
                icon500.color = redColor;
            }
        }

        if (CoreGame.Instance.SecondChanseCount > 0)
        {
            icon1k.color = goldColor;
        }
        else
        {
            if (CoreGame.Instance.FeltedCount >= CoreGame.PriceSecondChanse)
            {
                icon1k.color = greenColor;
            }
            else
            {
                icon1k.color = redColor;
            }
        }

        if (CoreGame.Instance.DrakkarCount > 0)
        {
            icon10k.color = goldColor;
        }
        else
        {
            if (CoreGame.Instance.FeltedCount >= CoreGame.PriceDrakkar)
            {
                icon10k.color = greenColor;
            }
            else
            {
                icon10k.color = redColor;
            }
        }
    }

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape)) Application.Quit();
    }

    public void LoadSummer()
    {
        SceneManager.LoadScene(SummerController.sceneName);
    }

    public void BuyItem200()
    {
        if (CoreGame.Instance.BuyScythe())
        {
            icon200.color = goldColor;
            ScytheAchievement();
        }
    }

    public void BuyItem500()
    {
        if (CoreGame.Instance.BuyHayfork())
        {
            icon500.color = goldColor;
            HayforkAchievement();
        }
    }

    public void BuyItem1k()
    {
        if (CoreGame.Instance.BuySecondChanse())
        {
            icon1k.color = goldColor;
        }
    }

    public void BuyItem10k()
    {
        if (CoreGame.Instance.BuyDrakkar())
        {
            icon10k.color = goldColor;
            DrakkarAchievement();
        }
    }

    #region achievements
    /// <summary>Купить косу</summary>
    private void ScytheAchievement()
    {
        description.text = string.Format(LanguageManager.Instance.GetTextValue("merchant_description"), CoreGame.Instance.FeltedCount);
        if (PlayerPrefs.HasKey(GPGSIds.achievement_scythe)) return;

        Social.ReportProgress(GPGSIds.achievement_scythe, 100.0f, (bool success) =>
        {
            // handle success or failure
            if (success)
            {
                PlayerPrefs.SetInt(GPGSIds.achievement_scythe, 100);
            }
        });
    }

    /// <summary>Купить вилы</summary>
    private void HayforkAchievement()
    {
        description.text = string.Format(LanguageManager.Instance.GetTextValue("merchant_description"), CoreGame.Instance.FeltedCount);
        if (PlayerPrefs.HasKey(GPGSIds.achievement_hayfork)) return;

        Social.ReportProgress(GPGSIds.achievement_hayfork, 100.0f, (bool success) =>
        {
            // handle success or failure
            if (success)
            {
                PlayerPrefs.SetInt(GPGSIds.achievement_hayfork, 100);
            }
        });
    }

    /// <summary>Купить драккар</summary>
    private void DrakkarAchievement()
    {
        description.text = string.Format(LanguageManager.Instance.GetTextValue("merchant_description"), CoreGame.Instance.FeltedCount);
        if (PlayerPrefs.HasKey(GPGSIds.achievement_drakkar)) return;

        Social.ReportProgress(GPGSIds.achievement_drakkar, 100.0f, (bool success) =>
        {
            // handle success or failure
            if (success)
            {
                PlayerPrefs.SetInt(GPGSIds.achievement_drakkar, 100);
            }
        });
    }
    #endregion
}
