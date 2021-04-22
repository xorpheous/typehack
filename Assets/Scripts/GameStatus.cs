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
    public string currentPanagram;                          //Current mission-ending panagram
    public List<string> keywords = new List<string>();      //List of keywords selected for the current mission

    public string[] missionObjectives = new string[15];     //Mission objectives indexed by mission level
    public string[] missionBriefing = new string[15];       //Mission briefing text indexed by mission level
    public string[] missionPanagrams = new string[15];      //Mission panagrams indexed by mission level

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
     * * * *        DEFINE THE KEYWORD FILES PATH AND FILENAMES        * * * */
    private void Start()
    {
        //Define Keywords
        keywordFilePath = Application.dataPath + "/WordLists/";
        keywordFilenames[0] = "homerow_words.txt";
        keywordFilenames[1] = "homerow_words_plusE.txt";
        keywordFilenames[2] = "homerow_words_plusI.txt";
        keywordFilenames[3] = "homerow_words_plusEI.txt";
        keywordFilenames[4] = "homerow_words_plusVN.txt";
        keywordFilenames[5] = "all_left.txt";
        keywordFilenames[6] = "all_right.txt";
        keywordFilenames[7] = "mission8.txt";           //keyboard numbers
        keywordFilenames[8] = "mission9.txt";           //numberpad numbers
        keywordFilenames[9] = "mission10.txt";          //alphanumeric
        keywordFilenames[10] = "mission11.txt";         //punctuation
        keywordFilenames[11] = "mission12.txt";         //phrases
        keywordFilenames[12] = "mission13.txt";         //poetry
        keywordFilenames[13] = "mission14.txt";         //jargon and code
        keywordFilenames[14] = "mission15.txt";         //some really hard stuff
    }

    /**************************************************************************
     * * * *             START AT MISSION LEVEL INDICATED              * * * */
    public void StartMission (int level)
    {
        GetMissionKeywords(level - 1);
        SceneManager.LoadScene("TerminalScreen");
    }

    /**************************************************************************
     * * * * LOAD IN MISSION KEYWORDS AND PANAGRAM FOR SELECTED LEVEL  * * * */
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

        //Define the current panagram for this mission 
        currentPanagram = missionPanagrams[level];
        keywords.Add(currentPanagram);
    }
}
