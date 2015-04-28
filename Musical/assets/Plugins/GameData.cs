using UnityEngine;
using System.Collections;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Collections.Generic;


public class GameData : MonoBehaviour {

	public static GameData dataControl;

	private string saveFile = "/playerInfo.dat";
	
//	private List<Level> levelsList = new List<Level>();
//	private List<LastStats> lastLevelPlayedStatsList = new List<LastStats>();
	
	//initialize dictionary
	public Dictionary<string, Level> levels = new Dictionary<string, Level>();
	
	public Dictionary<string, LastStats> lastLevelPlayedStats = new Dictionary<string, LastStats>();
	public float totalRyo;

	void Awake(){
//		Debug.Log (Application.persistentDataPath);
		//create levels for dictionary
		Level level1 = new Level("level1", 10, 50, 150, 10, 20, 30, 0);
		Level level2 = new Level("level2", 10, 75, 200, 15, 25, 40, 0);

		//add levels to dictionary
		levels.Add("level2", level2);
		levels.Add("level1", level1);

		LastStats lastStats = new LastStats("", "", 0, 0, 0);
		lastLevelPlayedStats.Add ("lastStats", lastStats);

		if(dataControl == null)
		{
			Debug.Log ("null");
			DontDestroyOnLoad(gameObject);
			dataControl = this;
		}
		else if(dataControl != this)
		{
			Destroy(gameObject);
		}
	}
	
	void Start()
	{
		Environment.SetEnvironmentVariable("MONO_REFLECTION_SERIALIZER", "yes");
	}
	public void Save()
	{
		BinaryFormatter bf = new BinaryFormatter();
		Debug.Log (Application.persistentDataPath);
		FileStream file = File.Create(GetFullPath());

		PlayerData data = new PlayerData();
		data.totalRyo = totalRyo;
//		data.levels = levels;
//		data.lastLevelPlayedStats = lastLevelPlayedStats;

		//clear values in levelsList 
		data.levelsList.Clear();

		//add values from levels Dict to levelsList
		foreach(KeyValuePair<string, Level> item in levels)
		{
			data.levelsList.Add (item.Value);
		}

		//clear values in lastLevelPlayedStatsList
		data.lastLevelPlayedStatsList.Clear ();

		//add values from lastLevelPlayedStats to stats list
		foreach(KeyValuePair<string, LastStats> item in lastLevelPlayedStats)
		{
			data.lastLevelPlayedStatsList.Add(item.Value);
		}

		//serlialize data and close file
		bf.Serialize(file, data);
		file.Close ();

	}

	public void Load()
	{
//		if(File.Exists (Application.persistentDataPath + "/playerInfo.dat"))
		if(File.Exists (GetFullPath()))
		{
			BinaryFormatter bf = new BinaryFormatter();
			FileStream file = File.Open(GetFullPath(), FileMode.Open);
			
			PlayerData data = (PlayerData)bf.Deserialize(file);

			file.Close ();
//			levelScores = data.levelScores;
			totalRyo = data.totalRyo;
//			lastLevelPlayedStats = data.lastLevelPlayedStats;
//			levels = data.levels;

			//clear levels dictionary
			levels.Clear();

			//set levels dictionary to values from data.levelsList
			foreach(Level item in data.levelsList)
			{
				levels.Add(item.name, item);
			}

			//clear levels dictionary
			lastLevelPlayedStats.Clear ();
			
			//set levels dictionary to values from data.levelsList
			foreach(LastStats stat in data.lastLevelPlayedStatsList)
			{
				lastLevelPlayedStats.Add("lastStats", stat);
			}

			bf.Serialize(file, data);
		}
	}

	public string GetFullPath(){
//		#if UNITY_EDITOR
		Debug.Log (Application.persistentDataPath);
		return Application.persistentDataPath + saveFile;
//		#endif
//
//		#if UNITY_IPHONE
//		//	//		return Application.dataPath + "/../../Documents/" + saveFile;
//		Debug.Log ("iphone path");
//		return Application.persistentDataPath + saveFile + "/";
//		#endif
	}
	

}

[Serializable]
class PlayerData
{
//	public List<float> levelScores = new List<float>();
//	public List<float>levelScores;
	public float totalRyo;
	public List<Level> levelsList =new List<Level>();
	public List<LastStats> lastLevelPlayedStatsList = new List<LastStats>();
//	public Dictionary<string, Level> levels;
//	public List<float> lastLevelPlayedStats;
//	public Dictionary<string, LastStats> lastLevelPlayedStats;
}

[Serializable]
public class LastStats
{
	public string thisLevel;
	public string nextLevel;
	public float score;
	public float stars;
	public float ryo;

	public LastStats(string newThisLevel, string newNextLevel, float newScore, float newStars, float newRyo)
	{
		thisLevel = newThisLevel;
		nextLevel = newNextLevel;
		score = newScore;
		stars = newStars;
		ryo = newRyo;
	}
}

[Serializable]
public class Level
{
	public string name;
	public float minScoreForStar1;
	public float minScoreForStar2;
	public float minScoreForStar3;
	public float ryoForStar1;
	public float ryoForStar2;
	public float ryoForStar3;
	public float starsEarned;

	public Level(string newName, float newMinScoreForStar1, float newMinScoreForStar2, float newMinScoreForStar3, float newRyoForStar1, float newRyoForStar2, float newRyoForStar3, float newStarsEarned)
	{
		name = newName;
		minScoreForStar1 = newMinScoreForStar1;
		minScoreForStar2 = newMinScoreForStar2;
		minScoreForStar3 = newMinScoreForStar3;
		ryoForStar1 = newRyoForStar1; 
		ryoForStar2 = newRyoForStar2;
		ryoForStar3 = newRyoForStar3;
		starsEarned = newStarsEarned;
	}
}

