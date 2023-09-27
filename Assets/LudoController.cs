using System.Collections;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class LudoController : Game
{


    State newGame = new State("newGame");
    State coreGame = new State("coreGame");
    State gameEnd = new State("gameEnd");
    State player1 = new State("player1");
    State player2 = new State("player2");
    State player3 = new State("player3");
    State player4 = new State("player4");
    Command start = new Command("start");
    Command nextPlayer = new Command("nextPlayer");
    // Start is called before the first frame update
    void Start()
    {
        transitions = new Dictionary<StateTransition, State>()
        {
            { new StateTransition(newGame, start), player1 },
            { new StateTransition(player1, nextPlayer), player2 },
            { new StateTransition(player2, nextPlayer), player3 },
            { new StateTransition(player3, nextPlayer), player4 },
            { new StateTransition(player4, nextPlayer), player1 },
        };

        newGame.EnterEvent += InitNewGameMenu;
        newGame.LeaveEvent += ClearRoot;
        newGame.LeaveEvent += InitLudoUI;
        //coreGame.EnterEvent += InitLudoUI;
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
        LudoGame game = new LudoGame();
        root.Add(new Label("Test"));
        Button startGameBtn = new Button();
        Button addPlayerBtn = new Button();
        addPlayerBtn.text = "Add player";
        addPlayerBtn.clicked += () => { game.AddPlayer("IDK", false); };
        startGameBtn.text = "Start";
        startGameBtn.clicked += () => { game.StartGame();
            MoveNext(start);
        };
        root.Add(addPlayerBtn);
        root.Add(startGameBtn);
    }
    private void InitLudoUI()
    {
        Button terningBtn = new Button();
        terningBtn.text = "Terning";
        terningBtn.clicked += () =>
        {
            System.Random rnd = new System.Random();
            Debug.Log(rnd.Next(1,7));
            Debug.Log(rnd.Next(1,7));
            Debug.Log(CurrentState);
            //move piece
            MoveNext(nextPlayer);

        };
        root.Add(terningBtn);
        //MoveNext(nextPlayer);

    }
}



public class LudoGame
{
    private List<LudoPlayer> ludoPlayers = new List<LudoPlayer>();
    //private Dictionary<int, ludo_piece> ludo_pieces = new Dictionary<int, ludo_piece>();
    private readonly Dictionary<int, string> colors = new Dictionary<int, string>() {
        {1,"Red" },
        {2,"Green" },
        {3,"Blue" },
        {4,"Yellow" }
    };
    private List<LudoPiece> ludoPieces = new List<LudoPiece>();

        
    public void StartGame()
    {
        Debug.Log("START GAME");
    }
    public int AddPlayer(string name, bool aiPlayer) //Returns player ID
    {
        if (ludoPlayers.Count < 4)//Check if player count exceeds max count of 4
        {
            LudoPlayer newLudoPlayer = new LudoPlayer(name,ludoPlayers.Count + 1, aiPlayer);
            ludoPlayers.Add(newLudoPlayer);
            LudoPiece LudoPiece;
            for (int i = 0; i < 4; i++)
            {
                LudoPiece = new LudoPiece(-1, newLudoPlayer.GetColor());
                ludoPieces.Add(LudoPiece);
                
                
            }
            int id = ludoPlayers.Count - 1;
            return id;//Return the player id
        }
        else
        {
            return -1;//Return -1 if player counter is 4
        }
    }
    public LudoPlayer GetPlayer(int playerId)//Returns player object
    {
        return ludoPlayers[playerId];
    }
    public LudoPlayer RmovePlayer(int playerId)//Returns removed player object
    {
        LudoPlayer ludoPlayerToRemove = ludoPlayers[playerId];
        ludoPlayers.RemoveAt(playerId);
        return ludoPlayerToRemove;
    }

    public List<LudoPlayer> GetPlayerList()//Returns a list of type object _ludo_player
    {
        return ludoPlayers;
    }

}

public class LudoPlayer
{
    private string name;
    private int color;
    private bool aiPlayer;
    //private List<LudoPiece> playerPieces = new List<LudoPiece>();

    public LudoPlayer(string name, int color, bool aiPlayer)
    {
        this.name = name;
        this.color = color;
        this.aiPlayer = aiPlayer;
    }
    public int GetColor()
    {
        return color;
    }
    public string GetName()
    {
        return name;
    }
    public void SetColor(int color)
    {
        this.color=color;
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
}


public class LudoBoard
{

}