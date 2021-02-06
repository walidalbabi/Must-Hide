using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class MatchTimerManager : MonoBehaviour
{

    public static MatchTimerManager instance;

    [SerializeField]
    private float MatchTime;
    [SerializeField]
    private float ReadyTime;

    [SerializeField]
    private GameObject HuntersPanel;
    [SerializeField]
    private GameObject MonstersPanel;

    private float timer = 10;

    [SerializeField]
    private Text TimerUI;
    public bool isChoosePanel, canCount, createPlayer;

    public int Team;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        canCount = true;

        Invoke("SetCanCount", 0.3f);

        Team = InGameManager.instance.nextPlayerTeam;

    }

    // Update is called once per frame
    void Update()
    {
    
        TimeFormat();
        CheckIfCanCount();
        if(timer <= 0)
        {
            if (isChoosePanel)
                isChoosePanel = false;
            createPlayer = true;
            canCount = false;
            Invoke("SetCanCount", 0.3f);
        }

        if(isChoosePanel)
        {
            if(Team == 1)
            {
                MonstersPanel.SetActive(true);
            }else if(Team == 2)
            {
                HuntersPanel.SetActive(true);
            }
        }
        else
        {
            HuntersPanel.SetActive(false);
            MonstersPanel.SetActive(false);
        }

    }

    private void CheckIfCanCount()
    {

        if (isChoosePanel && !canCount)
        {
            timer = ReadyTime;
        }
        else if (!isChoosePanel && !canCount)
        {
            
            timer = MatchTime;
        }

        if (canCount )
        {
            timer -= Time.deltaTime;
        }

    }

    private void SetCanCount()
    {
        canCount = true;
    }

    private void TimeFormat()
    {
    
        float minutes = Mathf.Floor(timer / 60);
        float seconds = Mathf.RoundToInt(timer % 60);
        string minutesS;
        string secondsS;
        if (minutes < 10)
        {
            minutesS = "0" + minutes.ToString();
        }
        else
        {
            minutesS = minutes.ToString();
        }
        if (seconds < 10)
        {
            secondsS = "0" + Mathf.RoundToInt(seconds).ToString();
        }
        else
        {
            secondsS = Mathf.RoundToInt(seconds).ToString();
        }
        TimerUI.text = minutesS + ":" + secondsS;
    }
}
