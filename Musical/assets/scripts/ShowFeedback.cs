using UnityEngine;
using System.Collections;

public class ShowFeedback : MonoBehaviour {

	public Sprite correct;
	public Sprite incorrect;

	public MoveToBeat moveScript;

	SpriteRenderer thisRenderer;

	bool destroySelf = false;
	float timeUntilDestruction = .05f;

	// Use this for initialization
	void Start () {

		thisRenderer = (SpriteRenderer)this.GetComponent (typeof(SpriteRenderer));
	}
	
	// Update is called once per frame
	void Update () {

		if(destroySelf )
		{
			if( timeUntilDestruction > 0 )
			{
				timeUntilDestruction -= Time.deltaTime;
			}
			else
			{
				DestroySelf();
			}
		}
	}

	public void ShowSuccess()
	{
		moveScript.targetReached = true;
		moveScript.continueToDestructionPosition = false;

		SetSprite (correct);
	
		TimeDestruction ();

//		Debug.Log ("call to destroy self");

	}

	public void TimeDestruction()
	{
		destroySelf = true;
	}

	public void SetSprite( Sprite newSprite )
	{
		thisRenderer.sprite = newSprite;
	}

	public void UnsuccessfulHoldContinuesToDestruction()
	{
		moveScript.SetPathToDestroySelf ();
	}

	public void StartHold()
	{
		moveScript.targetReached = true;
		moveScript.continueToDestructionPosition = false;

		moveScript.SnapToBar ();

	}

	public void DestroySelf()
	{
//		Debug.Log ("Destroyed : " + this.name);
		Destroy(this.gameObject);
	}

}

