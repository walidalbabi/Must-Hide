using PlayFab;
using PlayFab.ClientModels;
using PlayFab.PfEditor.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class PlayfabCloudSaving : MonoBehaviour
{

    public static PlayfabCloudSaving instance;

    #region CloudData

    [SerializeField]
    public bool[] MonstersCharacters;
    public bool[] HuntersCharacters;

    private void Start()
    {
        MenuManager.instance.SetUpData();
    }

    public void CharatersStringToData(string charIn, bool isMonsters)
    {
        for (int i = 0; i < charIn.Length; i++)
        {
            if(int.Parse(charIn[i].ToString()) > 0)
            {
                if (isMonsters)
                    MonstersCharacters[i] = true;
                else
                    HuntersCharacters[i] = true;
            }
            else
            {
                if (isMonsters)
                    MonstersCharacters[i] = false;
                else
                    HuntersCharacters[i] = false;
            }
        }

        MenuManager.instance.SetUpData();
    }

    public string CharatersStringToData(bool isMonster)
    {
        string tostring = "";

        if (isMonster)
        {
            for (int i = 0; i < MonstersCharacters.Length; i++)
            {
                if (MonstersCharacters[i] == true)
                {
                    tostring += "1";
                }
                else
                {
                    tostring += "0";
                }
            }
        }
        else
        {
            for (int i = 0; i < HuntersCharacters.Length; i++)
            {
                if (HuntersCharacters[i] == true)
                {
                    tostring += "1";
                }
                else
                {
                    tostring += "0";
                }
            }
        }
      

        return tostring;
    }

    public void SetUserData(string data , bool isMonster)
    {
        if (isMonster)
        {
               PlayFabClientAPI.UpdateUserData(new UpdateUserDataRequest()
                {
                    Data = new Dictionary<string, string>() {
                    {"Monsters", data},
                }
                },
                result =>
                {
                    Debug.Log("Successfully updated user data");
                },
                error => {
                    Debug.Log("Got error setting user data Ancestor to Arthur");
                    Debug.Log(error.GenerateErrorReport());
                });
        }
        else
        {
            PlayFabClientAPI.UpdateUserData(new UpdateUserDataRequest()
            {
                Data = new Dictionary<string, string>() {
                    {"Hunters", data},
                }
            },
              result =>
              {
                  Debug.Log("Successfully updated user data");
              },
              error => {
                  Debug.Log("Got error setting user data Ancestor to Arthur");
                  Debug.Log(error.GenerateErrorReport());
              });
        }
     
    }

    public void GetUserData(string myPlayFabeId)
    {
        PlayFabClientAPI.GetUserData(new GetUserDataRequest()
        {
            PlayFabId = myPlayFabeId,
            Keys = null
        }, result => {
            Debug.Log("Got user data:");
            MenuManager.instance.SetUpData();
            if (result.Data == null || !result.Data.ContainsKey("Monsters") || !result.Data.ContainsKey("Hunters"))
            {
                Debug.Log("Monsters");
                Debug.Log("Hunters");
            }
            else {
                CharatersStringToData(result.Data["Monsters"].Value , true);
                CharatersStringToData(result.Data["Hunters"].Value, false);
            }
            //if (result.Data == null || !result.Data.ContainsKey("Hunters"))
            //{
            //    Debug.Log("Hunters");
            //}
            //else
            //{
            //    CharatersStringToData(result.Data["Hunters"].Value, true);
            //}
        }, (error) => {
            Debug.Log("Got error retrieving user data:");
            Debug.Log(error.GenerateErrorReport());
        });
    }
    #endregion CloudData

    #region VarsStats
    private int _nova;
    private int _eyes;
    private int _totalWins;
    private int _totalLoses;
    private int _totalKills;
    private int _level;
    private int _xp;
    private int _maxXp;
    private int _firstLogin;

    public int _Nova { get { return _nova; } }
    public int _Eyes { get { return _eyes; } }
    public int _TotalWins { get { return _totalWins; } }
    public int _TotalLoses { get { return _totalLoses; } }
    public int _TotalKills { get { return _totalKills; } }
    public int _Level { get { return _level; } }
    public int _Xp { get { return _xp; } }
    public int _MaxXp { get { return _maxXp; } }
    public int _FirstLogin { get { return _firstLogin; } }

    private bool isDataSynced;

    #endregion VarsStats

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        if (_maxXp != 0)
        {
            if (_xp >= _maxXp && isDataSynced)
            {
                Update_XP((_Xp % _maxXp), true);
                Update_MaxXP(1000);
                Update_Level();
                StartCloudPlayerStats();
                isDataSynced = false;
            }
        }
       

        if(isDataSynced && _firstLogin == 0)
        {
            Update_FirstLogin();
            Update_MaxXP(1000);
            Update_Level();
            StartCloudPlayerStats();
            isDataSynced = false;
        }
    }

    #region Update Functions

    public void Update_Nova(int amount)
    {
        _nova += amount;
    }

    public void Update_Eyes(int amount)
    {
        _eyes += amount;
    }

    public void Update_TotalWins()
    {
        _totalWins++;
    }

    public void Update_TotalLoses(int amount)
    {
        _totalLoses++;
    }

    public void Update_TotalKills(int amount)
    {
        _totalKills++;
    }

    public void Update_Level()
    {
        _level++;
    }

    public void Update_XP(int amount, bool isReset)
    {
        if (!isReset)
            _xp += amount;
        else
            _xp = amount;
    }
    public void Update_MaxXP(int amount)
    {
        _maxXp += amount;
    }
    public void Update_FirstLogin()
    {
        _firstLogin = 1;
    }

    #endregion Update Functions

    #region PlayerStats

    // Build the request object and access the API
    public void StartCloudPlayerStats()
    {
        PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest()
        {
            FunctionName = "UpdatePlayerStats", // Arbitrary function name (must exist in your uploaded cloud.js file)
            FunctionParameter = new { Nova = _nova, Eyes = _eyes, TotalWins = _totalWins, TotalLoses = _totalLoses, TotalKills = _totalKills, Level = _level, Xp = _xp, MaxXp = _maxXp, FirstLogin = _firstLogin }, // The parameter provided to your function
            GeneratePlayStreamEvent = true, // Optional - Shows this event in PlayStream
        }, OnCloudPlayerStats, OnErrorShared);

        Invoke("GetStatistics", 2f);
    }
    // OnCloudHelloWorld defined in the next code block

    private static void OnCloudPlayerStats(ExecuteCloudScriptResult result)
    {
        // CloudScript returns arbitrary results, so you have to evaluate them one step and one parameter at a time
        Debug.Log(result.ToJson());
        JsonObject jsonResult = (JsonObject)result.FunctionResult;
        object messageValue;
        jsonResult.TryGetValue("messageValue", out messageValue); // note how "messageValue" directly corresponds to the JSON values set in CloudScript
        Debug.Log((string)messageValue);
    }

    private static void OnErrorShared(PlayFabError error)
    {
        Debug.Log(error.GenerateErrorReport());
    }

    public void SetStats()
    {
        PlayFabClientAPI.UpdatePlayerStatistics(new UpdatePlayerStatisticsRequest
        {
            // request.Statistics is a list, so multiple StatisticUpdate objects can be defined if required.
            Statistics = new List<StatisticUpdate> {
             new StatisticUpdate { StatisticName = "Nova", Value = _nova },
             new StatisticUpdate { StatisticName = "Eyes", Value = _eyes },
             new StatisticUpdate { StatisticName = "TotalWins", Value = _totalWins },
             new StatisticUpdate { StatisticName = "TotalLoses", Value = _totalLoses },
             new StatisticUpdate { StatisticName = "TotalKills", Value = _totalKills },
             new StatisticUpdate { StatisticName = "Level", Value = _level },
             new StatisticUpdate { StatisticName = "Xp", Value = _xp },
             new StatisticUpdate { StatisticName = "MaxXp", Value = _maxXp },
             new StatisticUpdate { StatisticName = "FirstLogin", Value = _firstLogin },
            }
        },
         result => { Debug.Log("User statistics updated"); },
         error => { Debug.LogError(error.GenerateErrorReport()); });
    }

    public void GetStatistics()
    {
        PlayFabClientAPI.GetPlayerStatistics(
            new GetPlayerStatisticsRequest(),
            OnGetStatistics,
            error => Debug.LogError(error.GenerateErrorReport())
        );
    }

    void OnGetStatistics(GetPlayerStatisticsResult result)
    {
        Debug.Log("Received the following Statistics:");
        foreach (var eachStat in result.Statistics)
        {
            Debug.Log("Statistic (" + eachStat.StatisticName + "): " + eachStat.Value);
            switch (eachStat.StatisticName)
            {
                case "Nova":
                    _nova = eachStat.Value;
                    break;
                case "Eyes":
                    _eyes = eachStat.Value;
                    break;
                case "TotalWins":
                    _totalWins = eachStat.Value;
                    break;
                case "TotalLoses":
                    _totalLoses = eachStat.Value;
                    break;
                case "TotalKills":
                    _totalKills = eachStat.Value;
                    break;
                case "Level":
                    _level = eachStat.Value;
                    break;
                case "Xp":
                    _xp = eachStat.Value;
                    break;
                case "MaxXp":
                    _maxXp = eachStat.Value;
                    break; 
                case "FirstLogin":
                    _firstLogin = eachStat.Value;
                    break;
            }
        }

        isDataSynced = true;

        if (MenuManager.instance)
            MenuManager.instance.SetProfileInfo();


    }

    #endregion PlayerStats

}