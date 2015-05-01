using UnityEngine;
using System.Collections;

public class MoveToBeat : MonoBehaviour {

	public ShowFeedback showFeedback;

	public float advanceBufferInSeconds;
	private float advanceBufferInBeats;
	
	public Metronome metronome;
	
	public bool targetReached = false;
	public bool continueToDestructionPosition = true;
	
	public float lerpSpeed;
	private Vector3 startPosition;
	private Vector3 targetPosition;
	private Vector3 destroyPosition;

	public float startBeat;
	private float endBeat;
	private float journeyLength;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if(metronome.currentPartialBeats >= startBeat){
			if(!targetReached || continueToDestructionPosition ){
				Lerp();
			}
		}
	}


	public void CalculateAndSetValues( Vector3 startPos, Vector3 targetPos, Vector3 destroyPos, float arrivalBeat, float bpm ){

		advanceBufferInBeats = advanceBufferInSeconds * (bpm/60);
		endBeat = arrivalBeat;

//		Debug.Log (" end beat : " + endBeat);

		startPosition = startPos;
//		Debug.Log ("start Position : " + startPos);

		targetPosition = targetPos;
		targetPosition.x = startPosition.x;

		destroyPosition = destroyPos;
		destroyPosition.x = startPosition.x;


//		Debug.Log (" end Position : " + targetPosition);
		
		//add half width of this object to endPosition
		SpriteRenderer thisRenderer = ( SpriteRenderer )gameObject.GetComponent( typeof( SpriteRenderer ));
		
		float halfHeight = thisRenderer.bounds.size.y;

		journeyLength = Vector3.Distance(startPosition, targetPosition);
//		Debug.Log (" journey length : " + journeyLength);
//		Debug.Log (" lerp speed : " + lerpSpeed);


		startBeat = endBeat - journeyLength/lerpSpeed;
	}

	void Lerp(){
		float distCovered = (metronome.currentPartialBeats - startBeat) * lerpSpeed;
//		Debug.Log ("current partial : " + metronome.currentPartialBeats);
//		Debug.Log (" start beat : " + startBeat);
//		Debug.Log (" change in beats : " + (metronome.currentPartialBeats - startBeat));
//		Debug.Log (" dist covered : " + distCovered);
		float fracJourney = distCovered/journeyLength;
//		Debug.Log ("fraction : " + fracJourney);

		if(fracJourney > 1) { //if lerp is complete

			//if already arrived at target && new target reached ( destruction position )
			if( targetReached )
			{
				showFeedback.DestroySelf();
			}
			else
			{
				targetReached = true;

				//travel to destruction position
				targetPosition = destroyPosition;
				journeyLength = Vector3.Distance(startPosition, targetPosition);

			}

	
		}
		else
		{
			transform.position = Vector3.Lerp(startPosition, targetPosition, fracJourney);
		}
	}

	public void SetPathToDestroySelf()
	{
		continueToDestructionPosition = true;
		targetReached = true;
		
		//travel to destruction position
		targetPosition = destroyPosition;
		journeyLength = Vector3.Distance(startPosition, targetPosition);

	}

	public void SnapToBar()
	{
		transform.position = targetPosition;
	}

}
