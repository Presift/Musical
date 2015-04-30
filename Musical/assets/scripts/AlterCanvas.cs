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

//	public void HidePlayer1Complete()
//	{
//		multiPlayerPanel.SetActive (false);
//	}

	public void StartPlayer2Turn()
	{
		Application.LoadLevel ("NEWGAME");
	}

	public void NewMultiPlayer()
	{
		GameData.dataControl.DeletePerformanceStats ();
		GameData.dataControl.twoPlayer = true;
		GameData.dataControl.Save ();
		Application.LoadLevel ("NEWGAME");
	}

	public void NewSinglePlayer()
	{
		GameData.dataControl.twoPlayer = false;
		GameData.dataControl.Save ();
		Application.LoadLevel ("NEWGAME");
	}
}
