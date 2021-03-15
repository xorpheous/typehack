using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This class is not needed to use the SaveData class.  It is simply to show a very basic example
//of how to use SaveData.
public class GameLogic : MonoBehaviour
{

    public SaveData saveData;


    //Using Awake since Awake is for initialization, and is called before start.
    private void Awake()
    {
        //Use the static method SaveData.LoadFile() to attempt to load savedata.json, which is stored
        //in StreamingAssets.
        SaveData tempData = SaveData.LoadFile();
        //If the load failed, then create a new instance of SaveData
        if(tempData == null)
        {
            saveData = new SaveData();
        }
        //If the load suceeded, then use the loaded file
        else
        {
            saveData = tempData;
        }

        //You are not limited to calling LoadFile here.  It can be called anytime you want to load
        //the save
    }

    // Update is called once per frame
    void Update()
    {
        //This means nothing, it is just proving that the data can be changed, saved, and loaded.
        if (Input.GetKeyDown(KeyCode.Space)) saveData.sampleData += 1;
    }

    private void OnDestroy()
    {
        //If the saveOnDestroy flag is set, data will automatically be saved.  This can be
        //triggered by scene changes, application ending, or manual deletion of the GameObject
        if (saveData.saveOnDestroy)
        {
            saveData.SaveToDisk();
        }
        //Again, not limited to calling SaveToDisk here.  It can be called any time you want to
        //save data.  Remember that the Project window will have to refresh before you see your
        //save file.  This happens automatically when recompiling or importing assets, or you can
        //right click in the Project window and select refresh.
    }
}
