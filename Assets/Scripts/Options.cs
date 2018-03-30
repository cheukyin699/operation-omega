using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Options : MonoBehaviour
{
    [SerializeField] private GameObject m_Player = null;
    [SerializeField] private float m_RotateThreshold = 10;

    private Quaternion m_InitRotation;
    private CheckObject m_Callback;

    void Start ()
    {
        if (m_Player == null) {
            Debug.LogError ("Error: Player game object not found. Maybe you forgot to get it?");
            return;
        }

        m_Callback = m_Player.GetComponent<CheckObject> ();
        if (m_Callback == null) {
            Debug.LogError ("Error: CheckObject script not found on player. Maybe you forgot to get it?");
        }
    }

    void OnEnable ()
    {
        // Obtain the initial camera transform
        m_InitRotation = Camera.main.transform.rotation;
    }

    void Update ()
    {
        if (enabled) {
            Vector3 q = Camera.main.transform.rotation.eulerAngles - m_InitRotation.eulerAngles;

            // Sometimes, we are on the 0-360 boundary, and we would need to normalize it or else there would
            // be a huge jump, leading to the wrong option being picked.
            if (q.y > 180) {
                q.y -= 360;
            } else if (q.y < -180) {
                q.y += 360;
            }

            // If rotation is greater than threshold, call the player callback function
            if (Mathf.Abs (q.y) >= m_RotateThreshold) {
                // if q.y < 0, "yes"; otherwise, "no"
                m_Callback.SelectOption (q.y < 0);
            }
        }
    }
}
