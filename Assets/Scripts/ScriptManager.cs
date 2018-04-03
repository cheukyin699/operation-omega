using SimpleJSON;
using System;
using System.Collections.Generic;
using UnityEngine;

public class ScriptManager {

    private Dictionary<string, Script> m_Dict;

    public bool Load (TextAsset t) {
        string text = t.text;

        JSONNode temp = JSON.Parse (text);
        foreach (string key in temp.Keys) {
            m_Dict [key] = new Script (temp [key]);
        }

        return true;
    }

    public ScriptManager (TextAsset ass) {
        m_Dict = new Dictionary<string, Script> ();
        if (!Load (ass)) {
            // Loading the file has failed
            Debug.LogError ("Error: cannot load the file");
        }
    }

    public Script this[string id]
    {
        get {
            Debug.Log (id);
            return m_Dict [id];
        }
    }
}
