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

    public Image icon200;
    public Image icon500;
    public Button summerButton;

    /// <summary>не хватает сукна</summary>
    public Color redColor;
    /// <summary>можно купить</summary>
    public Color greenColor;
    /// <summary>уже купил!</summary>
    public Color goldColor;

    void Awake()
    {
        description.text = string.Format(LanguageManager.Instance.GetTextValue("merchant_description"), CoreGame.Instance.FeltedCount);
        item200.text = LanguageManager.Instance.GetTextValue("merchant_item200");
        item500.text = LanguageManager.Instance.GetTextValue("merchant_item500");

        summerButton.GetComponentInChildren<Text>().text = LanguageManager.Instance.GetTextValue("summer_button");

        if (CoreGame.Instance.ScytheCount > 0)
        {
            icon200.color = goldColor;
        }
        else
        {
            if (CoreGame.Instance.FeltedCount >= 200)
            {
                icon200.color = greenColor;
            }
            else
            {
                icon200.color = redColor;
            }
        }

        if (CoreGame.Instance.HayforkCount > 0)
        {
            icon500.color = goldColor;
        }
        else
        {
            if (CoreGame.Instance.FeltedCount >= 500)
            {
                icon500.color = greenColor;
            }
            else
            {
                icon500.color = redColor;
            }
        }
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
            icon200.color = goldColor;
    }

    public void BuyItem500()
    {
        if (CoreGame.Instance.BuyHayfork())
            icon500.color = goldColor;
    }
}
