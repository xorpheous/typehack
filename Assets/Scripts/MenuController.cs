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
    public PlayerData playerData;

    string messageText = "";

    //Using Awake since Awake is for initialization, and is called before start.
    private void Awake()
    {
        PlayerData tempData = PlayerData.LoadPlayerData(playerData.playerName);

        //If the load failed, then create a new instance of SaveData
        if(tempData == null)
        {
            playerData = new PlayerData();
        }
        //If the load suceeded, then use the loaded file
        else
        {
            playerData = tempData;
        }

        playerNameField.DeactivateInputField();

        messageText = "> Enter your name by pressing the <color=#00FF00FF>F1</color> key. \n>";
        messagePanel.text = messageText;
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
        if (Keyboard.current.f3Key.wasPressedThisFrame) playerData = PlayerData.LoadPlayerData(playerData.playerName);
        if (Keyboard.current.f4Key.wasPressedThisFrame) playerData.SaveToDisk(playerData.playerName);
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
