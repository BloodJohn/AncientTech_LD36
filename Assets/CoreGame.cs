using UnityEngine;
using UnityEngine.SceneManagement;

public class CoreGame : MonoBehaviour
{
    #region variables
    public static CoreGame instance;
    /// <summary>население хутора</summary>
    public const int peopleCount = 12;
    /// <summary>овец в начале</summary>
    public const int sheepStartCount = 12;
    /// <summary>дней в сезоне</summary>
    public const int seasonDays = 24;
    /// <summary>лугов</summary>
    public const int landMax = 3000;
    /// <summary>рыбы в море</summary>
    public const int seaMax = 200;

    /// <summary>сколько зим прошло</summary>
    public int winterCount;
    /// <summary>сколько обычных зим прошло?</summary>
    public int longWinterCount;
    /// <summary>дней</summary>
    public int dayCount;
    /// <summary>овец</summary>
    public int sheepCount;
    /// <summary>сена</summary>
    public int haylageCount;
    /// <summary>шерсть</summary>
    public int woolCount;
    /// <summary>ткань</summary>
    public int feltedCount;
    /// <summary>мяса</summary>
    public int meatCount;
    /// <summary>трески</summary>
    public int fishCount;
    /// <summary>лугов</summary>
    public int landCount = 3000;
    /// <summary>рыбы в море</summary>
    public int seaCount = 200;

    #endregion


    void Awake()
    {
        DontDestroyOnLoad(this);
        instance = this;
    }

    public void RestartGame()
    {
        winterCount = 0;
        longWinterCount = 0;
        dayCount = 0;
        sheepCount = 12;
        haylageCount = 0;
        woolCount = 0;
        feltedCount = 0;
        meatCount = 0;
        fishCount = 0;
        landCount = landMax;
        seaCount = seaMax;
        SceneManager.LoadScene(SummerController.sceneName);
    }

    #region summer

    public void StartSummer()
    {
        dayCount = seasonDays;
        //после первой зимовки рыбы в море бывает разное количество (от 1 до 3 рыбин за улов)
        if (winterCount > 0)
            seaCount = Random.Range(100, 400);
        else
            seaCount = 200;

        //короткое лето после долгой зимы
        if (longWinterCount <= 0) dayCount--;
        Debug.LogFormat("seaCount {0} longWinter {1}", CoreGame.instance.seaCount, CoreGame.instance.longWinterCount);
    }

    public void TurnSummerDay()
    {
        if (landCount < sheepCount) sheepCount = landCount;
        landCount -= sheepCount;
        dayCount--;
    }

    public void FishingSummer()
    {
        TurnSummerDay();

        var production = Mathf.RoundToInt((float)CoreGame.instance.seaCount / 100);
        if (production <= 0) production = 1;

        CoreGame.instance.fishCount += production;
        CoreGame.instance.seaCount -= production;
    }

    public void HaylageSummer()
    {
        TurnSummerDay();

        var production = CoreGame.peopleCount * 2;
        if (production > CoreGame.instance.landCount) production = CoreGame.instance.landCount;

        CoreGame.instance.haylageCount += production;
        CoreGame.instance.landCount -= production;
    }
    #endregion
}
