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
        title.text = LanguageManager.Instance.GetTextValue("autumn_title");
        description.text = LanguageManager.Instance.GetTextValue("autumn_description");
        author.text = LanguageManager.Instance.GetTextValue("intro_author");
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            SceneManager.LoadScene(WinterController.sceneName);
        }
    }
}