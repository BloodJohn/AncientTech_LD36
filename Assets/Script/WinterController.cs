using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class WinterController : MonoBehaviour
{
    #region variables
    public const string sceneName = "Winter";
    public const string feltedCountKey = "felted";
    public const string longWinterKey = "longWinter";
    public const string winterCountKey = "winterCount";
    public const int dayMax = 24;
    
    /// <summary>дней</summary>
    private int dayCount;
    /// <summary>овец</summary>
    private int sheepCount;
    /// <summary>сена</summary>
    private int haylageCount;
    /// <summary>шерсть</summary>
    private int woolCount;
    /// <summary>ткань</summary>
    private int feltedCount;
    /// <summary>мяса</summary>
    private int meatCount;
    /// <summary>трески</summary>
    private int fishCount;
    #endregion

    #region UI variables
    /// <summary>обучающие стрелки производство ткани</summary>
    public Image woolHelp;
    /// <summary>обучающие стрелки съесть рыбу</summary>
    public Image foodHelp;
    /// <summary>обучающие стрелки расходы за день</summary>
    public Image dayHelp;
    /// <summary>обучающие стрелки расходы за день</summary>
    public Image shipHelp;
    /// <summary>Сколько дней осталось</summary>
    public Text title;
    /// <summary>Сколько сена</summary>
    public Text haylageLabel;
    /// <summary>Сколько овец</summary>
    public Text sheepLabel;
    /// <summary>сколько шерсти</summary>
    public Text woolLabel;
    /// <summary>сколько ткани</summary>
    public Text feltedLabel;
    /// <summary>сколько мяса</summary>
    public Text meatLabel;
    /// <summary>сколько рыбы</summary>
    public Text fishLabel;

    public Button slaughterButton;
    public Button woolButton;
    public Button fishButton;
    public Button summerButton;
    #endregion

    public void Awake()
    {
        dayCount = dayMax;
        feltedCount = PlayerPrefs.GetInt(feltedCountKey);
        var winterCount = PlayerPrefs.GetInt(winterCountKey, 0);
        winterCount++;
        PlayerPrefs.SetInt(winterCountKey,winterCount);

        var longWinterCount = PlayerPrefs.GetInt(longWinterKey, 0);
        longWinterCount++;
        if (longWinterCount > 5)
        {
            dayCount++;
            longWinterCount = 0;
        }
        PlayerPrefs.SetInt(longWinterKey, longWinterCount);


        woolHelp.gameObject.SetActive(feltedCount == 0);
        foodHelp.gameObject.SetActive(false);
        dayHelp.gameObject.SetActive(false);
        shipHelp.gameObject.SetActive(false);

        sheepCount = PlayerPrefs.GetInt(SummerController.sheepCountKey);
        haylageCount = PlayerPrefs.GetInt(SummerController.haylageCountKey);
        fishCount = PlayerPrefs.GetInt(SummerController.fishCountKey);
        woolCount = sheepCount;

        ShowStats();
    }

    public void ShowStats()
    {
        title.text = string.Format("Winter {0}", dayCount);
        haylageLabel.text = string.Format("{0}", haylageCount);
        sheepLabel.text = string.Format("{0}", sheepCount);
        woolLabel.text = string.Format("{0}", woolCount);
        feltedLabel.text = string.Format("{0}", feltedCount);
        meatLabel.text = string.Format("{0}", meatCount);
        fishLabel.text = string.Format("{0}", fishCount);



    var isWinter = dayCount > 0;


        slaughterButton.gameObject.SetActive(isWinter && sheepCount > 0);
        woolButton.gameObject.SetActive(isWinter && woolCount > 0);
        fishButton.gameObject.SetActive(isWinter);
        summerButton.gameObject.SetActive(!isWinter);

        if (sheepCount <= 0)
        {
            PlayerPrefs.SetInt(feltedCountKey, feltedCount);
            PlayerPrefs.SetInt(SummerController.sheepCountKey, sheepCount);
            SceneManager.LoadScene(DefeatController.sceneName);
        }
    }

    public void SlaughterClick()
    {
        sheepCount--;
        meatCount++;

        ShowStats();

        if (shipHelp.isActiveAndEnabled)
        {
            shipHelp.gameObject.SetActive(false);
        }
    }

    private void NextDay()
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
            SceneManager.LoadScene(DefeatController.sceneName);
            return;
        }

        if (dayHelp.isActiveAndEnabled)
        {
            dayHelp.gameObject.SetActive(false);
            shipHelp.gameObject.SetActive(true);
        }

        ShowStats();
    }

    public void WoolClick()
    {
        if (woolHelp.isActiveAndEnabled)
        {
            woolHelp.gameObject.SetActive(false);
            foodHelp.gameObject.SetActive(true);
        }

        if (woolCount > 0)
        {
            woolCount--;
            feltedCount++;
        }

        NextDay();
    }

    public void FishClick()
    {
        NextDay();

        if (foodHelp.isActiveAndEnabled)
        {
            foodHelp.gameObject.SetActive(false);
            dayHelp.gameObject.SetActive(true);
        }
    }

    public void SummerClick()
    {
        if (sheepCount > 1) sheepCount += sheepCount / 2;
        PlayerPrefs.SetInt(SummerController.sheepCountKey, sheepCount);
        PlayerPrefs.SetInt(feltedCountKey, feltedCount);

        var winterCount = PlayerPrefs.GetInt(winterCountKey, 1);

        SceneManager.LoadScene(SummerController.sceneName);
    }

    public void RestartClick()
    {
        PlayerPrefs.DeleteAll();
        SceneManager.LoadScene(SummerController.sceneName);
    }
}