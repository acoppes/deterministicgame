using UnityEngine;
using UnityEngine.UI;

public class RecorderView : MonoBehaviour {

	public Text text;
	public Text buttonText;

	public void StartRecording()
	{
		text.text = "Recording";
		buttonText.text = "Start playback";
	}

	public void StartPlayback()
	{
		text.text = "Playing";
		buttonText.text = "Start recording";
	}
}
