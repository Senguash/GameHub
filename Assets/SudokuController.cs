using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEditor.Experimental;
using UnityEngine;
using UnityEngine.UIElements;

public class SudokuController : Game
{
    Label errorsLabel;
    int errorsRemaining = 3;
    VisualElement gameBoard;
    Dictionary<Tuple<int, int>, Button> buttonDictionary;
    Dictionary<Tuple<int, int>, VisualElement> veDictionary;
    Dictionary<Tuple<int, int>, Label> labelDictionary;
    Label selectedNumberLabel;
    Tuple<int, int> selectedSpace;

    Sudoku sudoku;
    // Start is called before the first frame update
    void Start()
    {
        sudoku = new Sudoku();
        sudoku.GenerateRandom();
        InitSudokuUI();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            Vector2 touchPosition = Camera.main.ScreenToWorldPoint(touch.position);
            if (touch.phase == TouchPhase.Began)
            {
                UpdateUI();
                for (int i = 0; i < 9; i++)
                {
                    for (int j = 0; j < 9; j++)
                    {
                    }
                }
            }
        }
    }

    public void InitSudokuUI()
    {
        gameBoard = new VisualElement();
        gameBoard.style.flexDirection = FlexDirection.Column;
        VisualElement buffer = new VisualElement();
        VisualElement ve = new VisualElement();
        ve.style.backgroundColor = Color.gray;
        ve.style.width = Length.Percent(100);
        ve.style.height = Length.Percent(100);
        //ve.style.justifyContent = Justify.Center;
        ve.style.alignItems = Align.Center;

        buffer.style.height = Length.Percent(24);
        ve.Add(buffer);
        ve.Add(gameBoard);
        gameBoard.style.width = 216;
        gameBoard.style.height = 216;
        gameBoard.style.backgroundColor = Color.white;

        buttonDictionary = new Dictionary<Tuple<int, int>, Button>();
        veDictionary = new Dictionary<Tuple<int, int>, VisualElement>();
        labelDictionary = new Dictionary<Tuple<int, int>, Label>();

        selectedSpace = new Tuple<int, int>(0, 0);

        for (int i = 0; i < 9; i++)
        {
            VisualElement row = new VisualElement();
            row.style.width = 216;
            row.style.height = 24;
            row.style.flexDirection = FlexDirection.Row;
            row.AddToClassList("sudoku-row");
            for (int j = 0; j < 9; j++)
            {
                VisualElement v = new VisualElement();
                Button btn = new Button();
                btn.visible = true;
                btn.style.visibility = Visibility.Hidden;
                Label label = new Label();
                
                label.visible = true;

                label.style.marginTop = -1;

                v.style.justifyContent = Justify.Center;
                v.style.alignItems = Align.Center;
                label.style.visibility = Visibility.Visible;
                v.Add(label);
                v.style.visibility = Visibility.Visible;
                v.AddToClassList("sudoku-field");

                btn.Add(v);
                v.Add(label);
                int tmp_i = i;
                int tmp_j = j;
                btn.clicked += () => { DeselectSpace(selectedSpace); selectedSpace = new Tuple<int, int>(tmp_i, tmp_j); SelectSpace(selectedSpace); };
                btn.style.width = 24;
                btn.style.height = 24;
                btn.ClearClassList();
                btn.AddToClassList("sudoku-button");
                v.style.visibility = Visibility.Visible;
                row.Add(btn);

                buttonDictionary.Add(new Tuple<int, int>(i, j), btn);
                veDictionary.Add(new Tuple<int, int>(i, j), v);
                labelDictionary.Add(new Tuple<int, int>(i, j), label);
            }
            gameBoard.Add(row);
        }
        root.Add(ve);

        selectedNumberLabel = new Label();
        root.Add(selectedNumberLabel);

        VisualElement numberSelectionRow = new VisualElement();
        numberSelectionRow.style.flexDirection = FlexDirection.Row;
        numberSelectionRow.style.alignItems = Align.Center;
        for (int i = 1; i < 10; i++)
        {
            VisualElement v = new VisualElement();
            Button btn = new Button();
            btn.visible = true;
            int tmp = i;
            btn.clicked += () => { UISetValue(selectedSpace, tmp); };
            btn.text = i.ToString();
            btn.style.width = 24;
            btn.style.height = 24;
            btn.AddToClassList("sudoku-button");
            v.style.visibility = Visibility.Visible;
            //label.text = "0";

            numberSelectionRow.Add(btn);
        }
        root.Add(numberSelectionRow);

        UpdateUI();
    }

    void UpdateUI()
    {
        for (int i = 0; i < 9; i++)
        {
            for (int j = 0; j < 9; j++)
            {
                if (sudoku.nums[i, j].locked == true)
                {
                    Label l;
                    if (labelDictionary.TryGetValue(new Tuple<int, int>(i, j), out l))
                    {
                        l.style.color = new Color(0.3f, 0.3f, 0.3f); ;
                        l.text = sudoku.nums[i, j].GetRealValue().ToString();
                    }
                    VisualElement ve;
                    if (veDictionary.TryGetValue(new Tuple<int, int>(i, j), out ve))
                    {
                        ve.style.backgroundColor = new Color(0.85f, 0.85f, 1f);
                    }
                }
            }
        }
    }

    void SelectSpace(Tuple<int, int> coords)
    {
        ShowCross(coords);
        VisualElement ve;
        if (veDictionary.TryGetValue(coords, out ve))
        {
            ve.style.backgroundColor = new Color(0.6f, 0.6f, 0.6f);
        }
    }

    void DeselectSpace(Tuple<int, int> coords)
    {
        for (int i = 0; i < 9; i++)
        {
            for (int j = 0; j < 9; j++)
            {
                if (sudoku.nums[i, j].locked == false)
                {
                    VisualElement ve;
                    if (veDictionary.TryGetValue(new Tuple<int, int>(i, j), out ve))
                    {
                        ve.style.backgroundColor = Color.white;
                    }
                }
                
            }
        }
    }

    void UISetValue(Tuple<int, int> coords, int value)
    {
        Label l;
        if (labelDictionary.TryGetValue(coords, out l))
        {
            l.text = value.ToString();
        }
    }

    void ShowCross(Tuple<int, int> coords)
    {
        for (int i = 0; i < 9; i++)
        {
            VisualElement ve;
            if (veDictionary.TryGetValue(new Tuple<int, int>(coords.Item1, i), out ve))
            {
                ve.style.backgroundColor = new Color(0.85f, 0.85f, 0.85f);
            }
        }

        for (int i = 0; i < 9; i++)
        {
            VisualElement ve;
            if (veDictionary.TryGetValue(new Tuple<int, int>(i, coords.Item2), out ve))
            {
                ve.style.backgroundColor = new Color(0.85f, 0.85f, 0.85f);
            }
        }
    }
}



public class Sudoku
{
    public SudokuNumber[,] nums;
    public Sudoku() {
        nums = new SudokuNumber[9,9];
        for (int i = 0; i < 9; i++)
        {
            for (int j = 0; j < 9; j++)
            {
                nums[i, j] = new SudokuNumber(i, j);
            }
        }
    }

    public void GenerateRandom()
    {
        System.Random rand = new System.Random();
        //Placeholder
        for (int i = 0; i < 9; i++)
        {
            for (int j = 0; j < 9; j++)
            {
                nums[i, j].SetTrueValue(rand.Next(1, 9));
                if (rand.Next(0, 4) == 0)
                {
                    nums[i, j].Lock();
                }
            }
        }

    }
}


public class SudokuNumber : IEquatable<SudokuNumber>
{
    public readonly Tuple<int, int> coordinates;

    private int selectedValue;

    private int realValue;
    
    public bool locked { get; private set; }
    
    public SudokuNumber(int x, int y)
    {
        coordinates = new Tuple<int, int>(x, y);
    }

    public bool Equals (SudokuNumber other)
    {
        if (realValue == 0)
        {
            return false;
        } else if (realValue == other.realValue) 
        {
            return true;
        } else
        {
            return false;
        }
    }

    public bool IsCorrect()
    {
        if (selectedValue == realValue)
        {
            return true;
        } else
        {
            return false;
        }
    }

    public override string ToString()
    {
        if (realValue == 0)
        {
            return "";
        } else
        {
            return realValue.ToString();
        }
    }

    public int GetRealValue()
    {
        return realValue;
    }
    public void SelectValue(int value)
    {
        selectedValue = value;
    }

    public void SetTrueValue(int value)
    {
        realValue = value;
    }

    public void Lock()
    {
        locked = true;
    }
}