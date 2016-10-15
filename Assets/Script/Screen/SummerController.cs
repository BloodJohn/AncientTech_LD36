using SmartLocalization;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class SummerController : MonoBehaviour
{
    #region variables
    public const string sceneName = "Summer";

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
        CoreGame.Instance.StartSummer();

        helpFish.gameObject.SetActive(CoreGame.Instance.FeltedCount == 0);
        helpHay.gameObject.SetActive(CoreGame.Instance.FeltedCount == 0);

        ShowStats();

        Random.InitState(DateTime.Now.Second);
        for (var i = 0; i < CoreGame.Instance.SheepCount; i++) CreateSheep();

        longhouseButton.GetComponentInChildren<Text>().text = LanguageManager.Instance.GetTextValue("winter_button");
    }

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape)) Application.Quit();
        if (CoreGame.Instance.DayCount <= 0) return;

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

    public void WinterClick()
    {
        SceneManager.LoadScene(AutumnController.sceneName);
    }

    public void RestartClick()
    {
        CoreGame.Instance.RestartGame();
    }
    #endregion

    #region stuff
    private void ShowStats()
    {
        title.text = string.Format(LanguageManager.Instance.GetTextValue("summer_title"), CoreGame.Instance.DayCount);
        sheepLabel.text = string.Format("{0}", CoreGame.Instance.SheepCount);
        hayLabel.text = string.Format("{0}/{1}", CoreGame.Instance.HaylageCount, CoreGame.Instance.SheepCount * CoreGame.SeasonDays);
        fishLabel.text = string.Format("{0}/{1}", CoreGame.Instance.FishCount, CoreGame.SeasonDays);

        longhouseButton.gameObject.SetActive(CoreGame.Instance.DayCount <= 0);
    }

    private void HaylageClick(Vector2 point)
    {
        helpHay.gameObject.SetActive(false);
        
        CoreGame.Instance.HaylageSummer();
        ShowStats();

        var item = (GameObject)Instantiate(haylagePrefab, transform);
        item.transform.position = new Vector3(point.x, point.y, 0f);
    }

    private void FishingClick(Vector2 point)
    {
        helpFish.gameObject.SetActive(false);
        
        CoreGame.Instance.FishingSummer();
        ShowStats();

        var item = (GameObject)Instantiate(fishPrefab, transform);
        item.transform.position = new Vector3(point.x, point.y, 0f);
    }

    private void CreateSheep()
    {
        var isLand = false;
        var cnt = 0;
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
