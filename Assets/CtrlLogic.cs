using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CtrlLogic : MonoBehaviour
{
    VisualElement root;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Game start");
        HelperClass hc = new HelperClass();
        root = GetComponent<UIDocument>().rootVisualElement;

        VisualElement ve = new VisualElement();
        ve.style.height = Length.Percent(50);
        root.Add(ve);

        VisualElement selectGamePanel = new VisualElement();
        root.Add(selectGamePanel);

        Button ludoButton = new Button();
        ludoButton.text = "Play Ludo";
        ludoButton.clicked += () => { EnterLudo(); };
        selectGamePanel.Add(ludoButton);

        Button unoButton = new Button();
        unoButton.text = "Play Uno";
        selectGamePanel.Add(unoButton);


    }

    void EnterLudo()
    {
        root.Clear();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate()
    {
        
    }
}


public class HelperClass
{


    public HelperClass()
    {
        Debug.Log("Im helping");
    }

}