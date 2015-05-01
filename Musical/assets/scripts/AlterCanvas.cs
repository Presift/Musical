using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class AlterCanvas : MonoBehaviour {

	public Button player2Start;
	public Button newMultiPlayer;
	public Button newSinglePlayer;
	public Button replay;

	public GameObject multiPlayerPanel;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void ShowPlayer1Complete()
	{
		multiPlayerPanel.SetActive (true);
		player2Start.gameObject.SetActive (true);
	}

	public void ShowPlayer2Complete()
	{
		multiPlayerPanel.SetActive (true);
		newMultiPlayer.gameObject.SetActive (true);
		newSinglePlayer.gameObject.SetActive (true);
	}

	public void EndOfSinglePlayer()
	{
		multiPlayerPanel.SetActive (true);
		newMultiPlayer.gameObject.SetActive (true);
		newSinglePlayer.gameObject.SetActive (true);
		replay.gameObject.SetActive (true);
	}

	public void StartPlayer2Turn()
	{
		Debug.Log ("start player 2 ");
		multiPlayerPanel.SetActive (false);

		Application.LoadLevel ("NEWGAME");
	}

	public void NewMultiPlayer()
	{
		Debug.Log ("start new multiplayer ");
		multiPlayerPanel.SetActive (false);
		GameData.dataControl.DeletePerformanceStats ();
		GameData.dataControl.twoPlayer = true;
		GameData.dataControl.player1TurnComplete = false;

		GameData.dataControl.Save ();
		Application.LoadLevel ("NEWGAME");
	}

	public void NewSinglePlayer()
	{
		Debug.Log (" start new single player ");
		GameData.dataControl.twoPlayer = false;
		GameData.dataControl.player1TurnComplete = false;

		GameData.dataControl.Save ();
		Application.LoadLevel ("NEWGAME");
	}

	public void PlayAgain()
	{
		Debug.Log ("play again");
		multiPlayerPanel.SetActive (false);

		GameData.dataControl.Save ();
		Application.LoadLevel ("NEWGAME");
	}
}
