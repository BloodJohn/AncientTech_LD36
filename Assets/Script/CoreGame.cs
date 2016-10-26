using UnityEngine;
using UnityEngine.SceneManagement;

public class CoreGame : MonoBehaviour
{
    #region variables
    public static CoreGame Instance;
    /// <summary>население хутора</summary>
    public const int PeopleCount = 12;
    /// <summary>овец в начале</summary>
    public const int SheepStartCount = 12;
    /// <summary>дней в сезоне</summary>
    public const int SeasonDays = 24;
    /// <summary>лугов</summary>
    public const int LandMax = 3000;
    /// <summary>рыбы в море</summary>
    public const int SeaMax = 200;

    /// <summary>сколько зим прошло</summary>
    public int WinterCount;
    /// <summary>сколько обычных зим прошло?</summary>
    public int LongWinterCount;
    /// <summary>дней</summary>
    public int DayCount;
    /// <summary>овец</summary>
    public int SheepCount;
    /// <summary>сена</summary>
    public int HaylageCount;
    /// <summary>шерсть</summary>
    public int WoolCount;
    /// <summary>ткань</summary>
    public int FeltedCount;
    /// <summary>мяса</summary>
    public int MeatCount;
    /// <summary>трески</summary>
    public int FishCount;
    /// <summary>лугов</summary>
    public int LandCount = 3000;
    /// <summary>рыбы в море</summary>
    public int SeaCount = 200;

    #endregion


    void Awake()
    {
        DontDestroyOnLoad(this);
        Instance = this;
    }

    public void RestartGame()
    {
        WinterCount = 0;
        LongWinterCount = 0;
        DayCount = 0;
        SheepCount = SheepStartCount;
        HaylageCount = 0;
        WoolCount = 0;
        FeltedCount = 0;
        MeatCount = 0;
        FishCount = 0;
        LandCount = LandMax;
        SeaCount = SeaMax;
        SceneManager.LoadScene(SummerController.sceneName);
    }

    #region summer

    public void StartSummer()
    {
        if (WinterCount > 0 && SheepCount > 1)
        {
            SheepCount += SheepCount/2;
        }


        DayCount = SeasonDays;
        LandCount = LandMax;
        //после первой зимовки рыбы в море бывает разное количество (от 1 до 3 рыбин за улов)
        if (WinterCount > 0)
            SeaCount = Random.Range(100, 400);
        else
            SeaCount = 200;

        //короткое лето после долгой зимы
        if (LongWinterCount <= 0) DayCount--;
        Debug.LogFormat("seaCount {0} longWinter {1}", SeaCount, LongWinterCount);
    }

    public void TurnSummerDay()
    {
        if (LandCount < SheepCount) SheepCount = LandCount;
        LandCount -= SheepCount;
        DayCount--;
    }

    public int FishingSummer()
    {
        TurnSummerDay();

        var production = Mathf.RoundToInt((float)SeaCount / 100);
        if (production <= 0) production = 1;

        FishCount += production;
        SeaCount -= production;

        return production;
    }

    public void HaylageSummer()
    {
        TurnSummerDay();

        var production = PeopleCount * 2;
        if (production > LandCount) production = LandCount;

        HaylageCount += production;
        LandCount -= production;
    }
    #endregion

    #region winter

    public void StartWinter()
    {
        DayCount = SeasonDays;
        WinterCount++;
        LongWinterCount++;
        if (LongWinterCount > 5)
        {
            DayCount++;
            LongWinterCount = 0;
        }

        WoolCount = SheepCount;
    }

    public int TurnWinterDay()
    {
        var result = 0;

        DayCount--;
        //нет сена - забиваем скот
        while (HaylageCount < SheepCount && SheepCount > 0)
        {
            SheepCount--;
            MeatCount++;
        }
        HaylageCount -= SheepCount;

        //кормим людей
        if (MeatCount > 0)
        {
            MeatCount--;
            result = 1;
        }
        else if (FishCount > 0)
            FishCount--;
        else
        {
            //нет еды (мясо или рыба)
            SceneManager.LoadScene(DefeatController.sceneName);
            return -1;
        }

        //обрабатываем шерсть
        if (WoolCount > 0)
        {
            WoolCount--;
            FeltedCount++;
            result = 2;
        }

        return result;
    }

    public bool SlaughterWinter()
    {
        if (CoreGame.Instance.SheepCount <= 0) return false;

        SheepCount--;
        MeatCount++;

        return true;
    }
    #endregion
}
