using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;
using System;

public class Script
{
    public enum Type
    {
        NONE, LINEAR, BINARY
    }

    public const string NO_EFFECT = "none";
    public const string GAME_OVER = "game-over";
    public const float DELAY = 1.0f;

    private bool m_HasError = false;
    private string m_ErrorMessage = "";

    public int pos;                                          // Index position in the script (what dialog you are on)
    public Type type;
    public string effect;
    public Action<string> callback;
    private ArrayList m_Dialog;

    public Script (JSONNode n)
    {
        pos = 0;
        type = GetType (n);
        effect = GetEffect (n);
        JSONArray a = GetDialog (n);
        MakeDialog (a);

        // Log the last error
        if (m_HasError) {
            Debug.LogError (m_ErrorMessage);
        }
    }

    // Advances the dialog by one line
    public void Advance ()
    {
        pos++;
        if (IsEOD () && type == Type.BINARY) {
            // YOUR NOT DONE YET!!!
            pos--;
        }
        // Can only callback if the script is linear
        if (IsEOD () && type == Type.LINEAR && effect != NO_EFFECT) {
            callback (effect);
        }
    }

    public void DoCallback (bool yes)
    {
        if (yes) {
            callback (effect);
        }
    }

    // Checks to see if we have reached the end of the dialog
    public bool IsEOD ()
    {
        return pos >= m_Dialog.Count;
    }

    // Checks to see if we have reached the end of the dialog and need to choose an option
    public bool NeedOptions ()
    {
        return type == Type.BINARY &&
               pos  == m_Dialog.Count - 1;
    }

    // Gets the current line. Short for `this[Pos]`.
    public Line Get ()
    {
        return this[pos];
    }

    // Resets the script
    public void Reset ()
    {
        pos = 0;
    }

    public Line this[int i]
    {
        get {
            return (Line) m_Dialog [i];
        }
    }

    // Returns true only if the type is binary
    // Returns false otherwise
    private Type GetType (JSONNode n)
    {
        if (!n ["type"].IsString) {
            m_HasError = true;
            m_ErrorMessage = "Missing 'type' from script";
            return Type.NONE;
        }

        // Only allow 2 types, and disallow the rest
        switch (n ["type"].Value) {
        case "linear":
            return Type.LINEAR;
        case "binary":
            return Type.BINARY;
        default:
            m_HasError = true;
            m_ErrorMessage = "Invalid type = '" + n ["type"] + "'";
            return Type.NONE;
        }
    }

    // Returns the effect as a string
    // Returns empty string if attribute 'effect' is not found
    private string GetEffect (JSONNode n)
    {
        if (!n ["effect"].IsString) {
            m_HasError = true;
            m_ErrorMessage = "Missing 'effect' from script";
            return "";
        }
        return n ["effect"].Value;
    }

    // Returns the dialog as an array
    // Returns null if attribute 'dialog' is not found
    private JSONArray GetDialog (JSONNode n)
    {
        if (!n ["dialog"].IsArray) {
            m_HasError = true;
            m_ErrorMessage = "Invalid type of dialog. Must be Array<string>";
            return null;
        }
        return n ["dialog"].AsArray;
    }

    // Converts the JSONArray dialog into an ArrayList dialog
    // Checks for invalid strings (either non-string typed items, or
    // improperly formated strings)
    private void MakeDialog (JSONArray jar)
    {
        m_Dialog = new ArrayList (jar.Count);
        for (int i = 0; i < jar.Count; i++) {
            if (!jar [i].IsString) {
                m_HasError = true;
                m_ErrorMessage = "Invalid value from dialog";
                return;
            } else if (jar[i].Value.IndexOf(':') == -1) {
                m_HasError = true;
                m_ErrorMessage = "Cannot find ':' in dialog '" + jar [i].Value + "'";
            } else {
                m_Dialog.Add(new Line (jar [i].Value));
            }
        }
    }
}
