using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    [SerializeField] private GameObject m_GameOver = null;
    [SerializeField] private GameObject m_DialogPane = null;

    private Image m_Cursor;
    private Text m_Overlay;

    // Use this for initialization
    void Start ()
    {
        m_Cursor = GetComponent<Image> ();
        m_Overlay = GetComponent<Text> ();
    }
	
    // Update is called once per frame
    void Update ()
    {
    }

    void TriggerGameOver ()
    {
        m_Cursor.enabled = false;
        m_Overlay.enabled = false;
        m_DialogPane.SetActive (false);
        m_GameOver.SetActive (true);
    }
}
