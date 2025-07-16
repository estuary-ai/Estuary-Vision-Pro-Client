using System.Collections.Generic;
using PolySpatial.Samples;
using UnityEngine;

public class ButtonHandler : MonoBehaviour
{
    public ApplicationReferences appRef;
    
    [SerializeField] SpatialUIButton m_Button;
    
    [SerializeField] enum ButtonType
    {
        ToggleCommandMenu,
        OpenServerConnection
    }
    
    [SerializeField] ButtonType m_ButtonType;

    void OnEnable()
    {
        if (m_Button)
        {
            m_Button.WasPressed += WasPressed;
        }
    }

    void OnDisable()
    {
        if (m_Button)
        {
            m_Button.WasPressed -= WasPressed;
        }
    }

    void WasPressed(string buttonText, MeshRenderer meshrenderer)
    {
        Debug.Log($"Button pressed: {buttonText}");
        if (m_ButtonType == ButtonType.ToggleCommandMenu)
        {
            appRef.mainMenuController.ToggleCommandsMenuVisibility();
        }
        else if (m_ButtonType == ButtonType.OpenServerConnection)
        {
            
        }
    }
}
