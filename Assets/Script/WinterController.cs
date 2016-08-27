using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class WinterController : MonoBehaviour
{
    public const string feltedCountKey = "felted";

    /// <summary>Сколько дней осталось</summary>
    public Text title;
    /// <summary>Сколько овец</summary>
    public Text sheepLabel;
    /// <summary>сколько мяса</summary>
    public Text woolLabel;
    /// <summary>сколько еды (мяса и рыбы)</summary>
    public Text foodLabel;

    public Button slaughterButton;
    public Button woolButton;
    public Button idleButton;
    public Button summerButton;

    /// <summary>дней</summary>
    public int dayCount;
    /// <summary>овец</summary>
    public int sheepCount;
    /// <summary>сена</summary>
    public int haylageCount;
    /// <summary>шерсть</summary>
    public int woolCount;
    /// <summary>ткань</summary>
    public int feltedCount;
    /// <summary>мяса</summary>
    public int meatCount;
    /// <summary>трески</summary>
    public int fishCount;

    public void Awake()
    {
        feltedCount = PlayerPrefs.GetInt(feltedCountKey);
        sheepCount = PlayerPrefs.GetInt(SummerController.sheepCountKey);
        haylageCount = PlayerPrefs.GetInt(SummerController.haylageCountKey);
        fishCount = PlayerPrefs.GetInt(SummerController.fishCountKey);
        woolCount = sheepCount;
        ShowStats();
    }

    public void ShowStats()
    {
        title.text = string.Format("Winter {0}", dayCount);
        sheepLabel.text = string.Format("Sheeps {0} hay {1}", sheepCount, haylageCount);
        woolLabel.text = string.Format("Felted {0}/{1} wool", feltedCount, woolCount);
        foodLabel.text = string.Format("Meat {0}/{1} codfish", meatCount, fishCount);

        var isWinter = dayCount > 0;


        slaughterButton.gameObject.SetActive(isWinter && sheepCount > 0);
        woolButton.gameObject.SetActive(isWinter && woolCount > 0);
        idleButton.gameObject.SetActive(isWinter);
        summerButton.gameObject.SetActive(!isWinter);

        if (sheepCount <= 0)
        {
            PlayerPrefs.SetInt(feltedCountKey, feltedCount);
            PlayerPrefs.SetInt(SummerController.sheepCountKey, sheepCount);
            SceneManager.LoadScene(2);
        }
    }

    public void SlaughterClick()
    {
        sheepCount--;
        meatCount++;

        ShowStats();
    }


    public void WoolClick()
    {
        if (woolCount > 0)
        {
            woolCount--;
            feltedCount++;
        }

        DayClick();
    }

    public void DayClick()
    {
        dayCount--;
        //нет сена - забиваем скот
        while (haylageCount < sheepCount && sheepCount > 0) SlaughterClick();
        haylageCount -= sheepCount;

        //кормим людей
        if (meatCount > 0)
            meatCount--;
        else if (fishCount > 0)
            fishCount--;
        else
        {
            PlayerPrefs.SetInt(feltedCountKey, feltedCount);
            SceneManager.LoadScene(2);
            return;
        }

        ShowStats();
    }

    public void SummerClick()
    {
        if (sheepCount > 1) sheepCount += sheepCount / 2;
        PlayerPrefs.SetInt(SummerController.sheepCountKey, sheepCount);
        PlayerPrefs.SetInt(feltedCountKey, feltedCount);

        SceneManager.LoadScene(0);
    }

    public void RestartClick()
    {
        PlayerPrefs.DeleteAll();
        SceneManager.LoadScene(0);
    }
}
