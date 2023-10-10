using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.UIElements;

public class BackgammonController : Game
{
    new public string Name = "Backgammon";
    Sprite blackPieceSprite;
    Sprite whitePieceSprite;
    Dictionary<int, VisualElement> veDictionary;
    Dictionary<int, Button> btnDictionary;
    Backgammon backgammon;
    Button rollButton;
    VisualElement rollSection;
    Label turnLabel;
    State whiteRollPhase = new State("whiteRollPhase");
    State blackRollPhase = new State("blackRollPhase");
    State whiteMovePhase = new State("whiteMovePhase");
    State blackMovePhase = new State("blackMovePhase");
    Command advanceGame = new Command("changeTurn");
    List<int> availableDiceRolls = new List<int>();
    int selectedSpace;
    bool activeSelection;

    bool isRolling = false;
    int rollCounter;
    int rollTimer;

    void Start()
    {
        blackPieceSprite = Resources.Load<Sprite>("Backgammon/BackgammonPieceBlack");
        whitePieceSprite = Resources.Load<Sprite>("Backgammon/BackgammonPieceWhite");
        veDictionary = new Dictionary<int, VisualElement>();
        btnDictionary = new Dictionary<int, Button>();
        backgammon = new Backgammon();

        whiteRollPhase.EnterEvent += ResetRollSection;
        whiteRollPhase.EnterEvent += () =>
        {
            turnLabel.text = "Whites turn";
            turnLabel.style.color = Color.white;
        };
        blackRollPhase.EnterEvent += ResetRollSection;
        blackRollPhase.EnterEvent += () =>
        {
            turnLabel.text = "Blacks turn";
            turnLabel.style.color = Color.black;
        };
        whiteMovePhase.EnterEvent = ShowDiceInRollSection;
        blackMovePhase.EnterEvent = ShowDiceInRollSection;

        InitCoreGameUI();
        DrawPiecesOnBoard();

        SetInitialState(whiteRollPhase);

        transitions = new Dictionary<StateTransition, State>()
        {
            { new StateTransition(whiteRollPhase, advanceGame), whiteMovePhase },
            { new StateTransition(whiteMovePhase, advanceGame), blackRollPhase },
            { new StateTransition(blackRollPhase, advanceGame), blackMovePhase },
            { new StateTransition(blackMovePhase, advanceGame), whiteRollPhase },
        };
    }

    private void InitCoreGameUI()
    {
        Sprite background = Resources.Load<Sprite>("Backgammon/BackgammonBoard");

        VisualElement gameVE = UIGenerate.VisualElement(root, Length.Percent(100), Length.Percent(95));
        VisualElement turnLabelContainer = UIGenerate.VisualElement(gameVE, Length.Percent(100), Length.Percent(10), FlexDirection.Row, Align.Center);
        turnLabel = UIGenerate.Label(turnLabelContainer, "", 18);

        VisualElement gameBoard = UIGenerate.VisualElement(gameVE, 216, 288, FlexDirection.Column);
        gameBoard.style.backgroundImage = new StyleBackground(background);

        rollSection = UIGenerate.VisualElement(gameVE, Length.Percent(100), Length.Percent(15), FlexDirection.Row, Align.Center);
        rollButton = UIGenerate.Button(rollSection, "Roll");
        rollButton.clicked += () =>
        {
            if (IsRollPhase())
            {
                RollDice();
            } 
        };

        VisualElement gameBoardTop = UIGenerate.VisualElement(gameBoard, Length.Percent(100), Length.Percent(4));
        //UIGenerate.Button(gameBoardTop, "Test1");
        VisualElement gameBoardTopSection = UIGenerate.VisualElement(gameBoard, Length.Percent(100), Length.Percent(43), FlexDirection.Row);

        Button middleButton = UIGenerate.Button(gameBoard);
        middleButton.ClearClassList();
        middleButton.style.width = Length.Percent(100);
        middleButton.style.height   = Length.Percent(6);
        middleButton.clicked += () =>
        {
            FieldClicked(0);
        };
        btnDictionary.Add(0, middleButton);
        VisualElement gameBoardMiddle = UIGenerate.VisualElement(middleButton, Length.Percent(100), Length.Percent(100), FlexDirection.Row, Align.Center);
        veDictionary.Add(0, gameBoardMiddle);

        //UIGenerate.Button(gameBoardMiddle, "Middle");
        VisualElement gameBoardBottomSection = UIGenerate.VisualElement(gameBoard, Length.Percent(100), Length.Percent(43), FlexDirection.Row);
        VisualElement gameBoardBottom = UIGenerate.VisualElement(gameBoard, Length.Percent(100), Length.Percent(4));

        VisualElement gameBoardTopLeft = UIGenerate.VisualElement(gameBoardTopSection, Length.Percent(40), Length.Percent(100), FlexDirection.Column);
        VisualElement gameBoardTopCenter = UIGenerate.VisualElement(gameBoardTopSection, Length.Percent(20), Length.Percent(100));
        VisualElement gameBoardTopRight = UIGenerate.VisualElement(gameBoardTopSection, Length.Percent(40), Length.Percent(100), FlexDirection.Column);

        VisualElement gameBoardBottomLeft = UIGenerate.VisualElement(gameBoardBottomSection, Length.Percent(40), Length.Percent(100), FlexDirection.Column);
        VisualElement gameBoardBottomCenter = UIGenerate.VisualElement(gameBoardBottomSection, Length.Percent(20), Length.Percent(100));
        VisualElement gameBoardBottomRight = UIGenerate.VisualElement(gameBoardBottomSection, Length.Percent(40), Length.Percent(100), FlexDirection.Column);

        for (int i = 0; i < 6; i++)
        {

            GenerateBackgammonFieldButton(gameBoardTopLeft, 24 - i);
            GenerateBackgammonFieldButton(gameBoardTopRight, 1 + i);
            GenerateBackgammonFieldButton(gameBoardBottomLeft, 18 - i);
            GenerateBackgammonFieldButton(gameBoardBottomRight, 7 + i);
            /*
            VisualElement ve1 = UIGenerate.VisualElement(gameBoardTopLeft, Length.Percent(100), Length.Percent(16.66f), FlexDirection.Row);
            veDictionary.Add((24 - i), ve1);
            Button btn1 = UIGenerate.Button(ve1, (24 - i).ToString());
            btn1.style.flexDirection = FlexDirection.Row;
            btn1.style.alignItems = Align.Center;
            btnDictionary.Add((24 - i), btn1);
            btn1.ClearClassList();
            btn1.style.width = Length.Percent(100);
            btn1.style.height = Length.Percent(100);
            btn1.clicked += () =>
            {
                
            };

            VisualElement ve2 = UIGenerate.VisualElement(gameBoardTopRight, Length.Percent(100), Length.Percent(16.66f), FlexDirection.RowReverse);
            veDictionary.Add((1 + i), ve2);
            Button btn2 = UIGenerate.Button(ve2, (1 + i).ToString());
            btn2.style.flexDirection = FlexDirection.RowReverse;
            btn2.style.alignItems = Align.Center;
            btnDictionary.Add((1 + i), btn2);
            btn2.ClearClassList();
            btn2.style.width = Length.Percent(100);
            btn2.style.height = Length.Percent(100);


            VisualElement ve3 = UIGenerate.VisualElement(gameBoardBottomLeft, Length.Percent(100), Length.Percent(16.66f), FlexDirection.Row);
            veDictionary.Add((18 - i), ve3);
            Button btn3 = UIGenerate.Button(ve3, (18 - i).ToString());
            btn3.style.flexDirection = FlexDirection.Row;
            btn3.style.alignItems = Align.Center;
            btnDictionary.Add((18 - i), btn3);
            btn3.ClearClassList();
            btn3.style.width = Length.Percent(100);
            btn3.style.height = Length.Percent(100);


            VisualElement ve4 = UIGenerate.VisualElement(gameBoardBottomRight, Length.Percent(100), Length.Percent(16.66f), FlexDirection.RowReverse);
            veDictionary.Add((7 + i), ve2);
            Button btn4 = UIGenerate.Button(ve4, (7 + i).ToString());
            btn4.style.flexDirection = FlexDirection.RowReverse;
            btn4.style.alignItems = Align.Center;
            btnDictionary.Add((7 + i), btn4);
            btn4.ClearClassList();
            btn4.style.width = Length.Percent(100);
            btn4.style.height = Length.Percent(100);*/
        }
        void GenerateBackgammonFieldButton(VisualElement parent, int pos)
        {
            Button btn = UIGenerate.Button(parent, "");
            //btn.style.flexDirection = FlexDirection.Row;
            //btn.style.alignItems = Align.Center;
            btnDictionary.Add(pos, btn);
            btn.ClearClassList();
            btn.style.width = Length.Percent(100);
            btn.style.height = Length.Percent(16.66f);
            int tmp_pos = pos;
            btn.clicked += () =>
            {
                FieldClicked(tmp_pos);
            };
            VisualElement ve;
            if (pos >= 13)
            {
                ve = UIGenerate.VisualElement(btn, Length.Percent(100), Length.Percent(100), FlexDirection.Row, Align.Center, Justify.FlexStart);
            } else
            {
                ve = UIGenerate.VisualElement(btn, Length.Percent(100), Length.Percent(100), FlexDirection.RowReverse, Align.Center, Justify.FlexStart);
            }
            veDictionary.Add(pos, ve);
        }

    }
    private void ShowDiceInRollSection()
    {
        rollSection.Clear();
        foreach (int roll in availableDiceRolls)
        {
            rollSection.Add(UIGenerate.ShowDice(roll, 42, 8));
        }
    }
    private void ResetRollSection()
    {
        rollSection.Clear();
        rollSection.Add(rollButton);
    }
    private void FieldClicked(int tmp_pos)
    {
        if (!IsRollPhase())
        {
            if (activeSelection && (!HasToMoveFromHome() || selectedSpace == 0))
            {
                if (!TryMovePiece(tmp_pos))
                {
                    if (!TrySelectSpace(tmp_pos))
                    {
                        DrawPiecesOnBoard();
                    }
                }
                else
                {
                    ShowDiceInRollSection();
                    if (availableDiceRolls.Count == 0)
                    {
                        MoveNext(advanceGame);
                    }
                }
            }
            else
            {
                if (!TrySelectSpace(tmp_pos))
                {
                    DrawPiecesOnBoard();
                }
            }
        }
    }
    private bool HasToMoveFromHome()
    {
        List<BackgammonPiece> pieceOnSelected = backgammon.pieces.Where(x => x.GetPosition() == 0).ToList();
        foreach (BackgammonPiece piece in pieceOnSelected)
        {
            if (piece.owner == GetCurrentTurn())
            {
                return true;
            }
        }
        return false;
    }
    private bool TryMovePiece(int targetPos)
    {
        int modifier;
        if (GetCurrentTurn())
        {
            modifier = 1;
        } else
        {
            modifier = -1;
        }
        foreach (int roll in availableDiceRolls)
        {
            if ((selectedSpace + (roll * modifier) == targetPos) || ((selectedSpace == 0 && !GetCurrentTurn()) && (selectedSpace + 25 + (roll * modifier) == targetPos)) ) 
            {
                if (IsValidMove(targetPos))
                {
                    availableDiceRolls.Remove(roll);
                    backgammon.pieces.First(x => x.GetPosition() == selectedSpace).SetPosition(targetPos);
                    foreach (BackgammonPiece piece in backgammon.pieces.Where(x => x.GetPosition() == targetPos))
                    {
                        if (piece.owner != GetCurrentTurn())
                        {
                            piece.SetPosition(0);
                        }
                    }
                    activeSelection = false;
                    DrawPiecesOnBoard();
                    return true;
                }
            }
        }
        return false;
    }
    private bool IsValidMove(int targetPos)
    {
        List<BackgammonPiece> pieceOnSelected = backgammon.pieces.Where(x => x.GetPosition() == targetPos).ToList();
        if (pieceOnSelected.Count > 1)
        {
            if (pieceOnSelected[0].owner == GetCurrentTurn())
            {
                return true;
            } else
            {
                return false;
            }
        } else
        {
            return true;
        }
    }
    private bool GetCurrentTurn()
    {
        if (CurrentState == whiteRollPhase ||  CurrentState == whiteMovePhase)
        {
            return true; //White turn
        } else
        {
            return false; //Black turn
        }
    }

    private bool IsRollPhase()
    {
        if (CurrentState == whiteRollPhase || CurrentState == blackRollPhase)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private bool TrySelectSpace(int pos)
    {
        List<BackgammonPiece> pieceOnSelected = backgammon.pieces.Where(x => x.GetPosition() == pos).ToList();
        if (pieceOnSelected.Count > 0)
        {
            if (pieceOnSelected.Where(x => x.owner == GetCurrentTurn()).Any())
            {
                selectedSpace = pos;
                //Debug.Log("Selected " + pos);
                VisualElement ve;
                DrawPiecesOnBoard();
                if (veDictionary.TryGetValue(pos, out ve))
                {
                    ve.style.backgroundColor = new Color(0f, 0f, 1f, 0.5f);
                }
                DrawPossibleMoves();
                activeSelection = true;
                return true;
            }
        }
        activeSelection = false;
        return false;
    }

    private void DrawPossibleMoves()
    {
        int modifier;
        if (GetCurrentTurn())
        {
            modifier = 1;
        }
        else
        {
            modifier = -1;
        }
        for (int i = 1; i < 25; i++)
        {
            foreach (int roll in availableDiceRolls)
            {
                if ((selectedSpace + (roll * modifier) == i) || ((selectedSpace == 0 && !GetCurrentTurn()) && (selectedSpace + 25 + (roll * modifier) == i)))
                {
                    if (IsValidMove(i) && (selectedSpace == 0 || !HasToMoveFromHome()))
                    {
                        VisualElement ve;
                        if (veDictionary.TryGetValue(i, out ve))
                        {
                            ve.style.backgroundColor = new Color(1f, 0.8f, 0f, 0.5f);
                        }
                    }
                }
            }
        }
    }
    private void DrawPiecesOnBoard()
    {
        for (int i = 0; i < 25; i++)
        {
            int pieceCount = backgammon.pieces.Where(x =>  x.GetPosition() == i).Count();
            VisualElement ve;
            if (veDictionary.TryGetValue(i, out ve))
            {
                ve.Clear();
                ve.style.backgroundColor = new Color(1f, 1f, 1f, 0f);
                UIGenerate.VisualElement(ve, 8, Length.Percent(100));
                foreach (BackgammonPiece piece in backgammon.pieces.Where(x => x.GetPosition() == i))
                {
                    Image img = new Image();
                    img.style.width = 16;
                    img.style.height = 16;
                    if (piece.owner)
                    {
                        img.sprite = whitePieceSprite;
                    } else
                    {
                        img.sprite = blackPieceSprite;
                    }
                    ve.Add(img);
                }
            }
        }
    }

    private void RollDice()
    {
        isRolling = true;
        rollCounter = 15;
    }
    private void FixedUpdate()
    {
        if (isRolling)
        {
            if (rollCounter > 0)
            {
                if (rollTimer > 0)
                {
                    rollTimer--;
                }
                else
                {
                    rollTimer = 1;
                    rollCounter--;

                    availableDiceRolls.Clear();
                    System.Random rng = new System.Random();
                    int d1 = rng.Next(1, 7);
                    int d2 = rng.Next(1, 7);
                    availableDiceRolls.Add(d1);
                    availableDiceRolls.Add(d2);
                    ShowDiceInRollSection();
                }
            } else
            {
                isRolling = false;

                availableDiceRolls.Clear();
                System.Random rng = new System.Random();
                int d1 = rng.Next(1, 7);
                int d2 = rng.Next(1, 7);
                availableDiceRolls.Add(d1);
                availableDiceRolls.Add(d2);
                if (d1 == d2)
                {
                    availableDiceRolls.Add(d1);
                    availableDiceRolls.Add(d2);
                }
                MoveNext(advanceGame);
            }
        }
    }
}

public class Backgammon
{
    public List<BackgammonPiece> pieces;
    public Backgammon()
    {
        GeneratePieces();
    }
    private void GeneratePieces()
    {
        pieces = new List<BackgammonPiece>();
        for (int i = 0; i < 5; i++) {
            pieces.Add(new BackgammonPiece(false, 1));
            pieces.Add(new BackgammonPiece(false, 18));
            pieces.Add(new BackgammonPiece(true, 24));
            pieces.Add(new BackgammonPiece(true, 7));
        }
        for (int i = 0; i < 3; i++)
        {
            pieces.Add(new BackgammonPiece(false, 20));
            pieces.Add(new BackgammonPiece(true, 5));
        }
        for (int i = 0; i < 2; i++)
        {
            pieces.Add(new BackgammonPiece(false, 12));
            pieces.Add(new BackgammonPiece(true, 13));
        }
    }
}

public class BackgammonPiece
{
    public readonly bool owner;
    private int position;
    public BackgammonPiece(bool owner, int position)
    {
        this.owner = owner;
        this.position = position;
    }
    public int GetPosition()
    {
        return this.position;
    }
    public void SetPosition(int position)
    {
        this.position = position;
    }
}