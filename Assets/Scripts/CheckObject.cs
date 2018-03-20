using System;
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

    private Camera m_Camera;
    private Renderer[] m_SeeingRenders;
    private ScriptManager m_SMan;

    // Use this for initialization
    void Start ()
    {
        m_Camera = Camera.main;
        m_SeeingRenders = null;

        // Sanity checking
        if (m_GlowingMaterial == null) {
            // If you forgot to specify a material
            Debug.LogError ("Error: 'Glow Outline' material not found.");
        }
        if (m_Overlay == null) {
            // If you forgot to specify a text object
            Debug.LogError ("Error: Overlay not found; please assign it.");
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
        }
    }

    // Strips the name to bare bones
    // Removes all "duplicate" names
    String StripName (String s)
    {
        int ind = s.IndexOf (" (");
        return ind < 0 ? s : s.Substring (0, ind);
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
            }
        }
    }
}
