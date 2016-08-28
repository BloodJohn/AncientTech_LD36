using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SummerController : MonoBehaviour
{
    #region variables
    public const string sceneName = "Summer";
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

    public Button longhouseButton;

    public GameObject haylagePrefab;
    public GameObject fishPrefab;

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
    #endregion

    #region unity
    public void Awake()
    {
        sheepCount = PlayerPrefs.GetInt(sheepCountKey, 12);
        ShowStats();
    }

    void Update()
    {
        if (dayCount <= 0) return;

        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mouseWorldPos, Vector2.zero);
            if (hit != null && hit.transform != null)
            {
                if (hit.transform.name.Contains("sea")) FishingClick(hit.point);
                if (hit.transform.name.Contains("land")) HaylageClick(hit.point);

                //Debug.Log("Hit Collider: " + hit.transform.name);
            }
        }
    }
    #endregion

    #region stuff
    public void ShowStats()
    {
        title.text = string.Format("Summer {0}", dayCount);
        /*sheepLabel.text = string.Format("Sheeps {0}/{1} land", sheepCount, landCount);
        hayLabel.text = string.Format("Haylage {0}/{1} land", haylageCount, landCount);
        fishLabel.text = string.Format("Сodfish {0}/{1} sea", fishCount, seaCount);*/

        sheepLabel.text = string.Format("{0}", sheepCount);
        hayLabel.text = string.Format("{0}/{1}", haylageCount, sheepCount * 25);
        fishLabel.text = string.Format("{0}/{1}", fishCount, 25);

        longhouseButton.gameObject.SetActive(dayCount <= 0);

        if (sheepCount <= 0)
        {
            PlayerPrefs.SetInt(SummerController.sheepCountKey, sheepCount);
            SceneManager.LoadScene(DefeatController.sceneName);
        }
    }

    public void DayClick()
    {
        if (landCount < sheepCount) sheepCount = landCount;
        landCount -= sheepCount;
        dayCount--;
    }

    public void HaylageClick(Vector2 point)
    {
        DayClick();

        var production = peopleCount * 2;
        if (production > landCount) production = landCount;

        haylageCount += production;
        landCount -= production;

        ShowStats();

        var item = (GameObject)Instantiate(haylagePrefab, transform);
        item.transform.position = new Vector3(point.x, point.y, 0f);
    }

    public void FishingClick(Vector2 point)
    {
        DayClick();

        var production = seaCount / 1000;
        if (production > seaCount) production = seaCount;

        fishCount += production;
        seaCount -= production;
        ShowStats();

        var item = (GameObject)Instantiate(fishPrefab, transform);
        item.transform.position = new Vector3(point.x, point.y, 0f);
    }

    public void WinterClick()
    {
        PlayerPrefs.SetInt(sheepCountKey, sheepCount);
        PlayerPrefs.SetInt(haylageCountKey, haylageCount);
        PlayerPrefs.SetInt(fishCountKey, fishCount);
        SceneManager.LoadScene(WinterController.sceneName);
    }

    public void RestartClick()
    {
        PlayerPrefs.DeleteAll();
        SceneManager.LoadScene(SummerController.sceneName);
    }
    #endregion
}
