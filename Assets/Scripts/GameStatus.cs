using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameStatus : MonoBehaviour
{
    public PlayerData playerData;
    public int missionLevel = 0;
    string[] keywordFilenames = new string[15];
    string keywordFilePath = "";
    public string currentKeyword;
    public string currentPanagram;
    public List<string> keywords = new List<string>();
    public string[] missionObjectives = new string[15];
    public string[] missionBriefing = new string[15];
    public string[] missionPanagrams = new string[15];

    private void Awake()
    {
        PlayerData tempData = PlayerData.LoadPlayerData(playerData.playerName);

        //If the load failed, then create a new instance of SaveData
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
    // Start is called before the first frame update
    void Start()
    {
        //Define Keywords
        keywordFilePath = Application.dataPath + "TextFiles/";
        keywordFilenames[0] = "mission1.txt";
        keywordFilenames[1] = "mission2.txt";
        keywordFilenames[2] = "mission3.txt";
        keywordFilenames[3] = "mission4.txt";
        keywordFilenames[4] = "mission5.txt";
        keywordFilenames[5] = "mission6.txt";
        keywordFilenames[6] = "mission7.txt";
        keywordFilenames[7] = "mission8.txt";
        keywordFilenames[8] = "mission9.txt";
        keywordFilenames[9] = "mission10.txt";
        keywordFilenames[10] = "mission11.txt";
        keywordFilenames[11] = "mission12.txt";
        keywordFilenames[12] = "mission13.txt";
        keywordFilenames[13] = "mission14.txt";
        keywordFilenames[14] = "mission15.txt";

        //Define Mission Objectives
        missionObjectives[0] = "Mission 1 Objectives:\nDo stuff.";
        missionObjectives[1] = "Mission 2 Objectives:\nDo stuff.";
        missionObjectives[2] = "Mission 3 Objectives:\nDo stuff.";
        missionObjectives[3] = "Mission 4 Objectives:\nDo stuff.";
        missionObjectives[4] = "Mission 5 Objectives:\nDo stuff.";
        missionObjectives[5] = "Mission 6 Objectives:\nDo stuff.";
        missionObjectives[6] = "Mission 7 Objectives:\nDo stuff.";
        missionObjectives[7] = "Mission 8 Objectives:\nDo stuff.";
        missionObjectives[8] = "Mission 9 Objectives:\nDo stuff.";
        missionObjectives[9] = "Mission 10 Objectives:\nDo stuff.";
        missionObjectives[10] = "Mission 11 Objectives:\nDo stuff.";
        missionObjectives[11] = "Mission 12 Objectives:\nDo stuff.";
        missionObjectives[12] = "Mission 13 Objectives:\nDo stuff.";
        missionObjectives[13] = "Mission 14 Objectives:\nDo stuff.";
        missionObjectives[14] = "Mission 15 Objectives:\nDo stuff.";
    }

    public void StartMission (int level)
    {
        GetMissionKeywords(level);
        SceneManager.LoadScene("TerminalScreen");
    }

    public void GetMissionKeywords(int level)
    {
        keywords.Clear();

        level = Mathf.Clamp(level, 0, keywordFilenames.Length - 1);
        StreamReader missionFile = new StreamReader(keywordFilePath + keywordFilenames[level]);

        while (!missionFile.EndOfStream)
        {
            keywords.Add(missionFile.ReadLine());
        }
        currentPanagram = missionPanagrams[level];
    }
}
