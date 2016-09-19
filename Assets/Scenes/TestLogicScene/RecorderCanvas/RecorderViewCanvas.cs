using UnityEngine;
using UnityEngine.UI;

public class RecorderViewCanvas : MonoBehaviour, ReplayView {

	public Text text;
	public Text buttonText;

	ReplayController _replayController;

	public Slider slider;

	public void SetReplay(ReplayController replayController)
	{
		_replayController = replayController;
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
		if (_replayController == null)
			return;

		if (_replayController.IsRecording) {
			if (slider.isActiveAndEnabled)
				slider.gameObject.SetActive (false);
			return;
		}

		if (!slider.isActiveAndEnabled)
			slider.gameObject.SetActive (true);

		slider.minValue = 0;
		slider.maxValue = 1;
		slider.value = _replayController.NormalizedTime;
	}
}
