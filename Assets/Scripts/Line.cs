using UnityEngine;

public class Line
{
    private string m_Speaker;
    private string m_Dialog;

    public Line (string dialog)
    {
        m_Speaker = "";
        m_Dialog = "";

        int colon = dialog.IndexOf (":");
        if (colon == -1) {
            // Cannot find colon - invalid dialog
            Debug.LogError("Invalid dialog '" + dialog + "'; Missing colon");
            return;
        }

        m_Speaker = dialog.Substring (0, colon);
        m_Dialog = dialog.Substring (colon + 1);

        if (m_Speaker.Length == 0) {
            // Speaker name nonexistant - invalid speaker
            Debug.LogError("No speaker found");
            return;
        }
        if (m_Dialog.Length == 0) {
            // Dialog nonexistant - invalid dialog
            Debug.LogError("No dialog found");
            return;
        }
    }

    public override string ToString ()
    {
        return m_Speaker + ": " + m_Dialog;
    }

    public string Speaker
    {
        get {
            return m_Speaker;
        }
    }

    public string Dialog
    {
        get {
            return m_Dialog;
        }
    }
}
