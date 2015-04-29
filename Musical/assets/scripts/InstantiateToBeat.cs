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
//	GameObject tapAndHold;
	GameObject swipe;
//	GameObject swipeAndHold;
	GameObject endHold;
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

	Sprite tapSprite;
//	Sprite upSprite;

	void Awake (){

		tap = (GameObject)Resources.Load("Tap");
		swipe = (GameObject)Resources.Load ("Swipe");
		holdLine = (GameObject)Resources.Load ("Held Line");
		endHold = (GameObject)Resources.Load ("EndHold");

		SpriteRenderer tapRenderer = (SpriteRenderer)tap.GetComponent (typeof(SpriteRenderer));
		tapSprite = tapRenderer.sprite;
	}

	void Start () {

		ResizeWidthOfHeldLine ();

		targetPosition = target.transform.position;
		destroyPosition = targetPosition - new Vector3 (0, 2, 0);

		CreateMusicalObjects ();

		input.musicalObjects = musicObjects;
		input.currentObject = musicObjects [0];


		


		nextObject = musicObjects [currentTurn];

		arrivalBeat = nextObject.arrivalBeat;
	}
	
	// Update is called once per frame
	void Update () {
	
		//if time to instantiate new object
		if( metronome.currentPartialBeats > ( arrivalBeat - instantiationTimeBuffer) )
		{
			nextObject.Activate ( true );

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

//	void SetNewLineTransform( float beatsHeld, GameObject heldLine, Vector3 bottomOfLine )
//	{
//		float newLengthOfLine = beatsHeld * lengthPerBeat;
//
//		float currentLengthOfLine = originalLineLength;
//
//
//	}

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

		lengthPerBeat = moveScript.lerpSpeed;

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

			float endOfInitialInput = onTargetBeat + ( timeForLateResponse * (metronome.bpm / 60 ));

			float beatsToHold = arrivalBeats[ i ].z;

			int typeOfObject = ( int ) arrivalBeats[ i ].y;

			if( positionIndex == startPositions.Count )
			{
				positionIndex = 0;
			}

			Vector3 startPosition = startPositions[ positionIndex ];
			positionIndex ++;

			IncomingObject musicObject = new IncomingObject( typeOfObject, startOfInitialInput, endOfInitialInput, onTargetBeat, beatsToHold, GetUpcomingObject( typeOfObject), i );

			musicObject.CreateHeadOfObject( metronome, startPosition, targetPosition, destroyPosition );

			if( musicObject.held )
			{
				musicObject.CreateHeldObject( holdLine, endHold, tapSprite, lengthPerBeat, originalLineLength );
			}
//
			musicObjects.Add ( musicObject );

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



public class IncomingObject : ScriptableObject
{
	
	public inputType expected;
	public inputType currentInput;
	public bool inputRangeIsForHead;

	public float startInput;
	public float endInput;
	
	public float endHold;
	public GameObject holdLine;
//	SpriteRenderer lineRenderer;
	float previousLineLength;
	public GameObject secondInputObject;
	public bool held;
	float distancePerBeat;

	public bool isBeingHeld;

	public float beatsToHold;
	public float arrivalBeat;

	public GameObject prefab;

	public GameObject musicObject;
	public MoveToBeat moveScript;
	public ShowFeedback feedbackScript;

	public int index;

	public Vector3 startPosition;

	
	public IncomingObject( int typeOfInput, float start, float end, float arriveBeat, float beatsHeld, GameObject newObject, int indexNum )
	{
		expected = GetInputType( typeOfInput );
		currentInput = inputType.none;
		inputRangeIsForHead = true;

		startInput = start;
		endInput = end;

		arrivalBeat = arriveBeat;
//		Debug.Log (" arrival beat set to : " + arriveBeat);
		beatsToHold = beatsHeld;

		endHold = beatsHeld + arrivalBeat;

		prefab = newObject;

		index = indexNum;

		isBeingHeld = false;

//		GameObject newThing = ( GameObject ) Instantiate
		if( beatsToHold > 1 )
		{
			held = true;

			//if main music 
		}
		else
		{
			held = false;
		}

	}

	public void UpdateInputStartEndRange()
	{
		isBeingHeld = true;
		inputRangeIsForHead = false;
		startInput += beatsToHold;
		endInput += beatsToHold;
	}


	public void CreateHeadOfObject( Metronome metronome, Vector3 startPos, Vector3 target, Vector3 destroy )
	{
		startPosition = startPos;

		// instantiate object
		musicObject = ( GameObject ) Instantiate( prefab, startPosition, Quaternion.identity );
		musicObject.name += index;
		
		moveScript = ( MoveToBeat ) musicObject.GetComponent(typeof( MoveToBeat ));
		feedbackScript = ( ShowFeedback ) musicObject.GetComponent(typeof( ShowFeedback ));
		
		moveScript.metronome = metronome;
		// calculate its start values
		
		moveScript.CalculateAndSetValues( startPosition, target, destroy, arrivalBeat, metronome.bpm );

		Activate ( false );

	}

	public void CreateHeldObject( GameObject heldLine, GameObject tailPrefab, Sprite tapSprite, float lengthPerBeat, float originalLengthOfLine  )
	{
		distancePerBeat = lengthPerBeat;

		float lengthOfBar = lengthPerBeat * beatsToHold;
		Vector3 positionOfTail = startPosition + new Vector3 (0, lengthOfBar, 0);
		Vector3 positionOfBar = startPosition + new Vector3 (0, lengthOfBar / 2, 0);

		//create tail object
		secondInputObject = ( GameObject ) Instantiate( tailPrefab, positionOfTail, Quaternion.identity );

		//set sprites for head and tail objects
		if( expected != inputType.tap )
		{
			SpriteRenderer mainInputRenderer = ( SpriteRenderer ) musicObject.GetComponent( typeof( SpriteRenderer ));
			Sprite mainInputSprite = mainInputRenderer.sprite;

	
			//set tail to sprite in head
			SpriteRenderer tailRenderer = ( SpriteRenderer ) secondInputObject.GetComponent( typeof( SpriteRenderer ));
			tailRenderer.sprite = mainInputSprite;
			
			//set head to tap
			mainInputRenderer.sprite = tapSprite;
		}

		//instantiate new hold line and resize

		holdLine = ( GameObject ) Instantiate( heldLine, positionOfBar, Quaternion.identity );

//		lineRenderer = (SpriteRenderer)holdLine.GetComponent (typeof(SpriteRenderer));

//		Debug.Log ("length per beat : " + lengthPerBeat);
//		Debug.Log (" original : " + originalLengthOfLine);
		float scaleChangeOfLength = lengthOfBar / originalLengthOfLine;

		//size line for beat length

		holdLine.transform.localScale = new Vector3 ( holdLine.transform.localScale.x, holdLine.transform.localScale.y * scaleChangeOfLength, holdLine.transform.localScale.z );


		holdLine.transform.parent = musicObject.transform;
		secondInputObject.transform.parent = musicObject.transform;

		previousLineLength = lengthOfBar;

	}

	public void PositionAndScaleHeld( float remainingBeats )
	{
		if (remainingBeats > 0) 
		{
			Debug.Log (" line length : " + previousLineLength + ", line position : " + holdLine.transform.position + ",  input2 position : " + secondInputObject.transform.position);
			float newDistanceAwayFromBar = remainingBeats * distancePerBeat;
			
			//resize and reposition line
			float scaleChangeOfLength = newDistanceAwayFromBar / previousLineLength;
			
			previousLineLength = newDistanceAwayFromBar;
			
			holdLine.transform.localScale = new Vector3 ( holdLine.transform.localScale.x, holdLine.transform.localScale.y * scaleChangeOfLength, holdLine.transform.localScale.z );
			
			holdLine.transform.position = musicObject.transform.position + new Vector3 (0, newDistanceAwayFromBar / 2, 0);
			
			secondInputObject.transform.position = musicObject.transform.position + new Vector3 (0, newDistanceAwayFromBar, 0); 
		}


	}

	public void Activate( bool activate )
	{
		if( activate )
		{
			musicObject.SetActive ( true );
		}
		else
		{
			musicObject.SetActive ( false );
		}
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
