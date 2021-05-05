/******************************************************************************
 * PlayerData.cs is derivitive of Prof. Fleming's SaveData.cs template demoed 
 * in Serious Game Design (GAME 238).  This class holds the player's name,
 * the cleared status of each level, and the status of each achievement. 
 * Each level has three tiers of achievement and the status of each level is
 * stored as an integer, 0 for uncompleted, then completed status of 1, 2, or
 * 3 depending upon demonstrated proficiency.  Achievement status is simply
 * a bool (true/false).  
 * 
 * This class also contains methods for saving data to the hard drive and 
 * retrieving data.  Saved data file nomencature is based on the supplied
 * username so that multiple players can track their progress on the same
 * device.
 * 
 * J. Douglas Patterson
 * Johnson County Community College
 * dpatter@jccc.edu
 * 
 * v1.0, 02-APR-2021
 * 
 * ***************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

[System.Serializable]
public class PlayerData
{
    /**************************************************************************
     * * * *                      MEMBER VARIABLES                     * * * */
    public string playerName = "Player-1";
    public int[] levelStatus = new int[15];
    public bool[] achievements = new bool[10];
    
    public bool saveOnDestroy = true;    //Flag to determine if data should be autosaved when quitting.


    /**************************************************************************
     * SaveToDisk is a parameterless method that writes the current values
     * of the member variables to the hard drive using the built-in JSON 
     * serialization utility.                                                */
    public void SaveToDisk(string playerName)
    {
        //Construct the path
        string path = Path.Combine(Application.streamingAssetsPath, playerName + "_savedata.json");

        //Use Unity's JsonUtility class to convert this class to a json string
        string data = JsonUtility.ToJson(this);

        //Open the file in overwrite mode and save it.
        StreamWriter outFile = new StreamWriter(path, false);
        outFile.Write(data);
        outFile.Close();
    }

    /**************************************************************************
     * LoadPlayerData is a method that reads the current values of the member
     * variables from the hard drive using the built-in JSON serialization
     * utility.  The player name of the data to be loaded must be supplied 
     * to this method as a string.  A new instance of the PlayerData class
     * is returned by this method.                                          */
    public static PlayerData LoadPlayerData(string playerName)
    {
        PlayerData result = null;

        //validate existance of file
        string path = Path.Combine(Application.streamingAssetsPath, playerName + "_savedata.json");

        if (File.Exists(path))
        {
            //file exists, so read the file into a string.
            StreamReader inputFile = new StreamReader(path);
            string data = inputFile.ReadToEnd();

            inputFile.Close();

            //Use Unity's JsonUtility to create an instasnce of SaveData from the json string
            result = JsonUtility.FromJson<PlayerData>(data);
        }
        //done, so return null or the instance
        return result;
    }
}

