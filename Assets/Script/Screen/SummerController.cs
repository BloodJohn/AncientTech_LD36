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

    public GameObject sheepPrefab;
    public GameObject haylagePrefab;
    public GameObject fishPrefab;
    public GameObject fishbonePrefab;
    public GameObject stonePrefab;
    public GameObject sealPrefab;

    public Color blackColor;
    public Color redColor;
    #endregion

    #region unity
    public void Awake()
    {
        CoreGame.Instance.StartSummer();

        if (CoreGame.Instance.FeltedCount == 0) //обучение
        {
            helpHay.sprite = Resources.Load<Sprite>(LanguageManager.Instance.GetTextValue("summer_hay_spr"));
            helpFish.sprite = Resources.Load<Sprite>(LanguageManager.Instance.GetTextValue("summer_fish_spr"));

            helpFish.gameObject.SetActive(true);
            helpHay.gameObject.SetActive(true);
        }
        else
        {
            helpFish.gameObject.SetActive(false);
            helpHay.gameObject.SetActive(false);
        }

        helpFish.gameObject.SetActive(CoreGame.Instance.FeltedCount == 0);
        helpHay.gameObject.SetActive(CoreGame.Instance.FeltedCount == 0);

        ShowStats();

        Random.InitState(DateTime.Now.Second);
        for (var i = 0; i < CoreGame.Instance.SheepCount; i++) CreateSheep();

        longhouseButton.GetComponentInChildren<Text>().text = LanguageManager.Instance.GetTextValue("winter_button");
    }

    void Start()
    {
        BigFlockAchievement();
        Game10Achievement();
        LongWinterAchievement();
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
        CoreGame.Instance.Save(CoreGame.GameSaveKey);
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
        if (CoreGame.Instance.LongWinterCount == 0) title.color = redColor;
        sheepLabel.text = string.Format("{0}", CoreGame.Instance.SheepCount);

        if (CoreGame.Instance.StorageCapacity > CoreGame.Instance.FishCount + CoreGame.Instance.HaylageCount)
        {
            hayLabel.color = blackColor;
            hayLabel.text = string.Format("{0}/{1}", CoreGame.Instance.HaylageCount,
                Mathf.Min(CoreGame.Instance.SheepCount * CoreGame.SeasonDays,
                    CoreGame.Instance.StorageCapacity - CoreGame.Instance.FishCount));

            fishLabel.color = CoreGame.Instance.IsDeadSea ? redColor : blackColor;
            fishLabel.text = string.Format("{0}/{1}", CoreGame.Instance.FishCount,
                Mathf.Min(CoreGame.SeasonDays, CoreGame.Instance.StorageCapacity - CoreGame.Instance.HaylageCount));
        }
        else
        {
            hayLabel.color = redColor;
            hayLabel.text = string.Format("{0}", CoreGame.Instance.HaylageCount);

            fishLabel.color = redColor;
            fishLabel.text = string.Format("{0}", CoreGame.Instance.FishCount);
        }

        longhouseButton.gameObject.SetActive(CoreGame.Instance.DayCount <= 0);
    }

    private void HaylageClick(Vector2 point)
    {
        helpHay.gameObject.SetActive(false);

        var stone = CoreGame.Instance.HaylageSummer();
        ShowStats();

        var prefab = stone==0?haylagePrefab: stonePrefab;

        var item = (GameObject)Instantiate(prefab, transform);
        item.transform.position = new Vector3(point.x, point.y, 0f);
    }

    private void FishingClick(Vector2 point)
    {
        helpFish.gameObject.SetActive(false);

        var oldFish = CoreGame.Instance.FishCount;
        var seal = CoreGame.Instance.FishingSummer();
        BigFishAchievement(CoreGame.Instance.FishCount - oldFish);
        ShowStats();

        var prefab = fishPrefab;
        switch (seal)
        {
            case -1: prefab = fishbonePrefab; break;
            case 1: prefab = sealPrefab; break;
        }


        var item = (GameObject)Instantiate(prefab, transform);
        item.transform.position = new Vector3(point.x, point.y, 0f);
    }

    private void CreateSheep()
    {
        var cnt = 0;
        var height = Camera.allCameras[0].orthographicSize;
        var width = height * Camera.allCameras[0].aspect * height;

        while (cnt < 100)
        {
            cnt++;
            var point = new Vector2(Random.Range(-width, width), Random.Range(-height, height));

            var hit = Physics2D.Raycast(point, Vector2.zero);
            if (hit.transform != null)
            {
                if (hit.transform.name.Contains("land"))
                {
                    var item = (GameObject)Instantiate(sheepPrefab, transform);
                    item.transform.position = new Vector3(point.x, point.y, 0f);
                    return;
                }
            }
        }
    }
    #endregion

    #region achievements
    /// <summary>длинная зима</summary>
    private void LongWinterAchievement()
    {
        if (CoreGame.Instance.LongWinterCount > 0) return;
        if (PlayerPrefs.HasKey(GPGSIds.achievement_long_winter)) return;

        // unlock achievement (achievement ID "Cfjewijawiu_QA")
        Social.ReportProgress(GPGSIds.achievement_long_winter, 100.0f, (bool success) =>
        {
            // handle success or failure
            if (success)
            {
                PlayerPrefs.SetInt(GPGSIds.achievement_long_winter, 100);
            }
        });
    }

    /// <summary>большой улов</summary>
    private void BigFishAchievement(int fishing)
    {
        if (fishing <= 0) DeadSeaAchievement(fishing);

        if (fishing < 3) return;
        if (PlayerPrefs.HasKey(GPGSIds.achievement_big_fish)) return;

        Social.ReportProgress(GPGSIds.achievement_big_fish, 100.0f, (bool success) =>
        {
            // handle success or failure
            if (success)
            {
                PlayerPrefs.SetInt(GPGSIds.achievement_big_fish, 100);
            }
        });
    }

    /// <summary>метрвое море</summary>
    private void DeadSeaAchievement(int fishing)
    {
        if (fishing > 0) return;
        if (PlayerPrefs.HasKey(GPGSIds.achievement_dead_sea)) return;

        Social.ReportProgress(GPGSIds.achievement_dead_sea, 100.0f, (bool success) =>
        {
            // handle success or failure
            if (success)
            {
                PlayerPrefs.SetInt(GPGSIds.achievement_dead_sea, 100);
            }
        });
    }

    /// <summary>Стадо из 20 овец</summary>
    private void BigFlockAchievement()
    {
        if (CoreGame.Instance.SheepCount < 20) return;
        if (PlayerPrefs.HasKey(GPGSIds.achievement_big_flock))
        {
            GreatFlockAchievement();
            return;
        }


        // unlock achievement (achievement ID "Cfjewijawiu_QA")
        Social.ReportProgress(GPGSIds.achievement_big_flock, 100.0f, (bool success) =>
        {
            // handle success or failure
            if (success)
            {
                PlayerPrefs.SetInt(GPGSIds.achievement_big_flock, 100);
            }
        });
    }

    /// <summary>Стадо из 30 овец</summary>
    private void GreatFlockAchievement()
    {
        if (CoreGame.Instance.SheepCount < 30) return;
        if (PlayerPrefs.HasKey(GPGSIds.achievement_great_flock)) return;

        // unlock achievement (achievement ID "Cfjewijawiu_QA")
        Social.ReportProgress(GPGSIds.achievement_great_flock, 100.0f, (bool success) =>
        {
            // handle success or failure
            if (success)
            {
                PlayerPrefs.SetInt(GPGSIds.achievement_great_flock, 100);
            }
        });
    }

    /// <summary>сыграл 10 игр</summary>
    private void Game10Achievement()
    {
        if (CoreGame.Instance.WinterCount > 0) return;
        var games = PlayerPrefs.GetInt(GPGSIds.achievement_play_test, 0);
        games++;
        PlayerPrefs.SetInt(GPGSIds.achievement_play_test, games);
        if (games != 11) return;

        // unlock achievement (achievement ID "Cfjewijawiu_QA")
        Social.ReportProgress(GPGSIds.achievement_play_test, 100.0f, (bool success) =>
        {
            // handle success or failure
            if (success)
            {
                PlayerPrefs.SetInt(GPGSIds.achievement_play_test, 100);
            }
        });
    }
    #endregion
}
