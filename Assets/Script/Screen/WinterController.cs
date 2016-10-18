using SmartLocalization;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class WinterController : MonoBehaviour
{
    #region variables
    public const string sceneName = "Winter";

    public const int dayMax = 24;
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
        CoreGame.Instance.StartWinter();
        ShowStats();

        summerButton.GetComponentInChildren<Text>().text = LanguageManager.Instance.GetTextValue("summer_button");
        foodHelp.gameObject.SetActive(CoreGame.Instance.FeltedCount == 0);
        shipHelp.gameObject.SetActive(CoreGame.Instance.FeltedCount == 0);
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

        if (dayPrefab != null)
        {
            var item = (GameObject)Instantiate(dayPrefab, transform);
            item.transform.position = new Vector3(point.x, point.y, 0f);
        }

        foodHelp.gameObject.SetActive(false);
    }

    private void ShowStats()
    {
        title.text = string.Format(LanguageManager.Instance.GetTextValue("winter_title"), CoreGame.Instance.WinterCount, CoreGame.Instance.DayCount);
        haylageLabel.text = string.Format("{0}", CoreGame.Instance.HaylageCount);
        sheepLabel.text = string.Format("{0}", CoreGame.Instance.SheepCount);
        woolLabel.text = string.Format("{0}", CoreGame.Instance.WoolCount);
        feltedLabel.text = string.Format("{0}", CoreGame.Instance.FeltedCount);
        meatLabel.text = string.Format("{0}", CoreGame.Instance.MeatCount);
        fishLabel.text = string.Format("{0}", CoreGame.Instance.FishCount);

        var isWinter = CoreGame.Instance.DayCount > 0;

        slaughterButton.gameObject.SetActive(isWinter && CoreGame.Instance.SheepCount > 0);
        woolButton.gameObject.SetActive(isWinter && CoreGame.Instance.WoolCount > 0);
        fishButton.gameObject.SetActive(isWinter);
        summerButton.gameObject.SetActive(!isWinter);

        if (CoreGame.Instance.SheepCount <= 0)
        {
            //все овцы подохли
            SceneManager.LoadScene(DefeatController.sceneName);
        }
    }

    public void SummerClick()
    {
        SceneManager.LoadScene(SpringController.sceneName);
    }

    public void RestartClick()
    {
        CoreGame.Instance.RestartGame();
    }
    #endregion

    #region achievements

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
    #endregion
}