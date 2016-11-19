using SmartLocalization;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AutumnController : MonoBehaviour
{
    public const string sceneName = "Autumn";

    public Text title;
    public Text description;
    public Text author;

    void Awake()
    {
        if (CoreGame.Instance.WinterCount < 10)
        {
            title.text = LanguageManager.Instance.GetTextValue("autumn_title");
            description.text = LanguageManager.Instance.GetTextValue("autumn_description");
        }
        else if (CoreGame.Instance.WinterCount < 20)
        {
            title.text = LanguageManager.Instance.GetTextValue("autumn_title2");
            description.text = LanguageManager.Instance.GetTextValue("autumn_description2");
        }
        else
        {
            title.text = LanguageManager.Instance.GetTextValue("autumn_title2");
            description.text = LanguageManager.Instance.GetTextValue("autumn_description2");
        }

        author.text = LanguageManager.Instance.GetTextValue("intro_author");
    }

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape)) Application.Quit();
        if (Input.GetMouseButtonDown(0))
        {
            SceneManager.LoadScene(WinterController.sceneName);
        }
    }
}