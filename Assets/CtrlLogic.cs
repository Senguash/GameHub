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

        Button sudokuButton = new Button();
        sudokuButton.text = "Play Sudoku";
        sudokuButton.clicked += () => { EnterSudoku(); };
        selectGamePanel.Add(sudokuButton);

        Button ticTacToeButton = new Button();
        ticTacToeButton.text = "Play Tic Tac Toe";
        ticTacToeButton.clicked += () => { EnterTicTacToe(); };
        selectGamePanel.Add(ticTacToeButton);
    }

    void EnterLudo()
    {
        root.Clear();
        LudoController ludo = gameObject.AddComponent(typeof(LudoController)) as LudoController;
        root.Add(ludo.root);
    }
    void EnterSudoku()
    {
        root.Clear();
        SudokuController sudoku = gameObject.AddComponent(typeof(SudokuController)) as SudokuController;
        root.Add(sudoku.root);
    }
    void EnterTicTacToe()
    {
        root.Clear();
        TicTacToeController tic = gameObject.AddComponent(typeof(TicTacToeController)) as TicTacToeController;
        root.Add(tic.root);
    }
    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate()
    {
        
    }
}


public abstract class Game : MonoBehaviour
{

    public VisualElement root;

    public Game()
    {
        
    }

    private void Awake()
    {
        root = new VisualElement();
    }

    public VisualElement GetRootElement()
    {
        return root;
    }
}