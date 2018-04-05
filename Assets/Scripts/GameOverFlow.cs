using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverFlow : MonoBehaviour
{
    const string DIALOG = "SELF:???????";

    [SerializeField] private Canvas m_HUDCanvas = null;
    [SerializeField] private int m_ShotTimes = 3;

    // HUD for controlling things
    private HUD m_HUD;
    private AudioSource m_Gunshot;
    // Use states
    private State m_State = State.NONE;

    private enum State
    {
        NONE,
        DIALOG,
        GUNSHOT,
        TEXT
    }

    void Start ()
    {
        m_HUD = m_HUDCanvas.GetComponent<HUD> ();
        m_State = State.DIALOG;
        m_Gunshot = GetComponent<AudioSource> ();

        // Setup the states correctly
        SetupState ();
    }

    void SetupState ()
    {
        switch (m_State) {
        case State.DIALOG:
            // Displays confused dialog
            m_HUD.Overlay.text = DIALOG;
            m_HUD.DialogPane.SetActive (true);
            break;
        case State.GUNSHOT:
            // Creates gunshot sound (not here)
            m_HUD.DialogPane.SetActive (false);
            break;
        case State.TEXT:
            // Displays the game over text
            m_HUD.TriggerGameOver ();
            break;
        default:
        case State.NONE:
            // Something horribly wrong has occurred
            Debug.LogError ("Cannot display this game over state");
            break;
        }
    }

    void HandleState ()
    {
        switch (m_State) {
        case State.DIALOG:
            // Handles confused dialog
            if (Input.GetButtonUp ("Fire1")) {
                // If you left-click the dialog, it goes to the next state: gunshot!
                m_State = State.GUNSHOT;
                SetupState ();
            }
            break;
        case State.GUNSHOT:
            // Handles gunshot sound
            // As soon as the gunshot sound finishes playing, display the game over text
            if (!m_Gunshot.isPlaying && m_ShotTimes == 0) {
                m_State = State.TEXT;
                SetupState ();
            } else if (!m_Gunshot.isPlaying && m_ShotTimes != 0) {
                m_ShotTimes--;
                m_Gunshot.Play ();
            }
            break;
        case State.TEXT:
            // Handles anything for game over text
            // Thus, don't do anything and wait for Alt+F4
            break;
        default:
        case State.NONE:
            // Something horribly wrong has occurred
            Debug.LogError ("Cannot handle this game over state");
            break;
        }
    }

    void Update ()
    {
        HandleState ();
    }
}
