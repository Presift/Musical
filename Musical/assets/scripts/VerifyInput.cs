using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class VerifyInput : MonoBehaviour {

	public Metronome metronome;
	public bool readPlayerInput;
	public InstantiateToBeat instantiate;

	public Vector3 barCenter;
	GameObject userCreatedObject;

	public GameObject basicObject;

	public List< IncomingObject > musicalObjects;

	public IncomingObject currentObject;

	int currentObjectIndex = 0;

	float previousEndBeat = 0;

	float beatOfInput;

	Vector3 mouseDownPosition;
	float minDistanceForSwipe = 1.5f;


	void Awake(){

//		Debug.Log ("verify ");
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
				//reposition and rescale tail and train 
				currentObject.PositionAndScaleHeld( currentObject.endHold - metronome.currentPartialBeats );

				//if within range of second input and second input is not a swipe ( this means that player can hold too long and will still be successful )
				if( WithinInputRange() && currentObject.expected == inputType.up )
				{
					currentObject.feedbackScript.ShowSuccess();
				}


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


		if (!readPlayerInput) {
			//if within input range of current object
			if (WithinInputRange ()) {
				//if correct input is tap OR 
				if (currentObject.expected == inputType.tap) {
					
					
					if (currentObject.held) {
						currentObject.isBeingHeld = true;
						currentObject.feedbackScript.StartHold ();
						currentObject.UpdateInputStartEndRange ();
						currentObject.PositionAndScaleHeld (currentObject.endHold - metronome.currentPartialBeats);
						currentObject.expected = currentObject.secondInput;
					} else {
						// show successful feedback on musical object
						currentObject.feedbackScript.ShowSuccess ();
					}
				}
				
			}
		} else 
		{
			//create basic music object at center of bar
			userCreatedObject = ( GameObject ) Instantiate ( basicObject, barCenter, Quaternion.identity );
		}



	}

	float GetClosestHalfBeat( float beat )
	{
		float closest = Mathf.Round (beat / .5f) * .5f; 
//		Debug.Log (closest);
		return closest;
	}

	void EndOfInput()
	{
		float endBeat = metronome.currentPartialBeats;

		float beatsHeld = endBeat - beatOfInput;

		inputType swipe = DetermineSwipe (Input.mousePosition);
//		Debug.Log (" swipe : " + swipe);
		int interactionType = GetInputTypeAsInt (swipe);

		if( readPlayerInput )
		{
			//set sprite
			int swipeInt = GetInputTypeAsInt( swipe );
			Sprite newSprite = instantiate.GetSprite( swipeInt );
			//destroy user created object
			ShowFeedback feedbackScript = (ShowFeedback ) userCreatedObject.GetComponent( typeof(ShowFeedback));
			feedbackScript.SetSprite( newSprite );
			feedbackScript.TimeDestruction();


			float normalizedEndBeat = GetClosestHalfBeat( endBeat );
			float normalizedStartBeat = GetClosestHalfBeat( beatOfInput );

			string saveData = "";
			saveData += normalizedStartBeat + ",";
			saveData += normalizedEndBeat + ",";
			saveData += interactionType.ToString() + ",";

			//if previous input does not overlap with current input
			if( normalizedStartBeat > previousEndBeat )
			{
				//record data
				GameData.dataControl.SavePerformanceStats (saveData);
				previousEndBeat = normalizedEndBeat;
			}

		}
		if( !readPlayerInput )
		{
		
			Debug.Log (" received : " + swipe + ", expected : " + currentObject.expected );

			if (WithinInputRange ()) 
			{
//				Debug.Log (" received : " + swipe + ", expected : " + currentObject.expected );
				if( currentObject.expected == swipe )
				{
					if( swipe != inputType.tap && swipe != inputType.up )
					{
						Debug.Log ( swipe + " at " + metronome.currentPartialBeats );
					}

					Debug.Log ("ATTEMPTING TO DESTROY");
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
		case inputType.tap:
			return 0;
		case inputType.swipeLeft:
			return 1;
		case inputType.swipeRight:
			return 2;
		case inputType.swipeDown:
			return 3;
		case inputType.swipeUp:
			return 4;
		case inputType.up:
			return 5;
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

		return inputType.up;
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


