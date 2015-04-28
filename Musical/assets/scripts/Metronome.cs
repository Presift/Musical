using UnityEngine;
using System.Collections;

public class Metronome : MonoBehaviour {

	private float threshold = 0.02f; // 20 ms
	public float currentPartialBeats = 0; //tally of total beats 
	public int wholeBeats = 0; //tally of whole beats
	public float sampleRate; //samples per second
	//private var previousSample : float =0;
	public float bpm; //beats per minute
	public float previousPartialBeats;
	public float deltaBeats = 0;
	private float bps; //beats per second
	
	public float delayTime; 
	
	public bool newWholeBeat;
	
	public int countsPerBeat;
	public bool newPartialBeat;
	
	
	//these variables are for debugging
	public float previousSmoothedBeat = 0;
	
	private float audioVolume;

	public float holdTime = 0;

	public bool fadeIn = true;
	float fadeInRate = 1.01f;

	// Use this for initialization
	void Start () {
		GetComponent<AudioSource>().Play();
		bps = bpm/60;
		GetComponent<AudioSource>().PlayDelayed(delayTime);
		audioVolume = GetComponent<AudioSource>().volume;
	}
	
	// Update is called once per frame
	void Update () {

		if( fadeIn )
		{
			FadeIn();
		}

		if (Input.GetKeyDown (KeyCode.Space)) 
		{
			Debug.Log (currentPartialBeats );	
		}

		currentPartialBeats = BeatForSample(GetComponent<AudioSource>().timeSamples);           // total elapsed fractional beats, e.g. 2408.9974
		deltaBeats = currentPartialBeats - previousPartialBeats;
		wholeBeats = SmoothedBeatForSample(GetComponent<AudioSource>().timeSamples); // adjusted elapsed beats (int( with 20ms buffer, e.g. 2409
		//	DebugBeatDiscrepancy(wholeBeats, previousSmoothedBeat );
		
		float wholeFloored = Mathf.Floor(currentPartialBeats);
		float currentPartialFloored = FloorToNearestPartial(wholeFloored, currentPartialBeats);
		float previousPartialFloored = FloorToNearestPartial(wholeFloored, previousPartialBeats);

		newPartialBeat = IsNewPartialBeat( previousPartialFloored, currentPartialFloored );
		newWholeBeat = IsNewWholeBeat();

		previousPartialBeats = currentPartialBeats;
		previousSmoothedBeat = wholeBeats;
		
		//	Debug.Log(wholeBeats);
		
	}

	bool IsNewWholeBeat()
	{
		if(wholeBeats > previousSmoothedBeat)

		{
			return true;
		}
		
		return false;
	}

	bool IsNewPartialBeat( float pastFloor,  float currentFloor)
	{
		//if past floored does not equal current floored
		if(pastFloor != currentFloor)
		{
			return true;
		}

		return false;
		
	}
	
	void FadeOut()
	{
		if(audioVolume > .1)
		{
			audioVolume -= .1f * Time.deltaTime;
			GetComponent<AudioSource>().volume = audioVolume;
		}
	}

	void FadeIn()
	{
		if(audioVolume < 1)
		{
			fadeInRate *= fadeInRate;
//			Debug.Log ("rate : " + fadeInRate );
			audioVolume += .1f * fadeInRate * Time.deltaTime;
			GetComponent<AudioSource>().volume = audioVolume;
//			Debug.Log (" volume : " + GetComponent<AudioSource>().volume );
		}
		else
		{
			fadeIn = false;
			fadeInRate = 1.1f;
		}
	}

	float FloorToNearestPartial( float whole, float partialBeats )
	{
		float flooredResult = 0;
		float distFromWhole = partialBeats - whole;
		float increment = 1/countsPerBeat;
		
		if(distFromWhole <= increment)
		{
			flooredResult = 0;
		}else{
			for(int i = countsPerBeat - 1; i > 0; i--)
			{
				if (distFromWhole >= increment * i )
				{
					flooredResult =   ( increment * i );
				}
			}
		}
		return flooredResult;
		
	}
	
	float BeatForSample( float sample ) {
		float beat = sample / sampleRate * bps;
		return beat;
	}
	
	int SmoothedBeatForSample( int sample ) {
		float beat = sample / sampleRate * bps;
		int smoothedBeat = (int)Mathf.Floor(beat + bps * threshold); // 0.05 beats equals roughly 20 ms; move this up maybe
		return smoothedBeat;
	}
}
