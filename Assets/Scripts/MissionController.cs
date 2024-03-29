/******************************************************************************
 * TextInputHandler.cs manages the presented keywords and the typed input from 
 * the player.  It also keeps track of the error rate, average typing speed, 
 * and the current correct work streak for the player.
 * 
 * J. Douglas Patterson, 15-MAR-2021
 * Johnson County Community College
 * dpatter@jccc.edu
 * 
 * (Adapted from demo code by Richard Flemming, rfleming1@jccc.edu)
 * ***************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class MissionController : MonoBehaviour
{
    /**************************************************************************
     * * *                    VARIABLE DECLARATIONS                      * * */

    //Audio players, sound fx clips, and music tracks
    public AudioSource sfxPlayer;
    public AudioSource musicPlayer;
    public AudioClip keystrokeBlip;
    public AudioClip buzzer;
    public AudioClip chime;
    public AudioClip missionTrack;
    public AudioClip menuTrack;

    //Parameter display fields
    public Text keywordTextField;           //Display field for the keywords and phrases the player needs to type
    public Text wordStreakField;            //Display field for the current count of consecutively correctly typed words
    public Text errorCountField;            //Display field for the total errors made during the current mission
    public Text errorRateField;             //Display field for the percentage of incorrect keystrokes
    public Text avgSpeedField;              //Display field for the average typing speed during the current mission
    public Text terminalField;              //Display field for the center display terminal
    public Text terminalMissionObj;         //Display field for the mission objectives during active mission
    public Text briefingMissionObj;         //Display field for the mission objectives during the mission briefing
    public Text briefingDialogField;        //Display field for the mission briefing dialog

    public Image timeRemainingBar;          //Fill bar indicating the amount of time remaining to complete the mission.

    public Canvas missionBriefing;          //Canvas displaying the mission briefing imformation

    //Mission keyword display parameters and message text
    string originalKeyword;                 //Current unformatted keyword
    string displayedKeyword;                //Current keyword displayed with correct colour formatting
    string terminalText;                    //Text to be displayed on the central display terminal

    //Mission Status Parameters
    GameStatus gso;                         //Game Status object that holds persistent data and current game state

    bool briefingActive = true;             //Flag indicating whether the briefing is active or not
    bool missionActive = false;             //Flag indicating whether the mission is active or not

    float missionAlottedTime = 60.0f;       //Time alotted to complete the current mission (seconds)
    float missionRemainingTime = 0.0f;      //Time remaining to complete the current mission (seconds)
    float avgSpeed = 0.0f;                  //Average typing speed for the current mission (wpm, 5 characters = 1 word)
    float errorRate = 0.0f;                 //Percentage of erroneous key presses for the current mission

    int keywordsCompleted;                  //Number of keywords completed
    int wordStreak = 0;                     //Number of consecutively correctly typed words
    int numErrors = 0;                      //Total number of errors for the mission
    int numChars = 0;                       //Total number of characters typed for the mission including erroneous characters

    //Current Keyword Paramters
    public int charIndex = 0;               //Current character index to be typed by the player (leftmost character = 0)
    public int keywordIndex = 0;            //List index for the current keyword to be typed by the player

    /**************************************************************************
     * * *                INITIALIZE MISSION PARAMETERS                  * * */
    void Start()
    {
        //Find and assign the GameStatus game object for tracking and updating persistent data
        gso = GameObject.Find("GameStatus").GetComponent<GameStatus>();

        //Get keyword list and pangram for the current level
        gso.GetMissionKeywords(gso.missionLevel);

        //Enable the keyboard input listener
        Keyboard.current.onTextInput += OnTextInput;

        //Load the first keyword and set the text colour to amber
        keywordIndex = 0;
        //keywordIndex = Random.Range(0,gso.keywords.Count);
        originalKeyword = gso.keywords[keywordIndex];
        //gso.keywords.RemoveAt(keywordIndex);
        keywordTextField.color = new Color(1.0f, 0.7529412f, 0.0f);

        //Initialize the terminal text with player instructions
        terminalText = "> Enter the KEYWORDS shown below as they appear to counteract the cyberthreat.\n>\n> ";
        terminalField.text = terminalText;

        //Intiialize the number of keywords completed
        keywordsCompleted = 0;

        //Initialize the error count
        numErrors = 0;
        errorCountField.text = numErrors.ToString();

        //Initialize the number of consecutive correct words and the number of characters typed
        wordStreak = 0;
        numChars = 0;

        //Initialize the typing error rate
        errorRate = 0.0f;
        errorRateField.text = errorRate.ToString("00.0");

        //Prepare the mission timer
        missionRemainingTime = missionAlottedTime;

        //Play the mission music
        musicPlayer.clip = missionTrack;
        musicPlayer.loop = true;
        musicPlayer.Play();

        //Load mission objectives and briefing text into the appropriate text fields
        terminalMissionObj.text = gso.missionObjectives[gso.missionLevel - 1];
        briefingMissionObj.text = gso.missionObjectives[gso.missionLevel - 1];
        briefingDialogField.text = gso.missionBriefing[gso.missionLevel - 1];

        //Begin with the mission briefing
        BeginBriefing();
    }

    /**************************************************************************
     * * *              UPDATE MISSION STATUS EVERY FRAME                * * */
    void Update()
    {
        //Always allow ESC to return the player to the main menu.
        if (Keyboard.current.escapeKey.wasPressedThisFrame) SceneManager.LoadScene("MainMenu");

        //Mute button
        if (Keyboard.current.f12Key.wasPressedThisFrame) ToggleMute();

        //Update the mission status only while the mission is active
        if ((missionActive) && (!briefingActive))
        {
            //Increment the mission elapsed time and update the time remaining bar
            missionRemainingTime -= Time.deltaTime;
            timeRemainingBar.fillAmount = Mathf.Clamp(missionRemainingTime / missionAlottedTime, 0.0f, 1.0f);

            //If the alotted mission time has passed, then fail the mission.
            if (missionRemainingTime < 0.001f) MissionFailed();

            //Split the keyword into completed (left) and remaining (right) characters.
            string left = originalKeyword.Substring(0, charIndex);
            string right = originalKeyword.Substring(charIndex, originalKeyword.Length - charIndex);

            //Colour the completed characters green.
            displayedKeyword = string.Format("<color=#00FF00FF>{0}</color>{1}", left, right);
            keywordTextField.text = displayedKeyword;

            //Update the consecutively correct word count
            wordStreakField.text = wordStreak.ToString();

            //Calculate and display the current typing speed in words per minute (1 word = 5 characters)
            avgSpeed = 12.0f * (float)(numChars - numErrors) / (missionRemainingTime);
            avgSpeedField.text = avgSpeed.ToString("000");
        }
        else if (!briefingActive)
        {
            //If the mission is over, listen for the F1 key to restart
            if (Keyboard.current.f1Key.wasPressedThisFrame)
            {
                Debug.Log("F1 key was pressed");
                SceneManager.LoadScene("TerminalScreen");
            }
        }
        else
        {
            //If the mission is just starting, listen for the F1 key to end the briefing and start the mission
            if (Keyboard.current.f1Key.wasPressedThisFrame)
            {
                Debug.Log("F1 key was pressed during briefing");
                EndBriefing();
            }
        }
    }

    private void BeginBriefing()
    {
        briefingActive = true;
        missionActive = false;
        missionBriefing.enabled = true;
        //do whatever else needs to happen in the future here (music and whatnot)
    }

    private void EndBriefing()
    {
        briefingActive = false;
        missionBriefing.enabled = false;
        missionActive = true;
        //do whatever else needs to happen in the future here (music and whatnot)
    }

    public void ToggleMute()
    {
        AudioSource musicPlayer = GameObject.Find("Music_Player").GetComponent<AudioSource>();
        if (musicPlayer.volume < 0.5f)
        {
            musicPlayer.volume = 1.0f;
        }
        else
        {
            musicPlayer.volume = 0.0f;
        }
    }

    /**************************************************************************
     * * *                    EVALUATE PLAYER INPUT                      * * */
    private void OnTextInput(char obj)
    {
        //Only evaluate player input while the mission is active
        if (missionActive)
        {
            //Increment the total number of characters entered
            numChars += 1;

            //Check to see if the entered character matches the next character of the current keyword
            if (obj == originalKeyword[charIndex])
            {
                //Play keystroke sound
                sfxPlayer.PlayOneShot(keystrokeBlip);

                //Advance the character index to the next character of the current keyword
                charIndex += 1;

                //If that was the last character of the word, advance to the next keyword
                if (charIndex == originalKeyword.Length)
                {
                    //Play success chime
                    sfxPlayer.PlayOneShot(chime);

                    //Display the completed word on the terminal display
                    terminalText += gso.keywords[keywordIndex] + "\n> "; 
                    terminalField.text = terminalText;

                    //Reset the character index to point at the first character of the next word
                    charIndex = 0;

                    //Increment and display the current streak of correctly typed words
                    wordStreak += 1;
                    wordStreakField.text = wordStreak.ToString();

                    //Advance the keywords completed.
                    keywordsCompleted += 1;

                    //Increment the keyword index
                    keywordIndex++;
                    //keywordIndex = Random.Range(0, gso.keywords.Count);
                    //originalKeyword = gso.keywords[keywordIndex];
                    //gso.keywords.RemoveAt(keywordIndex);

                    //If the player has completed the last keyword or phrase, end the mission as a success
                    if (keywordIndex == gso.keywords.Count) MissionComplete();
                    //if (keywordsCompleted == 10) MissionComplete();

                    //Load the next keyword or phrase
                    originalKeyword = gso.keywords[keywordIndex];
                }
            }
            else
            {
                //Play buzzer sound
                sfxPlayer.PlayOneShot(buzzer);

                //Reset the streak of correctly typed words and go back to the beginning character of the current word
                wordStreak = 0;
                charIndex = 0;

                //Increment and display the total number of errors for this mission
                numErrors += 1;
                errorCountField.text = numErrors.ToString();
                                
                if (numErrors == 4) keywordTextField.color = Color.yellow;      //If there are four errors, warn the player with bright yellow text
                if (numErrors == 5) MissionFailed();                            //If five errors are made, fail the mission
            }

            //Calculate and display the error rate
            errorRate = 100.0f * ((float)numErrors / (float)(numChars + numErrors));
            errorRateField.text = errorRate.ToString("00.0");
        }
    }

    /**************************************************************************
     * * *               HANDLE MISSION FAILURE CONDITION                * * */
    private void MissionFailed()
    {
        //Deactivate the mission
        missionActive = false;

        //Switch to menu music
        musicPlayer.Stop();
        musicPlayer.clip = menuTrack;
        musicPlayer.Play();

        //Display the mission failed message in the keyword display field
        keywordIndex = 0;
        keywordTextField.color = Color.red;
        keywordTextField.text = "MISSION FAILED";

        //Display the mission failed message in the terminal display and invite the player to try again
        terminalText += "\n> MISSION FAILED.\n> Press [F1] to try again.\n> ";
        terminalField.text = terminalText;
    }

    /**************************************************************************
     * * *               HANDLE MISSION COMPLETE CONDITION               * * */
    private void MissionComplete()
    {
        string starRating = "";
        string achievementNotices = "";

        //Deactivate the mission
        missionActive = false;

        //Switch to menu music
        musicPlayer.Stop();
        musicPlayer.clip = menuTrack;
        musicPlayer.Play();

        //Determine mission star rating.
        if ((missionRemainingTime > 0.5f * missionAlottedTime) && (numErrors == 0))
        {
            gso.playerData.levelStatus[gso.missionLevel - 1] = 3;
            starRating = "<color=#FFFF00FF>* * *</color>  GREAT JOB!";
        }
        else if (missionRemainingTime > 0.25f * missionAlottedTime)
        {
            gso.playerData.levelStatus[gso.missionLevel - 1] = 2;
            starRating = "<color=#FFFFFFFF>* *</color>  Nicely done!";
        }
        else
        {
            gso.playerData.levelStatus[gso.missionLevel - 1] = 1;
            starRating = "* You're getting there, rookie!";
        }

        /*** Determine if any new achievements were earned ***/
        //Count number of three-star ratings
        int threeStarMissions = 0;
        for (int i = 0; i < 15; i++)
        {
            if (gso.playerData.levelStatus[i] > 2) threeStarMissions += 1;
        }

        //Completed first level
        if (!gso.playerData.achievements[0])
        {
            gso.playerData.achievements[0] = true;
            achievementNotices += "\n> First assignent complete!";
        }

        //First two-star level
        if ((!gso.playerData.achievements[1]) && (gso.playerData.levelStatus[gso.missionLevel - 1] > 1))
        {
            gso.playerData.achievements[1] = true;
            achievementNotices += "\n> First two-star rating!";
        }

        //First three-star level
        if ((!gso.playerData.achievements[2]) && (gso.playerData.levelStatus[gso.missionLevel - 1] > 2))
        {
            gso.playerData.achievements[2] = true;
            achievementNotices += "\n> First three-star rating!";
        }

        //Three three-star levels in a row
        if ((!gso.playerData.achievements[3]) && (gso.missionLevel > 2)
            && (gso.playerData.levelStatus[gso.missionLevel - 1] > 2)
            && (gso.playerData.levelStatus[gso.missionLevel - 2] > 2)
            && (gso.playerData.levelStatus[gso.missionLevel - 3] > 2))
        {
            gso.playerData.achievements[3] = true;
            achievementNotices += "\n> Three three-star ratings in a row!";
        }

        //Completed both left and right hand only levels with three stars
        if ((!gso.playerData.achievements[4]) && (gso.playerData.levelStatus[5] > 2) && (gso.playerData.levelStatus[6] > 2))
        {
            gso.playerData.achievements[4] = true;
            achievementNotices += "\n> Ambidexterous performance!";
        }

        //Completed pangram level with three-stars
        if ((!gso.playerData.achievements[5]) && (gso.missionLevel == 8) && (gso.playerData.levelStatus[7] > 2))
        {
            gso.playerData.achievements[5] = true;
            achievementNotices += "\n> Master of all letters!";
        }

        //Error-free performance
        if (!gso.playerData.achievements[6] && (numErrors == 0))
        {
            gso.playerData.achievements[6] = true;
            achievementNotices += "\n> An error-free performance!";
        }

        //Speed demon, 50 WPM average
        if (!gso.playerData.achievements[7] && (avgSpeed > 50.0f))
        {
            gso.playerData.achievements[7] = true;
            achievementNotices += "\n> You're a speed demon!  Over 50 WPM!";
        }

        //Completed fifteen missions
        if (!gso.playerData.achievements[8] && (gso.missionLevel > 9))
        {
            gso.playerData.achievements[8] = true;
            achievementNotices += "\n> Congratulations, Secret Agent" + gso.playerData.playerName + ".\n> You are now Licensed to Type!";
        }

        //Earned three-star ratings on all missions
        if (!gso.playerData.achievements[9] && (threeStarMissions > 14))
        {
            gso.playerData.achievements[9] = true;
            achievementNotices += "\n> LEGENDARY!  Three-star ratings on ALL missions!";
        }

        //Display the mission success message in the keyword display field
        keywordIndex = 0;
        keywordTextField.color = Color.green;
        keywordTextField.text = "MISSION SUCCESS";

        //Display the mission success message in the terminal display and invite the player to try again
        terminalText += "\n> MISSION SUCCESS!";
        terminalText += "\n> Rating: " + starRating + "\n> ";
        terminalText += achievementNotices + "\n> ";
        terminalText += "\n> Press [F1] to try again.\n> Press [ESC] to return to the main menu.\n>";
        terminalField.text = terminalText;

        //Save the player's progress
        gso.playerData.SaveToDisk(gso.playerData.playerName);
    }
}
