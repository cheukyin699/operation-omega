﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CheckObject : MonoBehaviour
{
    [SerializeField] private float m_MaxDistance = 0;
    [SerializeField] private Material m_GlowingMaterial = null;
    [SerializeField] private Text m_Overlay = null;
    [SerializeField] private TextAsset m_Dialog = null;
    [SerializeField] private Text m_DialogText = null;
    [SerializeField] private GameObject m_DialogPanel = null;

    private Camera m_Camera;
    private Renderer[] m_SeeingRenders;
    // Temporary variable for storing object renders
    private ScriptManager m_SMan;
    // Script manager
    private string m_SelectedObject;
    // Contains selected object ID
    private Script m_SelectedScript;
    // Selected object script

    // Use this for initialization
    void Start ()
    {
        m_Camera = Camera.main;
        m_SeeingRenders = null;
        m_SelectedObject = "";
        m_SelectedScript = null;

        // Sanity checking
        if (m_GlowingMaterial == null) {
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

        // Script management is also done here
        m_SMan = new ScriptManager (m_Dialog);
    }

    // Reverts existing game object back to the way God intended
    void Revert ()
    {
        if (m_SeeingRenders != null) {
            // Remove all the object's glowing material
            for (int i = 0; i < m_SeeingRenders.Length; i++) {
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
    void UpdateLine (Line l)
    {
        if (!m_DialogPanel.activeSelf) {
            // If it isn't visible, make it visible!
            m_DialogPanel.SetActive (true);
        }

        m_DialogText.text = l.ToString ();
    }

    // When you click, you check the object on your cursor
    void HandleClick ()
    {
        if (HasSelected () && !HasActiveDialog ()) {
            // No existing dialog - let's try and get some!
            try {
                m_SelectedScript = m_SMan [m_SelectedObject];
                UpdateLine (m_SelectedScript.Get ());
            } catch (Exception e) {
                Debug.LogError (e.ToString ());
            }
        } else if (HasActiveDialog ()) {
            // Already has active dialog - let's advance the dialog!
            // TODO Check dialog type before advancing, and use the correct overload functions
            ++m_SelectedScript.pos;
            if (m_SelectedScript.IsEOD ()) {
                // If we have reached the end of the dialog, delete the script
                m_SelectedScript.Reset ();
                m_SelectedScript = null;
                // Hide the dialog window
                m_DialogPanel.SetActive (false);
            } else {
                // If it is not the end, display it normally!
                UpdateLine (m_SelectedScript.Get ());
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
                for (int i = 0; i < rs.Length; i++) {
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
        if (Input.GetMouseButtonUp (0)) {
            // Left-click to trigger
            HandleClick ();
        }
    }
}
