using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ScreenShoot : MonoBehaviour {

    // Performs a screen-grab whenever you click
    void OnMouseDown() {
        string temp = Path.GetRandomFileName ();
        ScreenCapture.CaptureScreenshot (temp + ".png");

        Debug.Log (temp + ".png");
    }
}
