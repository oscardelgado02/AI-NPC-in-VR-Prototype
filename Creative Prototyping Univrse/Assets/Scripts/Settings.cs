using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class Settings
{
    //Singleton
    private Settings()
    {

    }

    private static Settings instance;
    public static Settings Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new Settings();
            }
            return instance;
        }
    }

    public bool useElevenLabs = false;
}
