using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SummerController : MonoBehaviour
{
    public const int peopleCount = 12;
    public const string sheepCountKey = "sheep";
    public const string haylageCountKey = "haylage";
    public const string fishCountKey = "fish";

    /// <summary>Сколько дней осталось</summary>
    public Text title;
    /// <summary>Сколько овец</summary>
    public Text sheepLabel;
    /// <summary>сколько сена(лугов)</summary>
    public Text hayLabel;
    /// <summary>сколько рыбы</summary>
    public Text fishLabel;

    public Button haylageButton;
    public Button fishingButton;
    public Button longhouseButton;

    /// <summary>дней</summary>
    public int dayCount;
    /// <summary>овец</summary>
    public int sheepCount;
    /// <summary>лугов</summary>
    public int landCount;
    /// <summary>сена</summary>
    public int haylageCount;
    /// <summary>трески</summary>
    public int fishCount;
    /// <summary>рыбы в море</summary>
    public int seaCount;

    public void Awake()
    {
        sheepCount = PlayerPrefs.GetInt(sheepCountKey, 12);
        ShowStats();
    }

    public void ShowStats()
    {
        title.text = string.Format("Summer {0}", dayCount);
        sheepLabel.text = string.Format("Sheeps {0}/{1} land", sheepCount, landCount);
        hayLabel.text = string.Format("Haylage {0}/{1} land", haylageCount, landCount);
        fishLabel.text = string.Format("Сodfish {0}/{1} sea", fishCount, seaCount);

        var isSummer = dayCount > 0;

        haylageButton.gameObject.SetActive(isSummer && landCount > sheepCount);
        fishingButton.gameObject.SetActive(isSummer && seaCount > 0);
        longhouseButton.gameObject.SetActive(!isSummer);

        if (sheepCount <= 0)
        {
            PlayerPrefs.SetInt(SummerController.sheepCountKey, sheepCount);
            SceneManager.LoadScene(2);
        }
    }

    public void DayClick()
    {
        if (landCount < sheepCount) sheepCount = landCount;
        landCount -= sheepCount;
        dayCount--;
    }

    public void HaylageClick()
    {
        DayClick();

        var production = peopleCount * 2;
        if (production > landCount) production = landCount;

        haylageCount += production;
        landCount -= production;

        ShowStats();
    }

    public void FishingClick()
    {
        DayClick();

        var production = seaCount / 1000;
        if (production > seaCount) production = seaCount;

        fishCount += production;
        seaCount -= production;
        ShowStats();
    }

    public void WinterClick()
    {
        PlayerPrefs.SetInt(sheepCountKey, sheepCount);
        PlayerPrefs.SetInt(haylageCountKey, haylageCount);
        PlayerPrefs.SetInt(fishCountKey, fishCount);
        SceneManager.LoadScene(1);
    }

    public void RestartClick()
    {
        PlayerPrefs.DeleteAll();
        SceneManager.LoadScene(0);
    }
}
