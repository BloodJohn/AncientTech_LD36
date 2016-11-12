﻿using UnityEngine;
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
    /// <summary>минимальный улов</summary>
    public const int SeaMin = 100;
    /// <summary>камней на амбар</summary>
    public const int StonePerHouse = 10;
    /// <summary>тюленей на лодку</summary>
    public const int SealPerBoat = 5;
    /// <summary>периодичность длинной зимы (раз в сколько лет?)</summary>
    public const int LongWinterCicle = 5;
    /// <summary>начальная сложность (первые 10 зим)</summary>
    public const int EasyWinters = 10;
    /// <summary>вероятность выпадения камня</summary>
    public const int StoneChanse = 10;
    /// <summary>вероятность выпадения тюленя</summary>
    public const int SealChanse = 10;



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
    /// <summary>амбаров (/10)</summary>
    public int HouseCount;
    /// <summary>лодок (/10)</summary>
    public int BoatCount;

    /// <summary>лугов</summary>
    public int LandCount = 3000;
    /// <summary>рыбы в море</summary>
    public int SeaCount = 200;

    #endregion

    #region function
    /// <summary>Дополнительные ходы при длинной зиме</summary>
    public int LongWinterTurns { get { return 1; } }

    /// <summary>сколько мы можем заготовить сена</summary>
    public int HaylageMaxStore { get { return (3 + HouseCount / StonePerHouse) * 100; } }

    /// <summary>сколько мы можем заготовить рыбы</summary>
    public int FishingSea
    {
        get
        {
            //люди могут использовать не более 4х лодок!
            var boat = Mathf.Min(BoatCount / SealPerBoat, PeopleCount / 3);
            return boat * SeaMin + SeaMin / 2;
        }
    }
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
        HouseCount = 0;
        BoatCount = SealPerBoat * 2; //вначале у нас есть 2 лодки

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
            SeaCount = SeaMax;
        else
        {
            //после первой зимовки рыбы в море бывает разное количество (от 1 до 3 рыбин за улов + нужно иметь 4 лодки!)
            SeaCount = Random.Range(SeaMin, FishingSea);
        }

        //короткое лето после долгой зимы
        if (LongWinterCount == 0) DayCount -= LongWinterTurns;
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
            if (WinterCount < EasyWinters)
                production = 1;
            else
                production = 0;
        }

        if (Random.Range(0, SealChanse) == 0) SealCount++; //WinterCount > 10 &&

        FishCount += production;
        SeaCount -= production;

        return production;
    }

    public void HaylageSummer()
    {
        TurnSummerDay();

        var production = PeopleCount * 2;
        if (production > LandCount) production = LandCount;
        if (Random.Range(0, StoneChanse) == 0) StoneCount++; //WinterCount > 10 &&

        HaylageCount += production;
        LandCount -= production;
        //если склады переполнены
        if (HaylageCount > HaylageMaxStore) HaylageCount = HaylageMaxStore;
    }
    #endregion

    #region winter

    public void StartWinter()
    {
        DayCount = SeasonDays;
        WinterCount++;
        LongWinterCount++;
        if (LongWinterCount > LongWinterCicle)
        {
            DayCount += LongWinterTurns;
            LongWinterCount = 0;
        }

        WoolCount = SheepCount;

        /*
        //каждую зиму достроенные амбары немного разрушаются
        if (HouseCount >= StonePerHouse)
        {
            HouseCount -= Mathf.FloorToInt((float)HouseCount / StonePerHouse);
        }

        //каждую зиму достроенные лодки немного разрушаются
        if (BoatCount >= SealPerBoat)
        {
            BoatCount -= Mathf.FloorToInt((float)BoatCount / SealPerBoat);
        }*/
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

        //на мясной диете удается работать в два раза больше
        var poduction = result > 0 ? 2 : 1;
        for (var i = 0; i < poduction; i++)
        {
            //обрабатываем шерсть
            if (WoolCount > 0)
            {
                WoolCount--;
                FeltedCount++;
                result = 2;
            }
            else //если осталось время стоим лодки и дома
            {
                if (SealCount > 0)
                {
                    SealCount--;
                    BoatCount++;
                }
                else if (StoneCount > 0)
                {
                    StoneCount--;
                    HouseCount++;
                }
            }
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
