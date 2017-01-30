using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;
    [SerializeField]
    private GameObject WinterSound;
    [SerializeField]
    private GameObject SummerSound;
    [SerializeField]
    private AudioSource SheepSound;

    void Awake()
    {
        DontDestroyOnLoad(this);
        Instance = this;

        Application.targetFrameRate = 10;
    }

    public void StopSound()
    {
        WinterSound.SetActive(false);
        SummerSound.SetActive(false);
    }

    public void PlaySummer()
    {
        SummerSound.SetActive(true);
    }

    public void PlayWinter()
    {
        WinterSound.SetActive(true);
    }

    public void PlaySheep()
    {
        SheepSound.pitch = Random.Range(0.4f, 0.6f);
        SheepSound.Play();
    }
}
