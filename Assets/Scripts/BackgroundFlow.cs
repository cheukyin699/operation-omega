using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BackgroundFlow : MonoBehaviour
{
    const string DIALOG = "You are Tom, a private investigator hired by the university to investigate" +
                          " a suicide. Please find out what happened.";

    [SerializeField] private Canvas m_HUDCanvas = null;

    // HUD for controlling things
    private HUD m_HUD;

    void Start ()
    {
        m_HUD = m_HUDCanvas.GetComponent<HUD> ();
        m_HUD.Overlay.text = DIALOG;
        m_HUD.DialogPane.SetActive (true);
    }

    void Update ()
    {
        if (Input.GetButtonUp ("Fire1")) {
            // Switch scenes when you go past the dialog
            SceneManager.LoadScene ("Tutorial");
        }
    }
}
