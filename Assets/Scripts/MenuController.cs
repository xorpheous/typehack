using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

//This class is not needed to use the SaveData class.  It is simply to show a very basic example
//of how to use SaveData.
public class MenuController : MonoBehaviour
{
    public Text messagePanel;
    public InputField playerNameField;
    public GameStatus gameStatus;
    public PlayerData playerData;
    public Text[] missionLabel;
    public Text[] missionClearedStatus;
    public Canvas creditsCanvas;
    public Canvas achievementsCanvas;
    public Canvas selectMissionCanvas;
    public Image[] achievementMedals;
    public AudioSource sfxPlayer;
    public AudioClip keystroke;

    GameObject gso;

    string messageText = "";

    private void Awake()
    {
        gso = GameObject.Find("GameStatus");
    }
    private void Start()
    {
        creditsCanvas.enabled = false;
        achievementsCanvas.enabled = false;
        selectMissionCanvas.enabled = false;

        playerData = gso.GetComponent<GameStatus>().playerData;
        playerNameField.DeactivateInputField();

        if ((playerData.playerName == "") || (playerData.playerName == "Player-1"))
        {
            messageText = "> Enter your name by pressing the <color=#00FF00FF>F1</color> key. \n>";
        }
        else
        {
            messageText = "> Welcome back, Agent " + playerData.playerName + ".\n> ";
        }

        messagePanel.text = messageText;
        UpdateMissions();
    }

    /**************************************************************************
     * * * *             LISTEN FOR MENU SELECTION INPUTS              * * * */
    void Update()
    {
        if (Keyboard.current.anyKey.wasPressedThisFrame) sfxPlayer.PlayOneShot(keystroke);

        if (Keyboard.current.f1Key.wasPressedThisFrame)
        {
            playerNameField.ActivateInputField();
            playerNameField.Select();
        }
        if (Keyboard.current.f2Key.wasPressedThisFrame) StartNewGame();

        if (Keyboard.current.f3Key.wasPressedThisFrame)
        {
            if ((playerData.playerName == "") || (playerData.playerName == "Player-1"))
            {
                messageText += "\n> <color=#FF0000FF>DATA NOT LOADED!</color>";
                messageText += "\n> To load your previous progress, press <color=#00FF00FF>F1</color> and provide your name.\n> ";
            }
            else
            {
                playerData = PlayerData.LoadPlayerData(playerData.playerName);
                messageText += "\n> <color=#00FF00FF>DATA SUCCESSFULLY LOADED.</color>\n> ";
                UpdateMissions();
            }
            messagePanel.text = messageText;
        }

        if (Keyboard.current.f4Key.wasPressedThisFrame)
        {
            if ((playerData.playerName == "") || (playerData.playerName == "Player-1"))
            {
                messageText += "\n> <color=#FF0000FF>PROGRESS NOT SAVED!</color>";
                messageText += "\n> To save your progress, press <color=#00FF00FF>F1</color> and provide your name.\n> ";
            }
            else
            {
                playerData.SaveToDisk(playerData.playerName);
                messageText += "\n> Progress saved, Agent " + playerData.playerName + ".  Excellent work.\n>";
            }
            messagePanel.text = messageText;

        }
        if (Keyboard.current.f5Key.wasPressedThisFrame) MissionSelect();
        if (Keyboard.current.f6Key.wasPressedThisFrame) ToggleAchievements();
        if (Keyboard.current.f7Key.wasPressedThisFrame) ToggleCredits();
        if (Keyboard.current.escapeKey.wasPressedThisFrame) PrepareToQuit();

        if (selectMissionCanvas.enabled)
        {
            if (Keyboard.current.digit1Key.wasPressedThisFrame) gso.GetComponent<GameStatus>().missionLevel = 1;
            if (Keyboard.current.digit2Key.wasPressedThisFrame) gso.GetComponent<GameStatus>().missionLevel = 2;
            if (Keyboard.current.digit3Key.wasPressedThisFrame) gso.GetComponent<GameStatus>().missionLevel = 3;
            if (Keyboard.current.digit4Key.wasPressedThisFrame) gso.GetComponent<GameStatus>().missionLevel = 4;
            if (Keyboard.current.digit5Key.wasPressedThisFrame) gso.GetComponent<GameStatus>().missionLevel = 5;
            if (Keyboard.current.digit6Key.wasPressedThisFrame) gso.GetComponent<GameStatus>().missionLevel = 6;
            if (Keyboard.current.digit7Key.wasPressedThisFrame) gso.GetComponent<GameStatus>().missionLevel = 7;
            if (Keyboard.current.digit8Key.wasPressedThisFrame) gso.GetComponent<GameStatus>().missionLevel = 8;
            if (Keyboard.current.digit9Key.wasPressedThisFrame) gso.GetComponent<GameStatus>().missionLevel = 9;
            if (Keyboard.current.aKey.wasPressedThisFrame) gso.GetComponent<GameStatus>().missionLevel = 10;
            if (Keyboard.current.bKey.wasPressedThisFrame) gso.GetComponent<GameStatus>().missionLevel = 11;
            if (Keyboard.current.cKey.wasPressedThisFrame) gso.GetComponent<GameStatus>().missionLevel = 12;
            if (Keyboard.current.dKey.wasPressedThisFrame) gso.GetComponent<GameStatus>().missionLevel = 13;
            if (Keyboard.current.eKey.wasPressedThisFrame) gso.GetComponent<GameStatus>().missionLevel = 14;
            if (Keyboard.current.fKey.wasPressedThisFrame) gso.GetComponent<GameStatus>().missionLevel = 15;
        }
    }

    public void SetPlayerName()
    {
        playerData.playerName = playerNameField.text;
        playerNameField.DeactivateInputField();
        messageText += "\n> Welcome Agent " + playerData.playerName + 
            ".\n>\n> Press <color=#00FF00FF>F2</color> to start from the beginning or press <color=#00FF00FF>F3</color> to resume from your previous mission.\n>";
        messagePanel.text = messageText;
    }

    void StartNewGame()
    {
        for (int i = 0; i < 15; i++)
        {
            playerData.levelStatus[i] = 0;
            if (i < 10) playerData.achievements[i] = false;
        }
        //load intro scene
    }
    public void MissionSelect()
    {
        selectMissionCanvas.enabled = !selectMissionCanvas.enabled;
        creditsCanvas.enabled = false;
        achievementsCanvas.enabled = false;
    }

    void ToggleAchievements()
    {
        achievementsCanvas.enabled = !achievementsCanvas.enabled;
        selectMissionCanvas.enabled = false;
        creditsCanvas.enabled = false;
    }

    void ToggleCredits()
    {
        creditsCanvas.enabled = !creditsCanvas.enabled;
        achievementsCanvas.enabled = false;
        selectMissionCanvas.enabled = false;
    }

    void PrepareToQuit()
    {

    }

    void UpdateMissions()
    {
        for (int i = 0; i < missionClearedStatus.Length; i++)
        {
            if (playerData.levelStatus[i] == 0)
            {
                if (i > 0)
                {
                    if (playerData.levelStatus[i-1] > 0)
                    {
                        missionLabel[i].color = Color.green;
                    }
                    else
                    {
                        missionLabel[i].color = new Color(0.0f, 0.25f, 0.0f);
                    }
                }
                missionClearedStatus[i].text = "";
            }
            else if (playerData.levelStatus[i] == 1)
            {
                missionLabel[i].color = Color.green;
                missionClearedStatus[i].color = Color.green;
                missionClearedStatus[i].text = "*";
            }
            else if (playerData.levelStatus[i] == 2)
            {
                missionLabel[i].color = Color.green;
                missionClearedStatus[i].color = Color.white;
                missionClearedStatus[i].text = "* *";
            }
            else
            {
                missionLabel[i].color = Color.yellow;
                missionClearedStatus[i].color = Color.yellow;
                missionClearedStatus[i].text = "* * *";
            }
        }
       for (int i=0; i< playerData.achievements.Length; i++)
        {
            achievementMedals[i].enabled = playerData.achievements[i];
        }
    }
    private void OnDestroy()
    {
        //If the saveOnDestroy flag is set, data will automatically be saved.  This can be
        //triggered by scene changes, application ending, or manual deletion of the GameObject
        if (playerData.saveOnDestroy)
        {
            playerData.SaveToDisk(playerData.playerName);
        }
        //Again, not limited to calling SaveToDisk here.  It can be called any time you want to
        //save data.  Remember that the Project window will have to refresh before you see your
        //save file.  This happens automatically when recompiling or importing assets, or you can
        //right click in the Project window and select refresh.
    }
}
