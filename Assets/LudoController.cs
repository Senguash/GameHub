using System.Collections;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.Animations;
using System.Linq;
using System.Net.NetworkInformation;
using UnityEditor.ShaderKeywordFilter;

public class LudoController : Game
{
    private Sprite redPieceSprite;
    private Sprite greenPieceSprite;
    private Sprite bluePieceSprite;
    private Sprite yellowPieceSprite;
    private Dictionary<int, Button> buttonDictionary;
    private State newGame = new State("newGame");
    private State gameEnd = new State("gameEnd");
    private Command start = new Command("start");
    private Command nextPlayer = new Command("nextPlayer");
    VisualElement VEdice;
    Button rollButton;
    int dice;
    private readonly Dictionary<int, string> PlayerColors = new Dictionary<int, string>() {
        {1,"Red" },
        {2,"Green" },
        {3,"Blue" },
        {4,"Yellow" }
    };



    private LudoGame game = new LudoGame();
    void Start()
    {
        transitions = new Dictionary<StateTransition, State>()
        {

        };

        newGame.EnterEvent += InitNewGameMenu;
        newGame.LeaveEvent += ClearRoot;
        newGame.LeaveEvent += InitLudoUI;
        SetInitialState(newGame);


    }

    // Update is called once per frame
    void Update()
    {

    }
    private void ClearRoot()
    {
        root.Clear();
    }
    private void InitNewGameMenu()
    {

        root.Add(new Label("LUDO"));
       
        Button startGameBtn = new Button();
        Button addPlayerBtn = new Button();
        addPlayerBtn.text = "Add player";
        addPlayerBtn.clicked += () => {
            int ret = AddPlayer();
            if (ret == 2)
            {
                startGameBtn.SetEnabled(true);
            }
            else if (ret == 4)
            {
                addPlayerBtn.SetEnabled(false);
            }
        };
        startGameBtn.text = "Start";
        startGameBtn.clicked += () => { StartGame();
            MoveNext(nextPlayer);
        };
        startGameBtn.SetEnabled(false);
        root.Add(addPlayerBtn);
        root.Add(startGameBtn);
        //root.Add(VEdice);
    }
    private void StartGame()
    {
        AddplayerStates();
    }
    private void AddplayerStates()
    {
        for (int i = 0; i < game.GetLudoPlayers().Count; i++)
        {
            if (i == 0)
            {
                transitions.Add(new StateTransition(newGame, nextPlayer), game.GetLudoPlayers()[i].GetPlayerState());
            }
            else
            {
                transitions.Add(new StateTransition(game.GetLudoPlayers()[i - 1].GetPlayerState(), nextPlayer), game.GetLudoPlayers()[i].GetPlayerState());
                if (i + 1 == game.GetLudoPlayers().Count)
                {
                    transitions.Add(new StateTransition(game.GetLudoPlayers()[i].GetPlayerState(), nextPlayer), game.GetLudoPlayers()[0].GetPlayerState());
                }
            }
            game.GetLudoPlayers()[i].GetPlayerState().LeaveEvent += UpdateUI;
            //game.ludoPlayers[i].GetPlayerState().LeaveEvent += game.NextPlayer;
        }
    }
    private void InitLudoUI()
    {

        buttonDictionary = new Dictionary<int, Button>();
        redPieceSprite = Resources.Load<Sprite>("Ludo/LudoPieceRed");
        greenPieceSprite = Resources.Load<Sprite>("Ludo/LudoPieceGreen");
        bluePieceSprite = Resources.Load<Sprite>("Ludo/LudoPieceBlue");
        yellowPieceSprite = Resources.Load<Sprite>("Ludo/LudoPieceYellow");

        Sprite gameBoardSprite = Resources.Load<Sprite>("Ludo/istockphoto-493120080-1024x1024");
        VisualElement gameBoard = UIGenerate.VisualElement(root, 240, 240, FlexDirection.Column);
        VEdice = UIGenerate.VisualElement(root, Length.Percent(100), Length.Percent(15), FlexDirection.Row, Align.Center);
        gameBoard.style.backgroundImage = new StyleBackground(gameBoardSprite);
        for (int y = 0; y < 15; y++)
        {
            VisualElement column = UIGenerate.VisualElement(gameBoard, 240, 16, FlexDirection.Row);
            for (int x = 0; x < 15; x++)
            {
                Button btn = UIGenerate.Button(column, "");
                btn.ClearClassList();
                btn.style.width = 24;
                btn.style.height = 24;
                btn.style.fontSize = 8;
                if (x == 6 && y > 8)
                {
                    buttonDictionary.Add(14 - y, btn);
                    int tmp = 14 - y;
                    btn.clicked += () =>
                    {
                        Debug.Log(tmp);
                        MovePiece(tmp);
                    };
                    btn.text = (14 - y).ToString();
                    //btn.style.backgrouunf
                    //btn.style.backgroundImage = new StyleBackground(redPieceSprite);
                }
                if (x == 1 && y == 10)
                {
                    buttonDictionary.Add(-1, btn);
                    btn.text = (-1).ToString();
                    btn.clicked += () =>
                    {
                        Debug.Log(-1);
                        MovePiece(-1);
                    };
                }
                if (x == 1 && y == 12)
                {
                    buttonDictionary.Add(-2, btn);
                    btn.text = (-2).ToString();
                    btn.clicked += () =>
                    {
                        Debug.Log(-2);
                        MovePiece(-2);
                    };
                }
                if (x == 3 && y == 10)
                {
                    buttonDictionary.Add(-3, btn);
                    btn.text = (-3).ToString();
                    btn.clicked += () =>
                    {
                        Debug.Log(-3);
                        MovePiece(-3);
                    };

                }
                if (x == 3 && y == 12)
                {
                    buttonDictionary.Add(-4, btn);
                    btn.text = (-4).ToString();
                    btn.clicked += () =>
                    {
                        Debug.Log(-4);
                        MovePiece(-4);
                    };
                }
                if (x == 10 && y == 10)
                {
                    buttonDictionary.Add(-41, btn);
                    btn.text = (-41).ToString();
                    btn.clicked += () =>
                    {
                        Debug.Log(-41);
                        MovePiece(-41);
                    };
                    //btn.style.backgroundImage = new StyleBackground(bluePieceSprite);
                }
                if (x == 10 && y == 12)
                {
                    buttonDictionary.Add(-42, btn);
                    btn.text = (-42).ToString();
                    btn.clicked += () =>
                    {
                        Debug.Log(-42);
                        MovePiece(-42);
                    };
                    //btn.style.backgroundImage = new StyleBackground(bluePieceSprite);
                }
                if (x == 12 && y == 10)
                {
                    buttonDictionary.Add(-43, btn);
                    btn.text = (-43).ToString();
                    btn.clicked += () =>
                    {
                        Debug.Log(-43);
                        MovePiece(-43);
                    };
                    //btn.style.backgroundImage = new StyleBackground(bluePieceSprite);

                }
                if (x == 12 && y == 12)
                {
                    buttonDictionary.Add(-44, btn);
                    btn.text = (-44).ToString();
                    btn.clicked += () =>
                    {
                        Debug.Log(-44);
                        MovePiece(-44);
                    };
                    //btn.style.backgroundImage = new StyleBackground(bluePieceSprite);

                }


                if (x == 1 && y == 1)
                {
                    buttonDictionary.Add(-15, btn);
                    btn.text = (-15).ToString();
                    btn.clicked += () =>
                    {
                        Debug.Log(-15);
                        MovePiece(-15);
                    };
                    //btn.style.backgroundImage = new StyleBackground(bluePieceSprite);
                }
                if (x == 1 && y == 3)
                {
                    buttonDictionary.Add(-16, btn);
                    btn.text = (-16).ToString();
                    btn.clicked += () =>
                    {
                        Debug.Log(-16);
                        MovePiece(-16);
                    };
                    //btn.style.backgroundImage = new StyleBackground(bluePieceSprite);
                }
                if (x == 3 && y == 1)
                {
                    buttonDictionary.Add(-17, btn);
                    btn.text = (-17).ToString();
                    btn.clicked += () =>
                    {
                        Debug.Log(-17);
                        MovePiece(-17);
                    };
                    // btn.style.backgroundImage = new StyleBackground(bluePieceSprite);

                }
                if (x == 3 && y == 3)
                {
                    buttonDictionary.Add(-18, btn);
                    btn.text = (-18).ToString();
                    btn.clicked += () =>
                    {
                        Debug.Log(-18);
                        MovePiece(-18);
                    };
                    //btn.style.backgroundImage = new StyleBackground(bluePieceSprite);

                }


                if (x == 10 && y == 1)
                {
                    buttonDictionary.Add(-28, btn);
                    btn.text = (-28).ToString();
                    btn.clicked += () =>
                    {
                        Debug.Log(-28);
                        MovePiece(-28);
                    };
                    //btn.style.backgroundImage = new StyleBackground(bluePieceSprite);
                }
                if (x == 10 && y == 3)
                {
                    buttonDictionary.Add(-29, btn);
                    btn.text = (-29).ToString();
                    btn.clicked += () =>
                    {
                        Debug.Log(-29);
                        MovePiece(-29);
                    };
                    //btn.style.backgroundImage = new StyleBackground(bluePieceSprite);
                }
                if (x == 12 && y == 1)
                {
                    buttonDictionary.Add(-30, btn);
                    btn.text = (-30).ToString();
                    btn.clicked += () =>
                    {
                        Debug.Log(-30);
                        MovePiece(-30);
                    };
                    //btn.style.backgroundImage = new StyleBackground(bluePieceSprite);

                }
                if (x == 12 && y == 3)
                {
                    buttonDictionary.Add(-31, btn);
                    btn.text = (-31).ToString();
                    btn.clicked += () =>
                    {
                        Debug.Log(-31);
                        MovePiece(-31);
                    };
                    //btn.style.backgroundImage = new StyleBackground(bluePieceSprite);

                }


                if (x < 6 && y == 8)
                {
                    buttonDictionary.Add(11 - x, btn);
                    btn.text = (11 - x).ToString();
                    int tmp = 11 - x;
                    btn.clicked += () =>
                    {
                        Debug.Log(tmp);
                        MovePiece(tmp);
                    };
                    //btn.style.backgroundImage = new StyleBackground(bluePieceSprite);
                }
                if (x < 6 && y == 6)
                {
                    buttonDictionary.Add(13 + x, btn);
                    btn.text = (13 + x).ToString();
                    int tmp = 13 + x;
                    btn.clicked += () =>
                    {
                        Debug.Log(tmp);
                        MovePiece(tmp);
                    };
                    //btn.style.backgroundImage = new StyleBackground(bluePieceSprite);
                }

                if (x == 6 && y < 6)
                {
                    buttonDictionary.Add(24 - y, btn);
                    btn.text = (24 - y).ToString();
                    int tmp = 24 - y;
                    btn.clicked += () =>
                    {
                        Debug.Log(tmp);
                        MovePiece(tmp);
                    };
                    //btn.style.backgroundImage = new StyleBackground(yellowPieceSprite);
                }
                if (x == 8 && y < 6)
                {
                    buttonDictionary.Add(26 + y, btn);
                    btn.text = (26 + y).ToString();
                    int tmp = 26 + y;
                    btn.clicked += () =>
                    {
                        Debug.Log(tmp);
                        MovePiece(tmp);
                    };
                    //btn.style.backgroundImage = new StyleBackground(yellowPieceSprite);
                }

                if (x > 8 && y == 6)
                {
                    buttonDictionary.Add(23 + x, btn);
                    btn.text = (23 + x).ToString();
                    int tmp = 23 + x;
                    btn.clicked += () =>
                    {
                        Debug.Log(tmp);
                        MovePiece(tmp);
                    };
                    //btn.style.backgroundImage = new StyleBackground(greenPieceSprite);
                }
                if (x > 8 && y == 8)
                {
                    buttonDictionary.Add(53 - x, btn);
                    btn.text = (53 - x).ToString();
                    int tmp = 53 - x;
                    btn.clicked += () =>
                    {
                        Debug.Log(tmp);
                        MovePiece(tmp);
                    };
                    //btn.style.backgroundImage = new StyleBackground(greenPieceSprite);
                }

                if (x == 8 && y > 8)
                {
                    buttonDictionary.Add(36 + y, btn);
                    btn.text = (36 + y).ToString();
                    int tmp = 36 + y;
                    btn.clicked += () =>
                    {
                        Debug.Log(tmp);
                        MovePiece(tmp);
                    };
                    //btn.style.backgroundImage = new StyleBackground(redPieceSprite);
                }
                if (x == 7 && y > 7 && y < 15)
                {
                    buttonDictionary.Add(65 - y, btn);
                    btn.text = (65 - y).ToString();
                    int tmp = 65 - y;
                    btn.clicked += () =>
                    {
                        Debug.Log(tmp);
                        MovePiece(tmp);
                    };
                }
                if (y == 7 && x == 0)
                {
                    buttonDictionary.Add(12, btn);
                    btn.text = (12).ToString();
                    btn.clicked += () =>
                    {
                        Debug.Log(12);
                        MovePiece(12);
                    };
                }
                if (y == 7 && x > 0 && x < 7)
                {
                    buttonDictionary.Add(64 + x, btn);
                    btn.text = (64 + x).ToString();
                    int tmp = 64 + x;
                    btn.clicked += () =>
                    {
                        Debug.Log(tmp);
                        MovePiece(tmp);
                    };
                }
                if (y == 0 && x == 7)
                {
                    buttonDictionary.Add(25, btn);
                    btn.text = (25).ToString();
                    btn.clicked += () =>
                    {
                        Debug.Log(25);
                        MovePiece(25);
                    };
                }
                if (x == 7 && y > 0 && y < 7)
                {
                    buttonDictionary.Add(77 + y, btn);
                    btn.text = (77 + y).ToString();
                    int tmp = 77 + y;
                    btn.clicked += () =>
                    {
                        Debug.Log(tmp);
                        MovePiece(tmp);
                    };
                }
                if (y == 7 && x == 14)
                {
                    buttonDictionary.Add(38, btn);
                    btn.text = (38).ToString();
                    btn.clicked += () =>
                    {
                        Debug.Log(38);
                        MovePiece(38);
                    };
                }
                if (y == 7 && x > 7 && x < 14)
                {
                    buttonDictionary.Add(97 - x + 7, btn);
                    btn.text = (97 - x + 7).ToString();
                    int tmp = 97 - x + 7;
                    btn.clicked += () =>
                    {
                        Debug.Log(tmp);
                        MovePiece(tmp);
                    };
                }
                btn.clicked += () =>
                {
                    Debug.Log("Test");
                };
            }
        }

        UpdateUI();
        //Button terningBtn = new Button();
        //terningBtn.text = "Terning";
        rollButton = UIGenerate.Button(VEdice, "Roll");
        rollButton.clicked += () =>
        {
            System.Random rnd = new System.Random();
            dice = rnd.Next(1, 7);
            //game.Move(dice, CurrentState);
            VEdice.Clear();
            VEdice.Add(UIGenerate.ShowDice(dice));
            LudoPlayer ludoPlayer = game.GetLudoPlayers().Where(x => (x.GetPlayerState() == CurrentState)).First();
            if (game.GetMoveablePieces(ludoPlayer.GetColor(), dice).ToList().Count == 0)
            {
                MoveNext(nextPlayer);
            }
            
            //MoveNext(nextPlayer);
        };
        //root.Add(terningBtn);
        //MoveNext(nextPlayer);
    }
    private void MovePiece(int button)
    {
        LudoPlayer ludoPlayer = game.GetLudoPlayers().Where(x => (x.GetPlayerState() == CurrentState)).First();
        List<LudoPiece> pieces = game.GetMoveablePieces(ludoPlayer.GetColor(), dice).ToList();
        Debug.Log("COLOR: " + ludoPlayer.GetColor());
        //Debug.Log(pieces);
        foreach (LudoPiece piece in pieces) {
            Debug.Log("ABS:     " + piece.GetAbsolutePosition());
            Debug.Log("PIECE:   " + (button + piece.GetOffset()));
            Debug.Log("PIECE+3: " + (button + piece.GetOffset() + 3));
            if (piece.GetAbsolutePosition() == button) 
            {
                game.MovePiece(piece, dice);
                MoveNext(nextPlayer);
                break;
            }else if(((button + piece.GetOffset()) <= piece.GetAbsolutePosition()) && ((button + piece.GetOffset()) >= (piece.GetAbsolutePosition()-3))) {
                game.MovePiece(piece, dice);
                MoveNext(nextPlayer);
                break;
            }
        
        }

    }
    private void ResetVEdice()
    {
        VEdice.Clear();
        VEdice.Add(rollButton);
    }
    private void UpdateUI()
    {
        ClearUI();
        ResetVEdice();
        foreach (LudoPiece piece in game.GetLudoPieces())
        {
            if (piece.GetAbsolutePosition() == -1)
            {
                for (int i = 0; i < 4; i++) {
                    //Debug.Log(buttonDictionary[piece.GetAbsolutePosition() - (piece.GetOffset() + i)].style.backgroundImage);
                    //Debug.Log(piece.GetAbsolutePosition() - (piece.GetOffset() + i));
                    if (buttonDictionary[piece.GetAbsolutePosition() - (piece.GetOffset() + i)].style.backgroundImage != new StyleBackground(GetSprite(piece.GetOffset())))
                    {
                        //Debug.Log("Add sprite");
                        buttonDictionary[piece.GetAbsolutePosition() - (piece.GetOffset() + i)].style.backgroundImage = new StyleBackground(GetSprite(piece.GetOffset()));
                        break;
                    }
                }

            }else if(piece.GetAbsolutePosition() >= 52)
            {
                buttonDictionary[piece.GetAbsolutePosition() + piece.GetOffset() - 1].style.backgroundImage = new StyleBackground(GetSprite(piece.GetOffset()));
            }
            else
            {
                if (buttonDictionary[piece.GetAbsolutePosition()].style.backgroundImage != null)
                {
                    //Try to add 2 images?
                    buttonDictionary[piece.GetAbsolutePosition()].style.backgroundImage = new StyleBackground(GetSprite(piece.GetOffset()));
                }
                else
                {
                    buttonDictionary[piece.GetAbsolutePosition()].style.backgroundImage = new StyleBackground(GetSprite(piece.GetOffset()));
                }
            }
        }
    }

    public void ClearUI()
    {
        foreach(var button in buttonDictionary)
        {
            buttonDictionary[button.Key].style.backgroundImage = null;

        }
    }

    private Sprite GetSprite(int offset)
    {
        switch (offset)
        {
            case 1:
                return bluePieceSprite;
            case 14:
                return redPieceSprite;
            case 27:
                return greenPieceSprite;
            case 40:
                return yellowPieceSprite;
            default:
                return null;
        }
        return null;
    }
    private int AddPlayer()
    {
        int count;
        count = game.AddLudoPlayer(new LudoPlayer("DEMO", PlayerColors[game.GetLudoPlayers().Count + 1], false, new State("player" + (game.GetLudoPlayers().Count + 1))));
        /*game.ludoPlayers.Add();
        //Debug.Log(ludoPlayers[ludoPlayers.Count-1].GetColor());
        game.CreatePieces(game.ludoPlayers[game.ludoPlayers.Count - 1].GetColor());*/
        return count;
        
    }
}



public class LudoGame
{
    
    //private Dictionary<int, ludo_piece> ludo_pieces = new Dictionary<int, ludo_piece>();
    private readonly Dictionary<int, string> colors = new Dictionary<int, string>() {
        {1,"Blue" },
        {14,"Red" },
        {27,"Green" },
        {40,"Yellow" }
    };
    private List<LudoPiece> ludoPieces = new List<LudoPiece>();
    private List<LudoPlayer> ludoPlayers = new List<LudoPlayer>();
    private int ColorNameToKey(string color)
    {
        return colors.FirstOrDefault(x => x.Value == color).Key;
    }
    private int GetAbsPos(int pos)
    {
        int tempPos;
        if(pos >= 0)
        {
            return pos;
        }
        else
        {
            tempPos = 51 - (pos - 1);
            if(tempPos < 0)
            {
                return GetAbsPos(tempPos);
            }
            else
            {
                return tempPos;
            }
        }
    }
    private int CFPI(LudoPiece piece)//Check for piece intersection
    {
        if(piece.GetAbsolutePosition() > 51)//Check if piece is on the final tiles 
        {
            return 0;
        }
        //add check to se if tile is a safe spot
        List<LudoPiece> pieces = ludoPieces.Where(x => (x.GetOffset() != piece.GetOffset()) && x.GetAbsolutePosition() != -1).ToList();

        //Check each piece loaction to se if the intersect
        foreach (LudoPiece cPiece in pieces)
        {
            if (cPiece.GetAbsolutePosition() != -1)//Check if piece is HOME(NOT necessary any more)
            {
                if (cPiece.GetAbsolutePosition() == piece.GetAbsolutePosition())//Check if the moved piece intersect with another piece
                {
                    List<LudoPiece> morePieces = ludoPieces.Where(x => (x.GetOffset() == cPiece.GetOffset() && x.GetAbsolutePosition() == cPiece.GetAbsolutePosition())).ToList();
                    if (morePieces.Count() > 1)//Check to see if the there is more than one piece on the same tile
                    {
                        piece.SetAbsolutePosition(-1);//Throw the moved piece HOME
                    }
                    else
                    {
                        cPiece.SetAbsolutePosition(-1);//Throw other piece HOME
                    }
                }
            }
        }
        return 0;
    }
    public int AddLudoPlayer(LudoPlayer ludoPlayer)
    {
        ludoPlayers.Add(ludoPlayer);
        CreatePieces(ludoPlayers[ludoPlayers.Count - 1].GetColor());
        return ludoPlayers.Count;

    }
    public List<LudoPlayer> GetLudoPlayers()
    {
        return ludoPlayers;
    }
    public int SetLudoPlayers(List<LudoPlayer> LudoPlayers)
    {
        ludoPlayers = LudoPlayers;
        return 0;
    }
    public List<LudoPiece> GetLudoPieces()
    {
        return ludoPieces;
    }
    public void StartGame()
    {
        Debug.Log("START GAME");
    }
    public int CreatePieces(string color) //Returns player ID
    {
        for(int i = 0;i < 4; i++)
        {
            ludoPieces.Add(new LudoPiece(-1, ColorNameToKey(color)));
        }
        return 0;
    }
    public int MovePiece(LudoPiece piece, int diceSum)
    {
        int maxpos = 57;
        int finishTiles = 5; 
        if (piece.GetAbsolutePosition() == -1)//Check if piece is home
        {
            piece.SetAbsolutePosition(piece.GetOffset());//Move piece to offset
        }
        else if (piece.GetAbsolutePosition() <= 51 & piece.GetAbsolutePosition()+diceSum > 51 && piece.GetOffset() != 1)
        {
            piece.SetAbsolutePosition(diceSum - 1);
        }
        else
        {
            int abspos = piece.GetAbsolutePosition();
            int dice = diceSum;
            if(piece.GetOffset() != 1 && piece.GetAbsRelativePosition() + diceSum > 51 && piece.GetAbsolutePosition() < 52)//Use Piece relative POS 
            {
                dice -= 52 - piece.GetAbsRelativePosition();
                piece.SetAbsolutePosition(52);
                
            }
            do
            {
                abspos = piece.GetAbsolutePosition();
                if (abspos == maxpos)
                {
                    if (dice > finishTiles)
                    {
                        piece.SetAbsolutePosition(abspos - finishTiles);
                        dice -= finishTiles;
                    }
                    else
                    {
                        piece.SetAbsolutePosition(abspos - dice);
                        dice = 0;
                    }
                }
                else
                {
                    if (dice + abspos > maxpos)
                    {
                        dice -= (maxpos - abspos);
                        piece.SetAbsolutePosition(maxpos);
                    }
                    else
                    {
                        piece.SetAbsolutePosition(abspos + dice);
                        dice = 0;

                    }
                }
            } while (dice != 0);
        }
        CFPI(piece);
        PrintPieces();
        return 0;
    }
    public List<LudoPiece> GetMoveablePieces(string color, int diceSum)
    {
        //remove pieces that are done/finish
        List<LudoPiece> pieces = ludoPieces.Where(x => x.GetOffset() == ColorNameToKey(color)).ToList();
        if (diceSum == 6)
        {
            return pieces.Where(x => x.GetAbsolutePosition() != 57).ToList();
        }
        else if(diceSum < 6)
        {
            return pieces.Where(x => x.GetAbsolutePosition() != -1 && x.GetAbsolutePosition() != 57).ToList();
        }
        return null;
    }
    public void PrintPieces()
    {
        foreach (LudoPiece piece in ludoPieces)
        {
            Debug.Log("Offset: " + piece.GetOffset());
            Debug.Log("ABS:    " + piece.GetAbsolutePosition());
            Debug.Log("RelABS: " + piece.GetAbsRelativePosition());
        }
    }
    
    public void Move(int dice, StateMachine.State currentState)
    {
        Debug.Log("DICE: " + dice);
        Debug.Log("Current Player: " + currentState.ToString());
        LudoPlayer cPlayer = ludoPlayers.Where(x => x.GetPlayerState() == currentState).First();
        Debug.Log("Player color: " + cPlayer.GetColor());
        List<LudoPiece> pieceList = GetMoveablePieces(cPlayer.GetColor(), dice);
        if (pieceList.Count > 0)
        {
            //Make user select
            MovePiece(pieceList.First(), dice);
        }
        else
        {
            Debug.Log("NO MOVE");
        }

    }

}

public class LudoPlayer
{     private string name;
    private string color;
    private bool aiPlayer;
    private StateMachine.State playerState;
    //private List<LudoPiece> playerPieces = new List<LudoPiece>();

    public LudoPlayer(string name, string color, bool aiPlayer, StateMachine.State playerState)
    {
        this.name = name;
        this.color = color;
        this.aiPlayer = aiPlayer;
        this.playerState = playerState;
    }
    public string GetColor()
    {
        return color;
    }
    public string GetName()
    {
        return name;
    }
    public void SetColor(string color)
    {
        this.color=color;
    }
    public StateMachine.State GetPlayerState()
    {
        return playerState;
    }
    
    public int SetPlayerState(StateMachine.State playerState)
    {
        this.playerState = playerState;
        return 0;
    }
    
}
public class LudoPiece
{
    private int absolutepPosition;
    private int offset;
    //private bool home;

    public LudoPiece(int absolutePosition, int offset)
    {
        this.absolutepPosition = absolutePosition;
        this.offset = offset;
    }

    public int GetAbsolutePosition()
    {
        return absolutepPosition;
    }

    public int SetAbsolutePosition(int absolutePosition)
    {
        this.absolutepPosition = absolutePosition;
        return this.absolutepPosition;
    }
    public int GetOffset()
    {
        return offset;
    }
    public int SetOffset(int offset)
    {
        this.offset = offset;
        return this.offset;
    }
    public int GetAbsRelativePosition()
    {
        if(absolutepPosition == -1)
        {
            return -1;
        }
        else
        {
            if (absolutepPosition < offset)
            {
                return 51 - ((offset - 2) - absolutepPosition);
            }
            else
            {
                return absolutepPosition - (offset-1);
            }
        }
        
    }
}

