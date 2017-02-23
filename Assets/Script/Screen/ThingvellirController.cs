using SmartLocalization;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ThingvellirController : MonoBehaviour
{
    public const string sceneName = "Thingvellir";

    public Text title;
    public Text description;
    public Text author;

    void Awake()
    {
            title.text = LanguageManager.Instance.GetTextValue("thing_title");
            description.text = LanguageManager.Instance.GetTextValue("thing_description");

        author.text = LanguageManager.Instance.GetTextValue("intro_author");
    }

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape)) Application.Quit();
        if (Input.GetMouseButtonDown(0))
        {
            SceneManager.LoadScene(SummerController.sceneName);
        }
    }
}
