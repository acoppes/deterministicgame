using UnityEngine;
using System.Collections;

public class FeedbackClick : MonoBehaviour {

	public Animator animator;

	public GameObject feedbackObject;

	void Awake()
	{
		feedbackObject.SetActive (false);
	}

	public void ShowFeedback(Vector2 position)
	{
		transform.position = position;
		feedbackObject.SetActive (true);
		animator.Play ("Feedback");
	}

	public void OnAnimationFinished()
	{
		feedbackObject.SetActive (false);
	}

}
