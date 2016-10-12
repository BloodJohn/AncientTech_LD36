using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class SummerController : MonoBehaviour
{
    #region variables
    public const string sceneName = "Summer";
    public const int peopleCount = 12;
    public const int sheepStartCount = 12;
    public const int dayMax = 24;
    public const string sheepCountKey = "sheep";
    public const string haylageCountKey = "haylage";
    public const string fishCountKey = "fish";

    /// <summary>лугов</summary>
    public int landCount;
    /// <summary>рыбы в море</summary>
    public int seaCount;

    /// <summary>дней</summary>
    private int dayCount;
    /// <summary>овец</summary>
    private int sheepCount;
    /// <summary>сена</summary>
    private int haylageCount;
    /// <summary>трески</summary>
    private int fishCount;
    #endregion

    #region UI variables
    /// <summary>обучающие стрелки ловить рыбу</summary>
    public Image helpFish;
    /// <summary>обучающие стрелки косить сено</summary>
    public Image helpHay;
    /// <summary>Сколько дней осталось</summary>
    public Text title;
    /// <summary>Сколько овец</summary>
    public Text sheepLabel;
    /// <summary>сколько сена(лугов)</summary>
    public Text hayLabel;
    /// <summary>сколько рыбы</summary>
    public Text fishLabel;

    public Button longhouseButton;

    public GameObject shipPrefab;
    public GameObject haylagePrefab;
    public GameObject fishPrefab;
    #endregion

    #region unity
    public void Awake()
    {
        dayCount = dayMax;
        var feltedCount = PlayerPrefs.GetInt(WinterController.feltedCountKey);
        var winterCount = PlayerPrefs.GetInt(WinterController.winterCountKey, 0);
        //после первой зимовки рыбы в море бывает разное количество (от 1 до 3 рыбин за улов)
        if (winterCount > 0)
            seaCount = Random.Range(100, 400);
        else
            seaCount = 200;

        //короткое лето после долгой зимы
        var longWinterCount = PlayerPrefs.GetInt(WinterController.longWinterKey, 0);
        if (longWinterCount <= 0) dayCount--;

        Debug.LogFormat("seaCount {0} longWinter {1}", seaCount, longWinterCount);

        helpFish.gameObject.SetActive(feltedCount == 0);
        helpHay.gameObject.SetActive(feltedCount == 0);

        sheepCount = PlayerPrefs.GetInt(sheepCountKey, sheepStartCount);
        ShowStats();

        Random.InitState(DateTime.Now.Second);
        for (var i = 0; i < sheepCount; i++) CreateShip();
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
                if (hit.transform.name.Contains("sea")) FishingClick(hit.point);
                if (hit.transform.name.Contains("land")) HaylageClick(hit.point);
            }
        }
    }
    #endregion

    #region stuff
    public void ShowStats()
    {
        title.text = string.Format("Summer {0}", dayCount);
        sheepLabel.text = string.Format("{0}", sheepCount);
        hayLabel.text = string.Format("{0}/{1}", haylageCount, sheepCount * dayMax);
        fishLabel.text = string.Format("{0}/{1}", fishCount, dayMax);

        longhouseButton.gameObject.SetActive(dayCount <= 0);

        if (sheepCount <= 0)
        {
            PlayerPrefs.SetInt(SummerController.sheepCountKey, sheepCount);
            SceneManager.LoadScene(DefeatController.sceneName);
        }
    }

    private void HaylageClick(Vector2 point)
    {
        helpHay.gameObject.SetActive(false);
        DayClick();

        var production = peopleCount * 2;
        if (production > landCount) production = landCount;

        haylageCount += production;
        landCount -= production;

        ShowStats();

        var item = (GameObject)Instantiate(haylagePrefab, transform);
        item.transform.position = new Vector3(point.x, point.y, 0f);
    }

    private void FishingClick(Vector2 point)
    {
        helpFish.gameObject.SetActive(false);
        DayClick();

        var production = Mathf.RoundToInt((float)seaCount / 100);
        if (production <= 0) production = 1;

        fishCount += production;
        seaCount -= production;
        ShowStats();

        var item = (GameObject)Instantiate(fishPrefab, transform);
        item.transform.position = new Vector3(point.x, point.y, 0f);
    }

    private void DayClick()
    {
        if (landCount < sheepCount) sheepCount = landCount;
        landCount -= sheepCount;
        dayCount--;
    }

    public void WinterClick()
    {
        PlayerPrefs.SetInt(sheepCountKey, sheepCount);
        PlayerPrefs.SetInt(haylageCountKey, haylageCount);
        PlayerPrefs.SetInt(fishCountKey, fishCount);
        SceneManager.LoadScene(AutumnController.sceneName);
    }

    public void RestartClick()
    {
        PlayerPrefs.DeleteAll();
        SceneManager.LoadScene(SummerController.sceneName);
    }

    private void CreateShip()
    {
        bool isLand = false;
        int cnt = 0;
        var height = Camera.allCameras[0].orthographicSize;
        var width = height * Camera.allCameras[0].aspect * height;

        while (!isLand && cnt < 100)
        {
            cnt++;
            var point = new Vector2(Random.Range(-width, width), Random.Range(-height, height));

            var hit = Physics2D.Raycast(point, Vector2.zero);
            if (hit.transform != null)
            {
                if (hit.transform.name.Contains("land"))
                {
                    var item = (GameObject)Instantiate(shipPrefab, transform);
                    item.transform.position = new Vector3(point.x, point.y, 0f);
                    return;
                }
            }
        }
    }
    #endregion
}
