using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CheckObject : MonoBehaviour
{
    [SerializeField] private float m_MaxDistance = 0;
    [SerializeField] private Material m_GlowingMaterial = null;
    [SerializeField] private Text m_Overlay = null;
    [SerializeField] private TextAsset m_Dialog = null;
    [SerializeField] private Text m_DialogText = null;
    [SerializeField] private GameObject m_DialogPanel = null;
    [SerializeField] private GameObject m_OptionsPanel = null;
    [SerializeField] private Canvas m_HUDCanvas = null;
    [SerializeField] private bool m_Highlight = true;

    private Camera m_Camera;
    private Renderer[] m_SeeingRenders;
    // Temporary variable for storing object renders
    private ScriptManager m_SMan;
    // Script manager
    private string m_SelectedObject;
    // Contains selected object ID
    private Script m_SelectedScript;
    // Disables the controls
    private bool m_DisableControls = false;
    // Audio things, for more control
    private AudioSource m_Ambient;
    private AudioSource m_PhoneRing;
    // HUD for controlling things
    private HUD m_HUD;
    // Game over condition
    private bool m_GameOver = false;
    // Phone ringing condition
    // Hardcoded for convenience
    private bool m_IsPhoneRing = false;

    // Use this for initialization
    void Start ()
    {
        m_Camera = Camera.main;
        m_SeeingRenders = null;
        m_SelectedObject = "";
        m_SelectedScript = null;
        m_Ambient = GetComponent<AudioSource> ();
        m_PhoneRing = gameObject.AddComponent<AudioSource> ();
        m_SMan = new ScriptManager (m_Dialog);
        m_HUD = m_HUDCanvas.GetComponent<HUD> ();

        // FIXME: Load the correct ringtone
        m_PhoneRing.clip = Resources.Load ("AmbientMusic") as AudioClip;
        m_PhoneRing.loop = true;
        m_PhoneRing.playOnAwake = false;

        // Sanity checking
        if (m_GlowingMaterial == null && !m_Highlight) {
            // If you forgot to specify a material
            Debug.LogError ("Error: 'Glow Outline' material not found.");
        }
        if (m_Overlay == null) {
            // If you forgot to specify a text object
            Debug.LogError ("Error: Overlay not found; please assign it.");
        }
        if (m_DialogText == null) {
            // If you forgot to specify a text object displaying the dialog
            Debug.LogError ("Error: Dialog Text not found; please assign it.");
        }
        if (m_DialogPanel == null) {
            // If you forgot to specify the dialog panel
            Debug.LogError ("Error: Dialog Panel not found; please assign it.");
        }
        if (m_OptionsPanel == null) {
            // If you forgot to specify the options panel
            Debug.LogError ("Error: Options Panel not found; please assign it.");
        }
    }

    // Reverts existing game object back to the way God intended
    void Revert ()
    {
        if (m_SeeingRenders != null) {
            // Remove all the object's glowing material
            for (int i = 0; i < m_SeeingRenders.Length && m_Highlight; i++) {
                Material[] ms = m_SeeingRenders [i].materials;
                int last = ms.Length - 1;
                // Replace the last material
                ms [last] = null;
                m_SeeingRenders [i].materials = ms;
            }
            m_SeeingRenders = null;

            // Reset overlay
            m_Overlay.text = "";
            // Reset selection
            m_SelectedObject = "";
        }
    }

    // Strips the name to bare bones
    // Removes all "duplicate" names
    string StripName (string s)
    {
        int ind = s.IndexOf (" (");
        return ind < 0 ? s : s.Substring (0, ind);
    }

    // Turns a name into an item id
    string NameToID (string s)
    {
        return StripName (s).Replace (" ", "").ToLower ();
    }

    // Checks to see if player has selected an object
    bool HasSelected ()
    {
        return m_SelectedObject != "";
    }

    // Checks to see if any dialog is active (i.e. you can see dialog on screen, currently)
    bool HasActiveDialog ()
    {
        return m_SelectedScript != null;
    }

    // Updates active in-game dialog, if dialog is activated
    void UpdateLine ()
    {
        // Do a quick check to see if it exists, because we seem to be running into a
        // null pointer exception.
        var line = m_SelectedScript.Get ();
        if (line != null) {
            UpdateLine (line.ToString ());
        }
    }

    void UpdateLine (string l)
    {
        if (!m_DialogPanel.activeSelf) {
            // If it isn't visible, make it visible!
            m_DialogPanel.SetActive (true);
        }

        m_DialogText.text = l;

        // Checks to see if you need to display binary decisions
        m_OptionsPanel.SetActive (m_SelectedScript.NeedOptions ());
    }

    // Called whenever you make a decision. True values reflect affirmative (usually yes).
    // False values reflect negative (usually no).
    public void SelectOption (bool choice)
    {
        if (HasActiveDialog () && m_SelectedScript.NeedOptions ()) {
            // Disable dialog panel and options
            m_DialogPanel.SetActive (false);

            // Must call .DoCallback() in order to call callback, if that's what is warranted
            m_SelectedScript.DoCallback (choice);
        }
        m_OptionsPanel.SetActive (false);
        m_SelectedScript.Reset ();
        m_SelectedScript = null;
    }

    // This is called whenever a non-trivial effect is executed
    // Non-trivial effects are formated as follows:
    //      effect:<data>
    // Where `effect` is the effect id (string), and `<data>` is the corresponding data.
    // Note the lack of spaces between the colon and the data.
    void HandleEffect (string effect)
    {
        int colon = effect.IndexOf (':');
        string name = effect.Substring (0, colon);
        string data = effect.Substring (colon + 1);

        switch (name) {
        case "scene":
            // Switch the active scene
            SceneManager.LoadScene (data);
            break;
        case "view":
            // View some media, whether it be graphics or video format
            switch (data) {
            case "autopsy":
            case "phone":
                // Images are grouped together
                // TODO
                m_HUD.SetImage (data);
                m_HUD.DisplayImage (true);

                // Set the flag for game over
                m_GameOver = data == "phone";
                break;
            case "footage":
                // Videos are grouped together
                DoVideo ();
                break;
            default:
                Debug.LogError ("Invalid media: '" + data + "'");
                break;
            }
            break;
        case "hear":
            // Listen to some media, must be audio
            // Since you are nearing the end of the game, disable all ambient musics
            m_PhoneRing.Play ();
            m_Ambient.Stop ();
            m_IsPhoneRing = data == "phone";
            break;
        default:
            Debug.LogError ("Invalid effect: '" + effect + "'");
            break;
        }
    }

    void DoVideo ()
    {
        var cam = m_Camera.gameObject;
        var player = cam.AddComponent<UnityEngine.Video.VideoPlayer> ();

        // Autoplay
        player.playOnAwake = true;

        // Use near plane
        player.renderMode = UnityEngine.Video.VideoRenderMode.CameraNearPlane;

        // Set the video to play
        // FIXME Set the correct video to play
        var vid = Resources.Load ("TestVid") as UnityEngine.Video.VideoClip;
        player.clip = vid;

        // No looping
        player.isLooping = false;

        // Disable all controls and musics until it ends
        player.loopPointReached += DonePlaying;
        m_DisableControls = true;
        m_Ambient.Pause ();

        // Play it
        player.Play ();
    }

    void DonePlaying (UnityEngine.Video.VideoPlayer p)
    {
        m_DisableControls = false;
        m_Ambient.Play ();

        // Remove the component
        Destroy (p);
    }

    // When you click, you check the object on your cursor
    // TODO use a kinect
    void HandleClick ()
    {
        if (HasSelected () && !HasActiveDialog ()) {
            // No existing dialog - let's try and get some!
            try {
                if (m_SelectedObject == "phone" && !m_IsPhoneRing) {
                    // Don't want the player to trigger it before it rings
                    // That's why we have this hardcoded!
                    m_SelectedScript = m_SMan ["phonebefore"];
                } else {
                    // The rest of the time, do everything normally
                    m_SelectedScript = m_SMan [m_SelectedObject];
                }

                // Set the callback function
                m_SelectedScript.callback = HandleEffect;

                UpdateLine ();
            } catch (Exception e) {
                Debug.LogError (e.ToString ());
            }
        } else if (HasActiveDialog ()) {
            // Already has active dialog - let's advance the dialog!
            m_SelectedScript.Advance ();

            // Check for end of dialog
            if (m_SelectedScript.IsEOD () && m_SelectedScript.type == Script.Type.LINEAR) {
                // If we have reached the end of the dialog, delete the script
                // Only delete the script if it is linear; don't if binary
                m_SelectedScript.Reset ();
                m_SelectedScript = null;

                // Hide the dialog window
                m_DialogPanel.SetActive (false);
            } else {
                // If it is not the end, display it normally!
                if (m_SelectedScript.HasDelay ()) {
                    UpdateLine (m_SelectedScript.Get ().Speaker + ":");
                    Invoke ("UpdateLine", Script.DELAY);
                } else {
                    UpdateLine ();
                }
            }
        }
    }
	
    // Update is called once per frame
    void Update ()
    {
        // Raycast to any object
        RaycastHit hit;

        if (!Physics.Raycast (m_Camera.transform.position, m_Camera.transform.forward, out hit, m_MaxDistance)) {
            // Raycast doesn't hit anything
            Revert ();
        } else {
            // Raycast hits
            Renderer[] rs = hit.collider.gameObject.GetComponentsInChildren<Renderer> ();

            if (rs != null && m_SeeingRenders != rs) {
                // Reverts everything, if necessary
                Revert ();
                // Renders exists; take it
                m_SeeingRenders = rs;

                // Do for every material in every render available...
                for (int i = 0; i < rs.Length && m_Highlight; i++) {
                    Material[] ms = rs [i].materials;
                    int last = ms.Length - 1;
                    // Replace the last material
                    ms [last] = new Material (m_GlowingMaterial);
                    rs [i].materials = ms;
                }

                // Update overlay with information (namely, the name of the game object
                m_Overlay.text = StripName (hit.collider.gameObject.name);
                // Update selected object with object ID
                m_SelectedObject = NameToID (hit.collider.gameObject.name);
            }
        }

        // Check for mouse button input
        if (Input.GetMouseButtonUp (0) && !m_DisableControls && !m_HUD.IsViewingImage ()) {
            // Left-click to trigger
            HandleClick ();
        } else if (Input.GetMouseButtonUp (0) && m_HUD.IsViewingImage ()) {
            // Snap out of image viewing mode
            m_HUD.DisplayImage (false);

            // If the game over flag is set, head to the game over
            if (m_GameOver) {
                SceneManager.LoadScene ("Game Over");
            }
        }

        // I'm gonna hate myself, but I cannot figure out another other than to hard-code
        // the code that stops the phone from ringing.
        // FIXME: never fix me
        if (m_SelectedObject == "phone" && m_PhoneRing.isPlaying) {
            m_PhoneRing.Stop ();
        }
    }
}
