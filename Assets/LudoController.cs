using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class LudoController : Game
{
    
    // Start is called before the first frame update
    void Start()
    {
        LudoGame game = new LudoGame();
        root.Add(new Label("Test"));
        Button startGameBtn = new Button();
        Button addPlayerBtn = new Button();
        addPlayerBtn.text = "Add player";
        addPlayerBtn.clicked += () => { game.AddPlayer("IDK",false); };
        startGameBtn.text = "Start";
        startGameBtn.clicked += () => { game.StartGame(); };


        root.Add(addPlayerBtn);
        root.Add(startGameBtn);
    }

    // Update is called once per frame
    void Update()
    {

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


        return 0;
    }
}