using System;
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
    /// <summary>обучающие стрелки съесть рыбу</summary>
    public Image foodHelp;
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

    public Image slaughterButton;
    public Image woolButton;
    public Image fishButton;
    public Button summerButton;

    public GameObject meatPrefab;
    public GameObject feltedPrefab;
    public GameObject fishPrefab;
    #endregion

    #region Unity
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

        foodHelp.gameObject.SetActive(feltedCount == 0);
        shipHelp.gameObject.SetActive(feltedCount == 0);

        sheepCount = PlayerPrefs.GetInt(SummerController.sheepCountKey);
        haylageCount = PlayerPrefs.GetInt(SummerController.haylageCountKey);
        fishCount = PlayerPrefs.GetInt(SummerController.fishCountKey);
        woolCount = sheepCount;
        ShowStats();
    }

    void Update()
    {
        if (dayCount <= 0) return;

        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            var hit = Physics2D.Raycast(mouseWorldPos, Vector2.zero);
            if (hit.transform != null)
            {
                if (hit.transform.name.Contains("sheep")) SheepClick(hit.point);
                if (hit.transform.name.Contains("day")) DayClick(hit.point);
            }
        }
    }

    #endregion

    private void SheepClick(Vector2 point)
    {
        if (sheepCount<=0) return;

        sheepCount--;
        meatCount++;

        ShowStats();

        var item = (GameObject)Instantiate(meatPrefab, transform);
        item.transform.position = new Vector3(point.x, point.y, 0f);

        shipHelp.gameObject.SetActive(false);
    }

    private void DayClick(Vector2 point)
    {
        var dayPrefab = fishPrefab;

        dayCount--;
        //нет сена - забиваем скот
        while (haylageCount < sheepCount && sheepCount > 0)
        {
            sheepCount--;
            meatCount++;
        }
        haylageCount -= sheepCount;

        //кормим людей
        if (meatCount > 0)
        {
            meatCount--;
            dayPrefab = meatPrefab;
        }
        else if (fishCount > 0)
            fishCount--;
        else
        {
            //нет еды (мясо или рыба)
            PlayerPrefs.SetInt(feltedCountKey, feltedCount);
            SceneManager.LoadScene(DefeatController.sceneName);
            return;
        }

        //обрабатываем шерсть
        if (woolCount > 0)
        {
            woolCount--;
            feltedCount++;
            dayPrefab = feltedPrefab;
        }

        ShowStats();

        var item = (GameObject)Instantiate(dayPrefab, transform);
        item.transform.position = new Vector3(point.x, point.y, 0f);

        foodHelp.gameObject.SetActive(false);
    }

    private void ShowStats()
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
            //все овцы подохли
            PlayerPrefs.SetInt(feltedCountKey, feltedCount);
            PlayerPrefs.SetInt(SummerController.sheepCountKey, sheepCount);
            SceneManager.LoadScene(DefeatController.sceneName);
        }
    }

    #region UI events

    public void SummerClick()
    {
        if (sheepCount > 1) sheepCount += sheepCount / 2;
        PlayerPrefs.SetInt(SummerController.sheepCountKey, sheepCount);
        PlayerPrefs.SetInt(feltedCountKey, feltedCount);

        var winterCount = PlayerPrefs.GetInt(winterCountKey, 1);

        SceneManager.LoadScene(SpringController.sceneName);
    }

    public void RestartClick()
    {
        PlayerPrefs.DeleteAll();
        SceneManager.LoadScene(SummerController.sceneName);
    }
    #endregion
}