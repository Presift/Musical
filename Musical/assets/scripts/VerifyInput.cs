using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class VerifyInput : MonoBehaviour {

	public Metronome metronome;

	public List< IncomingObject > musicalObjects;

	public IncomingObject currentObject;

	int currentObjectIndex = 0;


//	public bool checkInput;

	float beatOfInput;

	Vector3 mouseDownPosition;
	float minDistanceForSwipe = 1.0f;
//	float min

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

		if( EndInputForCurrentObject() && currentObjectIndex < ( musicalObjects.Count - 1 ))
		{
			currentObjectIndex ++;
			currentObject = musicalObjects[ currentObjectIndex ];
		}

		//if current object is being held
		if( !currentObject.isBeingHeld && currentObject.held )
		{
			//if remaining beats to hold and not expecting swipe
			if( WithinInputRange() && currentObject.expected == inputType.tap )
			{
				currentObject.feedbackScript.ShowSuccess();
			}
			currentObject.PositionAndScaleHeld( currentObject.endHold - metronome.currentPartialBeats );
		}
	}


	bool CheckInput()
	{
		float beats = metronome.currentPartialBeats;
		
		if( beats >= currentObject.startInput && beats <= currentObject.endInput &&  currentObject.currentInput == inputType.none  )
		{
			return true;
		}
		
		return false;
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
		Debug.Log (metronome.currentPartialBeats);
		beatOfInput = metronome.currentPartialBeats;
		mouseDownPosition = Input.mousePosition;

		//if within input range of current object
		if( WithinInputRange() )
		{
			//if correct input is tap OR 
			if( currentObject.expected == inputType.tap || ( currentObject.held && currentObject.isBeingHeld))
			{


				if( currentObject.held )
				{

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



	void OnMouseExit()
	{
		currentObject.isBeingHeld = false;
	}
			    

	void OnMouseUp()
	{


		inputType swipe = DetermineSwipe (Input.mousePosition);

		float beatsHeld = metronome.currentPartialBeats - beatOfInput;

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

		//if object should be held
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


