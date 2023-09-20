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
    private string name;
    private int color;
}

public class _ludo_game
{
    private List<_ludo_player> _ludo_players = new List<_ludo_player>();

    public int _add_player(_ludo_player _new_ludo_player) //Returns player ID
    {
        _ludo_players.Add(_new_ludo_player);

        return 0;
    }
    public _ludo_player _get_player()
}