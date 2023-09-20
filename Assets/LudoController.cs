using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class LudoController : Game
{
    // Start is called before the first frame update
    void Start()
    {

        root.Add(new Label("Test"));
        Button _add_player_btn = new Button();
        _add_player_btn.text = "Add player";
        root.Add(_add_player_btn);
    }

    // Update is called once per frame
    void Update()
    {

    }
}

public class _ludo_player
{
    private string _name;
    private int _color;
    private bool ai_player;

    public _ludo_player(string _name, int _color, bool ai_player)
    {
        this._name = _name;
        this._color = _color;
        this.ai_player = ai_player;
    }
    private string name;
    private int color;

    public _ludo_player(string _name, int _color, bool ai_player)
    {
        this._name = _name;
        this._color = _color;
        this.ai_player = ai_player;
    }
}

public class _ludo_game
{
    private List<_ludo_player> _ludo_players = new List<_ludo_player>();

    public int _add_player(_ludo_player _new_ludo_player) //Returns player ID
    {
        _ludo_players.Add(_new_ludo_player);
        int id = _ludo_players.Count - 1;
        return id;
    }
    public _ludo_player _get_player(int _player_id)//Returns player object
    {
        return _ludo_players[_player_id];
    }
    public _ludo_player _remove_player(int _player_id)//Returns removed player object
    public _ludo_player _get_player(int _player_id)
    {
        return _ludo_players[_player_id];
    }
    public _ludo_player _remove_player(int _player_id)//Returns removed player object
    {
        _ludo_player _ludo_player_to_remove = _ludo_players[_player_id];
        _ludo_players.RemoveAt(_player_id);
        return _ludo_player_to_remove;
    }

    public List<_ludo_player> _get_player_list(int _player_id)//Returns a list of type object _ludo_player
    {
        return _ludo_players;
    }

}

public class _ludo_board
{


        return 0;
    }
}