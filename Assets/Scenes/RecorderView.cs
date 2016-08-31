using UnityEngine;
using UnityEngine.UI;

public class RecorderView : MonoBehaviour {

	public Text text;
	public Text buttonText;

	Replay _replay;

	public Slider slider;

	public void SetReplay(Replay replay)
	{
		_replay = replay;
	}

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

	void LateUpdate()
	{
		if (_replay == null)
			return;

		if (_replay.IsRecording) {
			if (slider.isActiveAndEnabled)
				slider.gameObject.SetActive (false);
			return;
		}

		if (!slider.isActiveAndEnabled)
			slider.gameObject.SetActive (true);

		slider.minValue = 0;
		slider.maxValue = 1;
		slider.value = _replay.NormalizedTime;
	}
}
