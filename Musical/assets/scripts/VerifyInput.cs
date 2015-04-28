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
//			Debug.Log ( currentObjectIndex + ", object count : " + musicalObjects.Count);
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
			//if correct input is tap
			if( currentObject.expected == inputType.tap )
			{
				// show successful feedback on musical object
				currentObject.feedbackScript.ShowSuccess();
			}

		}

	}
		    

	void OnMouseUp()
	{
		inputType swipe = DetermineSwipe (Input.mousePosition);

		float beatsHeld = metronome.currentPartialBeats - beatOfInput;

//		Debug.Log (" swipe type : " + swipe);
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

//	public void StartCheckingInput()
//	{
//		checkInput = true;
//	}
//
//	public void StopCheckingInput()
//	{
//		checkInput = false;
//		currentInput = inputType.none;
//	}
//
//	public void SetExpectedInput()
//	{
//
//	}

//		public float SetInputStats( Metronome masterMetronome, float bpm, float arrivalBeat )
//		{
//			metronome = masterMetronome;
//			startInput = arrivalBeat - ( timeForEarlyResponse * (bpm / 60 ));
//			endInput = arrivalBeat + ( timeForLateResponse * (bpm / 60 ));
//			endHold = arrivalBeat + holdTime;								//may need to be adjusted for early response time
//			
//			return endInput;
//		}
	 
}


