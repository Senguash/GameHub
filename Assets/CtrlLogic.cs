using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Unity.VisualScripting;
using System.Linq;

public class CtrlLogic : MonoBehaviour
{
    VisualElement root;
    VisualElement gameContainer;
    VisualElement pauseMenu;
    VisualElement topBar;

    Game game;
    // Start is called before the first frame update
    void Start()
    {
        root = GetComponent<UIDocument>().rootVisualElement;

        var background = Resources.Load<Sprite>("Backgrounds/wood1");
        
        root.style.backgroundImage = new StyleBackground(background);

        StyleSheet uss = Resources.Load<StyleSheet>("StyleSheet");
        root.styleSheets.Add(uss);
        gameContainer = new VisualElement();
        gameContainer.style.height = Length.Percent(95);
        topBar = GenerateTopBar();
        root.Add(topBar);
        root.Add(gameContainer);
        pauseMenu = GeneratePauseMenu();
        root.Add(pauseMenu);
        pauseMenu.style.display = DisplayStyle.None;
        InitMainMenu();
    }

    void InitMainMenu()
    {
        LoadAllPersistent();
        topBar.style.display = DisplayStyle.None;
        pauseMenu.style.display = DisplayStyle.None;
        gameContainer.Clear();
        gameContainer.style.display = DisplayStyle.Flex;
        gameContainer.style.alignItems = Align.Center;

        Label title = UIGenerate.Label(gameContainer, "Game Hub", 24);

        VisualElement selectGamePanel = new VisualElement();
        selectGamePanel.style.justifyContent = Justify.Center;
        selectGamePanel.style.alignItems = Align.Center;
        selectGamePanel.style.height = Length.Percent(100);
        gameContainer.Add(selectGamePanel);

        ScrollView selectGameView = UIGenerate.ScrollView(selectGamePanel);

        Button ludoButton = UIGenerate.Button(selectGameView, "Ludo");
        ludoButton.clicked += () => { EnterLudo(); };

        Button sudokuButton = UIGenerate.Button(selectGameView, "Sudoku");
        sudokuButton.clicked += () => { EnterSudoku(); };

        Button ticTacToeButton = UIGenerate.Button(selectGameView, "Tic Tac Toe");
        ticTacToeButton.clicked += () => { EnterTicTacToe(); };

        Button backgammonButton = UIGenerate.Button(selectGameView, "Backgammon");
        backgammonButton.clicked += () => { EnterBackgammon(); };

    }

    VisualElement GenerateTopBar()
    {
        VisualElement ve = new VisualElement();
        ve.style.height = Length.Percent(5);
        ve.style.flexDirection = FlexDirection.RowReverse;
        Button b = new Button();
        b.style.width = Length.Percent(25);
        b.text = "...";
        b.clicked += () => { PauseGame(); };
        ve.Add(b);
        return ve;
    }

    VisualElement GeneratePauseMenu()
    {
        //gameContainer.style.display = DisplayStyle.None;
        VisualElement ve = new VisualElement();
        ve.style.justifyContent = Justify.Center;
        ve.style.alignItems = Align.Center;
        ve.style.height = Length.Percent(100);
        Button contButton = new Button();
        contButton.text = "Continue";
        contButton.clicked += () => { UnpauseGame();  };
        ve.Add(contButton);
        Button quitButton = new Button();
        quitButton.text = "Quit";
        quitButton.clicked += () => { SaveAllPersistent(); InitMainMenu();  };
        ve.Add(quitButton);
        return ve;
    }

    void PauseGame()
    {
        topBar.style.display = DisplayStyle.None;
        gameContainer.style.display = DisplayStyle.None;
        pauseMenu.style.display = DisplayStyle.Flex;
    }

    void UnpauseGame()
    {
        topBar.style.display = DisplayStyle.Flex;
        gameContainer.style.display = DisplayStyle.Flex;
        pauseMenu.style.display = DisplayStyle.None;
    }

    void PrepareEnterGame()
    {
        topBar.style.display = DisplayStyle.Flex;
        gameContainer.Clear();
    }

    void EnterLudo()
    {
        PrepareEnterGame();
        LudoController ludo = gameObject.AddComponent(typeof(LudoController)) as LudoController;
        gameContainer.Add(ludo.root);
    }
    void EnterSudoku()
    {
        PrepareEnterGame();
        game = gameObject.AddComponent(typeof(SudokuController)) as SudokuController;
        gameContainer.Add(game.root);
    }
    void EnterTicTacToe()
    {
        PrepareEnterGame();
        TicTacToeController tic = gameObject.AddComponent(typeof(TicTacToeController)) as TicTacToeController;
        gameContainer.Add(tic.root);
    }

    void EnterBackgammon()
    {
        PrepareEnterGame();
        BackgammonController backgammon = gameObject.AddComponent(typeof(BackgammonController)) as BackgammonController;
        gameContainer.Add(backgammon.root);
    }
    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate()
    {
        if (game != null)
        {
            if (game.exitInvoked)
            {
                game = null;
                InitMainMenu();
            }
        }
    }

    private const string filename = "savedata";
    private List<SaveData> saveData = new List<SaveData>();

    public void AddSaveData(SaveData sd)
    {
        int index = saveData.FindIndex(i => i.Name == sd.Name);
        if (index >= 0)
        {
            saveData.RemoveAt(index);
        }
        saveData.Add(sd);
    }
    public bool SaveDataExists(string name)
    {
        return saveData.Any(i => i.Name == name);
    }
    public void DeleteSaveData(string name)
    {
        int index = saveData.FindIndex(i => i.Name == name);
        if (index >= 0)
        {
            saveData.RemoveAt(index);
        }
    }
    public void SaveAllPersistent()
    {
        string destination = Application.persistentDataPath + "/" + filename;
        using (StreamWriter sw = new StreamWriter(destination, false))
        {
            sw.Write(JsonUtility.ToJson(saveData));
        }
        Debug.Log("Saving to: " + Application.persistentDataPath + "/" + filename);
    }

    public void LoadAllPersistent()
    {
        string destination = Application.persistentDataPath + "/" + filename;
        if (File.Exists(destination))
        {
            try
            {
                using (StreamReader reader = new StreamReader(destination))
                {
                    JsonUtility.FromJsonOverwrite(reader.ReadToEnd(), saveData);
                }
                Debug.Log("Loading from: " + Application.persistentDataPath + "/" + filename);
            }
            catch (Exception ex)
            {
                Debug.Log(ex.ToString());
                File.Delete(destination);
            }
        }
        else
        {
            Debug.Log("Save file not found");
        }
    }
}

public abstract class Game : StateMachine
{
    public string Name { get; protected set; }
    public bool exitInvoked = false;
    public VisualElement root;
    private static CtrlLogic ctrlLogic;
    public void SaveGame(params object[] data)
    {
        ctrlLogic.AddSaveData(new SaveData(Name, data));
    }
    public void CheckIfNameIsSet()
    {
        if (String.IsNullOrEmpty(Name))
        {
            throw new Exception("Field Name in class Game must be initialized in Awake or Start method of extention class");
        }
    }

    public object[] LoadGame()
    {
        object[] data = new object[0];
        return data;
    }
    public void ExitGame()
    {
        this.exitInvoked = true;
    }

    private void Awake()
    {
        ctrlLogic = FindObjectOfType<CtrlLogic>();
        root = new VisualElement();
        root.style.height = Length.Percent(100);
    }

    public VisualElement GetRootElement()
    {
        return root;
    }
}

public class SaveData
{
    public readonly string Name;
    private object[] Data;

    public SaveData(string name, object[] data)
    {
        Name = name;
        Data = data;
    }

    public object[] GetData()
    {
        return this.Data;
    }
}