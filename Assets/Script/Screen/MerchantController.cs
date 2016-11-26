using SmartLocalization;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MerchantController : MonoBehaviour
{
    public const string sceneName = "Merchant";
    public Text description;
    public Text item200;
    public Text item500;

    public Button summerButton; 

    void Awake()
    {
        description.text = LanguageManager.Instance.GetTextValue("merchant_description");
        item200.text = LanguageManager.Instance.GetTextValue("merchant_item200");
        item500.text = LanguageManager.Instance.GetTextValue("merchant_item500");

        summerButton.GetComponentInChildren<Text>().text = LanguageManager.Instance.GetTextValue("summer_button");
    }

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape)) Application.Quit();
        /*if (Input.GetMouseButtonDown(0))
        {
            SceneManager.LoadScene(WinterController.sceneName);
        }*/
    }

    public void LoadSummer()
    {
        SceneManager.LoadScene(SummerController.sceneName);
    }

    public void BuyItem200()
    {
        CoreGame.Instance.BuyScythe();
    }

    public void BuyItem500()
    {
        CoreGame.Instance.BuyHayfork();
    }
}
