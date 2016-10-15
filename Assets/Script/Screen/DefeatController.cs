﻿using SmartLocalization;
using UnityEngine;
using UnityEngine.UI;

public class DefeatController : MonoBehaviour
{
    public const string sceneName = "Defeat";
    public const string storeURL = "https://play.google.com/store/apps/details?id=com.StarAge.IceLand";
    public const string voteCountKey = "vote";

    public Text feltedLabel;
    public Text noFoodLabel;

    public Button restartButton;
    public Button voteButton;

    public void Awake()
    {
        feltedLabel.text = string.Format(LanguageManager.Instance.GetTextValue("defeat_result"), CoreGame.Instance.FeltedCount);

        if (CoreGame.Instance.SheepCount > 0)
        {
            noFoodLabel.text = LanguageManager.Instance.GetTextValue("defeat_noFood");
        }
        else
        {
            noFoodLabel.text = LanguageManager.Instance.GetTextValue("defeat_noSheep");
        }

        restartButton.GetComponentInChildren<Text>().text = LanguageManager.Instance.GetTextValue("defeat_restart");
        voteButton.GetComponentInChildren<Text>().text = LanguageManager.Instance.GetTextValue("defeat_vote");

        var showVoteBtn = CoreGame.Instance.FeltedCount >= 100 && !PlayerPrefs.HasKey(voteCountKey);

        restartButton.gameObject.SetActive(!showVoteBtn);
        voteButton.gameObject.SetActive(showVoteBtn);
    }

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape)) Application.Quit();
    }

    public void RestartClick()
    {
        CoreGame.Instance.RestartGame();
    }

    public void VoteClick()
    {
        Application.OpenURL(storeURL);
        PlayerPrefs.SetInt(voteCountKey, 1);

        restartButton.gameObject.SetActive(true);
        voteButton.gameObject.SetActive(false);
    }
}