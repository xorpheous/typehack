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

    public Image timeRemainingBar;          //Fill bar indicating the amount of time remaining to complete the mission.

    //Mission keyword display parameters and message text
    string originalKeyword;                 //Current unformatted keyword
    string displayedKeyword;                //Current keyword displayed with correct colour formatting
    string terminalText;                    //Text to be displayed on the central display terminal

    //Mission Status Parameters
    GameStatus gso;                         //Game Status object that holds persistent data and current game state

    bool missionActive = false;             //Flag indicating whether the mission is active or not

    float missionAlottedTime = 60.0f;       //Time alotted to complete the current mission (seconds)
    float missionRemainingTime = 0.0f;      //Time remaining to complete the current mission (seconds)
    float avgSpeed = 0.0f;                  //Average typing speed for the current mission (wpm, 5 characters = 1 word)
    float errorRate = 0.0f;                 //Percentage of erroneous key presses for the current mission

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

        //Get keyword list and panagram for the current level
        gso.GetMissionKeywords(gso.missionLevel);

        //Enable the keyboard input listener
        Keyboard.current.onTextInput += OnTextInput;

        //Load the first keyword and set the text colour to amber
        keywordIndex = 0;
        originalKeyword = gso.keywords[keywordIndex];
        keywordTextField.color = new Color(1.0f, 0.7529412f, 0.0f);

        //Initialize the terminal text with player instructions
        terminalText = "> Enter the KEYWORDS shown below as they appear to counteract the cyberthreat.\n>\n> ";
        terminalField.text = terminalText;

        //Initialize the error count
        numErrors = 0;
        errorCountField.text = numErrors.ToString();

        //Initialize the number of consecutive correct words and the number of characters typed
        wordStreak = 0;
        numChars = 0;

        //Initialize the typing error rate
        errorRate = 0.0f;
        errorRateField.text = errorRate.ToString("00.0");

        //Begin the mission timer
        missionRemainingTime = missionAlottedTime;
        missionActive = true;

        //Play the mission music
        musicPlayer.clip = missionTrack;
        musicPlayer.loop = true;
        musicPlayer.Play();
    }

    /**************************************************************************
     * * *              UPDATE MISSION STATUS EVERY FRAME                * * */
    void Update()
    {
        //Update the mission status only while the mission is active
        if (missionActive)
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
        else
        {
            //If the mission is over, listen for the F1 key to restart
            if (Keyboard.current.f1Key.wasPressedThisFrame)
            {
                Debug.Log("F1 key was pressed");
                SceneManager.LoadScene("TerminalScreen");
            }
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

                    //Advance the keyword index to point to the next word or phrase.
                    keywordIndex += 1;

                    //If the player has completed the last keyword or phrase, end the mission as a success
                    if (keywordIndex == gso.keywords.Count) MissionComplete();
                    
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
        //Deactivate the mission
        missionActive = false;

        //Switch to menu music
        musicPlayer.Stop();
        musicPlayer.clip = menuTrack;
        musicPlayer.Play();

        //Display the mission success message in the keyword display field
        keywordIndex = 0;
        keywordTextField.color = Color.green;
        keywordTextField.text = "MISSION SUCCESS";

        //Display the mission success message in the terminal display and invite the player to try again
        terminalText += "\n> MISSION SUCCESS.\n> Press [F1] to try again.\n> ";
        terminalField.text = terminalText;

        //Determine mission star rating.
        if ((missionRemainingTime > 0.5f * missionAlottedTime) && (numErrors == 0))
        {
            gso.playerData.levelStatus[gso.missionLevel - 1] = 3;
        }
        else if (missionRemainingTime > 0.25f * missionAlottedTime)
        {
            gso.playerData.levelStatus[gso.missionLevel - 1] = 2;
        }
        else
        {
            gso.playerData.levelStatus[gso.missionLevel - 1] = 1;
        }

        //Determine if any new achievements were earned
        if (!gso.playerData.achievements[0]) gso.playerData.achievements[0] = true;
        if ((!gso.playerData.achievements[1]) && (gso.playerData.levelStatus[gso.missionLevel - 1] > 1)) gso.playerData.achievements[1] = true;
        if ((!gso.playerData.achievements[2]) && (gso.playerData.levelStatus[gso.missionLevel - 1] > 2)) gso.playerData.achievements[2] = true;
        if ((!gso.playerData.achievements[3]) && (gso.missionLevel > 2)
            && (gso.playerData.levelStatus[gso.missionLevel - 1] > 2)
            && (gso.playerData.levelStatus[gso.missionLevel - 2] > 2)
            && (gso.playerData.levelStatus[gso.missionLevel - 3] > 2)) gso.playerData.achievements[3] = true;
        if ((!gso.playerData.achievements[4]) && (gso.missionLevel > 4)
            && (gso.playerData.levelStatus[gso.missionLevel - 1] > 2)
            && (gso.playerData.levelStatus[gso.missionLevel - 2] > 2)
            && (gso.playerData.levelStatus[gso.missionLevel - 3] > 2)
            && (gso.playerData.levelStatus[gso.missionLevel - 4] > 2)
            && (gso.playerData.levelStatus[gso.missionLevel - 5] > 2)) gso.playerData.achievements[4] = true;

        //Save the player's progress
        gso.playerData.SaveToDisk(gso.playerData.playerName);

        //load next mission briefing
    }
}
