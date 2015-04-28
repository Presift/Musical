using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CSVToList : MonoBehaviour {
//
//	// Use this for initialization
//	void Start () {
//	
//	}
//	
//	// Update is called once per frame
//	void Update () {
//	
//	}
//
//	static public string[,] Output2DArray(string[,] grid){
//		//make blank string array
//		float rows = (Mathf.Ceil(grid.GetUpperBound(0)/7)) -1;
//		//	Debug.Log(rows);
//		float[,] wholeArray = new float[rows, 3];
//		//iterate over each item in string
//		int i =0;
//		int j = 0;
//		for(int y = 0; y < grid.GetUpperBound(1); y++){
//			for(int x =0; x < grid.GetUpperBound(0) -1; x++){
//				if(i > (rows -1)){
//					return wholeArray;
//				}
//				//if row is divisible evenly by 7, 
//				if((x+1)%7 == 0){
//					//increase row count
//					i ++;
//					//reset column position
//					j = 0;
//				}else{
//					//if xy value is midi clock info
//					if(j == 1){
//						//parse as float
//						float midiClockTime = float.Parse(grid[x,y]);
//						//divide by 48; (the above number is  midi clock time, running at 24 ppqn (pules per quarter note)
//						float beat = midiClockTime/48;
//						//add value to array
//						wholeArray[i,0] = beat;	
//					}else if(j ==2) //if xy value is note_on or note_off
//					{
//						//if string value is Note_on_c
//						if(grid[x,y] == " Note_on_c")
//							//add 1
//						{
//							wholeArray[i, 2] = 1;
//							//						Debug.Log(1);
//						}
//						//if string value is Note_off_c
//						else
//							//add 0
//						{
//							wholeArray[i, 2] = 0;
//							//						Debug.Log(grid[x,y]);
//						}
//					}else if(j ==4){
//						//parse as float
//						float enemyType = float.Parse(grid[x, y]);
//						wholeArray[i, 1] = enemyType;
//					}
//					j++;
//				}
//			}
//		}
//		return wholeArray;
//	}
//}
//
//public class EnemyInfo
//{
//	public int enemyNumber;
//	public float arrivalBeat;
//	public float endBeat;
//	
//	public EnemyInfo(int enemyNum, float arrBeat, float lastBeat)
//	{
//		enemyNumber = enemyNum;
//		arrivalBeat = arrBeat;
//		endBeat = lastBeat;
//	}
}

