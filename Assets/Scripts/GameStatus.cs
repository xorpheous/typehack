using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GameStatus : MonoBehaviour
{
    public PlayerData playerData;
    public int missionLevel = 0;
    string[] keywordFilenames = new string[15];
    string keywordFilePath = "";
    public string currentKeyword;
    public string currentPanagram;
    public List<string> keywords = new List<string>();

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
    }

    // Update is called once per frame
    void Update()
    {
        
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
    }
}
