using SmartLocalization;
using UnityEngine;
using UnityEngine.UI;

public class DefeatController : MonoBehaviour
{
    #region variables
    public const string sceneName = "Defeat";
    public const string storeURL = "https://play.google.com/store/apps/details?id=com.StarAge.IceLand";
    public const string voteCountKey = "vote";

    public Text feltedLabel;
    public Text noFoodLabel;

    /// <summary>новая игра</summary>
    public Button restartButton;
    /// <summary>оценить игру в маркете</summary>
    public Button voteButton;
    /// <summary>вернуться к сохранению</summary>
    public Button secondChanceButton;
    /// <summary>пиво для разработчиков</summary>
    public Button beerButton;
    #endregion

    #region Unity
    public void Awake()
    {
        feltedLabel.text = string.Format(LanguageManager.Instance.GetTextValue("defeat_result"), CoreGame.Instance.TotalFelted);

        if (CoreGame.Instance.HaylageCount <= 0)
            noFoodLabel.text = LanguageManager.Instance.GetTextValue("defeat_noHay");
        else if (CoreGame.Instance.SheepCount <= 0)
            noFoodLabel.text = LanguageManager.Instance.GetTextValue("defeat_noSheep");
        else
            noFoodLabel.text = LanguageManager.Instance.GetTextValue("defeat_noFood");

        restartButton.GetComponentInChildren<Text>().text = LanguageManager.Instance.GetTextValue("defeat_restart");
        voteButton.GetComponentInChildren<Text>().text = LanguageManager.Instance.GetTextValue("defeat_vote");
        secondChanceButton.GetComponentInChildren<Text>().text = LanguageManager.Instance.GetTextValue("defeat_secondchance");

        restartButton.gameObject.SetActive(false);
        voteButton.gameObject.SetActive(false);
        secondChanceButton.gameObject.SetActive(false);

        if (CoreGame.Instance.SecondChanseCount > 0)
        {
            secondChanceButton.gameObject.SetActive(true);
        }
        else if (CoreGame.Instance.FeltedCount >= 100 && !PlayerPrefs.HasKey(voteCountKey))
        {
            voteButton.gameObject.SetActive(true);
        }
        else
        {
            restartButton.gameObject.SetActive(true);
        }

        PlayerPrefs.DeleteKey(CoreGame.GameSaveKey);

        beerButton.interactable = !CoreGame.Instance.HasBeer;

        SoundManager.Instance.SetFontScene();
    }

    public void Start()
    {
        LeaderBoard();
    }

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape)) Application.Quit();
    }
    #endregion

    #region buttons UI
    public void SecondChanceClick()
    {
        CoreGame.Instance.LoadSecondChance();
    }

    public void RestartClick()
    {
        CoreGame.Instance.RestartGame();
    }

    public void VoteClick()
    {
        Application.OpenURL(storeURL);
        PlayerPrefs.SetInt(voteCountKey, 1);

        restartButton.gameObject.SetActive(true);
        voteButton.gameObject.SetActive(false);
    }

    private void LeaderBoard()
    {
        var oldResult = PlayerPrefs.GetInt(GPGSIds.leaderboard_woolen_fabric, 0);
        if (CoreGame.Instance.TotalFelted <= oldResult) return;

        Social.ReportScore(CoreGame.Instance.TotalFelted, GPGSIds.leaderboard_woolen_fabric,
            (bool success) =>
            {
                if (success)
                {
                    PlayerPrefs.SetInt(GPGSIds.leaderboard_woolen_fabric, CoreGame.Instance.TotalFelted);
                    Social.ShowLeaderboardUI();
                }
            });
    }

    public void BeerClick()
    {
        CoreGame.Instance.Purchase.OnPurchase = s =>
        {
            noFoodLabel.text = s;
        };

        CoreGame.Instance.Purchase.OnFailed = s =>
        {
            noFoodLabel.text = s;
        };

        //заплатить на пиво разработчикам
        //Beer Click!
        CoreGame.Instance.BuyBeer();
        beerButton.interactable = false;
    }
    #endregion
}