using PlayFab;
using PlayFab.ClientModels;
using PlayFab.Json;
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


        Invoke("SetUPPlayerSavedDataOnUI", 2f);
    }

    private void SetUPPlayerSavedDataOnUI()
    {
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

    public void SetUserCharactersData(string data , bool isMonster, bool isNova , int price)
    {
        LoadingScript.instance.StartLoading("Unlocking Operator..");

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
                    LoadingScript.instance.StopLoading();
                    if(price > 0)
                    {
                        if (isNova)
                            Update_Nova(-price);
                        else
                            Update_Eyes(-price);
                    }
                },
                error => {
                    Debug.Log("Got error setting user data Ancestor to Arthur");
                    Debug.Log(error.GenerateErrorReport());
                    LoadingScript.instance.StopLoading();
                    ErrorScript.instance.StartErrorMsg(error.GenerateErrorReport(), false, false, true, "");
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
                  LoadingScript.instance.StopLoading();
                  if (price > 0)
                  {
                      LoadingScript.instance.StartLoading("Processing...");

                      if (isNova)
                          Update_Nova(-price);
                      else
                          Update_Eyes(-price);
                  }

              },
              error => {
                  Debug.Log("Got error setting user data Ancestor to Arthur");
                  Debug.Log(error.GenerateErrorReport());
                  LoadingScript.instance.StopLoading();
                  ErrorScript.instance.StartErrorMsg(error.GenerateErrorReport(), false, false, true,"");
              });
        }

    }

    public void SetUserData(string key ,string data)
    {
        if (key == "Eyes" || key == "Nova")
            LoadingScript.instance.StartLoading("Processing Data...");

        PlayFabClientAPI.UpdateUserData(new UpdateUserDataRequest()
        {
            Data = new Dictionary<string, string>() {
                    {key, data},
                }
        },
         result =>
         {
             Debug.Log("Successfully updated user data");
             LoadingScript.instance.StopLoading();
         },
         error => {
             Debug.Log("Got error setting user data Ancestor to Arthur");
             Debug.Log(error.GenerateErrorReport());
             LoadingScript.instance.StopLoading();
             ErrorScript.instance.StartErrorMsg(error.GenerateErrorReport(), false, false, true,"");
         });

        MenuManager.instance.SetProfileInfo();
    }


    public void GetUserData(string myPlayFabeId)
    {
        PlayFabClientAPI.GetUserData(new GetUserDataRequest()
        {
            PlayFabId = myPlayFabeId,
            Keys = null
        }, result => {
            Debug.Log("Got user data:");

            if (result.Data == null || !result.Data.ContainsKey("Monsters") || !result.Data.ContainsKey("Hunters") || !result.Data.ContainsKey("Nova"))
            {
                Debug.Log("Monsters");
                Debug.Log("Hunters");
            }
            else {
                CharatersStringToData(result.Data["Monsters"].Value , true);
                CharatersStringToData(result.Data["Hunters"].Value, false);
                _nova = int.Parse(result.Data["Nova"].Value);
                _eyes = int.Parse(result.Data["Eyes"].Value);
                _firstLogin = int.Parse(result.Data["FirstLogin"].Value);
                _level = int.Parse(result.Data["Level"].Value);
                _maxXp = int.Parse(result.Data["MaxXp"].Value);
                _xp = int.Parse(result.Data["Xp"].Value);
            }
            if (_firstLogin == 0)
            {
                MenuManager.instance.ShowFirstTimeLogin();
                MenuManager.instance.UnlockHunters(0);
                MenuManager.instance.UnlockMonster(0);
                MenuManager.instance.UnlockHunters(1);
                MenuManager.instance.UnlockMonster(1);
                MenuManager.instance.UnlockHunters(2);
                MenuManager.instance.UnlockMonster(2);
                MenuManager.instance.UnlockHunters(3);
                MenuManager.instance.UnlockMonster(3);
                Update_FirstLogin();
                Update_MaxXP(1000);
                Update_Level();
            }

            isDataSynced = true;
        }, (error) => {
            Debug.Log("Got error retrieving user data:");
            Debug.Log(error.GenerateErrorReport());
        });

        MenuManager.instance.SetUpData();
        MenuManager.instance.SetProfileInfo();
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
               // StartCloudPlayerStats();
                isDataSynced = false;
            }
        }
  
    }

    #region Update Functions

    public void Update_Nova(int amount)
    {
        _nova += amount;
        SetUserData("Nova",_nova.ToString());
    }

    public void Update_Eyes(int amount)
    {
        _eyes += amount;
        SetUserData("Eyes", _eyes.ToString());
    }

    public void Update_TotalWins()
    {
        _totalWins++;
    }

    public void Update_TotalLoses()
    {
        _totalLoses++;
    }

    public void Update_Level()
    {
        _level++;
        SetUserData("Level", _level.ToString());
    }

    public void Update_XP(int amount, bool isReset)
    {
        if (!isReset)
            _xp += amount;
        else
            _xp = amount;

        SetUserData("Xp", _xp.ToString());
    }
    public void Update_MaxXP(int amount)
    {
        _maxXp += amount;
        SetUserData("MaxXp", _maxXp.ToString());
    }
    public void Update_FirstLogin()
    {
        _firstLogin = 1;
        SetUserData("FirstLogin", _firstLogin.ToString());
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
             new StatisticUpdate { StatisticName = "TotalWins", Value = _totalWins },
             new StatisticUpdate { StatisticName = "TotalLoses", Value = _totalLoses },
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
            error => Debug.LogError("Getting Statistic " + error.GenerateErrorReport())
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
                case "TotalWins":
                    _totalWins = eachStat.Value;
                    break;
                case "TotalLoses":
                    _totalLoses = eachStat.Value;
                    break;
            }
        }

        PlayfabCloudSaving.instance.GetUserData(PlayFabLogin.instance.PlayerInfo.AccountInfo.PlayFabId);

    }

    #endregion PlayerStats

}
