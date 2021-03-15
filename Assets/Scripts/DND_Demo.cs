using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DND_Demo : MonoBehaviour
{
    public Text txtData;
    public int counter;

    static DND_Demo instance = null;

    // Start is called before the first frame update
    void Start()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddOne()
    {
        counter += 1;
        txtData.text = counter.ToString();
    }
}
