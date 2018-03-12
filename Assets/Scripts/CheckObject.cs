using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckObject : MonoBehaviour {

    [SerializeField] private float m_MaxDistance = 0;
    [SerializeField] private Material m_GlowingMaterial = null;
    [SerializeField] private Material m_NoneMaterial = null;

    private Camera m_Camera;
    private Renderer[] m_SeeingRenders;

	// Use this for initialization
	void Start () {
        m_Camera = Camera.main;
        m_SeeingRenders = null;

        // Sanity checking
        if (m_GlowingMaterial == null) {
            Debug.LogError ("Error: 'Glow Outline' material not found.");
        }
	}
	
	// Update is called once per frame
	void Update () {
        // Raycast to any object
        RaycastHit hit;
        if (!Physics.Raycast (m_Camera.transform.position, m_Camera.transform.forward, out hit, m_MaxDistance)) {
            // Raycast doesn't hit anything
            if (m_SeeingRenders != null) {
                // Remove all the object's glowing material
                for (int j = 0; j < m_SeeingRenders.Length; j++) {
                    Material[] ms = m_SeeingRenders[j].materials;
                    for (int i = 0; i < ms.Length; i++) {
                        // Revert all glowing materials into 'null'
                        if (ms [i].Equals(m_GlowingMaterial)) {
                            ms [i] = m_NoneMaterial;
                        }
                    }
                    m_SeeingRenders[j].materials = ms;
                }
            }
        } else {
            Renderer[] rs = hit.collider.gameObject.GetComponentsInChildren<Renderer> ();

            if (rs != null && m_SeeingRenders != rs) {
                // Render exists; take it
                m_SeeingRenders = rs;

                // Do for every material in every render available...
                for (int j = 0; j < rs.Length; j++) {
                    Material[] ms = rs[j].materials;
                    for (int i = 0; i < ms.Length; i++) {
                        // Turn all non-existant materials into the glowing variant
                        if (ms [i] == m_NoneMaterial) {
                            ms [i] = m_GlowingMaterial;
                        }
                    }
                    rs[j].materials = ms;
                }
            }
        }
    }
}
