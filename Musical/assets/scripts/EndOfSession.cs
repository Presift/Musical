using UnityEngine;
using System.Collections;

public class EndOfSession : MonoBehaviour {

	public Metronome metronome;
	public AlterCanvas canvas;

	public int turnTime = 45;


	public bool sessionCompleted = false;

	// Use this for initialization
	void Start () {
	
	}

	// Update is called once per frame
	void Update () {
	
		if (Time.time > turnTime && !sessionCompleted ) 
		{
			//fade out music
			metronome.fadeOut = true;

			sessionCompleted = true;

			//if player 1 turn of 2 players
			if( GameData.dataControl.twoPlayer && !GameData.dataControl.player1TurnComplete )
			{

				canvas.ShowPlayer1Complete();
			}
			else if( GameData.dataControl.twoPlayer && GameData.dataControl.player1TurnComplete )
			{
				canvas.ShowPlayer2Complete();
			}
			//end turn
		}
	}
}
