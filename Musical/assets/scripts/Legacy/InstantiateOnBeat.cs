using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InstantiateOnBeat : MonoBehaviour {

	public Metronome metronome;

	float instantiationTimeBuffer = 5;

	float arrivalBeat;

	float instantiationFrequency = 4;

	GameObject cubeA;
	GameObject cubeX;
	GameObject cubeXHeld;
	GameObject objectToInstantiate;

	Vector3 startPosition = new Vector3( 10, 0 , 0 );

	public GameObject target;

	float widthOfTarget;
	public List<Vector2> arrivalBeats;
	int currentTurn = 0;
	

	// Use this for initialization
	void Start () {
		MeshRenderer targetRenderer = ( MeshRenderer )target.GetComponent( typeof( MeshRenderer ));
		widthOfTarget = targetRenderer.bounds.extents.x;

		cubeA = (GameObject)Resources.Load("CubeA");
		cubeX = (GameObject)Resources.Load("CubeX");
		cubeXHeld = (GameObject)Resources.Load("CubeXHeld");
		objectToInstantiate = UpcomingObject((int)arrivalBeats[ currentTurn ].y);
		arrivalBeat = arrivalBeats[ currentTurn ].x;

	}
	
	// Update is called once per frame
	void Update () {
		//if time to instantiate new object
		if( metronome.currentPartialBeats > (arrivalBeat - instantiationTimeBuffer) )
		{
			// instantiate object
			GameObject newObject = ( GameObject ) Instantiate( objectToInstantiate, startPosition, Quaternion.identity );
			MoveByBeats moveScript = ( MoveByBeats ) newObject.GetComponent(typeof( MoveByBeats ));
			moveScript.metronome = metronome;
			// calculate its start values
			moveScript.CalculateStartValues( startPosition, arrivalBeat, target, widthOfTarget, metronome.bpm );

			currentTurn ++;
			if( currentTurn < arrivalBeats.Count )
			{
				arrivalBeat = arrivalBeats[ currentTurn ].x;
				objectToInstantiate = UpcomingObject((int)arrivalBeats[ currentTurn ].y);
			}
			else
			{
				arrivalBeat = 10000;
			}

		}

	}

	GameObject UpcomingObject( int index )
	{
		if( index == 0 )
		{
			return cubeA;
		}
		else if ( index == 1 )
		{
			return cubeX;
		}

		return cubeA;
	}

}
