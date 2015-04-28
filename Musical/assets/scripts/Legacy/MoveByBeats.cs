using UnityEngine;
using System.Collections;

public class MoveByBeats : MonoBehaviour {
	
	public Feedback feedback;
	//measurement of beats to arrive in advance to adjust endBeat so that sound will play on beat 
	//Note this may need to be normalized later to adjust for different bpms in different levels
	//translate advance buffer in seconds to advance buffer in beats 
	//time * bps    , bps = (bpm/60)
	public float advanceBufferInSeconds;
	private float advanceBufferInBeats;

	public Metronome metronome;

	public bool journeyComplete = false;

	public float lerpSpeed;
	private Vector3 startPosition;
	private Vector3 endPosition;
	public float startBeat;
	private float endBeat;
	private float journeyLength;
	
	public Vector3 moveDirection;

	//temporary public values
	public float arrivalBeat;

	public int beatsHeld = 0;
	public bool held;
	private Vector3 startHoldPosition;
	public bool holdPositionReached;

	void Awake()
	{
		held = HeldBeat();

		if( held )
		{
			SizeByBeats();
		}
		else
		{
			beatsHeld = 0;
		}
	}
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		//if current total Beats are greater than or equal to startBeat
		if(metronome.currentPartialBeats >= startBeat){
			if(!journeyComplete){
				Lerp();
			}
		}
	}

	public bool HeldBeat()
	{
		if( beatsHeld >= 1 )
		{
			return true;
		}
		return false;
	}

	public void SizeByBeats()
	{
		float lengthOfNewCube = beatsHeld * lerpSpeed;

		MeshRenderer renderer = ( MeshRenderer )GetComponent( typeof( MeshRenderer ));
		float originalWidth = renderer.bounds.extents.x * 2;

		transform.localScale = new Vector3 ( lengthOfNewCube, transform.localScale.y, transform.localScale.z );

	}

	public void CalculateStartValues(Vector3 startPos, float arrivalBeat, GameObject target, float widthOfTarget, float bpm ){


		feedback.SetFeedbackStats( metronome, bpm, arrivalBeat, beatsHeld );
		advanceBufferInBeats = advanceBufferInSeconds * (bpm/60);
		endBeat = arrivalBeat + ( beatsHeld /2 );

		startPosition = startPos;
		endPosition = target.transform.position + new Vector3(widthOfTarget, 0, 0);

		//add half width of this object to endPosition
		MeshRenderer thisRenderer = ( MeshRenderer )gameObject.GetComponent( typeof( MeshRenderer ));

		float halfWidthOfThis = thisRenderer.bounds.extents.x;
		endPosition += new Vector3(halfWidthOfThis, 0, 0);

		if(held)
		{
			startHoldPosition = endPosition;
			endPosition -= new Vector3( halfWidthOfThis, 0, 0 );
		}


		endPosition.y = startPosition.y;
		
		journeyLength = Vector3.Distance(startPosition, endPosition);
		startBeat = endBeat - journeyLength/lerpSpeed;
	}


	void Lerp(){
		float distCovered = (metronome.currentPartialBeats - startBeat) * lerpSpeed;
		float fracJourney = distCovered/journeyLength;
		if(fracJourney > 1) { //if lerp is complete
			//change colors
			journeyComplete = true;
//			if( held )
//			{
//				feedback.held = false;
//			}
			feedback.startShowingFeedback = true;

			feedback.ShowFeedback();
		}
		else
		{
			Vector3 currentPosition = transform.position;
			transform.position = Vector3.Lerp(startPosition, endPosition, fracJourney);

			HoldPositionReached();

			if( holdPositionReached )
			{
				feedback.CheckHoldInput();
				feedback.Resize( currentPosition );
//				changeInDistanceSinceHold += changeInDistance;
			}
		}
	}

	void HoldPositionReached()
	{
		if( held && !holdPositionReached )
		{
			if( transform.position.x <= startHoldPosition.x )
			{
//				Debug.Log (" hold position reached at : " + metronome.currentPartialBeats + " beats");
				feedback.startShowingFeedback = true;
				holdPositionReached =  true;
			}
		}

	}
	

	void Move(){
		transform.position += moveDirection * lerpSpeed * Time.deltaTime;
	}

	void OnBecameInvisible(){
		//finally, destroy, object
		Destroy(this.gameObject);
	}
	
	

}
