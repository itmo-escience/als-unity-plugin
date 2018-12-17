using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class TestALSPlugin : MonoBehaviour {
    public string UserName;

    public string InitText = "Init";
    public string StartText = "Start";
    public string StopText = "Stop";

    public Text InitStartStopButtonLabel;
    public Button InitStartStopButton;
    public Button SingleSearchButton;
    public Button UpdateButton;

    public Text AudioManagerStateLabel;
    public Text SearchManagerStateLabel;
    public Text ResultTextLabel;
    public Text ErrorTextLabel;

    private bool IsInit;

    public void Start() {
        ALSPlugin.OnStateChangeEvent += OnStateChange;
        ALSPlugin.OnSearchStateChangeEvent += OnSearchStateChange;

        InitStartStopButtonLabel.text = InitText;
        AudioManagerStateLabel.text = "AudioManager: " + ALSPlugin.AudioManagerState.ToString();
        SearchManagerStateLabel.text = "SearchManager: " + ALSPlugin.SearchManagerState.ToString();
        ResultTextLabel.text = "";
        ErrorTextLabel.text = "";

        SingleSearchButton.interactable = false;
        UpdateButton.interactable = false;
    }

    void OnStateChange(ALSAudioManagerState state, string error) {
        switch (state) {
            case ALSAudioManagerState.Denied:
                ErrorTextLabel.text = "Permission Denied";
                InitStartStopButtonLabel.text = InitText;
                InitStartStopButton.interactable = true;
                SingleSearchButton.interactable = false;
                UpdateButton.interactable = false;
                break;
            case ALSAudioManagerState.Error:
                ErrorTextLabel.text = error;
                InitStartStopButtonLabel.text = InitText;
                InitStartStopButton.interactable = true;
                SingleSearchButton.interactable = false;
                UpdateButton.interactable = false;
                break;
            case ALSAudioManagerState.Stopped:
                if (!IsInit) {
                    IsInit = true;
                    InitStartStopButtonLabel.text = StartText;
                    InitStartStopButton.interactable = true;
                    SingleSearchButton.interactable = true;
                    UpdateButton.interactable = true;
                }
                break;
            default:
                if (!string.IsNullOrEmpty(error)) {
                    ErrorTextLabel.text = error;
                }
                break;
        }
        AudioManagerStateLabel.text = "AudioManager: " + state.ToString();
    }

    private void OnSearchStateChange(ALSSearchManagerState state, string error, int trackId, int offset) {
        switch (state) {
            case ALSSearchManagerState.Paused:
                InitStartStopButtonLabel.text = StartText;
                InitStartStopButton.interactable = true;
                SingleSearchButton.interactable = true;
                break;
            case ALSSearchManagerState.Searching:
                InitStartStopButtonLabel.text = StopText;
                InitStartStopButton.interactable = true;
                SingleSearchButton.interactable = false;
                break;
            case ALSSearchManagerState.SingleSearching:
                InitStartStopButton.interactable = false;
                SingleSearchButton.interactable = false;
                break;
            case ALSSearchManagerState.Stopped:
                InitStartStopButtonLabel.text = StartText;
                InitStartStopButton.interactable = true;
                SingleSearchButton.interactable = true;
                break;
            default:
                break;
        }
        SearchManagerStateLabel.text = "SearchManager: " + state.ToString();
        if (!string.IsNullOrEmpty(error)) {
            ErrorTextLabel.text = error;
        }
        if (trackId == -1) {
            ResultTextLabel.text = "Not found";
        } else {
            ResultTextLabel.text = "TrackID: " + trackId;
            if (offset > -1) {
                ResultTextLabel.text += " Offset: " + offset + " sec";
            }
        }
        if (ALSPlugin.LastTrackId > -1) {
            ResultTextLabel.text += "\nLast TrackID: " + ALSPlugin.LastTrackId;
        }
        if (ALSPlugin.LastTrackOffset > -1) {
            ResultTextLabel.text += " Last Offset: " + ALSPlugin.LastTrackOffset;
        }
    }

    public void InitStartStopAction() {
        if (!ALSPlugin.IsPluginInit) {
            ALSPlugin.InitSDK(UserName);
            InitStartStopButton.interactable = false;
            return;
        }
        if (ALSPlugin.SearchManagerState == ALSSearchManagerState.Searching) {
            ALSPlugin.StopSearch();
        } else {
            ALSPlugin.StartSearch();
        }
    }

    public void SingleSearchAction() {
        ALSPlugin.SingleSearch();
    }
    
    public void UpdateAction() {
        ALSPlugin.UpdateLocalBase();
    }

}
