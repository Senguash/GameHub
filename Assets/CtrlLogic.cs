using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Unity.VisualScripting;
using System.Linq;
using System.Xml.Serialization;
using System.Text;
using System.Xml;
using System.Xml.Schema;

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

    void OnApplicationQuit()
    {
        SaveAllPersistent();
        Debug.Log("Application ending after " + Time.time + " seconds");
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
                SaveAllPersistent();
                InitMainMenu();
            }
        }
    }

    private const string filename = "savedata";

    public SaveData saveData;

    
    public void SaveAllPersistent()
    {
        //Debug.Log(saveData.GetData("sudoku").ToString());
        string saveDatafullPath = Application.persistentDataPath + "/" + filename;
        XmlSerializer ser = new XmlSerializer(typeof(SaveData));
        using (StreamWriter sw = new StreamWriter(saveDatafullPath, false))
        {
            ser.Serialize(sw, saveData);
        }
        Debug.Log("Saving to: " + saveDatafullPath);
    }

    public void LoadAllPersistent()
    {
        string saveDatafullPath = Application.persistentDataPath + "/" + filename;
        if (File.Exists(saveDatafullPath))
        {
            try
            {
                XmlSerializer ser = new XmlSerializer(typeof(SaveData));
                using (StreamReader sw = new StreamReader(saveDatafullPath, false))
                {
                    saveData = (SaveData) ser.Deserialize(sw);
                }
                Debug.Log("Loading from: " + saveDatafullPath);
            }
            catch (Exception ex)
            {
                Debug.Log(ex.ToString());
                File.Delete(saveDatafullPath);
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
    public string Name;
    public bool exitInvoked = false;
    public VisualElement root;
    private static CtrlLogic ctrlLogic;

    protected void SaveGame(string key, Type type, IXmlSerializable data)
    {
        XmlSerializer ser = new XmlSerializer(type);
        using (StringWriter textWriter = new StringWriter())
        {
            ser.Serialize(textWriter, data);
            Debug.Log(textWriter.ToString());
            ctrlLogic.saveData.Save(key, textWriter.ToString());
        }
    }

    protected object LoadGame(string key, Type type)
    {
        XmlSerializer ser = new XmlSerializer(type);

        using (StringReader tr = new StringReader(ctrlLogic.saveData.GetData(key)))
        {
            return ser.Deserialize(tr);
        }
    }

    protected bool SaveExists(string key)
    {
        return ctrlLogic.saveData.SaveExists(key);
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
[Serializable]
public class SaveDataItem
{
    [SerializeField]
    public string key;
    [SerializeField]
    public string data;

    public SaveDataItem(string key, string data)
    {
        this.key = key;
        this.data = data;
    }
    public void SetData(string data)
    {
        this.data = data;
    }

    public string GetData()
    {
        return this.data;
    }
    public override string ToString()
    {
        return "Key: " + this.key + " Data: " + this.data;
    }
}
[Serializable]
public class SaveData : IXmlSerializable
{
    private List<SaveDataItem> dataList;

    public SaveData()
    {
        dataList = new List<SaveDataItem>();
    }

    public bool SaveExists(string key)
    {
        return dataList.Exists(item => item.key.Equals(key, StringComparison.OrdinalIgnoreCase));
    }

    public string GetData(string key)
    {
        return dataList.Find(item => item.key.Equals(key, StringComparison.OrdinalIgnoreCase)).GetData();
    }

    public void Save(string key, string data)
    {
        if (SaveExists(key))
        {
            dataList.Find(item => item.key.Equals(key, StringComparison.OrdinalIgnoreCase)).SetData(data);
        } else
        {
            dataList.Add(new SaveDataItem(key, data));
        }
    }

    public void DeleteSave(string key)
    {
        dataList.Remove(dataList.Find(item => item.key.Equals(key, StringComparison.OrdinalIgnoreCase)));
    }

    public void WriteXml(XmlWriter writer)
    {
        StringBuilder sb = new StringBuilder();
        foreach (SaveDataItem sd in dataList)
        {
            sb.Append(sd.key);
            sb.Append("|");
            sb.Append(sd.data);
        }
        sb.Append(";;;");
        writer.WriteString(sb.ToString());
    }

    public void ReadXml(XmlReader reader)
    {
        string input = reader.ReadString();
        string[] dataSegments = input.Split(";;;");
        Debug.Log(dataSegments.Length);
        foreach(string s in dataSegments)
        {
            try
            {
                string[] dataItems = s.Split("|");
                dataList.Add(new SaveDataItem(dataItems[0], dataItems[1]));
            } catch
            {

            }
            
        }
    }

    public XmlSchema GetSchema()
    {
        return (null);
    }
}