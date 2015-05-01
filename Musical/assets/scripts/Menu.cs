using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Menu : MonoBehaviour {

	public Toggle twoPlayer;
//	public Button play;
	
	// Use this for initialization
	void Start () {
		GameData.dataControl.Load ();
		
		if( GameData.dataControl.twoPlayer )
		{
			twoPlayer.isOn = true;
		}
		else
		{
			twoPlayer.isOn = false;
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	
	public void NewGame()
	{

//		GameData.dataControl.Save ();
		
		Application.LoadLevel ("NEWGAME");
	}
	

	
	public void OnTwoPlayerDown()
	{
		if ( twoPlayer.isOn )
		{
			GameData.dataControl.twoPlayer = true;
			
		}
		else
		{
			GameData.dataControl.twoPlayer = false;
			
		}
		
		GameData.dataControl.Save();
	}
}
