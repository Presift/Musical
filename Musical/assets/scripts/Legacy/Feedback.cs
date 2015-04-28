using UnityEngine;
using System.Collections;

public class Feedback : MonoBehaviour {

	public bool correctInput;
	public float timeForEarlyResponse = .1f;
	public float timeForLateResponse = .08f;
	public string xBoxInput = "A";
	public bool held;
	public Metronome metronome;
	public MoveByBeats moveScript;

	public bool startShowingFeedback;
	public float startInput;
	public float endInput;
	public float endHold;

	public Material correctMaterial;
	public Rigidbody rigidBody;

	float timeWaitingToDestroy = -1;
	float timeToDestroy = .25f;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {

		if( CheckInput()) //only check input if input is not yet correct and within input window
		{
			correctInput = IsInputCorrect();
		}

		if(startShowingFeedback)
		{
			ShowFeedback();
		}

		if(timeWaitingToDestroy >= 0 )
		{
			WaitThenDestroy();
		}

	}

	public void CheckHoldInput()
	{
		if( held && correctInput && metronome.currentPartialBeats < endHold )
		{
			//if button is still held down
			if( Input.GetKey(KeyCode.Space) || Input.GetButton( xBoxInput ))
			{
				//rescale from distance changed
			}
			else
			{
				//mute reward track
				//stop lerping to target
				moveScript.journeyComplete = true;
				correctInput = false;
				GetComponent<Rigidbody>().useGravity = true;
			}
			

		}
	}

	public bool IsInputCorrect()
	{
		if( Input.GetKeyDown(KeyCode.Space) || Input.GetButtonDown( xBoxInput ))
		{
			//unmute rewardtrack
			return true;
		}			
		return false;
	}

	bool CheckInput()
	{
		float beats = metronome.currentPartialBeats;

		if( beats >= startInput && beats <= endInput &&  !correctInput )
		{
			return true;
		}

		return false;
	}

	public void ShowFeedback()
	{
		if(correctInput)
		{
			GetComponent<Renderer>().material = correctMaterial;
			startShowingFeedback = false;
			if ( !held )
			{
				timeWaitingToDestroy = 0;
			}
			else if( moveScript.journeyComplete )
			{
				Debug.Log (" held, should destroy");
				Destroy( this.gameObject);
			}

		}
		//if cut off for response and input is still incorrect
		else if( !correctInput && metronome.currentPartialBeats > endInput )
		{
//			moveScript.journeyComplete = false;
			startShowingFeedback = false;
			GetComponent<Rigidbody>().useGravity = true;
		}
	}

	public void Resize( Vector3 previousPosition ) // move back by HalfTraveledDistance
	{
		float changeInDistance = Vector3.Distance( previousPosition, transform.position );

		//shorten by change in distance
		transform.localScale = new Vector3 ( transform.localScale.x - ( changeInDistance * 2 ), transform.localScale.y, transform.localScale.z );

	}

	public float SetFeedbackStats( Metronome masterMetronome, float bpm, float arrivalBeat, float holdTime )
	{
		if( !held )
		{
			holdTime = 0;
		}
		metronome = masterMetronome;
		startInput = arrivalBeat - ( timeForEarlyResponse * (bpm / 60 ));
		endInput = arrivalBeat + ( timeForLateResponse * (bpm / 60 ));
		endHold = arrivalBeat + holdTime;								//may need to be adjusted for early response time

		return endInput;
	}

	public void ChangeColor()
	{
		GetComponent<Renderer>().material.color = Color.red;
	}

	void OnBecameInvisible(){
		//finally, destroy, object
		Destroy(this.gameObject);
	}

	void WaitThenDestroy(){
		if( timeWaitingToDestroy < timeToDestroy ){
			//play explosion sound
			timeWaitingToDestroy += Time.deltaTime;
			//		Debug.Log(timeWaiting);
		}else{
			//finally, destroy, object
			Destroy(this.gameObject);
		}
	}
}
