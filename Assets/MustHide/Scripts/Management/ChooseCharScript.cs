using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChooseCharScript : MonoBehaviour
{
    public static ChooseCharScript instance;

    public CharacterSelectBtn[] Monsters;
    public CharacterSelectBtn[] Hunters;

    public string Name;

    private PhotonPlayer currentplayer;
    private CharacterSelectionInfo _CharacterSelectionInfo;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        SetChooseCharVarToDefault();
        _CharacterSelectionInfo = MatchTimerManager.instance.CharacterPanelInfo.GetComponent<CharacterSelectionInfo>();
    }

    //Setting GameObjects After Scene Loaded
    private void SetChooseCharVarToDefault()
    {
        for (int i = 0; i < Monsters.Length; i++)
        {
            if (PlayfabCloudSaving.instance.MonstersCharacters[i] == false)
            {

                Monsters[i].SetState(CharacterSeletctBtnState.NotPurshased);
            }
            else
            {

                Monsters[i].SetState(CharacterSeletctBtnState.CanSelect);
            }

            Monsters[i].DisableMark();
        }

        for (int i = 0; i < Hunters.Length; i++)
        {
            if (PlayfabCloudSaving.instance.HuntersCharacters[i] == false)
            {

                Hunters[i].SetState(CharacterSeletctBtnState.NotPurshased);
            }
            else
            {

                Hunters[i].SetState(CharacterSeletctBtnState.CanSelect);
            }

            Hunters[i].DisableMark();
        }

    }


    public void GetCharacterNameFromBtn(string name)
    {
        Name = name;
        for (int i = 0; i < InGameManager.instance.photonPlayer.Count; i++)
        {
            if (InGameManager.instance.photonPlayer[i].choosenCharacterName == Name) break;
        }

        AcceptCharacterName();
      
    }

    private void AcceptCharacterName()
    {
        if (!MatchTimerManager.instance.CharacterPanelInfo.activeInHierarchy) MatchTimerManager.instance.CharacterPanelInfo.SetActive(true);

        _CharacterSelectionInfo.SetCharacterSelectionInfo(UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.gameObject.GetComponent<CharacterInfoHolder>());
        currentplayer.SetChosenCharacterName(Name);
    }

    public void SetCurrentPlayerComponent(PhotonPlayer photonPlayer)
    {
        currentplayer = photonPlayer;
    }

    public void UpdateState()
    {
        SetChooseCharVarToDefault();

        for (int i = 0; i < InGameManager.instance.photonPlayer.Count; i++)
        {


            for (int j = 0; j < Monsters.Length; j++)
            {
                if (InGameManager.instance.photonPlayer[i].choosenCharacterName == Monsters[j].gameObject.name)
                {
                    Debug.Log("Found");
                    if (Monsters[j].currentState == CharacterSeletctBtnState.IsSelected)
                    {
                        Debug.Log("Is Selected");
                        return;
                    }

                    if (Monsters[j].currentState == CharacterSeletctBtnState.NotPurshased)
                    {
                        Debug.Log("Selected By Other and Not Purchased");
                        Monsters[j].EnableMark();
                    }
                    else if (Monsters[j].currentState == CharacterSeletctBtnState.CanSelect)
                    {
                        Debug.Log("Selected By Other");
                        Monsters[j].SetState(CharacterSeletctBtnState.CantSelect);
                    }
                   
                }
            }


            for (int j = 0; j < Hunters.Length; j++)
            {
                if (InGameManager.instance.photonPlayer[i].choosenCharacterName == Hunters[j].gameObject.name)
                {
                    Debug.Log("Found");
                    if (Hunters[j].currentState == CharacterSeletctBtnState.IsSelected)
                    {
                        Debug.Log("Is Selected");
                        return;
                    }

                    if (Hunters[j].currentState == CharacterSeletctBtnState.NotPurshased)
                    {
                        Debug.Log("Selected By Other and Not Purchased");
                        Hunters[j].EnableMark();
                    }
                    else if (Hunters[j].currentState == CharacterSeletctBtnState.CanSelect)
                    {
                        Debug.Log("Selected By Other");
                        Hunters[j].SetState(CharacterSeletctBtnState.CantSelect);
                    }

                }
            }

        }
    }

    public CharacterSelectBtn CharacterNameToCharacterBtnComponent(string name)
    {
        foreach (var item in Monsters)
        {
            if (item.gameObject.name == name)
            {
                return item;
            }
        }

        foreach (var item in Hunters)
        {
            if (item.gameObject.name == name)
            {
                return item;
            }
        }

        return null;
    }
}
