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
    /// <summary>Ключ куда мы сохраним игру</summary>
    public const string GameSaveKey = "gameSave";

    /// <summary>сколько зим прошло</summary>
    public int WinterCount;
    /// <summary>сколько лет прошло</summary>
    public int SummerCount;
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
    /// <summary>камня</summary>
    public int StoneCount;
    /// <summary>тюленей</summary>
    public int SealCount;
    /// <summary>лугов</summary>
    public int LandCount = 3000;
    /// <summary>рыбы в море</summary>
    public int SeaCount = 200;

    #endregion

    #region constructor
    void Awake()
    {
        DontDestroyOnLoad(this);
        Instance = this;
    }

    public void RestartGame()
    {
        SummerCount = 0;
        WinterCount = 0;
        LongWinterCount = 0;
        DayCount = 0;
        SheepCount = SheepStartCount;
        HaylageCount = 0;
        WoolCount = 0;
        FeltedCount = 0;
        MeatCount = 0;
        FishCount = 0;
        StoneCount = 0;
        SealCount = 0;
        LandCount = LandMax;
        SeaCount = SeaMax;
        SceneManager.LoadScene(SummerController.sceneName);
    }

    public void Save()
    {
        var json = JsonUtility.ToJson(this);
        PlayerPrefs.SetString(GameSaveKey, json);
        //Debug.LogFormat("save: {0}", json);
    }

    public void LoadGame()
    {
        var json = PlayerPrefs.GetString(GameSaveKey, string.Empty);

        if (string.IsNullOrEmpty(json))
        {
            RestartGame();
            return;
        }

        JsonUtility.FromJsonOverwrite(json, this);

        if (SummerCount > WinterCount)
        {
            SceneManager.LoadScene(WinterController.sceneName);
        }
        else
        {
            SceneManager.LoadScene(SummerController.sceneName);
        }
    }
    #endregion

    #region summer

    public void StartSummer()
    {
        DayCount = SeasonDays;
        LandCount = LandMax;
        SummerCount++;

        //плодим овец
        if (WinterCount > 0 && SheepCount > 1) SheepCount += SheepCount / 2;


        if (WinterCount == 0)
            SeaCount = 200;
        else
        {
            //после первой зимовки рыбы в море бывает разное количество (от 1 до 3 рыбин за улов)
            SeaCount = Random.Range(100, 400);
        }

        //короткое лето после долгой зимы
        if (LongWinterCount <= 0)
            DayCount -= Mathf.Max(1, WinterCount / 10);
        Debug.LogFormat("seaCount {0} longWinter {1}", SeaCount, LongWinterCount);
    }

    private void TurnSummerDay()
    {
        if (LandCount < SheepCount) SheepCount = LandCount;
        LandCount -= SheepCount;
        DayCount--;
    }

    public int FishingSummer()
    {
        TurnSummerDay();

        var production = Mathf.RoundToInt((float)SeaCount / 100);
        if (production < 1)
        {
            //до 10 зимовки минимум добычи - одна рыба
            if (WinterCount < 10)
                production = 1;
            else
                production = 0;
        }

        if (Random.Range(0, 10) == 0) SealCount++; //WinterCount > 10 &&

        FishCount += production;
        SeaCount -= production;

        return production;
    }

    public void HaylageSummer()
    {
        TurnSummerDay();

        var production = PeopleCount * 2;
        if (production > LandCount) production = LandCount;
        if (Random.Range(0, 10) == 0) StoneCount++; //WinterCount > 10 &&

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
            DayCount += Mathf.Max(1, WinterCount / 10);
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
