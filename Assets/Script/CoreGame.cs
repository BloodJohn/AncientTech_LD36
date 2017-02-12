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
    /// <summary>Ключ2 куда мы сохраним игру </summary>
    public const string SecondChanseKey = "secondChanceSave";
    /// <summary>цена сукна за серп</summary>
    public const int PriceScythe = 200;
    /// <summary>цена сукна за вилы</summary>
    public const int PriceHayfork = 500;
    /// <summary>цена сукна за второй шанс</summary>
    public const int PriceSecondChanse = 1000;
    /// <summary>цена сукна за драккар</summary>
    public const int PriceDrakkar = 10000;

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
    /// <summary>всю зиму жрал только рыбу</summary>
    public bool WinterFishOnly;
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
    /// <summary>Второй шанс</summary>
    public int SecondChanseCount;
    /// <summary>Драккар</summary>
    public int DrakkarCount;

    /// <summary>рыбы в море</summary>
    private int SeaCount = 200;
    #endregion

    #region function

    /// <summary>Дополнительные ходы при длинной зиме</summary>
    private int LongWinterTurns
    {
        get
        {
            //до 4го амбара зима не растет
            if (HouseCount < StonePerHouse * 4) return 1;

            //зима растет до 10 недель
            return Mathf.Min(WinterCount / (2 * LongWinterCicle), 10); 
        }
    }
    /// <summary>Вместимость амбаров (сено+рыба)</summary>
    public int StorageCapacity { get { return (HouseCount / StonePerHouse) * 100; } }

    /// <summary>всего сукна полученно, включая уже потраченное</summary>
    public int TotalFelted { get { return FeltedCount + ScytheCount * 200 + HayforkCount * 500; } }

    /// <summary>на второе лето после долгой зимы приходит мертвое море</summary>
    public bool IsDeadSea { get { return LongWinterCount == 1 && WinterCount > EasyWinters; } }
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
        LongWinterCount = 1;
        DayCount = 0;
        SheepCount = SheepStartCount;
        HaylageCount = 0;
        WoolCount = 0;
        FeltedCount = 0;
        MeatCount = 0;
        FishCount = 0;
        StoneCount = 0;
        SealCount = 0;
        
        HouseCount = StonePerHouse * 3;
        BoatCount = SealPerBoat * 2; //вначале у нас есть 2 лодки

        ScytheCount = 0;
        HayforkCount = 0;
        SecondChanseCount = 0;
        DrakkarCount = 0;

        SeaCount = SeaFirst;
        SoundManager.Instance.StopSound();
        SceneManager.LoadScene(SummerController.sceneName);
    }

    public void Save(string saveKey)
    {
        SoundManager.Instance.StopSound();
        var json = JsonUtility.ToJson(this);
        PlayerPrefs.SetString(saveKey, json);
    }

    public void LoadSecondChance()
    {
        if (SecondChanseCount <= 0)
        {
            RestartGame();
        }
        else
        {
            var json = PlayerPrefs.GetString(SecondChanseKey, string.Empty);
            PlayerPrefs.DeleteKey(SecondChanseKey);

            if (string.IsNullOrEmpty(json))
            {
                RestartGame();
            }
            else
            {
                PlayerPrefs.SetString(GameSaveKey, json);
                LoadGame();
            }
        }
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
        SummerCount++;

        //плодим овец
        if (WinterCount > 0 && SheepCount > 1) SheepCount += SheepCount / 2;
        //портим рыбу
        if (WinterFishOnly && FishCount > 3) FishCount -= FishCount / 3;

        if (WinterCount == 0)
            SeaCount = SeaFirst;
        else
        {
            //после первой зимовки рыбы в море бывает разное количество (от 1 до 3 рыбин за улов)
            SeaCount = Random.Range(SeaMin + BoatCount, SeaMax);
            Debug.LogFormat("Sea {0} [{1}+{2}, {3}] = {4}", SeaCount, SeaMin, BoatCount, SeaMax, SeaCount / SeaMin);
        }

        //короткое лето после долгой зимы
        if (LongWinterCount == 0) DayCount -= LongWinterTurns;

        SoundManager.Instance.PlaySummer();
    }

    private void TurnSummerDay()
    {
        DayCount--;
    }

    public int FishingSummer()
    {
        TurnSummerDay();

        var seal = 0;
        var production = Mathf.Max(0, SeaCount / SeaMin);

        //до 4го амбара кормим рыбой
        if (HouseCount < StonePerHouse * 4) production = Mathf.Max(2, production);
        //драккар ловит большую рыбу
        if (DrakkarCount > 0) production++;

        if (IsDeadSea)
        {
            production = 0;
            seal = -1;
        }
        //если есть рыба - попадаются и тюлени
        else if (seal == 0 && Random.Range(0, SealChanse) == 0)
        {
            SealCount++;
            seal = 1;
        }


        FishCount += production;
        SeaCount -= production;
        if (FishCount > StorageCapacity) FishCount = StorageCapacity;

        //отсечка по вместимости амбаров
        if (FishCount + HaylageCount > StorageCapacity) //&& HaylageCount <= StorageCapacity
            //FishCount = StorageCapacity - HaylageCount;
            HaylageCount = StorageCapacity - FishCount; //если нет места для рыбы - выбрасываем сено!

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
        if (HaylageCount > StorageCapacity) HaylageCount = StorageCapacity;

        //отсечка по вместимости амбаров
        if (HaylageCount + FishCount > StorageCapacity) //&& FishCount <= StorageCapacity
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
            LongWinterCount = Random.Range(1, 4);
        }

        WoolCount = SheepCount;
        WinterFishOnly = true;

        SoundManager.Instance.PlayWinter();
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
            WinterFishOnly = false;
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
        if (FeltedCount >= PriceScythe && ScytheCount == 0)
        {
            Debug.LogFormat("buy item 200");
            FeltedCount -= PriceScythe;
            ScytheCount++;
            return true;
        }

        Debug.LogFormat("heed more Felted {0}/200", FeltedCount);
        return false;
    }

    public bool BuyHayfork()
    {
        if (FeltedCount >= PriceHayfork && HayforkCount == 0)
        {
            Debug.LogFormat("buy item 500");
            FeltedCount -= PriceHayfork;
            HayforkCount++;
            return true;
        }
        Debug.LogFormat("heed more Felted {0}/500", FeltedCount);
        return false;
    }

    public bool BuySecondChanse()
    {
        if (FeltedCount >= PriceSecondChanse && SecondChanseCount == 0)
        {
            Debug.LogFormat("buy item 1k");
            FeltedCount -= PriceSecondChanse;
            Save(SecondChanseKey);
            SecondChanseCount++;
            Save(GameSaveKey);
            return true;
        }
        Debug.LogFormat("heed more Felted {0}/1k", FeltedCount);
        return false;
    }

    public bool BuyDrakkar()
    {
        if (FeltedCount >= PriceDrakkar && DrakkarCount == 0)
        {
            Debug.LogFormat("buy item 10k");
            FeltedCount -= PriceDrakkar;
            DrakkarCount++;
            return true;
        }
        Debug.LogFormat("heed more Felted {0}/10k", FeltedCount);
        return false;
    }
    #endregion
}
