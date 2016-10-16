using GooglePlayGames;
using GooglePlayGames.BasicApi;
using SmartLocalization;
using UnityEngine;
using UnityEngine.UI;

public class IntroductionController : MonoBehaviour
{
    public const string sceneName = "Introduction";

    public Text title;
    public Text description;
    public Text author;

    void Awake()
    {
        if (Application.systemLanguage == SystemLanguage.Russian)
        {
            LanguageManager.Instance.ChangeLanguage("ru");
        }
        else
        {
            LanguageManager.Instance.ChangeLanguage("en");
        }

        LanguageManager.SetDontDestroyOnLoad();

        title.text = LanguageManager.Instance.GetTextValue("intro_title");
        description.text = LanguageManager.Instance.GetTextValue("intro_description");
        author.text = LanguageManager.Instance.GetTextValue("intro_author");
    }

    void Start()
    {
        GooglePlayServices();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            CoreGame.Instance.RestartGame();
        }

        if (Input.GetKeyUp(KeyCode.Escape)) Application.Quit();
    }

    private void GooglePlayServices()
    {
        Debug.LogFormat("GooglePlayServices");
        var config = new PlayGamesClientConfiguration.Builder()
        // enables saving game progress.
        //.EnableSavedGames()
        // registers a callback to handle game invitations received while the game is not running.
        //.WithInvitationDelegate(< callback method >)
        // registers a callback for turn based match notifications received while the
        // game is not running.
        //.WithMatchDelegate(< callback method >)
        // require access to a player's Google+ social graph (usually not needed)
        //.RequireGooglePlus()
        .Build();

        PlayGamesPlatform.InitializeInstance(config);
        // recommended for debugging:
        PlayGamesPlatform.DebugLogEnabled = true;
        // Activate the Google Play Games platform
        PlayGamesPlatform.Activate();


        // authenticate user:
        Social.localUser.Authenticate((bool success) =>
        {
            // handle success or failure
            if (!success)
            {
                //пишем для отладки, потом надо убрать.
                author.text = "Social.localUser.Authenticate - failed";
            }
        });
    }
}
