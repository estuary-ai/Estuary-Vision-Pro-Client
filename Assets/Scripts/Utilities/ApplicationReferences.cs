using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApplicationReferences : MonoBehaviour
{
    public Transform camTrans;
    public NavManager navManager;

    [Header("Menus")]
    public HandMenu mainMenuController;
    

    [Header("Yokai")]
    public YokaiManager yokaiManager;
    public MischiefManager mischiefManager;
}
