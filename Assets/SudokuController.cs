using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UIElements;

public class SudokuController : Game
{
    Label errorsLabel;
    int errorsRemaining = 5;
    VisualElement gameBoard;
    Dictionary<Tuple<int, int>, Button> buttonDictionary;
    Dictionary<Tuple<int, int>, VisualElement> veDictionary;
    Dictionary<Tuple<int, int>, Label> labelDictionary;
    Tuple<int, int> selectedSpace;

    Color col_selected;
    Color col_row_col_box;
    Color col_locked;
    Color col_base;
    Color col_success;
    Color col_error;

    State newOrContinue = new State();
    State difficultySelect = new State();
    State coreGame = new State();
    Command start = new Command();
    Command cont = new Command();

    Sudoku sudoku;
    // Start is called before the first frame update
    void Start()
    {
        transitions = new Dictionary<StateTransition, State>()
        {
            { new StateTransition(newOrContinue, start), difficultySelect },
            { new StateTransition(difficultySelect, start), coreGame},
            { new StateTransition(newOrContinue, cont), coreGame },
        };

        SetInitialState(newOrContinue);
        sudoku = new Sudoku();
        sudoku.GenerateRandom(45);
        Debug.Log(sudoku.ToString());


        InitSudokuUI();
    }

    // Update is called once per frame
    void Update()
    {
        /*if (Input.touchCount > 0)
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
        }*/
    }

    private void InitNewGameMenu()
    {
        bool ifGameExists = false; //To be implemented
        root.Clear();
        var ve = new VisualElement();
        root.Add(ve);
        var continueButton = new Button();
        continueButton.text = "Continue";
        continueButton.clicked += () =>
        {
            MoveNext(cont);
        };
        ve.Add(continueButton);

        continueButton.SetEnabled(ifGameExists);

        var newButton = new Button();
        newButton.text = "New Game";
        newButton.clicked += () =>
        {
            MoveNext(start);
        };
        ve.Add(newButton);
    }
    

    private void InitSudokuUI()
    {
        root.Clear();
        gameBoard = new VisualElement();
        gameBoard.style.flexDirection = FlexDirection.Column;
        VisualElement buffer = new VisualElement();
        buffer.style.alignItems = Align.Center;
        errorsLabel = new Label();
        buffer.Add(errorsLabel);
        errorsLabel.text = "Allowed Errors Remaining " + errorsRemaining.ToString();
        VisualElement ve = new VisualElement();
        root.Add(ve);
        //ve.style.backgroundColor = Color.gray;
        ve.style.width = Length.Percent(100);
        ve.style.height = Length.Percent(90);
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
                //v.AddToClassList("sudoku-field");

                btn.Add(v);
                v.Add(label);
                int tmp_i = i;
                int tmp_j = j;
                btn.clicked += () => { 
                    DeselectSpace(selectedSpace); 
                    selectedSpace = new Tuple<int, int>(tmp_i, tmp_j); 
                    SelectSpace(selectedSpace); 
                    UpdateUI();
                };
                btn.style.width = 24;
                btn.style.height = 24;
                btn.ClearClassList();
                btn.AddToClassList("sudoku-button");
                v.style.visibility = Visibility.Visible;
                if (i == 2 || i==5)
                {
                    v.style.borderBottomWidth = 2;
                } else
                {
                    v.style.borderBottomWidth = 1;
                }
                if (j == 2 || j == 5)
                {
                    v.style.borderRightWidth = 2;
                }
                else
                {
                    v.style.borderRightWidth = 1;
                }
                
                v.style.borderRightColor = Color.black;
                v.style.borderBottomColor = Color.black;
                row.Add(btn);

                buttonDictionary.Add(new Tuple<int, int>(i, j), btn);
                veDictionary.Add(new Tuple<int, int>(i, j), v);
                labelDictionary.Add(new Tuple<int, int>(i, j), label);
            }
            gameBoard.Add(row);
        }
        

        VisualElement numberSelectionRow = new VisualElement();
        numberSelectionRow.style.flexDirection = FlexDirection.Row;
        numberSelectionRow.style.justifyContent = Justify.Center;
        numberSelectionRow.style.alignItems = Align.Center;
        numberSelectionRow.style.width = Length.Percent(100);
        numberSelectionRow.style.height = Length.Percent(10);
        for (int i = 1; i < 10; i++)
        {
            VisualElement v = new VisualElement();
            Button btn = new Button();
            btn.visible = true;
            int tmp = i;
            btn.clicked += () => { 
                if (sudoku.SelectValue(selectedSpace, tmp))
                {
                    UISetValue(selectedSpace, tmp);
                } else
                {
                    errorsRemaining--;
                    Debug.Log("Wrong number");
                    errorsLabel.text = "Allowed Errors Remaining " + errorsRemaining.ToString();
                }
                UpdateUI();
            };
            btn.text = i.ToString();
            btn.style.width = 24;
            btn.style.height = 24;
            btn.AddToClassList("sudoku-button");
            v.style.visibility = Visibility.Visible;
            //label.text = "0";

            numberSelectionRow.Add(btn);
        }
        ve.Add(numberSelectionRow);

        UpdateUI();
    }

    private void UpdateUI()
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
                        l.text = sudoku.nums[i, j].GetValue().ToString();
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

    private void SelectSpace(Tuple<int, int> coords)
    {
        ShowCross(coords);
        VisualElement ve;
        if (veDictionary.TryGetValue(coords, out ve))
        {
            ve.style.backgroundColor = new Color(0.6f, 0.6f, 0.6f);
        }
    }

    private void DeselectSpace(Tuple<int, int> coords)
    {
        for (int i = 0; i < 9; i++)
        {
            for (int j = 0; j < 9; j++)
            {
                if (sudoku.nums[i, j].locked)
                {
                    VisualElement ve;
                    if (veDictionary.TryGetValue(new Tuple<int, int>(i, j), out ve))
                    {
                        ve.style.backgroundColor = new Color(0.85f, 0.85f, 1f);
                    }
                } 
                else
                {
                    VisualElement ve;
                    if (veDictionary.TryGetValue(new Tuple<int, int>(i, j), out ve))
                    {
                        if (sudoku.CheckComplete())
                        {
                            ve.style.backgroundColor = new Color(0.85f, 1f, 0.85f);
                        } else
                        {
                            ve.style.backgroundColor = Color.white;
                        }
                    }
                }
                
            }
        }
    }

    private void UISetValue(Tuple<int, int> coords, int value)
    {
        Label l;
        if (labelDictionary.TryGetValue(coords, out l))
        {
            l.text = value.ToString();
        }
    }

    private bool CheckLocked(Tuple<int, int> coords)
    {
        return sudoku.nums[coords.Item1, coords.Item2].locked;
    }

    private void ShowCross(Tuple<int, int> coords)
    {
        List<Tuple<int, int>> pointsInSquare = SudokuHelper.GetPointsInSameSquare(coords);
        foreach (Tuple<int, int> p in pointsInSquare)
        {
            VisualElement ve;
            if (veDictionary.TryGetValue(p, out ve))
            {
                if (!CheckLocked(p))
                {
                    ve.style.backgroundColor = new Color(0.85f, 0.85f, 0.85f);
                } else
                {
                    ve.style.backgroundColor = new Color(0.75f, 0.75f, 0.9f);
                }
                
            }
        }
        for (int i = 0; i < 9; i++)
        {
            VisualElement ve;
            Tuple<int, int> p = new Tuple<int, int>(coords.Item1, i);
            if (veDictionary.TryGetValue(p, out ve))
            {
                if (!CheckLocked(p))
                {
                    ve.style.backgroundColor = new Color(0.85f, 0.85f, 0.85f);
                }
                else
                {
                    ve.style.backgroundColor = new Color(0.75f, 0.75f, 0.9f);
                }
            }
        }

        for (int i = 0; i < 9; i++)
        {
            VisualElement ve;
            Tuple<int, int> p = new Tuple<int, int>(i, coords.Item2);
            if (veDictionary.TryGetValue(p, out ve))
            {
                if (!CheckLocked(p))
                {
                    ve.style.backgroundColor = new Color(0.85f, 0.85f, 0.85f);
                }
                else
                {
                    ve.style.backgroundColor = new Color(0.75f, 0.75f, 0.9f);
                }
            }
        }
    }
}



public class Sudoku
{
    public SudokuNumber[,] nums;

    private int[,] trueValues;
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

    public bool SelectValue(Tuple<int, int> coords, int value)
    {
        if (trueValues[coords.Item1, coords.Item2] == value)
        {
            this.nums[coords.Item1, coords.Item2].SetValue(value);
            return true;
        } else
        {
            return false;
        }
    }

    Sudoku Copy()
    {
        Sudoku sudoku = new Sudoku();
        for (int i = 0; i < 9; i++)
        {
            for (int j = 0; j < 9; j++)
            {
                sudoku.nums[i, j].SetValue(this.nums[i, j].GetValue());
            }
        }
        return sudoku;
    }

    public void GenerateRandom(int targetRemovedNumbers)
    {
        Sudoku.FillSudoku(this);
        trueValues = GetIntArray();
        Debug.Log(this.ToString());
        this.Reduce(targetRemovedNumbers);
    }

    public void Reduce(int targetRemovedNumbers)
    {
        int removedNumbers = 0;
        int attempts = 10;
        int[,] attemptBackup = this.Backup(); //Remove compiler error
        System.Random rand = new System.Random();
        while (attempts > 0 && removedNumbers <= targetRemovedNumbers) {

            attemptBackup = this.Backup();

            int x = rand.Next(0, 9);
            int y = rand.Next(0, 9);
            while (this.nums[x, y].GetValue() == 0)
            {
                x = rand.Next(0, 9);
                y = rand.Next(0, 9);
            }
            this.nums[x, y].SetValue(0);
            removedNumbers++;

            int counter = 0;
            int[,] checkBackup = this.Backup();
            GetSolutionCount(this, ref counter);
            if (counter != 1)
            {
                this.RestoreFromBackup(attemptBackup);
                removedNumbers--;
                attempts--;
            }
            else
            {
                this.RestoreFromBackup(checkBackup);
            }
        }
        this.RestoreFromBackup(attemptBackup);


        for (int i = 0; i < 9; i++)
        {
            for (int j = 0; j < 9; j++)
            {
                if (this.nums[i, j].GetValue() != 0)
                {
                    this.nums[i, j].Lock();
                }
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
            if (sudoku.nums[x, y].GetValue() == 0)
            {
                List<int> numbers = GetNumberList();
                foreach (int val in numbers)
                {
                    if (sudoku.TestValue(new Tuple<int, int>(x, y), val))
                    {
                        sudoku.nums[x, y].SetValue(val);
                        if (sudoku.CheckComplete())
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
        sudoku.nums[x, y].SetValue(0);
        return false;
    }

    public static bool GetSolutionCount(Sudoku sudoku, ref int counter)
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
                        sudoku.nums[x, y].SetValue(val);
                        if (sudoku.CheckComplete())
                        {
                            counter++;
                            break;
                        }
                        else
                        {
                            if (GetSolutionCount(sudoku, ref counter))
                            {
                                return true;
                            }
                        }
                    }
                }
                break;
            }
        }
        sudoku.nums[x, y].SetValue(0);
        return false;
    }

    public bool CheckComplete()
    {
        for (int i = 0; i < 9; i++)
        {
            for (int j = 0; j < 9; j++)
            {
                if (this.nums[i, j].GetValue() == 0)
                {
                    return false;
                }
            }
        }
        return true;
    }

    public int[,] Backup()
    {
        return GetIntArray();
    }

    public void RestoreFromBackup(int[,] backup) 
    {
        for (int i = 0; i < 9; i++)
        {
            for (int j = 0; j < 9; j++)
            {
                this.nums[i, j].SetValue(backup[i, j]);
            }
        }
    }

    private int[,] GetIntArray()
    {
        int[,] a = new int[9, 9];
        for (int i = 0; i < 9; i++)
        {
            for (int j = 0; j < 9; j++)
            {
                a[i, j] = this.nums[i, j].GetValue();

            }
        }
        return a;
    }

    private bool TestValue(Tuple<int, int>coords, int value)
    {
        for (int i = 0; i < 9; i++)
        {
            if (nums[coords.Item1, i].GetValue() == value)
            {
                return false;
            }
        }

        for (int i = 0; i < 9; i++)
        {
            if (nums[i, coords.Item2].GetValue() == value)
            {
                return false;
            }
        }

        List<Tuple<int, int>> pointsInSquare = SudokuHelper.GetPointsInSameSquare(coords);
        foreach (Tuple<int, int> p in pointsInSquare)
        {
            if (nums[p.Item1, p.Item2].GetValue() == value)
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
        List<int> numbers = new List<int>() { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
        numbers.Shuffle();
        return numbers;
    }

}


public class SudokuNumber : IEquatable<SudokuNumber>
{
    public readonly Tuple<int, int> coordinates;

    private int value;
    
    public bool locked { get; private set; }
    
    public SudokuNumber(int x, int y)
    {
        coordinates = new Tuple<int, int>(x, y);
    }

    public bool Equals (SudokuNumber other)
    {
        if (value == 0)
        {
            return false;
        } else if (value == other.value) 
        {
            return true;
        } else
        {
            return false;
        }
    }

    public override string ToString()
    {
        if (value == 0)
        {
            return "";
        } else
        {
            return value.ToString();
        }
    }
    public void SetValue(int value)
    {
        this.value = value;
    }

    public int GetValue()
    {
        return value;
    }

    public void Lock()
    {
        locked = true;
    }
    public bool IsSet()
    {
        if (value == 0)
        {
            return false;
        } else
        {
            return true;
        }
    }
}

public static class SudokuHelper
{
    //from: https://stackoverflow.com/questions/273313/randomize-a-listt
    public static void Shuffle<T>(this IList<T> list)
    {
        System.Random rand = new System.Random();
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rand.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }
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