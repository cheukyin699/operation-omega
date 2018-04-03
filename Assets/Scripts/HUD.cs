using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    [SerializeField] private GameObject m_GameOver = null;
    [SerializeField] public GameObject DialogPane = null;

    private Image m_Cursor;
    private Image m_Image;
    public Text Overlay;

    // Use this for initialization
    void Start ()
    {
        foreach (Image i in GetComponentsInChildren<Image> ()) {
            switch (i.name) {
            case "Cursor":
                m_Cursor = i;
                break;
            case "Image Overlay":
                m_Image = i;
                m_Image.enabled = false;
                break;
            }
        }
    }

    public void SetImage (string img)
    {
        m_Image.sprite = Resources.Load<Sprite> (img);
    }

    public void DisplayImage(bool yes)
    {
        m_Image.enabled = yes;
    }

    public bool IsViewingImage ()
    {
        return m_Image.enabled;
    }
	
    public void TriggerGameOver ()
    {
        m_Cursor.enabled = false;
        Overlay.enabled = false;
        DialogPane.SetActive (false);
        m_GameOver.SetActive (true);
    }
}
