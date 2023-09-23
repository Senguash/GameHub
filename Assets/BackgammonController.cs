using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class BackgammonController : Game
{
    Sprite blackPieceSprite;
    Sprite whitePieceSprite;
    Dictionary<int, VisualElement> veDictionary;
    Dictionary<int, Button> btnDictionary;
    void Start()
    {
        blackPieceSprite = Resources.Load<Sprite>("Backgammon/BackgammonPieceBlack");
        whitePieceSprite = Resources.Load<Sprite>("Backgammon/BackgammonPieceWhite");
        veDictionary = new Dictionary<int, VisualElement>();
        InitCoreGame();
    }

    private void InitCoreGame()
    {
        Sprite background = Resources.Load<Sprite>("Backgammon/BackgammonBoard");

        VisualElement gameVE = UIGenerate.VisualElement(root, Length.Percent(100), Length.Percent(95));

        VisualElement gameBoard = UIGenerate.VisualElement(gameVE, 216, 288, FlexDirection.Column);
        gameBoard.style.backgroundImage = new StyleBackground(background);

        VisualElement gameBoardTop = UIGenerate.VisualElement(gameBoard, Length.Percent(100), Length.Percent(4));
        //UIGenerate.Button(gameBoardTop, "Test1");
        VisualElement gameBoardTopSection = UIGenerate.VisualElement(gameBoard, Length.Percent(100), Length.Percent(43), FlexDirection.Row) ;
        VisualElement gameBoardMiddle = UIGenerate.VisualElement(gameBoard, Length.Percent(100), Length.Percent(6), FlexDirection.Row);
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
            VisualElement ve1 = UIGenerate.VisualElement(gameBoardTopLeft, Length.Percent(100), Length.Percent(16.66f), FlexDirection.Row);
            veDictionary.Add((24 - i), ve1);
            Button btn1 = UIGenerate.Button(ve1, (24 - i).ToString());
            btnDictionary.Add((24 - i), btn1);
            btn1.ClearClassList();
            btn1.style.width = Length.Percent(100);
            btn1.style.height = Length.Percent(100);
            VisualElement ve2 = UIGenerate.VisualElement(gameBoardTopRight, Length.Percent(100), Length.Percent(16.66f), FlexDirection.RowReverse);
            veDictionary.Add((1 + i), ve2);
            Button btn2 = UIGenerate.Button(ve2, (1 + i).ToString());
            btnDictionary.Add((1 + i), btn2);
            btn2.ClearClassList();
            btn2.style.width = Length.Percent(100);
            btn2.style.height = Length.Percent(100);
            VisualElement ve3 = UIGenerate.VisualElement(gameBoardBottomLeft, Length.Percent(100), Length.Percent(16.66f), FlexDirection.Row);
            veDictionary.Add((18 - i), ve3);
            Button btn3 = UIGenerate.Button(ve3, (18 - i).ToString());
            btnDictionary.Add((18 - i), btn3);
            btn3.ClearClassList();
            btn3.style.width = Length.Percent(100);
            btn3.style.height = Length.Percent(100);
            VisualElement ve4 = UIGenerate.VisualElement(gameBoardBottomRight, Length.Percent(100), Length.Percent(16.66f), FlexDirection.RowReverse);
            veDictionary.Add((7 + i), ve2);
            Button btn4 = UIGenerate.Button(ve4, (7 + i).ToString());
            btnDictionary.Add((7 + i), btn4);
            btn4.ClearClassList();
            btn4.style.width = Length.Percent(100);
            btn4.style.height = Length.Percent(100);
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
            pieces.Add(new BackgammonPiece(true, 1));
            pieces.Add(new BackgammonPiece(true, 18));
            pieces.Add(new BackgammonPiece(false, 24));
            pieces.Add(new BackgammonPiece(false, 7));
        }
        for (int i = 0; i < 3; i++)
        {
            pieces.Add(new BackgammonPiece(true, 20));
            pieces.Add(new BackgammonPiece(false, 5));
        }
        for (int i = 0; i < 2; i++)
        {
            pieces.Add(new BackgammonPiece(true, 12));
            pieces.Add(new BackgammonPiece(false, 13));
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
}