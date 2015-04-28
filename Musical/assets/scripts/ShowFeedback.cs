using UnityEngine;
using System.Collections;

public class ShowFeedback : MonoBehaviour {

	public Sprite correct;
	public Sprite incorrect;
	

//	public Metronome metronome;
	public MoveToBeat moveScript;

	SpriteRenderer thisRenderer;

	bool destroySelf = false;
	float timeUntilDestruction = .25f;

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

	public void ShowSuccess( bool held )
	{
		moveScript.targetReached = true;
		moveScript.continueToDestructionPosition = false;

		thisRenderer.sprite = correct;

		if (!held) 
		{
			destroySelf = true;
		}

	}

	public void DestroySelf()
	{
//		Debug.Log ("Destroyed : " + this.name);
		Destroy(this.gameObject);
	}

}

