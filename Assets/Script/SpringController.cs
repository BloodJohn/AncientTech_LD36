using SmartLocalization;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SpringController : MonoBehaviour
{
    public const string sceneName = "Spring";

    public Text title;
    public Text description;
    public Text author;

    void Awake()
    {
        title.text = LanguageManager.Instance.GetTextValue("spring_title");
        description.text = LanguageManager.Instance.GetTextValue("spring_description");
        author.text = LanguageManager.Instance.GetTextValue("intro_author");
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            SceneManager.LoadScene(SummerController.sceneName);
        }
    }
}
