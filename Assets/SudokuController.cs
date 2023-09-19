using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Unity.Collections.LowLevel.Unsafe;
using UnityEditor.Experimental;
using UnityEditorInternal;
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
        //sudoku.GenerateRandom();
        Sudoku.FillSudoku(sudoku);
        Debug.Log(sudoku.ToString());
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
                if (sudoku.nums[i, j].locked == true || true)
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

    bool CheckLocked(Tuple<int, int> coords)
    {
        return sudoku.nums[coords.Item1, coords.Item2].locked;
    }

    void ShowCross(Tuple<int, int> coords)
    {
        List<Tuple<int, int>> pointsInSquare = SudokuHelper.GetPointsInSameSquare(coords);
        foreach (Tuple<int, int> p in pointsInSquare)
        {
            VisualElement ve;
            if (veDictionary.TryGetValue(p, out ve) && !CheckLocked(p))
            {
                ve.style.backgroundColor = new Color(0.85f, 0.85f, 0.85f);
            }
        }
        for (int i = 0; i < 9; i++)
        {
            VisualElement ve;
            Tuple<int, int> p = new Tuple<int, int>(coords.Item1, i);
            if (veDictionary.TryGetValue(p, out ve) && !CheckLocked(p))
            {
                ve.style.backgroundColor = new Color(0.85f, 0.85f, 0.85f);
            }
        }

        for (int i = 0; i < 9; i++)
        {
            VisualElement ve;
            Tuple<int, int> p = new Tuple<int, int>(i, coords.Item2);
            if (veDictionary.TryGetValue(p, out ve) && !CheckLocked(p))
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
                int newValue;
                int timeout = 0;
                do
                {
                    timeout++;
                    newValue = rand.Next(1, 10);
                } while (!TestValue(new Tuple<int, int>(i, j), newValue) && timeout < 100000);
                if (timeout > 90000)
                {
                    Debug.Log("Timeout");
                }
                nums[i, j].SetTrueValue(newValue);
                if (rand.Next(0, 4) == 0)
                {
                    nums[i, j].Lock();
                }
                nums[i, j].Lock();
                //Debug.Log(this.ToString());
            }
        }

    }
    //With help from: https://www.101computing.net/sudoku-generator-algorithm/
    public static bool FillSudoku(Sudoku sudoku)
    {
        int x = 0;
        int y = 0;
        for (int i = 0; i < 81; i++)
        {
            x = i / 9;
            y = i % 9;
            if (sudoku.nums[x, y].GetRealValue() == 0)
            {
                List<int> numbers = GetNumberList();
                foreach (int val in numbers)
                {
                    if (sudoku.TestValue(new Tuple<int, int>(x, y), val))
                    {
                        sudoku.nums[x, y].SetTrueValue(val);
                        if (CheckComplete(sudoku))
                        {
                            return true;
                        } else
                        {
                            if (FillSudoku(sudoku))
                            {
                                return true;
                            }
                        }
                    }
                }
                break;
            }  
        }
        sudoku.nums[x, y].SetTrueValue(0);
        /*if (x < 9 && y < 9)
        {
            Debug.Log("i:" + x + " j:" + y);
            
        }*/
        return false;
    }

    public static bool SolveSudoku(Sudoku sudoku, ref int counter)
    {
        int x = 0;
        int y = 0;
        for (int i = 0; i < 81; i++)
        {
            x = i / 9;
            y = i % 9;
            if (sudoku.nums[x, y].GetValue() == 0)
            {
                List<int> numbers = GetNumberList();
                foreach (int val in numbers)
                {
                    if (sudoku.TestValue(new Tuple<int, int>(x, y), val))
                    {
                        sudoku.nums[x, y].SetTrueValue(val);
                        if (CheckComplete(sudoku))
                        {
                            counter++;
                            break;
                        }
                        else
                        {
                            if (SolveSudoku(sudoku, ref counter))
                            {
                                return true;
                            }
                        }
                    }
                }
                break;
            }
        }
        sudoku.nums[x, y].SetTrueValue(0);
        /*if (x < 9 && y < 9)
        {
            Debug.Log("i:" + x + " j:" + y);
            
        }*/
        return false;
    }

    private static bool CheckComplete(Sudoku sudoku)
    {
        for (int i = 0; i < 9; i++)
        {
            for (int j = 0; j < 9; j++)
            {
                if (sudoku.nums[i, j].GetRealValue() == 0)
                {
                    return false;
                }
            }
        }
        return true;
    }

    private bool TestValue(Tuple<int, int>coords, int value)
    {
        for (int i = 0; i < 9; i++)
        {
            if (nums[coords.Item1, i].GetRealValue() == value)
            {
                return false;
            }
        }

        for (int i = 0; i < 9; i++)
        {
            if (nums[i, coords.Item2].GetRealValue() == value)
            {
                return false;
            }
        }

        List<Tuple<int, int>> pointsInSquare = SudokuHelper.GetPointsInSameSquare(coords);
        foreach (Tuple<int, int> p in pointsInSquare)
        {
            if (nums[p.Item1, p.Item2].GetRealValue() == value)
            {
                return false;
            }
        }

        return true;
    }

    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();
        sb.Append("Sudoku:" + Environment.NewLine);

        for (int j = 0; j < 9; j++)
        {
            for (int i = 0; i < 9; i++)
            {
                sb.Append(this.nums[j, i].ToString());
            }
            sb.Append(Environment.NewLine);
        }   
        return sb.ToString();
    }

    private static List<int> GetNumberList()
    {
        System.Random rand = new System.Random();
        List<int> numbers = new List<int>() { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
        var shuffled = numbers.OrderBy(i => rand.Next()).ToList();
        return shuffled;
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
        if (selectedValue == 0)
        {
            return "";
        } else
        {
            return selectedValue.ToString();
        }
    }
    public string GetRealValueAsString()
    {
        if (realValue == 0)
        {
            return "";
        }
        else
        {
            return realValue.ToString();
        }
    }

    public int GetRealValue()
    {
        return realValue;
    }
    public void SetValue(int value)
    {
        selectedValue = value;
    }

    public int GetValue()
    {
        return selectedValue;
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

public static class SudokuHelper
{
    public static List<Tuple<int, int>> GetPointsInSameSquare(Tuple<int, int> coords)
    {
        List<Tuple<int, int>> points = new List<Tuple<int, int>>();
        Tuple<int, int> x_range = GetRange(coords.Item1);
        //Debug.Log(x_range);
        Tuple<int, int> y_range = GetRange(coords.Item2);
        //Debug.Log(y_range);
        for (int i = x_range.Item1; i < x_range.Item2; i++)
        {
            for (int j = y_range.Item1; j < y_range.Item2; j++)
            {
                if (!(i == coords.Item1 && j == coords.Item2))
                {
                    points.Add(new Tuple<int, int>(i, j));
                }
            }
        }
        //Debug.Log("Points in square: "+points.Count);
        return points;
    }

    public static Tuple<int, int> GetRange(int value)
    {
        if (0 <= value && value <= 2)
        {
            return new Tuple<int, int>(0, 3);
        }
        else if (3 <= value && value <= 5)
        {
            return new Tuple<int, int>(3, 6);
        }
        else if (6 <= value && value <= 8)
        {
            return new Tuple<int, int>(6, 9);
        }
        throw new IndexOutOfRangeException();
    }
}