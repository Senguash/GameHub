using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.UIElements;

public class SudokuController : Game
{
    Label errorsLabel;
    int errorsRemaining;
    int maxErrorsAllowed = 5;
    float redness;
    bool errorStateActive = false;
    VisualElement gameBoard;
    Dictionary<Tuple<int, int>, Button> buttonDictionary;
    Dictionary<Tuple<int, int>, VisualElement> veDictionary;
    Dictionary<Tuple<int, int>, Label> labelDictionary;
    Tuple<int, int> selectedSpace;

    static Color col_selected_background = new Color(0.6f, 0.6f, 0.6f);
    static Color col_row_col_selected_background = new Color(0.7f, 0.7f, 0.7f);
    static Color col_square_selected_background = new Color(0.8f, 0.8f, 0.8f);
    static Color col_locked_background =  new Color(0.85f, 0.85f, 1f);
    static Color col_locked_selected_background = new Color(0.6f, 0.6f, 0.8f);
    static Color col_locked_selected_by_proxy_background = new Color(0.7f, 0.7f, 0.85f);
    static Color col_base_background = new Color(1f, 1f, 1f);
    static Color col_base_text = new Color(0f, 0f, 0f);
    static Color col_locked_text = new Color(0.25f, 0.25f, 0.4f);
    static Color col_game_over_label = new Color(0.15f, 0f, 0f);
    static Color col_success_background = new Color(0.7f, 1f, 0.7f);
    static Color col_success_text = new Color(0f, 0.2f, 0f);

    State newOrContinue = new State("newOrContinue");
    State difficultySelect = new State("difficultySelect");
    State coreGame = new State("coreGame");
    State gameOver = new State("gameOver");
    Command start = new Command("start");
    Command cont = new Command("cont");
    Command end = new Command("end");

    Sudoku sudoku;
    // Start is called before the first frame update
    void Start()
    {
        Name = "Sudoku";
        transitions = new Dictionary<StateTransition, State>()
        {
            { new StateTransition(newOrContinue, start), difficultySelect },
            { new StateTransition(difficultySelect, start), coreGame},
            { new StateTransition(newOrContinue, cont), coreGame },
            { new StateTransition(coreGame, end), gameOver },
            { new StateTransition(gameOver, start), difficultySelect },
        };
        
        StateChanged += ClearRoot;
        newOrContinue.EnterEvent += InitNewGameMenu;
        difficultySelect.EnterEvent += InitDifficultyMenu;
        coreGame.EnterEvent += InitSudokuUI;
        gameOver.EnterEvent += InitGameOverMenu;

        SetInitialState(newOrContinue);
    }

    private void ClearRoot()
    {
        root.Clear();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (errorStateActive)
        {
            UpdateErrorUI();
        }
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

    private void UpdateErrorUI()
    {
        if (redness > 0f)
        {
            VisualElement ve;
            if (veDictionary.TryGetValue(selectedSpace, out ve))
            {
                ve.style.backgroundColor = new Color(Math.Max(redness, 0.6f), 0.6f, 0.6f);
            }
            errorsLabel.style.color = new Color(redness, 0f, 0f);
            redness -= 0.02f;
            if (redness <= 0f)
            {
                errorStateActive = false;
                redness = 0f;
            }
        } else
        {
            VisualElement ve;
            if (veDictionary.TryGetValue(selectedSpace, out ve))
            {
                ve.style.backgroundColor = col_selected_background;
            }
            errorsLabel.style.color = col_base_text;
            errorStateActive = false;
        }
    }

    private void SetUIToError()
    {
        errorStateActive = true;
        redness = 1f;
    }
    private void InitGameOverMenu()
    {
        VisualElement ve = UIGenerate.VisualElement(root, Length.Percent(100), Length.Percent(100),  FlexDirection.Column, Align.Stretch, Justify.Center);

        VisualElement topVE = UIGenerate.VisualElement(ve, Length.Percent(100), Length.Percent(30), FlexDirection.Column, Align.Center, Justify.Center);

        var gameOverLabel = UIGenerate.Label(topVE, "Game Over", 24);
        gameOverLabel.style.color = col_game_over_label;

        var tryAgainButton = UIGenerate.Button(ve, "Try Again");
        tryAgainButton.clicked += () =>
        {
            MoveNext(start);
        };

        var exitButton = UIGenerate.Button(ve, "Exit");
        exitButton.clicked += () =>
        {
            ExitGame();
        };
    }
    private void InitNewGameMenu()
    {
        VisualElement ve = UIGenerate.VisualElement(root, Length.Percent(100), Length.Percent(100), FlexDirection.Column, Align.Center, Justify.Center );
        Button continueButton = UIGenerate.Button(ve, "Continue");
        continueButton.clicked += () =>
        {
            ContinueGame();
            MoveNext(cont);
        };
        continueButton.SetEnabled(SaveExists());

        Button newButton = UIGenerate.Button(ve, "New Game");
        newButton.clicked += () =>
        {
            MoveNext(start);
        };
    }

    private void InitDifficultyMenu()
    {
        var ve = UIGenerate.VisualElement(root, Length.Percent(100), Length.Percent(100), FlexDirection.Column, Align.Stretch, Justify.Center);

        var diff1Button = UIGenerate.Button(ve, "Easy");
        diff1Button.clicked += () =>
        {
            StartNewGame(35);
        };

        var diff2Button = UIGenerate.Button(ve, "Medium");
        diff2Button.clicked += () =>
        {
            StartNewGame(40);
        };

        var diff3Button = UIGenerate.Button(ve, "Hard");
        diff3Button.clicked += () =>
        {
            StartNewGame(45);
        };

        var diff4Button = UIGenerate.Button(ve, "Very Hard");
        diff4Button.clicked += () =>
        {
            StartNewGame(50);
        };

        var diff5Button = UIGenerate.Button(ve, "Extreme");
        diff5Button.clicked += () =>
        {
            StartNewGame(55);
        };
    }

    private void StartNewGame(int diff)
    {
        errorsRemaining = maxErrorsAllowed;
        sudoku = new Sudoku();
        sudoku.GenerateRandom(diff);
        MoveNext(start);
    }

    private void ContinueGame()
    {
        sudoku = (Sudoku) LoadGame(typeof(Sudoku));
        Debug.Log(sudoku.ToString());
        for (int i = 0; i < 9; i++)
        {
            for (int j = 0; j < 9; j++)
            {
                if (sudoku.nums[i, j].locked)
                {
                    Label l;
                    if (labelDictionary.TryGetValue(new Tuple<int, int>(i, j), out l))
                    {
                        l.style.color = col_locked_text;
                        l.text = sudoku.nums[i, j].GetValue().ToString();
                    }
                    VisualElement ve;
                    if (veDictionary.TryGetValue(new Tuple<int, int>(i, j), out ve))
                    {
                        ve.style.backgroundColor = new Color(0.85f, 0.85f, 1f);
                    }
                } else
                {
                    /*Label l;
                    if (labelDictionary.TryGetValue(new Tuple<int, int>(i, j), out l))
                    {
                        l.style.color = col_base_text;
                        l.text = sudoku.nums[i, j].GetValue().ToString();
                    }
                    VisualElement ve;
                    if (veDictionary.TryGetValue(new Tuple<int, int>(i, j), out ve))
                    {
                        ve.style.backgroundColor = col_base_background;
                    }*/
                }
            }
        }
    }


    private void InitSudokuUI()
    {

        VisualElement ve = UIGenerate.VisualElement(root, Length.Percent(100), Length.Percent(90), FlexDirection.Column, Align.Center, Justify.Center);
        VisualElement buffer = UIGenerate.VisualElement(ve, Length.Percent(100), Length.Percent(10), FlexDirection.Column, Align.Center);
        errorsLabel = UIGenerate.Label(buffer, "Allowed Errors Remaining " + errorsRemaining.ToString(), 12);
        gameBoard = UIGenerate.VisualElement(ve, 216, 216, FlexDirection.Column, Align.Center);
        gameBoard.style.backgroundColor = col_base_background;

        buttonDictionary = new Dictionary<Tuple<int, int>, Button>();
        veDictionary = new Dictionary<Tuple<int, int>, VisualElement>();
        labelDictionary = new Dictionary<Tuple<int, int>, Label>();

        selectedSpace = new Tuple<int, int>(0, 0);

        for (int i = 0; i < 9; i++)
        {
            VisualElement row = UIGenerate.VisualElement(gameBoard, 216, 24, FlexDirection.Row);
            row.AddToClassList("sudoku-row");
            for (int j = 0; j < 9; j++)
            {
                Button btn = new Button();
                btn.visible = true;
                btn.style.visibility = Visibility.Hidden;

                VisualElement v = new VisualElement();
                v.style.justifyContent = Justify.Center;
                v.style.alignItems = Align.Center;
                v.style.visibility = Visibility.Visible;
                //v.AddToClassList("sudoku-field");

                Label label = new Label();
                label.visible = true;
                label.style.marginTop = -1;
                label.style.visibility = Visibility.Visible;
                v.Add(label);
                btn.Add(v);
                int tmp_i = i;
                int tmp_j = j;
                btn.clicked += () => {
                    UpdateUI();
                    DeselectSpace(selectedSpace); 
                    selectedSpace = new Tuple<int, int>(tmp_i, tmp_j); 
                    SelectSpace(selectedSpace);
                    if (sudoku.CheckComplete())
                    {
                        ShowCompleteUI();
                    }
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
        }

        VisualElement numberSelectionRow = UIGenerate.VisualElement(ve, Length.Percent(100), Length.Percent(10), FlexDirection.Row, Align.Center, Justify.Center);
        for (int i = 1; i < 10; i++)
        {
            VisualElement v = new VisualElement();
            Button btn = new Button();
            btn.visible = true;
            int tmp = i;
            btn.clicked += () => { 
                if (!sudoku.nums[selectedSpace.Item1, selectedSpace.Item2].locked)
                {
                    if (sudoku.SelectValue(selectedSpace, tmp))
                    {
                        UISetValue(selectedSpace, tmp);
                        SaveGame(typeof(Sudoku), this.sudoku);
                    }
                    else
                    {
                        errorsRemaining--;
                        Debug.Log("Wrong number");
                        errorsLabel.text = "Allowed Errors Remaining " + errorsRemaining.ToString();
                        SetUIToError();
                        if (errorsRemaining == 0)
                        {
                            MoveNext(end);
                        }
                    }
                    UpdateUI();
                }
                if (sudoku.CheckComplete())
                {
                    ShowCompleteUI();
                }
            };
            btn.text = i.ToString();
            btn.style.width = 24;
            btn.style.height = 24;
            btn.AddToClassList("sudoku-button");
            v.style.visibility = Visibility.Visible;
            //label.text = "0";

            numberSelectionRow.Add(btn);
        }
        UpdateUI();
    }

    private void UpdateUI()
    {
        for (int i = 0; i < 9; i++)
        {
            for (int j = 0; j < 9; j++)
            {
                if (sudoku.nums[i, j].locked)
                {
                    Label l;
                    if (labelDictionary.TryGetValue(new Tuple<int, int>(i, j), out l))
                    {
                        l.style.color = col_locked_text;
                        l.text = sudoku.nums[i, j].GetValue().ToString();
                    }
                    VisualElement ve;
                    if (veDictionary.TryGetValue(new Tuple<int, int>(i, j), out ve))
                    {
                        ve.style.backgroundColor = col_locked_background;
                    }
                }
            }
        }
    }

    private void ShowCompleteUI()
    {
        for (int i = 0; i < 9; i++)
        {
            for (int j = 0; j < 9; j++)
            {
                Label l;
                if (labelDictionary.TryGetValue(new Tuple<int, int>(i, j), out l))
                {
                    if (sudoku.nums[i, j].locked)
                    {
                        l.style.color = col_locked_text;
                    } else
                    {
                        l.style.color = col_success_text;
                    }
                    l.text = sudoku.nums[i, j].GetValue().ToString();
                }
                VisualElement ve;
                if (veDictionary.TryGetValue(new Tuple<int, int>(i, j), out ve))
                {
                    if (sudoku.nums[i, j].locked)
                    {
                        ve.style.backgroundColor = col_locked_background;
                    }
                    else
                    {
                        ve.style.backgroundColor = col_success_background;
                    }
                }
            }
        }
        /*SaveGame(JsonUtility.ToJson(this.sudoku));
        XmlSerializer ser = new XmlSerializer(typeof(Sudoku));
        using (StringWriter textWriter = new StringWriter())
        {
            ser.Serialize(textWriter, sudoku);
            Debug.Log(textWriter.ToString());
        }
        Debug.Log("Called Save Game");*/
    }

    private void SelectSpace(Tuple<int, int> coords)
    {
        ShowCross(coords);
        VisualElement ve;
        if (veDictionary.TryGetValue(coords, out ve))
        {
            if (sudoku.nums[coords.Item1, coords.Item2].locked)
            {
                ve.style.backgroundColor = col_locked_selected_background;
            } else
            {
                ve.style.backgroundColor = col_selected_background;
            }
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
                        ve.style.backgroundColor = col_locked_background;
                    }
                } 
                else
                {
                    VisualElement ve;
                    if (veDictionary.TryGetValue(new Tuple<int, int>(i, j), out ve))
                    {
                        if (sudoku.CheckComplete())
                        {
                            ve.style.backgroundColor = col_success_background;
                        } else
                        {
                            ve.style.backgroundColor = col_base_background;
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
                    ve.style.backgroundColor = col_square_selected_background;
                } else
                {
                    ve.style.backgroundColor = col_locked_selected_by_proxy_background;
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
                    ve.style.backgroundColor = col_row_col_selected_background;
                }
                else
                {
                    ve.style.backgroundColor = col_locked_selected_by_proxy_background;
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
                    ve.style.backgroundColor = col_row_col_selected_background;
                }
                else
                {
                    ve.style.backgroundColor = col_locked_selected_by_proxy_background;
                }
            }
        }
    }
}


[Serializable]
public class Sudoku : IXmlSerializable
{
    [SerializeField]
    public SudokuNumber[,] nums;
    [SerializeField]
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

    public void WriteXml(XmlWriter writer)
    {
        StringBuilder sb = new StringBuilder();
        sb.Append(this.ToString(true));
        //sb.Append(",");
        sb.Append(this.ToString(false));
        writer.WriteString(sb.ToString());
    }

    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();
        sb.Append(this.ToString(true));
        //sb.Append(",");
        sb.Append(this.ToString(false));
        return sb.ToString();
    }

    public void ReadXml(XmlReader reader)
    {
        this.nums = new SudokuNumber[9, 9];
        for (int i = 0; i < 9; i++)
        {
            for (int j = 0; j < 9; j++)
            {
                nums[i, j] = new SudokuNumber(i, j);
            }
        }
        this.trueValues = new int[9, 9];
        string input = reader.ReadString();
        string[] numbersAsStrings = input.Split(",");
        int x = 0;
        int y = 0;
        for (int i = 0; i < 81; i++)
        {
            x = i / 9;
            y = i % 9;
            int n;
            if (int.TryParse(numbersAsStrings[i], out n))
            {
                try
                {
                    this.trueValues[x, y] = n;
                } catch (Exception e)
                {
                    Debug.Log(x + " " + y);
                }
                
            }
        }
        for (int i = 0; i < 81; i++)
        {
            x = i / 9;
            y = i % 9;
            int n;
            if (int.TryParse(numbersAsStrings[81+i], out n))
            {
                this.nums[x, y].SetValue(n);
            } else
            {
                this.nums[x, y].SetValue(0);
            }
        }
    }

    public XmlSchema GetSchema()
    {
        return (null);
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
        Debug.Log("Numbers removed: " + (81 - this.VisibleNumbers()).ToString());
    }

    public void Reduce(int targetRemovedNumbers)
    {
        int removedNumbers = 0;
        int attempts = 20;
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

    public string ToString(bool hiddenValues)
    {
        StringBuilder sb = new StringBuilder();
        for (int j = 0; j < 9; j++)
        {
            sb.Append("{");
            for (int i = 0; i < 9; i++)
            {
                if (hiddenValues)
                {
                    sb.Append(trueValues[j, i].ToString());
                } else
                {
                    sb.Append(this.nums[j, i].ToString());
                }
                sb.Append(",");
            }
            sb.Append("}" + Environment.NewLine);
        }   
        return sb.ToString();
    }

    private static List<int> GetNumberList()
    {
        List<int> numbers = new List<int>() { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
        numbers.Shuffle();
        return numbers;
    }

    public int VisibleNumbers()
    {
        int n = 0;
        for (int j = 0; j < 9; j++)
        {
            for (int i = 0; i < 9; i++)
            {
                if (this.nums[j, i].IsSet())
                {
                    n++;
                }
            }
        }
        return n;
    }
}

[Serializable]
public class SudokuNumber : IEquatable<SudokuNumber>
{
    [SerializeField]
    public Tuple<int, int> coordinates;

    [SerializeField]
    private int value;

    [SerializeField]
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