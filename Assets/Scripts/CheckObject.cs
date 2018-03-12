using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckObject : MonoBehaviour {

    private const int PLAYER_LAYER = 1 << 8;

    [SerializeField] private float m_MaxDistance = 0;
    [SerializeField] private Material m_GlowingMaterial;

    private Camera m_Camera;
    private Material[] m_SeeingMaterials;

	// Use this for initialization
	void Start () {
        m_Camera = Camera.main;
        m_SeeingMaterials = null;

        // Sanity checking
        if (m_GlowingMaterial == null) {
            Debug.LogError ("Error: 'Glow Outline' material not found.");
        }
	}
	
	// Update is called once per frame
	void Update () {
        RaycastHit hit;
        if (!Physics.Raycast (m_Camera.transform.position, m_Camera.transform.forward, out hit, m_MaxDistance)) {
            // Raycast doesn't hit anything
            if (m_SeeingMaterials != null) {
                // Remove the object's halo material
                Array.Resize<Material> (ref m_SeeingMaterials, m_SeeingMaterials.Length - 1);
            }
        } else {
            Renderer r = hit.collider.gameObject.GetComponent<Renderer> ();
            Debug.Log (r.materials);

            if (r != null && m_SeeingMaterials != r.materials) {
                // Material exists; increase material size
                m_SeeingMaterials = r.materials;
                Array.Resize<Material> (ref m_SeeingMaterials, m_SeeingMaterials.Length + 1);
                // Add glowing material
                m_SeeingMaterials[m_SeeingMaterials.Length - 1] = m_GlowingMaterial;
            }
        }
    }
}
