using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InstantiateToBeat : MonoBehaviour {

	public Metronome metronome;
	public VerifyInput input;
	float instantiationTimeBuffer = 5;
	
	float arrivalBeat;
	
	float instantiationFrequency = 4;
	
	GameObject tap;
	GameObject tapAndHold;
	GameObject swipe;
	GameObject swipeAndHold;

	GameObject holdLine;

	float lengthPerBeat;
	float originalLineLength;

//	GameObject objectToInstantiate;

	public List< Vector3 > startPositions;
//	Vector3 startPosition = new Vector3( 0, 6.25f , 0 );
	Vector3 targetPosition;
	Vector3 destroyPosition;
	
	public GameObject target;
	
//	float widthOfTarget;
	public List<Vector3> arrivalBeats;
	List<IncomingObject> musicObjects;

	public float timeForEarlyResponse = .1f;
	public float timeForLateResponse = .08f;

	int currentTurn = 0;

	public IncomingObject nextObject;

	void Awake (){

		tap = (GameObject)Resources.Load("Tap");
		swipe = (GameObject)Resources.Load ("Swipe");
		holdLine = (GameObject)Resources.Load ("Held Line");

	}

	void Start () {

		ResizeWidthOfHeldLine ();

		CreateMusicalObjects ();

		input.musicalObjects = musicObjects;
		input.currentObject = musicObjects [0];

		targetPosition = target.transform.position;
		destroyPosition = targetPosition - new Vector3 (0, 2, 0);
		


		nextObject = musicObjects [currentTurn];

		arrivalBeat = nextObject.arrivalBeat;
	}
	
	// Update is called once per frame
	void Update () {
	
		//if time to instantiate new object
		if( metronome.currentPartialBeats > ( arrivalBeat - instantiationTimeBuffer) )
		{
			// instantiate object
			GameObject newObject = ( GameObject ) Instantiate( nextObject.prefab, nextObject.startPosition, Quaternion.identity );
			newObject.name += nextObject.index;
			MoveToBeat moveScript = ( MoveToBeat ) newObject.GetComponent(typeof( MoveToBeat ));
			ShowFeedback feedback = ( ShowFeedback ) newObject.GetComponent(typeof( ShowFeedback ));

			moveScript.metronome = metronome;
			// calculate its start values

			moveScript.CalculateAndSetValues( nextObject.startPosition, targetPosition, destroyPosition, arrivalBeat, metronome.bpm );

			//set create objects in musical objects
			nextObject.musicObject = newObject;
			nextObject.moveScript = moveScript;
			nextObject.feedbackScript = feedback;

			if( nextObject.beatsToHold >= 1 )
			{
				GameObject newLine = ( GameObject ) Instantiate( holdLine, nextObject.startPosition, Quaternion.identity );
				nextObject.CreateHeldObject( newLine );
			}

			currentTurn ++;

			if( currentTurn < musicObjects.Count )
			{
				nextObject = musicObjects [currentTurn];

				arrivalBeat = nextObject.arrivalBeat;

			}
			else
			{
				arrivalBeat = 10000;
			}
			
		}

	}

	void SetNewLineTransform( float beatsHeld, GameObject heldLine, Vector3 bottomOfLine )
	{
		float newLengthOfLine = beatsHeld * lengthPerBeat;

		float currentLengthOfLine = originalLineLength;


	}

	void ResizeWidthOfHeldLine()
	{

		SpriteRenderer circleRenderer = (SpriteRenderer)tap.GetComponent (typeof(SpriteRenderer));
		float widthOfInputCircle = circleRenderer.bounds.size.x;

		SpriteRenderer lineRenderer  = (SpriteRenderer)holdLine.GetComponent (typeof(SpriteRenderer));
		float currentWidthOfLine = lineRenderer.bounds.size.x;

		float desiredWidthOfLine = widthOfInputCircle / 3;
		float scaleChangeOfLine = desiredWidthOfLine / currentWidthOfLine;

		holdLine.transform.localScale *= scaleChangeOfLine;

		MoveToBeat moveScript = (MoveToBeat)tap.GetComponent (typeof(MoveToBeat));

		float lengthPerBeat = moveScript.lerpSpeed;

		originalLineLength = lineRenderer.bounds.size.y;
		 
	}

	void CreateMusicalObjects()
	{
		musicObjects = new List<IncomingObject> ();
		int positionIndex = 0;

		for( int i = 0; i < arrivalBeats.Count; i ++ )
		{
			float onTargetBeat = arrivalBeats[ i ].x;
			float startOfInitialInput = onTargetBeat - ( timeForEarlyResponse * (metronome.bpm / 60 ));
//			Debug.Log ("start : " + startOfInitialInput );
			float endOfInitialInput = onTargetBeat + ( timeForLateResponse * (metronome.bpm / 60 ));
//			Debug.Log ("end : " + endOfInitialInput );
			float beatsToHold = arrivalBeats[ i ].z;

			int typeOfObject = ( int ) arrivalBeats[ i ].y;

			if( positionIndex == startPositions.Count )
			{
				positionIndex = 0;
			}

			Vector3 startPosition = startPositions[ positionIndex ];
			positionIndex ++;

			IncomingObject newObject = new IncomingObject( typeOfObject, startOfInitialInput, endOfInitialInput, onTargetBeat, beatsToHold, GetUpcomingObject( typeOfObject), i, startPosition );

			musicObjects.Add ( newObject );

		}

	}


	GameObject GetUpcomingObject( int index )
	{
		switch( index )
		{
		case 0:
			return tap;
//		case 1:
//			return inputType.hold;
		case 1:
			return swipe;
		case 2:
			return swipe;
		default:
			Debug.Log (" invalid index ");
//			return inputType.none;
			return tap;
		}
	}
}



public class IncomingObject
{
	
	public inputType expected;
	public inputType currentInput;
	
	public float startInput;
	public float endInput;
	
	public float endHold;
	public GameObject secondInputObject;
	public bool held;

	public float beatsToHold;
	public float arrivalBeat;

	public GameObject prefab;

	public GameObject musicObject;
	public MoveToBeat moveScript;
	public ShowFeedback feedbackScript;

	public int index;

	public Vector3 startPosition;

	
	public IncomingObject( int typeOfInput, float start, float end, float arriveBeat, float beatsHeld, GameObject newObject, int indexNum, Vector3 startPos )
	{
		expected = GetInputType( typeOfInput );
		currentInput = inputType.none;
		
		startInput = start;
		endInput = end;
		
		arrivalBeat = arriveBeat;
		beatsToHold = beatsHeld;
		endHold = beatsHeld + arrivalBeat;

		prefab = newObject;

		index = indexNum;

		startPosition = startPos;

		if( beatsToHold > 1 )
		{
			held = true;
		}
		else
		{
			held = false;
		}
	}

	public void CreateHeldObject( GameObject heldLine )
	{
		SpriteRenderer lineRenderer = (SpriteRenderer)heldLine.GetComponent (typeof(SpriteRenderer));
	}
	
	inputType GetInputType( int typeOfInput )
	{
		switch( typeOfInput )
		{
		case 0:
			return inputType.tap;
		case 2:
			return inputType.swipeLeft;
		case 3:
			return inputType.swipeRight;
		case 4:
			return inputType.swipeUp;
		case 5:
			return inputType.swipeDown;
		default:
			return inputType.none;
		}
	}
	
}

public enum inputType  { tap, swipeLeft, swipeRight, swipeUp, swipeDown, none };
