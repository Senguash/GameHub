using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Unity.VisualScripting;

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
        Debug.Log("Game start");
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

        Button ludoButton = new Button();
        ludoButton.text = "Play Ludo";
        ludoButton.clicked += () => { EnterLudo(); };
        selectGameView.Add(ludoButton);

        Button sudokuButton = new Button();
        sudokuButton.text = "Play Sudoku";
        sudokuButton.clicked += () => { EnterSudoku(); };
        selectGameView.Add(sudokuButton);

        Button ticTacToeButton = new Button();
        ticTacToeButton.text = "Play Tic Tac Toe";
        ticTacToeButton.clicked += () => { EnterTicTacToe(); };
        selectGameView.Add(ticTacToeButton);


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
        quitButton.clicked += () => { InitMainMenu();  };
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

    void EnterGame()
    {
        topBar.style.display = DisplayStyle.Flex;
        gameContainer.Clear();
    }

    void EnterLudo()
    {
        EnterGame();
        LudoController ludo = gameObject.AddComponent(typeof(LudoController)) as LudoController;
        gameContainer.Add(ludo.root);
    }
    void EnterSudoku()
    {
        EnterGame();
        game = gameObject.AddComponent(typeof(SudokuController)) as SudokuController;
        gameContainer.Add(game.root);
    }
    void EnterTicTacToe()
    {
        EnterGame();
        TicTacToeController tic = gameObject.AddComponent(typeof(TicTacToeController)) as TicTacToeController;
        gameContainer.Add(tic.root);
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
}

public abstract class Game : StateMachine
{
    public bool exitInvoked = false;
    public VisualElement root;

    public Game()
    {
        
    }

    public void ExitGame()
    {
        this.exitInvoked = true;
    }

    private void Awake()
    {
        root = new VisualElement();
        root.style.height = Length.Percent(100);
    }

    public VisualElement GetRootElement()
    {
        return root;
    }
}