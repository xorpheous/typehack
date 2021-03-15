using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Basic save system.  It saves this class to a JSON file called savedata.json in
/// the StreamingAssets folder.  If you need multiple file, you will need to modift
/// </summary>
[System.Serializable]
public class SaveData
{
    //These are just sample variables.  They do not matter and can be removed/changed/added to
    public int sampleData = 0;
    public int clearedLevel = 1;
    public int storedXP = 0;
    public string someString = "meep";
    
    //This is NOT a sample variable.  You can use this flag to determine if data should be autosaved when quitting.
    public bool saveOnDestroy = true;

    /// <summary>
    /// Saves the data to savedata.json in StreamingAssets
    /// </summary>
    public void SaveToDisk()
    {
        //Construct the path
        string path = System.IO.Path.Combine(Application.streamingAssetsPath, "savedata.json");
        //Use Unity's JsonUtility class to convert this class to a json string
        string data = JsonUtility.ToJson(this);
        //Open the file in overwrite mode and save it.
        System.IO.StreamWriter sw = new System.IO.StreamWriter(path, false);
        sw.Write(data);
        sw.Close();
    }

    /// <summary>
    /// Tries to load savedata.json from StreamingAssets
    /// </summary>
    /// <returns>Null if file not found, an instance of SaveData if file is found</returns>
    public static SaveData LoadFile()
    {
        SaveData result = null;
        //validate existance of file
        string path = System.IO.Path.Combine(Application.streamingAssetsPath, "savedata.json");
        if (System.IO.File.Exists(path))
        {
            //file exists, so read the file into a string.
            System.IO.StreamReader sr = new System.IO.StreamReader(path);
            string data = sr.ReadToEnd();
            sr.Close();
            //Use Unity's JsonUtility to create an instasnce of SaveData from the json string
            result = JsonUtility.FromJson<SaveData>(data);
        }
        //done, so return null or the instance
        return result;
    }


    //These are utility functions.  Since they use stuff from the UnityEditor namespace, they
    //must be excluded from a stand-a-lone build.
#if UNITY_EDITOR
    
    [UnityEditor.MenuItem("Tools/Basic Save System/Create save directory (StreamingAssets)")]
    public static void CreateSaveDirectory()
    {
        if(!UnityEditor.AssetDatabase.IsValidFolder("assets/StreamingAssets"))
        {
            UnityEditor.AssetDatabase.CreateFolder("assets", "StreamingAssets");
            Debug.Log("StreamingAssets folder created");
        }
        else
        {
            Debug.Log("StreamingAssets folder already exists - no action taken");
        }
    }

    [UnityEditor.MenuItem("Tools/Basic Save System/Delete Save Data")]
    public static void DeleteSaveData()
    {
        string path = System.IO.Path.Combine(Application.streamingAssetsPath, "savedata.json");
        if (System.IO.File.Exists(path))
        {
            System.IO.File.Delete(path);
            Debug.Log("Save data deleted");
        }
        else
        {
            Debug.Log("Save data did not exist - no action taken.");
        }
    }
#endif
}

