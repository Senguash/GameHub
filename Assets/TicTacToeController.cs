using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Security.Cryptography;
using Unity.PlasticSCM.Editor.WebApi;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
//using UnityEngine.UI;
using UnityEngine.UIElements;

public class TicTacToeController : Game
{
    // Start is called before the first frame update
    Dictionary<Tuple<int, int>, Button> buttonDictionary;
    Label PlayersTurn;
    TicTacToe game;
    UltimadeTTT UGame;
    int gameType;
    //add states
    State gameStart = new State("Start");
    State player1 = new State("player1");
    State player2 = new State("player2");
    State gameOver = new State("gameOver");
    Command next = new Command("next");
    Command endGame = new Command("end");

    //colors
    static Color col_background1 = new Color(1f, 1f, 1f);
    static Color col_background2 = new Color(0.752f, 0.752f, 0.752f);
    static Color col_border = new Color(0f, 0f, 0f);
    static Color col_borderActive = new Color(1f, 0f, 0f);
    void Start()
    {
        transitions = new Dictionary<StateTransition, State>()
        {
            { new StateTransition(gameStart, next), player1},
            { new StateTransition(player1, next), player2 },
            { new StateTransition(player2, next), player1},
            { new StateTransition(player2, endGame), gameOver},
            { new StateTransition(player1, endGame), gameOver},
            { new StateTransition(gameOver, next), gameStart},
        };
        gameStart.LeaveEvent += () => { root.Clear(); };
        gameStart.EnterEvent +=()=>{ InitStartUI(); };
        gameOver.EnterEvent +=()=>{ UIgameover();};
        gameOver.LeaveEvent += () => { root.Clear(); };
        SetInitialState(gameStart);
        //InitStartUI();
        
        //InitUI();
    }


    void InitStartUI()
    {
        VisualElement ve = UIGenerate.VisualElement(root, Length.Percent(100), Length.Percent(100), FlexDirection.Column);
        VisualElement veText = UIGenerate.VisualElement(ve, Length.Percent(100), Length.Percent(10), FlexDirection.Column, Align.Center, Justify.Center);
        VisualElement veBtn = UIGenerate.VisualElement(ve, Length.Percent(100), Length.Percent(30), FlexDirection.Column, Align.Center);
        UIGenerate.Label(veText, " Choose TicTacToe Version",16);


        var btn = UIGenerate.Button(veBtn, "Normal");
        btn.style.width=Length.Percent(35);
        btn.clicked += () =>
        {
            StartNormalTTTbtn();
        };
        btn = UIGenerate.Button(veBtn, "Ultimate");
        btn.style.width = Length.Percent(35);
        btn.clicked += () =>
        {
            StartUltimateTTTbtn();
        };

    }


    void InitUINormalTTT()
    {
        buttonDictionary = new Dictionary<Tuple<int, int>, Button>();
        buttonDictionary.Clear();
        VisualElement ve1 = UIGenerate.VisualElement(root, Length.Percent(100), Length.Percent(100),FlexDirection.Column);
        VisualElement veText = UIGenerate.VisualElement(ve1,Length.Percent(100), Length.Percent(10),FlexDirection.Row,Align.Center,Justify.Center);
        VisualElement veV = UIGenerate.VisualElement(ve1, Length.Percent(100), Length.Percent(50), FlexDirection.Column,Align.Center);
        PlayersTurn = UIGenerate.Label(veText, "Player 1 [X] has turn", 17);
        for (int i = 0; i < 3; i++)
        {
            VisualElement ve = UIGenerate.VisualElement(veV, Length.Percent(100), Length.Percent(30), FlexDirection.Row, Align.Center);
            for (int j = 0; j < 3; j++)
            {
                int xTemp = i;
                int yTemp = j;
                var btn = UIGenerate.Button(ve, "");
                btn.style.fontSize = 20;
                btn.style.width = Length.Percent(30);
                btn.style.height = Length.Percent(70);

                buttonDictionary.Add(new Tuple<int, int>(i, j), btn);
                btn.clicked += () =>                
                {
                    PressedBottonTTT(xTemp, yTemp);
                };        
            }
        }
    }
    void InitUIUltimateTTT()
    {
        buttonDictionary = new Dictionary<Tuple<int, int>, Button>();
        buttonDictionary.Clear();
        VisualElement ve1 = UIGenerate.VisualElement(root, Length.Percent(100), Length.Percent(100));
        VisualElement veText = UIGenerate.VisualElement(ve1, Length.Percent(100), Length.Percent(10), FlexDirection.Row, Align.Center, Justify.Center);
        VisualElement veboard = UIGenerate.VisualElement(ve1, 216, 216,FlexDirection.Column,Align.Center,Justify.Center);
        PlayersTurn = UIGenerate.Label(veText, "Player 1 [X] has Trun", 17);
        for (int i = 0; i < 9; i++)
        {
            VisualElement ve = UIGenerate.VisualElement(veboard, 216, 24, FlexDirection.Row, Align.Center,Justify.Center);
            for (int j = 0; j < 9; j++)
            {
                int xTemp = i;
                int yTemp = j;
                var btn = UIGenerate.Button(ve, "");
                btn.ClearClassList();

                btn.style.justifyContent = Justify.Center;
                btn.style.alignItems = Align.Center;
                btn.style.visibility = Visibility.Visible;
                btn.style.width = 24;
                btn.style.height = 24;
                btn.style.backgroundColor = col_background1;

                btn.style.unityTextAlign = TextAnchor.MiddleCenter;
                btn.style.fontSize = 20;

                btn.style.borderBottomWidth = 1;
                btn.style.borderBottomColor = col_border;
                btn.style.borderTopWidth = 1;
                btn.style.borderTopColor = col_border;
                btn.style.borderLeftWidth = 1;
                btn.style.borderLeftColor = col_border;
                btn.style.borderRightWidth = 1;
                btn.style.borderRightColor = col_border;

                

                if( (((i/3) == 1) || ((j / 3) == 1)) && (j / 3) != (i / 3))
                {
                    btn.style.backgroundColor = col_background2;
                }
                buttonDictionary.Add(new Tuple<int, int>(i, j), btn);
                btn.clicked += () =>
                {
                    PressedBottonUTTT(xTemp, yTemp);
                };

            }
        }

    }
    // Start knap til normal TicTactoe
    void StartNormalTTTbtn()
    {
        MoveNext(next);
        gameType = 1;
        game = new TicTacToe();
        InitUINormalTTT();
    }
    // Start knap til Ultimate TicTactoe
    void StartUltimateTTTbtn()
    {
        MoveNext(next);
        gameType = 2;
        UGame = new UltimadeTTT();
        InitUIUltimateTTT();
    }
    void PressedBottonUTTT(int x, int y)
    {
        //Debug.Log(x + " " + y + "TTTUltimate");
        if (CurrentState == gameOver)
        {
            MoveNext(next);
            return;
        }
        if (UGame.TryPlaceBlock(x, y, GetCurrentPlayer()))
        {
            if (buttonDictionary.TryGetValue(new Tuple<int, int>(x, y), out var btn))    //hent knap fra dictionary
            {
                if (GetCurrentPlayer() == 1) { btn.text = "X"; }
                else { btn.text = "O"; }
            }
            if (UGame.CheckGame()) { MoveNext(endGame); }
            else
            {
                PlayersTurn.text = "Player " + GetNextPlayer() + " has Turn";
                UIActiveGameHighlight(x / 3, y / 3, col_border);
                if(!UGame.getSmallState(x%3,y%3)) { 
                UIActiveGameHighlight(x % 3, y % 3, col_borderActive);
                }

                MoveNext(next);
            }
        }
    }
    void PressedBottonTTT(int x, int y)
    {
        Debug.Log(x + " " + y + "TTTNormal");
        if(CurrentState == gameOver)
        {
            MoveNext(next);
            return;
        }
        if (game.TryPlaceBlock(x, y, GetCurrentPlayer()))       ///er pladsen ledig 
        {
            if(buttonDictionary.TryGetValue(new Tuple<int, int>(x, y), out var btn))    //hent knap fra dictionary
            {
                if (GetCurrentPlayer() == 1){btn.text = "X";}
                else { btn.text = "O"; }
                
            }
            if (game.CheckGame()) { MoveNext(endGame); }
            else
            {
                PlayersTurn.text = "Player " + GetNextPlayer() + " has Turn";
                MoveNext(next);
            }
        }
    }
    void UIgameover()
    {
        if (gameType == 1)
        {
            if (game.getGameOver() != -1)
            {
                PlayersTurn.text = "Player " + GetPreviousPlayer() + " Wins"; 
            }
            else
            {
                PlayersTurn.text = "This game is a Draw";
            }
        }
        else if (gameType == 2)
        {
            if (UGame.getGameOver() != -1)
            {
                PlayersTurn.text = "Player " + GetPreviousPlayer() + " Wins";
            }
            else
            {
                PlayersTurn.text = "This game is a Draw";
            }

        }
    }
    void UIActiveGameHighlight(int x, int y, Color color)
    {
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                if (buttonDictionary.TryGetValue(new Tuple<int, int>(x*3+i, y*3+j), out var btn))    //hent knap fra dictionary
                {
                    if (i == 0)
                    {
                        btn.style.borderTopColor = color;
                    }
                    if (i == 2)
                    {
                        btn.style.borderBottomColor = color;
                    }
                    if (j == 0)
                    {
                        btn.style.borderLeftColor = color;
                    }
                    if (j == 2)
                    {
                        btn.style.borderRightColor = color;
                    }
                }
            }
        }
    }
    // Update is called once per frame
    void Update()
    {

    }

     int GetCurrentPlayer()
    {
        if (CurrentState == player1)
        {
            return 1;
        }
        else if (CurrentState == player2)
        {
            return 2;
        }
        else
        {
            throw new Exception("Not anyones turn");
        }
    }
    string GetPreviousPlayer()
    {
        if (PreviousState == player1)
        {
            return "1 [X]";
        }
        else
        {
            return "2 [O]";
        }
    }
    string GetNextPlayer()
    {
        if (CurrentState == player1)
        {
            return "2 [O]";
        }
        else if (CurrentState == player2)
        {
            return "1 [X]";
        }
        else
        {
            throw new Exception("Not anyones turn");
        }
    }
    
}

public class UltimadeTTT { 
    public UltimadeTTT() { }
    private TicTacToe[,] gameboard = { {new TicTacToe(), new TicTacToe(), new TicTacToe() },
                                       {new TicTacToe(), new TicTacToe(), new TicTacToe() },
                                       {new TicTacToe(), new TicTacToe(), new TicTacToe() } };
    private int currentGamex = -1;
    private int currentGamey = -1;
    private int gameOver = 0;

    public bool TryPlaceBlock(int x, int y, int player)
    {

        if(currentGamex == -1 && currentGamey == -1 && gameboard[x/3,y/3].getGameOver() == 0)
        {
            if(gameboard[x / 3, y / 3].TryPlaceBlock(x % 3, y % 3, player))
            {
                CheckSmallGame(x, y);
                if (gameboard[x % 3, y % 3].getGameOver() != 0)
                {
                    currentGamex = -1;
                    currentGamey = -1;
                }
                else
                {
                    currentGamex = x % 3;
                    currentGamey = y % 3; 
                }
                return true;
            }
            else
            {
                return false;
            }

        } 
        else if(x/3 ==currentGamex && y/3 == currentGamey && gameboard[x / 3, y / 3].getGameOver() == 0)
        {
            if (gameboard[x / 3, y / 3].TryPlaceBlock(x % 3, y % 3, player))
            {
                CheckSmallGame(x, y);
                if (gameboard[x % 3, y % 3].getGameOver() != 0)
                {
                    currentGamex = -1;
                    currentGamey = -1;
                }
                else
                {
                    currentGamex = x % 3;
                    currentGamey = y % 3;
                }
                return true;
            } else
            {
                Debug.Log("x:" + currentGamex + " Y:" + currentGamey);
                return false;
            }
        }
        else
        {
            Debug.Log("x:" + currentGamex + " Y:" + currentGamey);
            return false;
        }


    }
    public int getGameOver()
    {
        return gameOver;
    }
    public bool CheckSmallGame(int x, int y)
    {
        return gameboard[x / 3, y / 3].CheckGame();
    }
    public bool getSmallState(int x, int y)
    {
        if (gameboard[x, y].getGameOver() != 0) { return true; }
        else { return false; }
    }
     
    public bool CheckGame()
    {
        for (int i = 0; i < gameboard.GetLength(0); i++)
        {
            if (gameboard[i, i].getGameOver() != 0)
            {
                switch (i)
                {
                    case 0:
                        if (gameboard[0, 0].getGameOver() == gameboard[0, 1].getGameOver() && gameboard[0, 0].getGameOver() == gameboard[0, 2].getGameOver())
                        {
                            gameOver = gameboard[0, 0].getGameOver();
                            return true;
                        }
                        if (gameboard[0, 0].getGameOver() == gameboard[1, 0].getGameOver() && gameboard[0, 0].getGameOver() == gameboard[2, 0].getGameOver())
                        {
                            gameOver = gameboard[0, 0].getGameOver();
                            return true;
                        }
                        if (gameboard[0, 0].getGameOver() == gameboard[1, 1].getGameOver() && gameboard[0, 0].getGameOver() == gameboard[2, 2].getGameOver())
                        {
                            gameOver = gameboard[0, 0].getGameOver();
                            return true;
                        }
                        break;
                    case 1:
                        if (gameboard[1, 1].getGameOver() == gameboard[0, 1].getGameOver() && gameboard[1, 1].getGameOver() == gameboard[2, 1].getGameOver())
                        {
                            gameOver = gameboard[1, 1].getGameOver();
                            return true;
                        }
                        if (gameboard[1, 1].getGameOver() == gameboard[1, 0].getGameOver() && gameboard[1, 1].getGameOver() == gameboard[1, 2].getGameOver())
                        {
                            gameOver = gameboard[1, 1].getGameOver();
                            return true;
                        }
                        if (gameboard[1, 1].getGameOver() == gameboard[2, 0].getGameOver() && gameboard[1, 1].getGameOver() == gameboard[0, 2].getGameOver())
                        {
                            gameOver = gameboard[1, 1].getGameOver();
                            return true;
                        }
                        break;
                    case 2:
                        if (gameboard[2, 2].getGameOver() == gameboard[1, 2].getGameOver() && gameboard[2, 2].getGameOver() == gameboard[0, 2].getGameOver())
                        {
                            gameOver = gameboard[2, 2].getGameOver();
                            return true;
                        }
                        if (gameboard[2, 2].getGameOver() == gameboard[2, 1].getGameOver() && gameboard[2, 2].getGameOver() == gameboard[2, 0].getGameOver())
                        {
                            gameOver = gameboard[2, 2].getGameOver();
                            return true;
                        }
                        break;
                }
            }
        }
        return false;
    }

}
public class TicTacToe
{
    public TicTacToe() { }
    private int[,] gameboard = { { 0, 0, 0 }, { 0, 0, 0 }, { 0, 0, 0 } };
    int GameOver = 0;
    int pieces = 0;
    public bool TryPlaceBlock(int x, int y, int player)
    {
        if (gameboard[x, y] == 0)
        {
            this.gameboard[x, y] = player;
            pieces++;
            return true;
        }
        else
        {
            return false;
        }
    }
    public int getGameOver()
    {
        return GameOver;
    }
    public bool CheckGame()
    {
        Debug.Log("" + gameboard[0, 0] + "" + gameboard[0, 1] + "" + gameboard[0, 2] + "\n"
                     + gameboard[1, 0] + "" + gameboard[1, 1] + "" + gameboard[1, 2] + "\n"
                     + gameboard[2, 0] + "" + gameboard[2, 1] + "" + gameboard[2, 2]);

        for (int i = 0; i < gameboard.GetLength(0); i++)
        {   
            if (gameboard[i, i] != 0)
            {
                switch(i)
                { 
                    case 0:
                        if (gameboard[0, 0] == gameboard[0, 1] && gameboard[0, 0] == gameboard[0, 2])
                        {
                            GameOver = gameboard[0,0];
                            return true;
                        }
                        if (gameboard[0, 0] == gameboard[1, 0] && gameboard[0, 0] == gameboard[2, 0])
                        {
                            GameOver = gameboard[0, 0];
                            return true;
                        }
                        if (gameboard[0, 0] == gameboard[1, 1] && gameboard[0, 0] == gameboard[2, 2])
                        {
                            GameOver = gameboard[0, 0];
                            return true;
                        }
                        break;
                    case 1:
                        if (gameboard[1, 1] == gameboard[0, 1] && gameboard[1, 1] == gameboard[2, 1])
                        {
                            GameOver = gameboard[1, 1];
                            return true;
                        }
                        if (gameboard[1, 1] == gameboard[1, 0] && gameboard[1, 1] == gameboard[1, 2])
                        {
                            GameOver = gameboard[1, 1];
                            return true;
                        }
                        if (gameboard[1, 1] == gameboard[2, 0] && gameboard[1, 1] == gameboard[0, 2])
                        {
                            GameOver = gameboard[1, 1];
                            return true;
                        }
                        break; 
                    case 2:
                        if (gameboard[2, 2] == gameboard[1, 2] && gameboard[2, 2] == gameboard[0, 2])
                        {
                            GameOver = gameboard[2, 2];
                            return true;
                        }
                        if (gameboard[2, 2] == gameboard[2, 1] && gameboard[2, 2] == gameboard[2, 0])
                        {
                            GameOver = gameboard[2, 2];
                            return true;
                        }
                        break;
                }
             }
        }
        if(pieces == 9)
        {
            GameOver = -1;
            return true;
        }
        return false;
    }
}
