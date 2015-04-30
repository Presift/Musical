using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class VerifyInput : MonoBehaviour {

	public Metronome metronome;
	public bool readPlayerInput;

	public List< IncomingObject > musicalObjects;

	public IncomingObject currentObject;

	int currentObjectIndex = 0;

//	public bool twoPlayer;

	float beatOfInput;

	Vector3 mouseDownPosition;
	float minDistanceForSwipe = 1.5f;


	void Awake(){

		Debug.Log ("verify ");
		if ( !GameData.dataControl.player1TurnComplete && GameData.dataControl.twoPlayer ) 
		{
			readPlayerInput = true;
		}
		else
		{
			readPlayerInput = false;
		}

	}
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

		if( !readPlayerInput )
		{
			if( EndInputForCurrentObject() && currentObjectIndex < ( musicalObjects.Count - 1 ))
			{
				currentObjectIndex ++;
				currentObject = musicalObjects[ currentObjectIndex ];
			}
			
			//if current object is being held
			if( currentObject.isBeingHeld && currentObject.held )
			{
				//if remaining beats to hold and not expecting swipe
				if( WithinInputRange() && currentObject.expected == inputType.tap )
				{
					currentObject.feedbackScript.ShowSuccess();
				}
				currentObject.PositionAndScaleHeld( currentObject.endHold - metronome.currentPartialBeats );
			}
		}

	}



	bool WithinInputRange()
	{
		return (metronome.currentPartialBeats <= currentObject.endInput && metronome.currentPartialBeats >= currentObject.startInput);

	}

	bool EndInputForCurrentObject()
	{
		if( metronome.currentPartialBeats > currentObject.endInput )
		{
//			Debug.Log (" current beats : " + metronome.currentPartialBeats + ", end input : " + currentObject.endInput );
			return true;
		}

		return false;
	}

	void OnMouseDown()
	{
		beatOfInput = metronome.currentPartialBeats;
		mouseDownPosition = Input.mousePosition;


		if (!readPlayerInput) 
		{
			//if within input range of current object
			if( WithinInputRange() )
			{
				//if correct input is tap OR 
				if( currentObject.expected == inputType.tap ||  currentObject.held )
				{
					
					
					if( currentObject.held )
					{
						currentObject.isBeingHeld = true;
						currentObject.feedbackScript.StartHold();
						currentObject.UpdateInputStartEndRange();
						currentObject.PositionAndScaleHeld( currentObject.endHold - metronome.currentPartialBeats );
						
						//					Debug.Log ( 
					}
					else
					{
						// show successful feedback on musical object
						currentObject.feedbackScript.ShowSuccess();
					}
				}
				
			}
		}



	}

	float GetClosestHalfBeat( float beat )
	{
		float closest = Mathf.Round (beat / .5f) * .5f; 
		Debug.Log (closest);
		return closest;
	}

	void EndOfInput()
	{
		float endBeat = metronome.currentPartialBeats;

		float beatsHeld = endBeat - beatOfInput;

		inputType swipe = DetermineSwipe (Input.mousePosition);

		int interactionType = GetInputTypeAsInt (swipe);

		if( readPlayerInput )
		{
			string saveData = "";
			saveData += GetClosestHalfBeat( beatOfInput).ToString() + ",";
			saveData += GetClosestHalfBeat( endBeat ).ToString() + ",";
			saveData += interactionType.ToString() + ",";

			GameData.dataControl.SavePerformanceStats (saveData);
		}
		if( !readPlayerInput )
		{
		
			if (WithinInputRange ()) 
			{
				if( currentObject.expected == swipe )
				{
					
					currentObject.feedbackScript.ShowSuccess();
					
				}

				else if( currentObject.expected == inputType.tap && !currentObject.isBeingHeld )

				{
					currentObject.feedbackScript.ShowSuccess();
				}
			}
			//if held object is let go early
			else if( currentObject.held && currentObject.isBeingHeld )
			{
				currentObject.feedbackScript.UnsuccessfulHoldContinuesToDestruction();
			}
		
		currentObject.isBeingHeld = false;

		}


	}

	int GetInputTypeAsInt( inputType input )
	{


		switch ( input )
		{
		case inputType.none:
			return 0;
		case inputType.swipeLeft:
			return 1;
		case inputType.swipeRight:
			return 2;
		case inputType.swipeDown:
			return 3;
		case inputType.swipeUp:
			return 4;
		default:
			return 0;

		}
	}

	void OnMouseExit()
	{
		EndOfInput();
	}
			    

	void OnMouseUp()
	{


		EndOfInput ();

		
	



	}

	inputType DetermineSwipe( Vector3 endPosition )
	{

		float horizontalDistance = HorizontalDistance (mouseDownPosition, endPosition);

		float verticalDistance = VerticalDistance (mouseDownPosition, endPosition);

		if( Mathf.Abs ( horizontalDistance ) >= minDistanceForSwipe && Mathf.Abs( horizontalDistance ) > Mathf.Abs( verticalDistance ))
		{
			if( horizontalDistance < 0 )
			{

				return inputType.swipeLeft;

			}

			return inputType.swipeRight;

		}

		if( Mathf.Abs ( verticalDistance ) >= minDistanceForSwipe )
		{
			if( verticalDistance < 0 )
			{
				return inputType.swipeDown;
			}

			return inputType.swipeUp;

		}

		return inputType.none;
	}



	float HorizontalDistance( Vector3 start, Vector3 end )
	{
		float horizontalDistance = end.x - start.x;

		return horizontalDistance;
	}

	float VerticalDistance( Vector3 start, Vector3 end )
	{
		float verticalDistance = end.y - start.y;
		
		return verticalDistance;
	}

}


