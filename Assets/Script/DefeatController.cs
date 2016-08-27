using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DefeatController : MonoBehaviour
{
    public const string sceneName = "Defeat";
    public Text feltedLabel;
    public Text noFoodLabel;
    public Text noSheepLabel;

    /// <summary>ткань</summary>
    public int feltedCount;
    /// <summary>овец</summary>
    public int sheepCount;

    public void Awake()
    {
        feltedCount = PlayerPrefs.GetInt(WinterController.feltedCountKey);
        sheepCount = PlayerPrefs.GetInt(SummerController.sheepCountKey, 0);

        feltedLabel.text = string.Format("Your score: {0} Felted Wool", feltedCount);
        noSheepLabel.gameObject.SetActive(sheepCount <= 0);
        noFoodLabel.gameObject.SetActive(sheepCount > 0);
    }

    public void RestartClick()
    {
        PlayerPrefs.DeleteAll();
        SceneManager.LoadScene(SummerController.sceneName);
    }
}