using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyboardDebug : MonoBehaviour
{
    private TouchScreenKeyboard keyboard;

    void Start()
    {
    }

    public void OpenKeyboard()
    {
        TouchScreenKeyboard.Open("", TouchScreenKeyboardType.Default);
    }
}
