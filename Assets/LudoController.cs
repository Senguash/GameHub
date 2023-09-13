using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class LudoController : Game
{
    // Start is called before the first frame update
    void Start()
    {
        root.Add(new Label("Test"));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}