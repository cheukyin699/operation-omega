using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ScreenShoot : MonoBehaviour {
    void OnMouseDown() {
        string temp = Path.GetRandomFileName ();
        ScreenCapture.CaptureScreenshot (temp + ".png");

        Debug.Log (temp + ".png");
    }
}
