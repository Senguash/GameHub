using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
//using UnityEngine.UI;
using UnityEngine.UIElements;

public class TicTacToeController : Game
{
    // Start is called before the first frame update
    Dictionary<Tuple<int, int>, Button> buttonDictionary;
    TicTacToe game;
    //add state
    State gameStart = new State("Start");
    State player1 = new State("player1");
    State player2 = new State("player2");
    State gameOver = new State("gameOver");
    Command next = new Command("next");
    Command endGame = new Command("end");
    
    
    void Start()
    {
        transitions = new Dictionary<StateTransition, State>()
        {
            { new StateTransition(player1, next), player2 },
            { new StateTransition(player2, next), player1},
            { new StateTransition(player2, endGame), gameOver},
            { new StateTransition(player1, endGame), gameOver},
            { new StateTransition(gameOver, next), gameStart},
        };

        SetInitialState(player2);
        game = new TicTacToe();
        InitUI();
    }

    void InitUI()
    {
        VisualElement ve1 = UIGenerate.VisualElement(root, Length.Percent(100), Length.Percent(100));
        VisualElement veV = UIGenerate.VisualElement(ve1, 180, 180, FlexDirection.Column);
        for (int i = 0; i < 3; i++)
        {
            VisualElement ve = UIGenerate.VisualElement(veV, 180, 60, FlexDirection.Row);
            for (int j = 0; j < 3; j++)
            {
                int xTemp = i;
                int yTemp = j;
                var btn = UIGenerate.Button(ve, j.ToString() + i.ToString());
                buttonDictionary.Add(new Tuple<int,int>(i,j), btn);
                btn.clicked += () =>
                {
                    PressedBotton(xTemp, yTemp);
                };
            }

        }

    }
    void PressedBotton(int x, int y)
    {
         Debug.Log(x +" "+ y);
        if (game.TryPlaceBlock(x, y, GetCurrentPlayer()))
        {
            if(game.CheckGame())
                MoveNext(endGame);
            else
            {
                MoveNext(next);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void checkGame()
    {
        game.CheckGame();
    }

    private int GetCurrentPlayer()
    {
        if (CurrentState == player1)
        {
            return 1;
        }
        else if (CurrentState == player2)
        {
            return 2;
        } else
        {
            throw new Exception("Not anyones turn");
        }
    } 
}

public class TicTacToe
{
    public TicTacToe() { }
    private int[,] gameboard =  { { 0, 0, 0 }, { 0, 0, 0 }, { 0, 0, 0 } } ;   

    public bool TryPlaceBlock(int x, int y, int player)
    {
        if (gameboard[x,y] == 0)
        {
            this.gameboard[x, y] = player;
            return true;
        }
        else
        {
            return false;
        }
    }
    public bool CheckGame()
    {
        for (int i = 0; i < gameboard.GetLength(0); i++)
        {
            for (int j = 0; j < gameboard.GetLength(1); j++)
            {
                Console.WriteLine("" + gameboard[i, j]);
            }
        }
            return true;
    }
}
