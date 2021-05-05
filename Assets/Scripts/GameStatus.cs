/******************************************************************************
 * GameStatus stores and manages key mission parameters.  The object to which 
 * this is attached should also have the DoNotDestroy.cs script attached so 
 * that the data contained in this class persists between scene changes.
 * 
 * J. Douglas Patterson
 * Johnson County Community College
 * dpatter@jccc.edu
 * 
 * v1.0, 22-APR-2021
 * 
 * ***************************************************************************/
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameStatus : MonoBehaviour
{
    /**************************************************************************
     * * * *                   VARIABLE DECLARATIONS                   * * * */
    public PlayerData playerData;                           //Class holding all of the player progression data
    public int missionLevel = 0;                            //Current mission level selected

    string[] keywordFilenames = new string[15];             //Array of keyword filenames indexed by mission level
    string keywordFilePath = "";                            //Full path to the folder holding the mission keyword lists

    public string currentKeyword;                           //Current keyword presented to the player to type
    public string currentPangrams;                          //Current mission-ending pangram
    public List<string> keywords = new List<string>();      //List of keywords selected for the current mission
    public string[] missionPangrams = new string[15];       //Mission pangrams indexed by mission level

    public string[] missionObjectives = new string[15];     //Mission objectives indexed by mission level
    public string[] missionBriefing = new string[15];       //Mission briefing text indexed by mission level

    /**************************************************************************
     * Before we do anything else, load in the saved player data.  If no save
     * file exist, create a new set of player data.                          */
    private void Awake()
    {
        PlayerData tempData = PlayerData.LoadPlayerData(playerData.playerName);

        //If the load failed, then create a new instance of PlayerData
        if (tempData == null)
        {
            playerData = new PlayerData();
        }
        //If the load suceeded, then use the loaded file
        else
        {
            playerData = tempData;
        }
    }

    /**************************************************************************
     * * * *                   INITIALIZE VARIABLES                    * * * */
    private void Start()
    {
        //Define Keyword Path and Filename
        keywordFilePath = Application.streamingAssetsPath + "/WordLists/";
        keywordFilenames[0] = "homerow_words.txt";
        keywordFilenames[1] = "homerow_words_plusE.txt";
        keywordFilenames[2] = "homerow_words_plusI.txt";
        keywordFilenames[3] = "homerow_words_plusEI.txt";
        keywordFilenames[4] = "homerow_words_plusVN.txt";
        keywordFilenames[5] = "all_left.txt";
        keywordFilenames[6] = "all_right.txt";
        keywordFilenames[7] = "pangrams.txt";           //pangrams
        keywordFilenames[8] = "keyboard_numbers.txt";   //keyboard numbers
        keywordFilenames[9] = "alphanumeric.txt";       //alphanumeric
        keywordFilenames[10] = "addresses.txt";         //addresses
        keywordFilenames[11] = "punctuation.txt";       //punctuation
        keywordFilenames[12] = "science_jargon.txt";    //jargon
        keywordFilenames[13] = "dickinson.txt";         //poetry
        keywordFilenames[14] = "linesofcode.txt";       //code

        //Mission Objectives
        missionObjectives[0] = "Mission 1 Objectives:\n\nMaster the home row.\n\n Left:  A S D F G\n Right: H J K L";
        missionObjectives[1] = "Mission 2 Objectives:\n\nHome Row + E\n\n Left:  ASFG E\n Right: HJKL";
        missionObjectives[2] = "Mission 3 Objectives:\n\nHome Row + I\n\n Left:  ASDFG\n Right: HJKL I";
        missionObjectives[3] = "Mission 4 Objectives:\n\nHome Row + E & I\n\n Left:  ASDFGE\n Right: HJKLI";
        missionObjectives[4] = "Mission 5 Objectives:\n\nHome Row + V & N\n\n Left:  ASDFG V\n Right: HJKL N";
        missionObjectives[5] = "Mission 6 Objectives:\n\nLeft Hand Only!\n\n Q W E R T\n A S D F G\n Z X C V B";
        missionObjectives[6] = "Mission 7 Objectives:\n\nRight Hand Only!\n\n Y U I O P\n H J K L\n N M";
        missionObjectives[7] = "Mission 8 Objectives:\n\nPangrams\n\n Using all the letters!";
        missionObjectives[8] = "Mission 9 Objectives:\n\nNumbers 0-9\n\n Use the keyboard numbers.\n Do not use the number pad.";
        missionObjectives[9] = "Mission 10 Objectives:\n\nAlpha numeric\n\n Combine a two-digit lett and four-digit number.";
        missionObjectives[10] = "Mission 11 Objectives:\n\nAddresses\n\n  Numbers, letters, and some punctuation.";
        missionObjectives[11] = "Mission 12 Objectives:\n\nPunctuation Marks\n\n Left: !@#$%^\n Right: &*()-=+[];:,./?<>";
        missionObjectives[12] = "Mission 13 Objectives:\n\nScience Jargon\n\n Quickly type unfamiliar words";
        missionObjectives[13] = "Mission 14 Objectives:\n\nPoetry\n\n Quickly type lines of poety\n Non-standard capitalization and punctuation";
        missionObjectives[14] = "Mission 15 Objectives:\n\nCoding\n\n Type lines of code as they appear.\n Spacing and capitalization are important";

        //Mission Briefing Text
        missionBriefing[0] = "Your first mission is to defeat a low-level threat.  " +
            "This hacker is only using the letters in the middle row of the keyboard.  " +
            "Place your fingers on the middle row of keys as shown.  " +
            "This is called the <color=#ff8000ff>Homerow Position.</color>" +
            "Use your pinky to hold the <color=#ff8000ff>SHIFT</color> key for capital letters.  " +
            "Use your thumb for the <color=#ff8000ff>SPACEBAR</color>.";
        missionBriefing[1] = "It looks like the threat is expanding their arsenal.  " +
            "Use your left middle finger for the <color=#ff8000ff>E</color> key.";
        missionBriefing[2] = "Now it seems they're using words with the letter <color=#ff8000ff>I</color>.  " +
            "Use your right middle finger for the <color=#ff8000ff>I</color> key.";
        missionBriefing[3] = "The threat is using both the <color=#ff8000ff>E</color> and <color=#ff8000ff>I</color> keys.  " +
            "Remember to keep your hands in the Homerow Position.";
        missionBriefing[4] = "Your adversary has adapted and is now using the bottom row, too.  " +
            "Use your index fingers for the <color=#ff8000ff>V</color> and <color=#ff8000ff>N</color> keys on the bottom row.";
        missionBriefing[5] = "Most words use letters on both sides of the keyboard, but not all do.  " +
            "This threat will force you to use only your left hand.  " +
            "Remember to keep your fingers in the Homerow Position.";
        missionBriefing[6] = "Most words use letters on both sides of the keyboard, but not all do.  " +
            "This threat will force you to use only your right hand.  " +
            "Remember to keep your fingers in the Homerow Position.";
        missionBriefing[7] = "For this mission, you will need to type pangrams.  " +
            "Pangrams are sentences that use every single letter.";
        missionBriefing[8] = "Having been thwarted so far, your adversary has evoled their tactics and is using numbers now.  " +
            "Keep your hands on the homerow and stretch up to reach the number keys.";
        missionBriefing[9] = "The threat is now using combinations of letters and numbers.  " +
            "Remember to use your pinky to hold the <color=#ff8000ff>SHIFT</color> key for capital letters.";
        missionBriefing[10] = "Addresses can be challenging.  In addition to the combination of letters and numbers, " +
            "there are also punctuation marks.  The comma and period keys are on the bottom row and reachable with your right hand..";
        missionBriefing[11] = "The threat has expanded their arsenal once again.  " +
            "They're using a wide variety of punctuation marks and special characters.  " +
            "Most of these are accessible using the SHIFT key in combination with the number keys.";
        missionBriefing[12] = "Your adversary has been using relatively short and familiar words.  " +
            "They are now using longer technical jargon.  " +
            "You will need to pay close attention to the unfamilar terms.";
        missionBriefing[13] = "It seems that the threat is a fan of poetry.  Rather than typing single words or phrases, " +
            "you need to type entire lines of poetry correctly to neutralize the threat.  " +
            "Watch out for non-standard capitalization or punctuation.";
        missionBriefing[14] = "Are you up for this final challenge?  The threat is now trying to access the programming code directly.  " +
            "Type in the lines of code exactly as they appear.  Pay close attention to the spacing and capitalization.  " +
            "They will be far from standard.";
    }

    /**************************************************************************
     * * * *             START AT MISSION LEVEL INDICATED              * * * */
    public void StartMission (int level)
    {
        GetMissionKeywords(level - 1);
        SceneManager.LoadScene("TerminalScreen");
    }

    /**************************************************************************
     * * * * LOAD IN MISSION KEYWORDS AND pangram FOR SELECTED LEVEL  * * * */
    public void GetMissionKeywords(int level)
    {
        level = level - 1;                                  //Change from level number to level index

        List<string> masterWordList = new List<string>();   //List of all valid words for this level
        keywords.Clear();                                   //Clear the keyword list

        //Make sure the supplied level is a suitable index for the mission filenames array
        level = Mathf.Clamp(level, 0, keywordFilenames.Length - 1);

        //Open the mission keyword file for the selected level
        StreamReader missionFile = new StreamReader(keywordFilePath + keywordFilenames[level]);

        //Read all words in the file and store them in the Master Word List
        while (!missionFile.EndOfStream)
        {
            masterWordList.Add(missionFile.ReadLine());
        }

        //Choose ten words at random from the Master Word List to use for this instance of the mission
        for (int i = 0; i < 10; i++)
        {
            int wordIndex = Random.Range(0, masterWordList.Count - 1);
            keywords.Add(masterWordList[wordIndex]);
            masterWordList.RemoveAt(wordIndex);
        }

        //Define the current pangram for this mission 
        currentPangrams = missionPangrams[level];
        keywords.Add(currentPangrams);
    }
}
