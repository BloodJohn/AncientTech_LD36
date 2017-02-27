using UnityEngine;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;
    public bool IsSound;

    [SerializeField]
    private GameObject WinterSound;
    [SerializeField]
    private GameObject SummerSound;
    [SerializeField]
    private AudioSource SheepSound;
    /// <summary>Фонт для всех текстовых полей в игре</summary>
    [SerializeField]
    private Font TextFont;

    private static readonly string SoundKey = "muteSound";

    void Awake()
    {
        DontDestroyOnLoad(this);
        Instance = this;

        Application.targetFrameRate = 10;

        IsSound = PlayerPrefs.GetInt(SoundKey, 100) > 0;
    }

    public void SetFontScene()
    {
        foreach (var item in GameObject.FindObjectsOfType<Text>())
        {
            item.font = TextFont;
        }
    }

    public void MuteSound()
    {
        if (!IsSound)
        {
            IsSound = true;
            PlaySheep();
        }
        else
        {
            IsSound = false;
            StopSound();
        }

        PlayerPrefs.SetInt(SoundKey,IsSound?100:0);
    }

    public void StopSound()
    {
        WinterSound.SetActive(false);
        SummerSound.SetActive(false);
    }

    public void PlaySummer()
    {
        if (!IsSound) return;
        SummerSound.SetActive(true);
    }

    public void PlayWinter()
    {
        if (!IsSound) return;
        WinterSound.SetActive(true);
    }

    public void PlaySheep()
    {
        if (!IsSound) return;
        SheepSound.pitch = Random.Range(0.4f, 0.6f);
        SheepSound.Play();
    }
}
