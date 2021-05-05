using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DoNotDestroy : MonoBehaviour
{
    static  DoNotDestroy instance = null;

    void Start()
    {
        if (instance == null)
        {
            //If this is the first instance of this object, don't let it be destroyed.
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            //If this is a clone, destroy it. We don't want duplicate objects.
            Destroy(this.gameObject);
        }
    }
}
