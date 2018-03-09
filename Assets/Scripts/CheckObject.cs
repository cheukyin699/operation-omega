using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckObject : MonoBehaviour {

    [SerializeField] private float m_MaxDistance = 0;

    private Camera m_Camera;

	// Use this for initialization
	void Start () {
        m_Camera = Camera.main;
	}
	
	// Update is called once per frame
	void Update () {
        RaycastHit hit;
        if (!Physics.Raycast (m_Camera.transform.position, m_Camera.transform.forward, out hit, m_MaxDistance))
            return;
        else {
            GameObject obj = hit.collider.gameObject;
            Renderer r = obj.GetComponent<Renderer> ();

            if (r != null) {
                // Material exists
                Material[] mats = r.materials;
                Array.Resize<Material> (ref mats, mats.Length + 1);
        }
	}
}
