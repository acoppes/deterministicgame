using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class RecorderView : MonoBehaviour {

	public Text text;

	public void StartRecording()
	{
		text.text = "Recording";
	}

	public void StartPlayback()
	{
		text.text = "Play";
	}
}
