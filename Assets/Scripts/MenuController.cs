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

    GameObject gso;

    string messageText = "";

    private void Awake()
    {
        gso = GameObject.Find("GameStatus");
    }
    private void Start()
    {
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
                UpdateMissions();
            }
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
        if (Keyboard.current.f5Key.wasPressedThisFrame) ToggleAchievements();
        if (Keyboard.current.f6Key.wasPressedThisFrame) ToggleCredits();
        if (Keyboard.current.escapeKey.wasPressedThisFrame) PrepareToQuit();
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

    void ToggleAchievements()
    {

    }

    void ToggleCredits()
    {

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
