using SmartLocalization;
using UnityEngine;
using UnityEngine.SceneManagement;
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

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            CoreGame.instance.RestartGame();
        }
    }
}
