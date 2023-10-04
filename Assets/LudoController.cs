using System.Collections;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.Animations;
using System.Linq;

public class LudoController : Game
{

    State newGame = new State("newGame");
    State gameEnd = new State("gameEnd");
    Command start = new Command("start");
    Command nextPlayer = new Command("nextPlayer");
    // Start is called before the first frame update
    private readonly Dictionary<int, string> PlayerColors = new Dictionary<int, string>() {
        {1,"Red" },
        {2,"Green" },
        {3,"Blue" },
        {4,"Yellow" }
    };
    
    private LudoGame game = new LudoGame();
    void Start()
    {
        transitions = new Dictionary<StateTransition, State>()
        {
            
        };

        newGame.EnterEvent += InitNewGameMenu;
        newGame.LeaveEvent += ClearRoot;
        newGame.LeaveEvent += InitLudoUI;
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
        
        root.Add(new Label("LUDO"));
        Button startGameBtn = new Button();
        Button addPlayerBtn = new Button();
        addPlayerBtn.text = "Add player";
        addPlayerBtn.clicked += () => {
            int ret = AddPlayer();
            if (ret == 2)
            {
                startGameBtn.SetEnabled(true);
            }
            else if(ret == 4)
            {
                addPlayerBtn.SetEnabled(false);
            }
        };
        startGameBtn.text = "Start";
        startGameBtn.clicked += () => { StartGame();
            MoveNext(nextPlayer);
        };
        startGameBtn.SetEnabled(false);
        root.Add(addPlayerBtn);
        root.Add(startGameBtn);
    }
    private void StartGame()
    {
        AddplayerStates();
    }
    private void AddplayerStates()
    {
        for (int i = 0; i < game.ludoPlayers.Count; i++)
        {
            if(i == 0)
            {
                transitions.Add(new StateTransition(newGame, nextPlayer), game.ludoPlayers[i].GetPlayerState());
            }
            else
            {
                transitions.Add(new StateTransition(game.ludoPlayers[i - 1].GetPlayerState(), nextPlayer), game.ludoPlayers[i].GetPlayerState());
                if (i+1 == game.ludoPlayers.Count)
                {
                    transitions.Add(new StateTransition(game.ludoPlayers[i].GetPlayerState(), nextPlayer), game.ludoPlayers[0].GetPlayerState());
                }
            }
        }
    }
    private void InitLudoUI()
    {
        Button terningBtn = new Button();
        terningBtn.text = "Terning";
        terningBtn.clicked += () =>
        {
            System.Random rnd = new System.Random();
            int sum = rnd.Next(1,7) + rnd.Next(1,7);
            Debug.Log(sum);
            Debug.Log(CurrentState.ToString());
            LudoPlayer cPlayer = game.ludoPlayers.Where(x => x.GetPlayerState() == CurrentState).First();
            Debug.Log("SOME PLAYER STUFF: " + cPlayer.GetColor());

            game.MovePiece(-1, cPlayer.GetColor(), sum);
            Debug.Log("END CALL");
            //move piece
            
            MoveNext(nextPlayer);

        };
        root.Add(terningBtn);
        //MoveNext(nextPlayer);
    }
    private int AddPlayer()
    {
        game.ludoPlayers.Add(new LudoPlayer("DEMO", PlayerColors[game.ludoPlayers.Count + 1], false, new State("player" + (game.ludoPlayers.Count + 1))));
        //Debug.Log(ludoPlayers[ludoPlayers.Count-1].GetColor());
        game.CreatePieces(game.ludoPlayers[game.ludoPlayers.Count - 1].GetColor());
        return game.ludoPlayers.Count;
        
    }
}



public class LudoGame
{
    
    //private Dictionary<int, ludo_piece> ludo_pieces = new Dictionary<int, ludo_piece>();
    private readonly Dictionary<int, string> colors = new Dictionary<int, string>() {
        {1,"Red" },
        {2,"Green" },
        {3,"Blue" },
        {4,"Yellow" }
    };
    private List<LudoPiece> ludoPieces = new List<LudoPiece>();
    public List<LudoPlayer> ludoPlayers = new List<LudoPlayer>();
    private int ColorNameToKey(string color)
    {
        return colors.FirstOrDefault(x => x.Value == color).Key;
    }
        
    public void StartGame()
    {
        Debug.Log("START GAME");
    }
    public int CreatePieces(string color) //Returns player ID
    {
        for(int i = 0;i < 4; i++)
        {
            ludoPieces.Add(new LudoPiece(-1, ColorNameToKey(color)));
        }
        return 0;
    }
    public int MovePiece(int absolutepPosition, string color, int amount)
    {
        if(absolutepPosition == -1 && amount == 12)
        {
            //Move a player piece to relative potition 0
            Debug.Log("Number of pieces: " + ludoPieces.Count);
            foreach(LudoPiece piece in ludoPieces)
            {
                //Debug.Log(ColorNameToKey(color));
                if(piece.GetOffset() == ColorNameToKey(color))
                {
                    //Debug.Log("START MOVE MABEY");
                    if(piece.GetAbsolutePosition() == absolutepPosition)
                    {
                        piece.SetAbsolutePosition(piece.GetOffset());
                        Debug.Log("MOVE");
                        PrintPieces();
                        return 0;
                    }
                }
            }
        }
        else
        {
            Debug.Log("No move");
            return 1;
        }
        return -1;
    }
    public List<LudoPiece> GetMoveablePieces(string color, int diceSum)
    {

        return ludoPieces.Where(x => x.GetOffset() == ColorNameToKey(color)).ToList();
    }
    public void PrintPieces()
    {
        foreach (LudoPiece piece in ludoPieces)
        {
            Debug.Log(piece.GetOffset());
            Debug.Log(piece.GetAbsolutePosition());
        }
    }


}

public class LudoPlayer
{     private string name;
    private string color;
    private bool aiPlayer;
    private StateMachine.State playerState;
    //private List<LudoPiece> playerPieces = new List<LudoPiece>();

    public LudoPlayer(string name, string color, bool aiPlayer, StateMachine.State playerState)
    {
        this.name = name;
        this.color = color;
        this.aiPlayer = aiPlayer;
        this.playerState = playerState;
    }
    public string GetColor()
    {
        return color;
    }
    public string GetName()
    {
        return name;
    }
    public void SetColor(string color)
    {
        this.color=color;
    }
    public StateMachine.State GetPlayerState()
    {
        return playerState;
    }
    
    public int SetPlayerState(StateMachine.State playerState)
    {
        this.playerState = playerState;
        return 0;
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
    public int GetRelativePosition()
    {
        return absolutepPosition - offset;
    }
}


public class LudoBoard
{

}