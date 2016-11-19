﻿using SmartLocalization;
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
        if (CoreGame.Instance.WinterCount <= 10)
        {
            title.text = LanguageManager.Instance.GetTextValue("spring_title");
            description.text = LanguageManager.Instance.GetTextValue("spring_description");
        }
        else if (CoreGame.Instance.WinterCount <= 20)
        {
            title.text = LanguageManager.Instance.GetTextValue("spring_title2");
            description.text = LanguageManager.Instance.GetTextValue("spring_description2");
        }
        else
        {
            title.text = LanguageManager.Instance.GetTextValue("spring_title3");
            description.text = LanguageManager.Instance.GetTextValue("spring_description3");
        }

        author.text = LanguageManager.Instance.GetTextValue("intro_author");
    }

    void Start()
    {

        FirstWinterAchievement();
        LongWinterAchievement();
    }

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape)) Application.Quit();
        if (Input.GetMouseButtonDown(0))
        {
            SceneManager.LoadScene(SummerController.sceneName);
        }
    }

    /// <summary>длинная зима</summary>
    private void LongWinterAchievement()
    {
        if (CoreGame.Instance.LongWinterCount > 0) return;
        if (PlayerPrefs.HasKey(GPGSIds.achievement_long_winter)) return;

        // unlock achievement (achievement ID "Cfjewijawiu_QA")
        Social.ReportProgress(GPGSIds.achievement_long_winter, 100.0f, (bool success) =>
        {
            // handle success or failure
            if (success)
            {
                PlayerPrefs.SetInt(GPGSIds.achievement_long_winter, 100);
            }
        });
    }

    /// <summary>первая зима</summary>
    private void FirstWinterAchievement()
    {
        if (CoreGame.Instance.WinterCount != 1)
        {
            Winter10Achievement();
            return;
        };
        if (PlayerPrefs.HasKey(GPGSIds.achievement_first_winter)) return;

        // unlock achievement (achievement ID "Cfjewijawiu_QA")
        Social.ReportProgress(GPGSIds.achievement_first_winter, 100.0f, (bool success) =>
        {
            // handle success or failure
            if (success)
            {
                PlayerPrefs.SetInt(GPGSIds.achievement_first_winter, 100);
            }
        });
    }

    /// <summary>пережил 10 зим</summary>
    private void Winter10Achievement()
    {
        if (CoreGame.Instance.WinterCount != 10) return;
        if (PlayerPrefs.HasKey(GPGSIds.achievement_teenager)) return;

        // unlock achievement (achievement ID "Cfjewijawiu_QA")
        Social.ReportProgress(GPGSIds.achievement_teenager, 100.0f, (bool success) =>
        {
            // handle success or failure
            if (success)
            {
                PlayerPrefs.SetInt(GPGSIds.achievement_teenager, 100);
            }
        });
    }
}
