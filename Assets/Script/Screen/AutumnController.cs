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
        if (CoreGame.Instance.WinterCount < 2)
        {
            title.text = LanguageManager.Instance.GetTextValue("autumn_title");
            description.text = LanguageManager.Instance.GetTextValue("autumn_description");
        }
        else if (CoreGame.Instance.WinterCount < 5)
        {
            title.text = LanguageManager.Instance.GetTextValue("autumn_title4");
            description.text = LanguageManager.Instance.GetTextValue("autumn_description4");
        }
        else if (CoreGame.Instance.WinterCount < 10)
        {
            title.text = LanguageManager.Instance.GetTextValue("autumn_title2");
            description.text = LanguageManager.Instance.GetTextValue("autumn_description2");
        }
        else if (CoreGame.Instance.WinterCount < 15)
        {
            title.text = LanguageManager.Instance.GetTextValue("autumn_title3");
            description.text = LanguageManager.Instance.GetTextValue("autumn_description3");
        }
        else
        {
            title.text = LanguageManager.Instance.GetTextValue("autumn_title5");
            description.text = LanguageManager.Instance.GetTextValue("autumn_description5");
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