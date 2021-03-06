﻿using UnityEngine;
using System.Collections;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
//using System.IO.StreamWriter;
using System.Collections.Generic;

public class GameData : MonoBehaviour {
	
	public static GameData dataControl;


//	public TextAsset playerComposedData;

	private string composerInput = "/composerInput.txt";
	private string gameSetUp = "/playerInfo.txt";

	public bool twoPlayer;
	public bool player1TurnComplete;
	public bool player2TurnComplete;

	void Awake(){

		if(dataControl == null)
		{
			DontDestroyOnLoad(gameObject);
			dataControl = this;
		}

		else if(dataControl != this)
		{
			Destroy(gameObject);
		}

		Debug.Log ("player turn set ");
//		PlayerTurn ();
	}
	
	void Start()
	{
	
	}
	
	public void PlayerTurn()
	{
		if (System.IO.File.Exists (GetFullPath (composerInput))) 
		{
			player1TurnComplete = true;
		}

		else
		{
			player1TurnComplete = false;
		}
	}

	public void DeletePerformanceStats()
	{
		string filePath = GetFullPath (composerInput);
		System.IO.File.Delete (filePath);
	}
	
	public void SavePerformanceStats( string stats )
	{
		string filePath = GetFullPath (composerInput);
//		Debug.Log (filePath);
		System.IO.File.AppendAllText(filePath, "\n" + stats );
	}

	public string ReadPlayerComposition()
	{
		string filePath = GetFullPath (composerInput);

		StreamReader data = new StreamReader (filePath);

		string composition = data.ReadToEnd ();

		Debug.Log (filePath);
		return composition;

	}
	
	public void Save()
	{
		
		StreamWriter file = new StreamWriter (GetFullPath (gameSetUp));
		
		PlayerData data = new PlayerData();

		data.twoPlayer = twoPlayer;
		data.player1TurnComplete = player1TurnComplete;
		data.player2TurnComplete = player2TurnComplete;

		file.WriteLine ( data.twoPlayer );
		file.WriteLine (data.player1TurnComplete);
		file.WriteLine (data.player2TurnComplete);
		
		file.Close ();
		
	}

	public void Load()
	{
		if(File.Exists (GetFullPath( gameSetUp )))
		{
			string filePath = GetFullPath( gameSetUp );
			Debug.Log (filePath);
			
			StreamReader data = new StreamReader( filePath );
					
			twoPlayer = Convert.ToBoolean( data.ReadLine ());
			player1TurnComplete = Convert.ToBoolean( data.ReadLine ());
			player2TurnComplete = Convert.ToBoolean( data.ReadLine ());

			data.Close ();
		}
		
	}


	
	public string GetFullPath( string saveFile ){

		return Application.persistentDataPath + saveFile;
		
	}
}

class PlayerData
{
	public bool twoPlayer;
	public bool player1TurnComplete;
	public bool player2TurnComplete;

}
