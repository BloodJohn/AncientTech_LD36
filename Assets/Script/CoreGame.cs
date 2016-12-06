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
    //public const int LandMax = 3000;
    /// <summary>рыбы в море</summary>
    public const int SeaFirst = 299;
    /// <summary>минимальный улов</summary>
    public const int SeaMin = 100;
    /// <summary>максимальный улов</summary>
    public const int SeaMax = 400;
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
    public const int SealChanse = 20;



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
    /// <summary>Косы для срезания сена</summary>
    public int ScytheCount;
    /// <summary>Вилы для разбрасывания сена</summary>
    public int HayforkCount;

    /// <summary>лугов</summary>
    //public int LandCount = 3000;
    /// <summary>рыбы в море</summary>
    private int SeaCount = 200;

    #endregion

    #region function

    /// <summary>Дополнительные ходы при длинной зиме</summary>
    private int LongWinterTurns
    {
        get
        {
            if (WinterCount < LongWinterCicle * 4) return 1;

            return Mathf.Min(WinterCount / (2 * LongWinterCicle), 4);
        }
    }
    /// <summary>Вместимость амбаров (сено+рыба)</summary>
    public int StorageCapacity { get { return (HouseCount / StonePerHouse) * 100; } }

    public  int TotalFelted { get { return FeltedCount + ScytheCount*200 + HayforkCount*500; } }
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
        ScytheCount = 0;
        HayforkCount = 0;
        HouseCount = StonePerHouse * 3;
        BoatCount = SealPerBoat * 2; //вначале у нас есть 2 лодки

        //LandCount = LandMax;
        SeaCount = SeaFirst;
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
        //LandCount = LandMax;
        SummerCount++;

        //плодим овец
        if (WinterCount > 0 && SheepCount > 1) SheepCount += SheepCount / 2;


        if (WinterCount == 0)
            SeaCount = SeaFirst;
        else
        {
            //после первой зимовки рыбы в море бывает разное количество (от 1 до 3 рыбин за улов + нужно иметь 4 лодки!)
            SeaCount = Random.Range(SeaMin + BoatCount, SeaMax);
            Debug.LogFormat("Sea {0} [{1}+{2}, {3}] = {4}", SeaCount, SeaMin, BoatCount, SeaMax, SeaCount / SeaMin);
        }

        //короткое лето после долгой зимы
        if (LongWinterCount == 0) DayCount -= LongWinterTurns;
        Debug.LogFormat("seaCount {0} longWinter {1}", SeaCount, LongWinterCount);
    }

    private void TurnSummerDay()
    {
        /*if (LandCount < SheepCount) SheepCount = LandCount;
        LandCount -= SheepCount;*/
        DayCount--;
    }

    public int FishingSummer()
    {
        TurnSummerDay();

        var seal = 0;
        var production = SeaCount / SeaMin;
        if (production < 1)
        {
            //до 10 зимовки минимум добычи - одна рыба
            if (WinterCount < EasyWinters)
                production = 2;
            else
            {
                production = 0;
                seal = -1; //нет рыбы - нет тюленей
            }
        }


        if (Random.Range(0, SealChanse) == 0)
        {
            SealCount++;
            seal = 1;
        }

        FishCount += production;
        SeaCount -= production;

        //отсечка по вместимости амбаров
        if (FishCount > StorageCapacity - HaylageCount && HaylageCount <= StorageCapacity)
            FishCount = StorageCapacity - HaylageCount;

        return seal;
    }

    public int HaylageSummer()
    {
        TurnSummerDay();

        var production = PeopleCount * 2;
        if (ScytheCount > 0) production += 1;
        if (HayforkCount > 0) production += 2;

        var stone = 0;
        if (Random.Range(0, StoneChanse) == 0)
        {
            StoneCount++;
            stone = 1;
        }

        HaylageCount += production;
        //LandCount -= production;

        //отсечка по вместимости амбаров
        if (HaylageCount > StorageCapacity - FishCount && FishCount <= StorageCapacity)
            HaylageCount = StorageCapacity - FishCount;

        return stone;
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
        else if (LongWinterCount == 1)
        {
            LongWinterCount = Random.Range(1, 3);
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
        //кормим овец
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

    #region merchant

    public bool BuyScythe()
    {
        if (FeltedCount >= 200)
        {
            Debug.LogFormat("buy item 200");
            FeltedCount -= 200;
            ScytheCount++;
            return true;
        }

        Debug.LogFormat("heed more Felted 200/{0}", FeltedCount);
        return false;
    }

    public bool BuyHayfork()
    {
        if (FeltedCount >= 500)
        {
            Debug.LogFormat("buy item 500");
            FeltedCount -= 500;
            HayforkCount++;
            return true;
        }
        Debug.LogFormat("heed more Felted 500/{0}", FeltedCount);
        return false;
    }
    #endregion
}
