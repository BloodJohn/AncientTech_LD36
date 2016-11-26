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

    public Button button200;
    public Button button500;
    public Button summerButton;

    void Awake()
    {
        description.text = string.Format(LanguageManager.Instance.GetTextValue("merchant_description"), CoreGame.Instance.FeltedCount);
        item200.text = LanguageManager.Instance.GetTextValue("merchant_item200");
        item500.text = LanguageManager.Instance.GetTextValue("merchant_item500");

        summerButton.GetComponentInChildren<Text>().text = LanguageManager.Instance.GetTextValue("summer_button");

        button200.gameObject.SetActive(CoreGame.Instance.ScytheCount <= 0 && CoreGame.Instance.FeltedCount >= 200);
        button500.gameObject.SetActive(CoreGame.Instance.HayforkCount <= 0 && CoreGame.Instance.FeltedCount >= 200);
    }

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape)) Application.Quit();
    }

    public void LoadSummer()
    {
        SceneManager.LoadScene(SummerController.sceneName);
    }

    public void BuyItem200()
    {
        if (CoreGame.Instance.BuyScythe())
            button200.gameObject.SetActive(false);
    }

    public void BuyItem500()
    {
        if (CoreGame.Instance.BuyHayfork())
            button500.gameObject.SetActive(false);
    }
}
