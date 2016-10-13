using SmartLocalization;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DefeatController : MonoBehaviour
{
    public const string sceneName = "Defeat";
    public const string storeURL = "https://play.google.com/store/apps/details?id=com.StarAge.IceLand";
    public const string voteCountKey = "vote";

    public Text feltedLabel;
    public Text noFoodLabel;

    public Button restartButton;
    public Button voteButton;

    /// <summary>ткань</summary>
    private int feltedCount;
    /// <summary>овец</summary>
    private int sheepCount;

    public void Awake()
    {
        feltedCount = PlayerPrefs.GetInt(WinterController.feltedCountKey);
        sheepCount = PlayerPrefs.GetInt(SummerController.sheepCountKey, 0);

        feltedLabel.text = string.Format(LanguageManager.Instance.GetTextValue("defeat_result"), feltedCount);

        if (sheepCount > 0)
        {
            noFoodLabel.text = LanguageManager.Instance.GetTextValue("defeat_noFood");
        }
        else
        {
            noFoodLabel.text = LanguageManager.Instance.GetTextValue("defeat_noSheep");
        }

        restartButton.GetComponentInChildren<Text>().text = LanguageManager.Instance.GetTextValue("defeat_restart");
        voteButton.GetComponentInChildren<Text>().text = LanguageManager.Instance.GetTextValue("defeat_vote");

        restartButton.enabled = (feltedCount < 100);
        voteButton.enabled = (feltedCount >= 100);
    }

    public void RestartClick()
    {
        PlayerPrefs.DeleteAll();
        SceneManager.LoadScene(SummerController.sceneName);
    }

    public void VoteClick()
    {
        Application.OpenURL(storeURL);
        RestartClick();
    }
}