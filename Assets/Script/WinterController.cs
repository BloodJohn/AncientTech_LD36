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
        CoreGame.instance.dayCount = dayMax;
        CoreGame.instance.winterCount++;
        CoreGame.instance.longWinterCount++;
        if (CoreGame.instance.longWinterCount > 5)
        {
            CoreGame.instance.dayCount++;
            CoreGame.instance.longWinterCount = 0;
        }

        foodHelp.gameObject.SetActive(CoreGame.instance.feltedCount == 0);
        shipHelp.gameObject.SetActive(CoreGame.instance.feltedCount == 0);

        CoreGame.instance.woolCount = CoreGame.instance.sheepCount;
        ShowStats();

        summerButton.GetComponentInChildren<Text>().text = LanguageManager.Instance.GetTextValue("summer_button");
    }

    void Update()
    {
        if (CoreGame.instance.dayCount <= 0) return;

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
        if (CoreGame.instance.sheepCount <=0) return;

        CoreGame.instance.sheepCount--;
        CoreGame.instance.meatCount++;

        ShowStats();

        var item = (GameObject)Instantiate(meatPrefab, transform);
        item.transform.position = new Vector3(point.x, point.y, 0f);

        shipHelp.gameObject.SetActive(false);
    }

    private void DayClick(Vector2 point)
    {
        var dayPrefab = fishPrefab;

        CoreGame.instance.dayCount--;
        //нет сена - забиваем скот
        while (CoreGame.instance.haylageCount < CoreGame.instance.sheepCount && CoreGame.instance.sheepCount > 0)
        {
            CoreGame.instance.sheepCount--;
            CoreGame.instance.meatCount++;
        }
        CoreGame.instance.haylageCount -= CoreGame.instance.sheepCount;

        //кормим людей
        if (CoreGame.instance.meatCount > 0)
        {
            CoreGame.instance.meatCount--;
            dayPrefab = meatPrefab;
        }
        else if (CoreGame.instance.fishCount > 0)
            CoreGame.instance.fishCount--;
        else
        {
            //нет еды (мясо или рыба)
            SceneManager.LoadScene(DefeatController.sceneName);
            return;
        }

        //обрабатываем шерсть
        if (CoreGame.instance.woolCount > 0)
        {
            CoreGame.instance.woolCount--;
            CoreGame.instance.feltedCount++;
            dayPrefab = feltedPrefab;
        }

        ShowStats();

        var item = (GameObject)Instantiate(dayPrefab, transform);
        item.transform.position = new Vector3(point.x, point.y, 0f);

        foodHelp.gameObject.SetActive(false);
    }

    private void ShowStats()
    {
        title.text = string.Format(LanguageManager.Instance.GetTextValue("winter_title"), CoreGame.instance.winterCount, CoreGame.instance.dayCount);
        haylageLabel.text = string.Format("{0}", CoreGame.instance.haylageCount);
        sheepLabel.text = string.Format("{0}", CoreGame.instance.sheepCount);
        woolLabel.text = string.Format("{0}", CoreGame.instance.woolCount);
        feltedLabel.text = string.Format("{0}", CoreGame.instance.feltedCount);
        meatLabel.text = string.Format("{0}", CoreGame.instance.meatCount);
        fishLabel.text = string.Format("{0}", CoreGame.instance.fishCount);

        var isWinter = CoreGame.instance.dayCount > 0;

        slaughterButton.gameObject.SetActive(isWinter && CoreGame.instance.sheepCount > 0);
        woolButton.gameObject.SetActive(isWinter && CoreGame.instance.woolCount > 0);
        fishButton.gameObject.SetActive(isWinter);
        summerButton.gameObject.SetActive(!isWinter);

        if (CoreGame.instance.sheepCount <= 0)
        {
            //все овцы подохли
            SceneManager.LoadScene(DefeatController.sceneName);
        }
    }

    #region UI events

    public void SummerClick()
    {
        if (CoreGame.instance.sheepCount > 1) CoreGame.instance.sheepCount += CoreGame.instance.sheepCount / 2;
        SceneManager.LoadScene(SpringController.sceneName);
    }

    public void RestartClick()
    {
        CoreGame.instance.RestartGame();
    }
    #endregion
}