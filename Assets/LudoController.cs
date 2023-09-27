using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class LudoController : Game
{
    
    // Start is called before the first frame update
    void Start()
    {
        ludo_game game = new ludo_game();
        ludo_player player = new ludo_player("test", 1, false);
        root.Add(new Label("Test"));
        Button add_player_btn = new Button();
        add_player_btn.text = "Add player";
        add_player_btn.clicked += () => { add_player(game, player); };
        
        root.Add(add_player_btn);
    }

    // Update is called once per frame
    void Update()
    {

    }
    void add_player(ludo_game game, ludo_player player)
    {
        Debug.Log(game.add_player(player));
        Debug.Log("Player added");
    }
}



public class ludo_game
{
    private List<ludo_player> ludo_players = new List<ludo_player>();
    //private Dictionary<int, ludo_piece> ludo_pieces = new Dictionary<int, ludo_piece>();
    private List<ludo_piece> ludo_pieces = new List<ludo_piece>();

    public int add_player(ludo_player new_ludo_player) //Returns player ID
    {
        if (ludo_players.Count < 4)//Check if player count exceeds max count of 4
        {
            ludo_players.Add(new_ludo_player);
            ludo_piece _ludo_piece;
            for (int i = 0; i < 5; i++)
            {
                _ludo_piece = new ludo_piece(-1, new_ludo_player.get_color());
                ludo_pieces.Add(_ludo_piece);
                
            }
            int id = ludo_players.Count - 1;
            return id;//Return the player id
        }
        else
        {
            return -1;//Return -1 if player counter is 4
        }
    }
    public ludo_player get_player(int player_id)//Returns player object
    {
        return ludo_players[player_id];
    }
    public _ludo_player _get_player(int _player_id)
    {
        return _ludo_players[_player_id];
    }
    public ludo_player remove_player(int player_id)//Returns removed player object
    {
        ludo_player ludo_player_to_remove = ludo_players[player_id];
        ludo_players.RemoveAt(player_id);
        return ludo_player_to_remove;
    }

    public List<ludo_player> get_player_list(int player_id)//Returns a list of type object _ludo_player
    {
        return ludo_players;
    }

}

public class ludo_player
{
    private string name;
    private int color;
    private bool ai_player;
    private List<ludo_piece> ludo_player_pieces = new List<ludo_piece>();

    public ludo_player(string name, int color, bool ai_player)
    {
        this.name = name;
        this.color = color;
        this.ai_player = ai_player;
    }
    public int get_color()
    {
        return color;
    }
    public string get_name()
    {
        return name;
    }

}
public class ludo_piece
{
    private int absolute_position;
    private int offset;
    //private bool home;

    public ludo_piece(int absolute_position, int offset)
    {
        this.absolute_position = absolute_position;
        this.offset = offset;
    }

    public int get_absolute_position()
    {
        return absolute_position;
    }

    public int set_absolute_position(int absolute_position)
    {
        this.absolute_position = absolute_position;
        return this.absolute_position;
    }
    public int get_offset()
    {
        return offset;
    }
    public int set_offset()
    {
        this.offset = offset;
        return this.offset;
    }
}


public class ludo_board
{


        return 0;
    }
}