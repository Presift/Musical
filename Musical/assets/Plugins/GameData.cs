using UnityEngine;
using System.Collections;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
//using System.IO.StreamWriter;
using System.Collections.Generic;

public class GameData : MonoBehaviour {
	
	public static GameData dataControl;

	public TextAsset playerComposedData;

	public string composerInput = "/composerInput.txt";
	private string gameSetUp = "/playerInfo.txt";

	public bool twoPlayer;
	public bool player1TurnComplete;

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
		PlayerTurn ();
	}
	
	void Start()
	{
	
	}
	
	public void PlayerTurn()
	{
		if (System.IO.File.Exists (GetFullPath (composerInput))) 
		{
			player1TurnComplete = true;
//			playerComposedData = GetFullPath( composerInput );
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

//		Debug.Log (composition);
		return composition;

	}
	
	public void Save()
	{
		
//		StreamWriter file = new StreamWriter (GetFullPath (dataFile));
//		
//		PlayerData data = new PlayerData();
//		data.previousFinalLevel = previousFinalLevel;
//		data.debugOn = debugOn;
//		data.consecutiveModusTollensIncorrect = consecutiveModusTollensIncorrect;
//		
//		file.WriteLine ( data.previousFinalLevel );
//		file.WriteLine (data.debugOn);
//		file.WriteLine (data.consecutiveModusTollensIncorrect);
//		file.WriteLine (data.fitTestTaken);
//		file.WriteLine (data.shortGame);
//		
//		file.Close ();
		
	}

	public void Load()
	{
//		if(File.Exists (GetFullPath( dataFile )))
//		{
//			string filePath = GetFullPath( dataFile );
//			Debug.Log (Application.persistentDataPath);
//			
//			StreamReader data = new StreamReader( filePath );
//			
//			//			previousFinalLevel = Convert.ToInt32( data.ReadLine() );
//			previousFinalLevel = float.Parse( data.ReadLine ());
//			
//			debugOn = Convert.ToBoolean( data.ReadLine ());
//			
//			consecutiveModusTollensIncorrect = Convert.ToInt32( data.ReadLine() );
//			
//			fitTestTaken = Convert.ToBoolean( data.ReadLine ());
//			
//			shortGame = Convert.ToBoolean( data.ReadLine ());
//			
//			data.Close ();
//		}
		
	}


	
	public string GetFullPath( string saveFile ){

		return Application.persistentDataPath + saveFile;
		
	}
}

class PlayerData
{
	public float previousFinalLevel;
	public bool debugOn;
	public int consecutiveModusTollensIncorrect;
	public bool fitTestTaken;
	public bool shortGame;
}
