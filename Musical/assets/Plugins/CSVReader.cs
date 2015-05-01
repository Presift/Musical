using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq; 


public class CSVReader : MonoBehaviour 
{
	public TextAsset csvFile; 

	public List< List<float> > levelingInfo;


	
	void Awake()
	{
		string[,] grid = null;

		GameData.dataControl.Load ();

		//if two player and is second player's turn
		if (GameData.dataControl.twoPlayer && GameData.dataControl.player1TurnComplete ) 
		{
			Debug.Log ("reading player composition");
			grid = SplitCsvGrid(GameData.dataControl.ReadPlayerComposition());

		
		}
		else if( !GameData.dataControl.twoPlayer )
		{
			grid = SplitCsvGrid(csvFile.text);
		}

		if( grid != null )
		{
			
			levelingInfo = OutputLevelInfo (grid);
			Debug.Log (" object count : " + levelingInfo.Count );
		}
		 
	}
	
	
	
	// outputs the content of a 2D array, useful for checking the importer
	static public void DebugOutputGrid(string[,] grid)
	{
		string textOutput = ""; 
		
		for (int y = 0; y < grid.GetUpperBound(1); y++) {	
			for (int x = 0; x < grid.GetUpperBound(0); x++) {

				textOutput += grid[x,y]; 
				textOutput += "|";
			}
			textOutput += "\n"; 
		}
		Debug.Log(textOutput);
	}
	
	static public List< List<float> > OutputLevelInfo(string[,] grid)
	{
		List< List<float> > levelingInfo = new List<List<float>> ();
		
		
		for (int y = 1; y < grid.GetUpperBound(1); y++) 
		{	
			List< float > singleLevelInfo = new List<float>();
			
			for (int x = 0; x < grid.GetUpperBound(0); x++) 
			{

				float value = float.Parse( grid[x,y] );
//				Debug.Log (value);
				singleLevelInfo.Add ( value );

			}
			levelingInfo.Add ( singleLevelInfo );
		}
		
		return levelingInfo;
	}
	
	
	// splits a CSV file into a 2D string array
	static public string[,] SplitCsvGrid(string csvText)
	{
		string[] lines = csvText.Split("\n"[0]); 
		// finds the max width of row
		int width = 0; 
		for (int i = 0; i < lines.Length; i++)
		{
			string[] row = SplitCsvLine( lines[i] ); 
			width = Mathf.Max(width, row.Length);

		}
		
		// creates new 2D string grid to output to
		string[,] outputGrid = new string[width + 1, lines.Length + 1]; 
		for (int y = 0; y < lines.Length; y++)
		{
			string[] row = SplitCsvLine( lines[y] ); 
			for (int x = 0; x < row.Length; x++) 
			{
				outputGrid[x,y] = row[x]; 
				
				// This line was to replace "" with " in my output. 
				// Include or edit it as you wish.
				outputGrid[x,y] = outputGrid[x,y].Replace("\"\"", "\"");
			}
		}
		
		return outputGrid; 
	}
	
	// splits a CSV row 
	static public string[] SplitCsvLine(string line)
	{
		return (from System.Text.RegularExpressions.Match m in System.Text.RegularExpressions.Regex.Matches(line,
		                                                                                                    @"(((?<x>(?=[,\r\n]+))|""(?<x>([^""]|"""")+)""|(?<x>[^,\r\n]+)),?)", 
		                                                                                                    System.Text.RegularExpressions.RegexOptions.ExplicitCapture)
		        select m.Groups[1].Value).ToArray();
	}
}