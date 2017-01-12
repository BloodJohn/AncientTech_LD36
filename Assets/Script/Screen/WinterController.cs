using SmartLocalization;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class WinterController : MonoBehaviour
{
    #region variables
    public const string sceneName = "Winter";
    public Color blackColor;
    public Color redColor;

    /// <summary>сколько рыбы было в начале зимы?</summary>
    private int startFishCount;
    /// <summary>сколько останется рыбы, если питаться только ею</summary>
    private int seaFoodCount;
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
    /// <summary>сколько амбаров</summary>
    public Text houseLabel;
    /// <summary>сколько лодок</summary>
    public Text boatLabel;

    public Button summerButton;

    public GameObject meatPrefab;
    public GameObject feltedPrefab;
    public GameObject fishPrefab;
    #endregion

    #region Unity
    public void Awake()
    {
        CoreGame.Instance.StartWinter();
        startFishCount = CoreGame.Instance.FishCount;
        seaFoodCount = CoreGame.Instance.FishCount - CoreGame.Instance.DayCount;
        ShowStats();

        summerButton.GetComponentInChildren<Text>().text = LanguageManager.Instance.GetTextValue("summer_button");

        if (CoreGame.Instance.FeltedCount == 0)
        {
            shipHelp.sprite = Resources.Load<Sprite>(LanguageManager.Instance.GetTextValue("winter_ship_spr"));
            foodHelp.sprite = Resources.Load<Sprite>(LanguageManager.Instance.GetTextValue("winter_turn_spr"));

            foodHelp.gameObject.SetActive(true);
            shipHelp.gameObject.SetActive(true);
        }
        else
        {
            foodHelp.gameObject.SetActive(false);
            shipHelp.gameObject.SetActive(false);
        }
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
                if (hit.transform.name.Contains("sheep")) SheepClick(hit.point);
                if (hit.transform.name.Contains("day")) DayClick(hit.point);
            }
        }
    }

    #endregion

    #region UI events
    private void SheepClick(Vector2 point)
    {
        if (!CoreGame.Instance.SlaughterWinter()) return;

        ShowStats();

        var item = (GameObject)Instantiate(meatPrefab, transform);
        item.transform.position = new Vector3(point.x, point.y, 0f);

        shipHelp.gameObject.SetActive(false);
    }

    private void DayClick(Vector2 point)
    {
        GameObject dayPrefab = null;

        switch (CoreGame.Instance.TurnWinterDay())
        {
            case 0: dayPrefab = fishPrefab; break;
            case 1: dayPrefab = meatPrefab; break;
            case 2:
                dayPrefab = feltedPrefab;
                FeltedWoolAchievement();
                break;
        }

        ShowStats();
        WareHouseAchievement();

        if (dayPrefab != null)
        {
            var item = (GameObject)Instantiate(dayPrefab, transform);
            item.transform.position = new Vector3(point.x, point.y, 0f);
        }

        foodHelp.gameObject.SetActive(false);
    }

    private void ShowStats()
    {
        var isWinter = CoreGame.Instance.DayCount > 0;

        sheepLabel.gameObject.SetActive(isWinter && CoreGame.Instance.SheepCount > 0);
        woolLabel.gameObject.SetActive(isWinter && CoreGame.Instance.WoolCount > 0);
        fishLabel.gameObject.SetActive(isWinter);
        summerButton.gameObject.SetActive(!isWinter);

        title.text = string.Format(LanguageManager.Instance.GetTextValue("winter_title"), CoreGame.Instance.WinterCount, CoreGame.Instance.DayCount);
        if (CoreGame.Instance.LongWinterCount == 0) title.color = redColor;
        haylageLabel.text = string.Format("{0}", CoreGame.Instance.HaylageCount);
        sheepLabel.text = string.Format("{0}", CoreGame.Instance.SheepCount);
        woolLabel.text = string.Format("{0}", CoreGame.Instance.WoolCount);
        feltedLabel.text = string.Format("{0}", CoreGame.Instance.FeltedCount);
        meatLabel.text = string.Format("{0}", CoreGame.Instance.MeatCount);
        fishLabel.text = string.Format("{0}", CoreGame.Instance.FishCount);

        //амбар
        houseLabel.gameObject.SetActive(isWinter);
        if (CoreGame.Instance.StoneCount > 0)
        {
            houseLabel.text = string.Format("{0}+{1}", (float)CoreGame.Instance.HouseCount / CoreGame.StonePerHouse, (float)CoreGame.Instance.StoneCount / CoreGame.StonePerHouse);
        }
        else
        {
            if (CoreGame.Instance.HouseCount > 0)
                houseLabel.text = string.Format("{0}", (float)CoreGame.Instance.HouseCount / CoreGame.StonePerHouse);
            else
                houseLabel.gameObject.SetActive(false);
        }

        //лодки
        boatLabel.gameObject.SetActive(isWinter);
        if (CoreGame.Instance.SealCount > 0)
        {
            boatLabel.text = string.Format("{0}+{1}", (float)CoreGame.Instance.BoatCount / CoreGame.SealPerBoat, (float)CoreGame.Instance.SealCount / CoreGame.SealPerBoat);
        }
        else
        {
            if (CoreGame.Instance.BoatCount > 0)
                boatLabel.text = string.Format("{0}", (float)CoreGame.Instance.BoatCount / CoreGame.SealPerBoat);
            else
                boatLabel.gameObject.SetActive(false);
        }


        //все овцы подохли
        if (CoreGame.Instance.SheepCount <= 0)
        {

            SceneManager.LoadScene(DefeatController.sceneName);
        }

        //режем овец которых не прокормить
        if (CoreGame.Instance.SheepCount > CoreGame.Instance.HaylageCount)
        {
            sheepLabel.text = string.Format("{0}/{1}", CoreGame.Instance.SheepCount, CoreGame.Instance.HaylageCount);
            sheepLabel.color = redColor;
        }
        else
        {
            sheepLabel.color = blackColor;
        }

        //если сена не хватит до конца зимы..
        if (CoreGame.Instance.HaylageCount < CoreGame.Instance.SheepCount * CoreGame.Instance.DayCount)
        {
            haylageLabel.color = redColor;
        }
        else
        {
            haylageLabel.color = blackColor;
        }

        //предупреждение, если еды не хваатает
        if (CoreGame.Instance.DayCount > CoreGame.Instance.FishCount + CoreGame.Instance.MeatCount)
        {
            meatLabel.text = string.Format("{0}/{1}", CoreGame.Instance.MeatCount, CoreGame.Instance.DayCount - CoreGame.Instance.FishCount);
            meatLabel.color = redColor;
        }
        else
        {
            meatLabel.color = blackColor;
        }

        if (!isWinter)
        {
            SeaFoodAchievement();
            CarnivoreAchievement();
            //Debug.LogFormat("fish {0} begin {1} end {2}", CoreGame.Instance.FishCount, startFishCount, seaFoodCount);
        }
    }

    public void SummerClick()
    {
        CoreGame.Instance.Save();
        //SceneManager.LoadScene(MerchantController.sceneName);
        if (CoreGame.Instance.LongWinterCount == 0)
            SceneManager.LoadScene(MerchantController.sceneName);
        else
            SceneManager.LoadScene(SpringController.sceneName);
    }

    public void RestartClick()
    {
        CoreGame.Instance.RestartGame();
    }
    #endregion

    #region achievements

    /// <summary>постройка 4 сарая</summary>
    private void WareHouseAchievement()
    {
        if (CoreGame.Instance.HouseCount < CoreGame.StonePerHouse * 4) return;
        if (PlayerPrefs.HasKey(GPGSIds.achievement_warehouse)) return;

        Social.ReportProgress(GPGSIds.achievement_warehouse, 100.0f, (bool success) =>
        {
            // handle success or failure
            if (success)
            {
                PlayerPrefs.SetInt(GPGSIds.achievement_warehouse, 100);
            }
        });
    }

    /// <summary>партия из 50 шерсти</summary>
    private void FeltedWoolAchievement()
    {
        if (CoreGame.Instance.FeltedCount != 50) return;
        if (PlayerPrefs.HasKey(GPGSIds.achievement_first_batch_of_fabric)) return;

        // unlock achievement (achievement ID "Cfjewijawiu_QA")
        Social.ReportProgress(GPGSIds.achievement_first_batch_of_fabric, 100.0f, (bool success) =>
        {
            // handle success or failure
            if (success)
            {
                PlayerPrefs.SetInt(GPGSIds.achievement_first_batch_of_fabric, 100);
            }
        });
    }

    /// <summary>рыбная диета</summary>
    private void SeaFoodAchievement()
    {
        if (CoreGame.Instance.FishCount > seaFoodCount) return;
        if (PlayerPrefs.HasKey(GPGSIds.achievement_seafood)) return;

        // unlock achievement (achievement ID "Cfjewijawiu_QA")
        Social.ReportProgress(GPGSIds.achievement_seafood, 100.0f, (bool success) =>
        {
            // handle success or failure
            if (success)
            {
                PlayerPrefs.SetInt(GPGSIds.achievement_seafood, 100);
            }
        });
    }

    /// <summary>мясная диета</summary>
    private void CarnivoreAchievement()
    {
        if (CoreGame.Instance.FishCount < startFishCount) return;
        if (PlayerPrefs.HasKey(GPGSIds.achievement_arnivore)) return;

        // unlock achievement (achievement ID "Cfjewijawiu_QA")
        Social.ReportProgress(GPGSIds.achievement_arnivore, 100.0f, (bool success) =>
        {
            // handle success or failure
            if (success)
            {
                PlayerPrefs.SetInt(GPGSIds.achievement_arnivore, 100);
            }
        });
    }
    #endregion
}