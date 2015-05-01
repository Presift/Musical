using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InstantiateToBeat : MonoBehaviour {

	public Metronome metronome;
	public VerifyInput input;
	public CSVReader text;


	float instantiationTimeBuffer = 5;
	
	float arrivalBeat;
	
	float instantiationFrequency = 4;
	
	GameObject tap;

	GameObject swipe;

	GameObject endHold;
	GameObject holdLine;

	float lengthPerBeat;
	float originalLineLength;
	

	public List< Vector3 > startPositions;

	Vector3 targetPosition;
	Vector3 destroyPosition;
	
	public GameObject target;

	public List<List<float>> beatsAndInputs;

	List<IncomingObject> musicObjects;

	public float timeForEarlyResponse = .15f;
	public float timeForLateResponse = .15f;

	int currentTurn = 0;

	public IncomingObject nextObject;

	public Sprite tapSprite;
	public Sprite swipeLeft;
	public Sprite swipeRight;
	public Sprite swipeDown;
	public Sprite swipeUp;



	void Awake (){

//		Debug.Log ("instantiate ");
		tap = (GameObject)Resources.Load("Tap");
		swipe = (GameObject)Resources.Load ("Swipe");
		holdLine = (GameObject)Resources.Load ("Held Line");
		endHold = (GameObject)Resources.Load ("EndHold");

	}

	void Start () {

		input.barCenter = target.transform.position;

		if( !input.readPlayerInput )
		{
			beatsAndInputs = text.levelingInfo;
			
			ResizeWidthOfHeldLine ();
			
			targetPosition = target.transform.position;
			destroyPosition = targetPosition - new Vector3 (0, 2, 0);
			
			CreateMusicalObjects ();
			
			input.musicalObjects = musicObjects;
			input.currentObject = musicObjects [0];
			
			nextObject = musicObjects [currentTurn];
			
			arrivalBeat = nextObject.arrivalBeat;
		}

	}
	
	// Update is called once per frame
	void Update () {
	
		if( !input.readPlayerInput )
		{
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

		lengthPerBeat = moveScript.lerpSpeed;

		originalLineLength = lineRenderer.bounds.size.y;
		 
	}

	void CreateMusicalObjects()
	{
		musicObjects = new List<IncomingObject> ();
		int positionIndex = 0;

		for( int i = 0; i < beatsAndInputs.Count; i ++ )
		{
			float onTargetBeat = beatsAndInputs[ i ][ 0 ];
			float startOfInitialInput = onTargetBeat - ( timeForEarlyResponse * (metronome.bpm / 60 ));

			float endOfInitialInput = onTargetBeat + ( timeForLateResponse * (metronome.bpm / 60 ));

			float beatsToHold = beatsAndInputs[ i ][ 1 ] - onTargetBeat;

			int typeOfObject = ( int ) beatsAndInputs[ i ][ 2 ];

			if( positionIndex == startPositions.Count )
			{
				positionIndex = 0;
			}

			Vector3 startPosition = startPositions[ positionIndex ];
			positionIndex ++;

			IncomingObject musicObject = new IncomingObject( typeOfObject, tap, GetSprite( typeOfObject ), startOfInitialInput, endOfInitialInput, onTargetBeat, beatsToHold, i );

			musicObject.CreateHeadOfObject( metronome, startPosition, targetPosition, destroyPosition );

			if( musicObject.held )
			{
				musicObject.CreateHeldObject( holdLine, endHold, lengthPerBeat, originalLineLength );
			}

			Debug.Log ("first input : " + musicObject.firstInput + " second input : " + musicObject.secondInput );
			musicObjects.Add ( musicObject );

		}

	}

	public Sprite GetSprite( int index )
	{
		switch( index )
		{
		case 0:
			return tapSprite;
		case 1:
			return swipeLeft;
		case 2:
			return swipeRight;
		case 3:
			return swipeDown;
		case 4:
			return swipeUp;
		case 5:
			return tapSprite;
		default:
			Debug.Log (" invalid index ");
			//			return inputType.none;
			return tapSprite;
		}
	}
	
}


public class IncomingObject : ScriptableObject
{
	public inputType firstInput;
	public inputType secondInput;

	public inputType expected;
//	public inputType currentInput;
	public bool inputRangeIsForHead;

	public float startInput;
	public float endInput;
	
	public float endHold;
	public GameObject holdLine;

	float previousLineLength;
	public GameObject secondInputObject;
	public bool held;
	float distancePerBeat;

	public bool isBeingHeld;

	public float beatsToHold;
	public float arrivalBeat;

	public GameObject prefab;
	public Sprite mainSprite;

	public GameObject musicObject;
	public MoveToBeat moveScript;
	public ShowFeedback feedbackScript;

	public int index;

	public Vector3 startPosition;

	
	public IncomingObject( int typeOfInput, GameObject newPrefab, Sprite mainInteractionSprite, float start, float end, float arriveBeat, float beatsHeld, int indexNum )
	{
//		expected = GetInputType( typeOfInput );

		inputRangeIsForHead = true;

		startInput = start;
		endInput = end;

		arrivalBeat = arriveBeat;

		beatsToHold = beatsHeld;

		endHold = beatsHeld + arrivalBeat;

		prefab = newPrefab;
		mainSprite = mainInteractionSprite;

		index = indexNum;

		isBeingHeld = false;

		if( beatsToHold > 1 )
		{
			held = true;

			firstInput = inputType.tap;
			secondInput = GetInputType( typeOfInput );

			if( secondInput == inputType.tap )
			{
				secondInput = inputType.up;
			}
		}
		else
		{
			held = false;
			firstInput = GetInputType( typeOfInput );
		}

		expected = firstInput;

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

		//if not a tap object  
		if (!held && expected != inputType.tap) 
		{
			SpriteRenderer mainRenderer = ( SpriteRenderer ) musicObject.GetComponent( typeof( SpriteRenderer ));
			mainRenderer.sprite = mainSprite;
		}
		moveScript = ( MoveToBeat ) musicObject.GetComponent(typeof( MoveToBeat ));
		feedbackScript = ( ShowFeedback ) musicObject.GetComponent(typeof( ShowFeedback ));
		
		moveScript.metronome = metronome;
		// calculate its start values
		
		moveScript.CalculateAndSetValues( startPosition, target, destroy, arrivalBeat, metronome.bpm );

		Activate ( false );

	}

	public void CreateHeldObject( GameObject heldLine, GameObject tailPrefab, float lengthPerBeat, float originalLengthOfLine  )
	{
		distancePerBeat = lengthPerBeat;

		float lengthOfBar = lengthPerBeat * beatsToHold;
		Vector3 positionOfTail = startPosition + new Vector3 (0, lengthOfBar, 0);
		Vector3 positionOfBar = startPosition + new Vector3 (0, lengthOfBar / 2, 0);

		//create tail object
		secondInputObject = ( GameObject ) Instantiate( tailPrefab, positionOfTail, Quaternion.identity );

		//set sprites for head and tail objects
		Debug.Log (" second input : " + secondInput);
		if( secondInput != inputType.up )
		{
			//set tail to sprite in head

			SpriteRenderer tailRenderer = ( SpriteRenderer ) secondInputObject.GetComponent( typeof( SpriteRenderer ));
			tailRenderer.sprite = mainSprite;
		}

		//instantiate new hold line and resize

		holdLine = ( GameObject ) Instantiate( heldLine, positionOfBar, Quaternion.identity );

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
//			Debug.Log (" remaining beats : " + remainingBeats );
//			Debug.Log (" line length : " + previousLineLength + ", line position : " + holdLine.transform.position + ",  input2 position : " + secondInputObject.transform.position);
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
		case 1:
			return inputType.swipeLeft;
		case 2:
			return inputType.swipeRight;
		case 3:
			return inputType.swipeDown;
		case 4:
			return inputType.swipeUp;
		case 5: 
			return inputType.up;
		default:
			return inputType.tap;
		}
	}
	
}

public enum inputType  { tap, swipeLeft, swipeRight, swipeUp, swipeDown, up };


