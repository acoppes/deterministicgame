using UnityEngine;
using Gemserk.Lockstep.Replays;
using System;
using UnityEngine.UI;

[Serializable]
public class MyGameReplayPlayer : GameReplayPlayer
{
	public float totalTime;
	public float maxAllowedUpdate;

	#region GameReplayPlayer implementation

	public void Reset ()
	{

	}

	public float GetMaxAllowedUpdateTime ()
	{
		return maxAllowedUpdate;
	}

	public float GetTotalTime ()
	{
		return totalTime;
	}

	public void Update (float dt)
	{

	}

	#endregion
}

public class ReplayControlsUI : MonoBehaviour {

	public MyGameReplayPlayer myReplayPlayerMock;

	ReplayPlayerControls replayControls;

	public Text playerStateText;

	public Button playButton;
	public Button pauseButton;

	public Slider seekSlider;

	// Use this for initialization
	void Start () {
		replayControls = new ReplayPlayerControlsImplementation (myReplayPlayerMock);
	
		seekSlider.minValue = 0;
		seekSlider.maxValue = myReplayPlayerMock.totalTime;

		playButton.onClick.AddListener (delegate {
			replayControls.Play();
		});

		pauseButton.onClick.AddListener (delegate {
			replayControls.Pause();	
		});

		seekSlider.onValueChanged.AddListener (delegate(float t) {
			replayControls.Seek(t);
		});
	}
	
	// Update is called once per frame
	void Update () {
		replayControls.Update (Time.deltaTime);	
	}

	void LateUpdate()
	{
		float playbackTime = replayControls.GetPlaybackTime ();		

		string state = "";

		switch (replayControls.State) {
		case ReplayPlayerControlsState.Paused:
			state = "PAUSED";
			break;
		case ReplayPlayerControlsState.Playing:
			state = "PLAYING";
			break;
		case ReplayPlayerControlsState.Seeking:
			state = "SEEKING";
			break;
		}

		playerStateText.text = string.Format("{0}: {1}s", state, playbackTime);

		playButton.interactable = replayControls.State != ReplayPlayerControlsState.Playing;
		pauseButton.interactable = replayControls.State != ReplayPlayerControlsState.Paused;
	}
		
}
