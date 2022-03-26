/******************************************************************************
 * MenuController.cs handles all of the logic and data management involved 
 * in the Main Menu of typehack.  This includes processing keyboard inputs
 * for entering the player's name, loading and saving player data, clearing 
 * the player data to start a new game, displaying the game credits and player
 * achievement badges, and intiating a typing mission.
 * 
 * J. Douglas Patterson
 * Johnson County Community College
 * dpatter@jccc.edu
 * 
 * v0.1 05-MAY-2021
 * 
 * ***************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    /**************************************************************************
     * * * *                    DECLARE VARIABLES                      * * * */
    public InputField playerNameField;
    public Text messagePanel;
    public Text[] missionLabel;
    public Text[] missionClearedStatus;
    public Canvas creditsCanvas;
    public Canvas achievementsCanvas;
    public Canvas selectMissionCanvas;
    public Canvas quitCanvas;
    public Image[] achievementMedals;
    public AudioSource sfxPlayer;
    public AudioClip keystroke;
    public AudioClip buzz;

    string messageText = "";
    GameStatus gso;

    /**************************************************************************
    * * * *                INITIALIZE INTERNAL VARIABLES               * * * */
    private void Awake()
    {
        //Hide and lock the mouse cursor
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        //Start with all specialty canvases hidden
        achievementsCanvas.enabled = false;
        creditsCanvas.enabled = false;
        selectMissionCanvas.enabled = false;
        quitCanvas.enabled = false;

        //Make sure the input field for the player's name is not active from the start
        playerNameField.DeactivateInputField();
    }

    /**************************************************************************
    * * * *     INITIALIZE VARIABLES AND DEFINE INITIAL GAME STATE     * * * */
    private void Start()
    {
        gso = GameObject.Find("GameStatus").GetComponent<GameStatus>();
        if (gso == null) Debug.Log("GameStatus object not found");

        //Check to see if they player has already registered their name.  If not, ask for their name.
        if ((gso.playerData.playerName == "") || (gso.playerData.playerName == "Player-1"))
        {
            messageText = "> Enter your name by pressing the <color=#00FF00FF>F1</color> key. \n>";
        }
        else
        {
            messageText = "> Welcome back, Agent " + gso.playerData.playerName + ".\n> ";
        }

        //Update the text in the message panel and the mission status display.
        messagePanel.text = messageText;
        UpdateMissions();
    }

    /**************************************************************************
     * * * *             LISTEN FOR MENU SELECTION INPUTS              * * * */
    void Update()
    {
        //Play a "bomp" sound for every keystroke
        if (Keyboard.current.anyKey.wasPressedThisFrame) sfxPlayer.PlayOneShot(keystroke);

        //Activate and select the input field so the player may enter their name
        if (Keyboard.current.f1Key.wasPressedThisFrame)
        {
            playerNameField.ActivateInputField();
            playerNameField.Select();
        }

        //Clear all player achievements and mission progress
        if (Keyboard.current.f2Key.wasPressedThisFrame) StartNewGame();

        //Load in the save data from disk and update the mission status display if and only if they have provided a name.
        if (Keyboard.current.f3Key.wasPressedThisFrame)
        {
            if ((gso.playerData.playerName == "") || (gso.playerData.playerName == "Player-1"))
            {
                messageText += "\n> <color=#FF0000FF>DATA NOT LOADED!</color>";
                messageText += "\n> To load your previous progress, press <color=#00FF00FF>F1</color> and provide your name.\n> ";
            }
            else
            {
                gso.playerData = PlayerData.LoadPlayerData(gso.playerData.playerName);
                messageText += "\n> <color=#00FF00FF>DATA SUCCESSFULLY LOADED.</color>\n> ";
                UpdateMissions();
            }
            messagePanel.text = messageText;
        }

        //Save the player data to the disk if and only if they have provided a name.
        if (Keyboard.current.f4Key.wasPressedThisFrame)
        {
            if ((gso.playerData.playerName == "") || (gso.playerData.playerName == "Player-1"))
            {
                messageText += "\n> <color=#FF0000FF>PROGRESS NOT SAVED!</color>";
                messageText += "\n> To save your progress, press <color=#00FF00FF>F1</color> and provide your name.\n> ";
            }
            else
            {
                gso.playerData.SaveToDisk(gso.playerData.playerName);
                messageText += "\n> Progress saved, Agent " + gso.playerData.playerName + ".  Excellent work.\n>";
            }
            messagePanel.text = messageText;
        }

        if (Keyboard.current.f5Key.wasPressedThisFrame) MissionSelect();            //Toggle the visibility of the mission select canvas
        if (Keyboard.current.f6Key.wasPressedThisFrame) ToggleAchievements();       //Toggle the visibility of the achievements canvas
        if (Keyboard.current.f7Key.wasPressedThisFrame) ToggleCredits();            //Toggle the visibility of the credits canvas
        if (Keyboard.current.escapeKey.wasPressedThisFrame) ToggleQuit();           //Toggle the visibility of the confirm quit canvas

        //Mute button
        if (Keyboard.current.f12Key.wasPressedThisFrame)
        {
            AudioSource musicPlayer = GameObject.Find("MusicPlayer").GetComponent<AudioSource>();
            if (musicPlayer.volume < 0.5f)
            {
                musicPlayer.volume = 1.0f;
            }
            else
            {
                musicPlayer.volume = 0.0f;
            }
        }

        //If the mission canvas is visible, allow the player to use the number keys to select a mission to begin, but
        //do not allow the player to start a mission if they have not successfully completed the previous mission.
        if (selectMissionCanvas.enabled)
        {
            if (Keyboard.current.digit1Key.wasPressedThisFrame)
            {
                gso.missionLevel = 1;
                gso.StartMission(1);
            }
            if (Keyboard.current.digit2Key.wasPressedThisFrame)
            {
                if (gso.playerData.levelStatus[0] > 0)
                {
                    gso.missionLevel = 2;
                    gso.StartMission(2);
                }
                else
                {
                    sfxPlayer.PlayOneShot(buzz);
                }
            }
            if (Keyboard.current.digit3Key.wasPressedThisFrame)
            {
                if (gso.playerData.levelStatus[1] > 0)
                {
                    gso.missionLevel = 3;
                    gso.StartMission(3);
                }
                else
                {
                    sfxPlayer.PlayOneShot(buzz);
                }
            }
            if (Keyboard.current.digit4Key.wasPressedThisFrame)
            {
                if (gso.playerData.levelStatus[2] > 0)
                {
                    gso.missionLevel = 4;
                    gso.StartMission(4);
                }
                else
                {
                    sfxPlayer.PlayOneShot(buzz);
                }
            }
            if (Keyboard.current.digit5Key.wasPressedThisFrame)
            {
                if (gso.playerData.levelStatus[3] > 0)
                {
                    gso.missionLevel = 5;
                    gso.StartMission(5);
                }
                else
                {
                    sfxPlayer.PlayOneShot(buzz);
                }
            }
            if (Keyboard.current.digit6Key.wasPressedThisFrame)
            {
                if (gso.playerData.levelStatus[4] > 0)
                {
                    gso.missionLevel = 6;
                    gso.StartMission(6);
                }
                else
                {
                    sfxPlayer.PlayOneShot(buzz);
                }
            }
            if (Keyboard.current.digit7Key.wasPressedThisFrame)
            {
                if (gso.playerData.levelStatus[5] > 0)
                {
                    gso.missionLevel = 7;
                    gso.StartMission(7);
                }
                else
                {
                    sfxPlayer.PlayOneShot(buzz);
                }
            }
            if (Keyboard.current.digit8Key.wasPressedThisFrame)
            {
                if (gso.playerData.levelStatus[6] > 0)
                {
                    gso.missionLevel = 8;
                    gso.StartMission(8);
                }
                else
                {
                    sfxPlayer.PlayOneShot(buzz);
                }
            }
            if (Keyboard.current.digit9Key.wasPressedThisFrame)
            {
                if (gso.playerData.levelStatus[7] > 0)
                {
                    gso.missionLevel = 9;
                    gso.StartMission(9);
                }
                else
                {
                    sfxPlayer.PlayOneShot(buzz);
                }
            }
            if (Keyboard.current.aKey.wasPressedThisFrame)
            {
                if (gso.playerData.levelStatus[8] > 0)
                {
                    gso.missionLevel = 10;
                    gso.StartMission(10);
                }
                else
                {
                    sfxPlayer.PlayOneShot(buzz);
                }
            }
            if (Keyboard.current.bKey.wasPressedThisFrame)
            {
                if (gso.playerData.levelStatus[9] > 0)
                {
                    gso.missionLevel = 11;
                    gso.StartMission(11);
                }
                else
                {
                    sfxPlayer.PlayOneShot(buzz);
                }
            }
            if (Keyboard.current.cKey.wasPressedThisFrame)
            {
                if (gso.playerData.levelStatus[10] > 0)
                {
                    gso.missionLevel = 12;
                    gso.StartMission(12);
                }
                else
                {
                    sfxPlayer.PlayOneShot(buzz);
                }
            }
            if (Keyboard.current.dKey.wasPressedThisFrame)
            {
                if (gso.playerData.levelStatus[11] > 0)
                {
                    gso.missionLevel = 13;
                    gso.StartMission(13);
                }
                else
                {
                    sfxPlayer.PlayOneShot(buzz);
                }
            }
            if (Keyboard.current.eKey.wasPressedThisFrame)
            {
                if (gso.playerData.levelStatus[12] > 0)
                {
                    gso.missionLevel = 14;
                    gso.StartMission(14);
                }
                else
                {
                    sfxPlayer.PlayOneShot(buzz);
                }
            }
            if (Keyboard.current.fKey.wasPressedThisFrame)
            {
                if (gso.playerData.levelStatus[13] > 0)
                {
                    gso.missionLevel = 15;
                    gso.StartMission(15);
                }
                else
                {
                    sfxPlayer.PlayOneShot(buzz);
                }
            }
        }

        //If the quit canvas is enabled, ask the player to confirm that they wish to quit the game
        if (quitCanvas.enabled)
        {
            if (Keyboard.current.yKey.wasPressedThisFrame) Application.Quit();
            if (Keyboard.current.nKey.wasPressedThisFrame) ToggleQuit();
        }
    }

    /**************************************************************************
     * * * *                 MAIN MENU PUBLIC METHODS                  * * * */
    public void SetPlayerName()
    {
        if (gso.playerData == null) Debug.Log("The Player Data Object is not defined in SetPlayerName.");

        gso.playerData.playerName = playerNameField.text;
        playerNameField.DeactivateInputField();
        messageText += "\n> Welcome Agent " + gso.playerData.playerName +
            ".\n>\n> Press <color=#00FF00FF>F2</color> to start from the beginning or press <color=#00FF00FF>F3</color> to resume from your previous mission.\n>";
        messagePanel.text = messageText;
        //gso.playerData = PlayerData.LoadPlayerData(gso.playerData.playerName);
        UpdateMissions();
    }

    void StartNewGame()
    {
        for (int i = 0; i < 15; i++)
        {
            gso.playerData.levelStatus[i] = 0;
            if (i < 10) gso.playerData.achievements[i] = false;
        }
        gso.playerData.SaveToDisk(gso.playerData.playerName);
        UpdateMissions();
    }

    /**************************************************************************
    * * * *                  TOGGLE CANVAS VISIBILITY                  * * * */
    private void MissionSelect()
    {
        selectMissionCanvas.enabled = !selectMissionCanvas.enabled;
        achievementsCanvas.enabled = false;
        creditsCanvas.enabled = false;
        quitCanvas.enabled = false;
    }

    private void ToggleAchievements()
    {
        achievementsCanvas.enabled = !achievementsCanvas.enabled;
        creditsCanvas.enabled = false;
        quitCanvas.enabled = false;
        selectMissionCanvas.enabled = false;
    }

    private void ToggleCredits()
    {
        creditsCanvas.enabled = !creditsCanvas.enabled;
        achievementsCanvas.enabled = false;
        selectMissionCanvas.enabled = false;
        quitCanvas.enabled = false;
    }

    private void ToggleQuit()
    {
        quitCanvas.enabled = !quitCanvas.enabled;
        achievementsCanvas.enabled = false;
        creditsCanvas.enabled = false;
        selectMissionCanvas.enabled = false;
    }

    /**************************************************************************
    * * * *   UPDATE THE MISSION STATUS INDICATORS ON THE MAIN MENU    * * * */
    private void UpdateMissions()
    {
        for (int i = 0; i < missionClearedStatus.Length; i++)
        {
            if (gso == null) Debug.Log("The GameStatus Object is not defined.");
            if (gso == null) Debug.Log("The GameStatus Object is not defined.");
            if (gso.playerData == null) Debug.Log("The Player Data Object is not defined in UpdateMissions.");
            if (gso.playerData.levelStatus[i] == 0)
            {
                if (i > 0)
                {
                    if (gso.playerData.levelStatus[i-1] > 0)
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
            else if (gso.playerData.levelStatus[i] == 1)
            {
                missionLabel[i].color = Color.green;
                missionClearedStatus[i].color = Color.green;
                missionClearedStatus[i].text = "*";
            }
            else if (gso.playerData.levelStatus[i] == 2)
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
       for (int i=0; i< gso.playerData.achievements.Length; i++)
        {
            achievementMedals[i].enabled = gso.playerData.achievements[i];
        }
    }

    /**************************************************************************
    * * * *         SAVE THE PLAYER DATA BEFORE THE GAME QUITS         * * * */
    private void OnDestroy()
    {
        if (gso.playerData.saveOnDestroy)
        {
            gso.playerData.SaveToDisk(gso.playerData.playerName);
        }
    }
}
