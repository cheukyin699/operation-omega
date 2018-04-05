using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LookAtThings : MonoBehaviour {

    [SerializeField] private float LowerBound = 10f;
    [SerializeField] private float UpperBound = 20f;

    private static GameObject[] m_AllObjects;
    private float m_CountDown;
    private float m_CountUp;
    private Quaternion m_Target;
    private Quaternion m_OrigRot;

	// Use this for initialization
	void Start () {
        if (m_AllObjects == null) {
            // Obtain all game objects if they aren't there
            // Because it is static, it only runs once
            m_AllObjects = GameObject.FindGameObjectsWithTag ("Lookable");
        }

        // Randomly pick a target
        m_CountDown = 0;
        m_CountUp = 0;
        m_Target = transform.rotation;
        m_OrigRot = transform.rotation;

        // Subscribe to scene changes
        SceneManager.activeSceneChanged += OnSceneChange;
	}

    void OnSceneChange (Scene current, Scene next)
    {
        // Update objects
        m_AllObjects = GameObject.FindGameObjectsWithTag ("Lookable");
    }
	
	// Update is called once per frame
	void Update () {
        if (m_CountDown > 0) {
            m_CountDown -= Time.deltaTime;

            if (m_CountUp < 1) {
                m_CountUp += Time.deltaTime;
            }
        } else {
            // Slowly look at your (new) target
            m_CountDown = Random.Range(LowerBound, UpperBound);
            m_CountUp = 0;
            m_OrigRot = transform.rotation;

            // Let unity handle looking at things; copy the rotation and set it as the goal
            var o = m_AllObjects [(int) Random.Range(0, m_AllObjects.Length - 1)];
            while (o.transform == transform) {
                // Make sure that you don't target yourself
                o = m_AllObjects [(int) Random.Range(0, m_AllObjects.Length - 1)];
            }
            transform.LookAt (o.transform);
            m_Target = transform.rotation;
            // Keep the x rotation so the guys don't bend over. No Michael Jacksons here.
            // Because they don't allow you to edit Euler angles directly, they have to be edited with
            // a temporary variable.
            var v3 = m_Target.eulerAngles;
            v3.x = m_OrigRot.x;
            m_Target.eulerAngles = v3;
        }
        
        transform.rotation = Quaternion.Lerp (m_OrigRot, m_Target, m_CountUp);
	}
}
