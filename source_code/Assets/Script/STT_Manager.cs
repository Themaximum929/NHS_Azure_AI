using System.Collections.Generic;
using UnityEngine;
using Microsoft.CognitiveServices.Speech;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;
using System.Linq.Expressions;

public class STT_Manager : MonoBehaviour
{
    private const string SubscriptionKey = "cf9cd3b039f94297b70fd8d3e4352633";
    private const string Region = "eastasia";

    public Button startRecordButton;

    public GameManager gameManager;

    private object threadLocker = new object();
    private bool waitingForReco;
    private string message;

    private bool micPermissionGranted = false;
    private bool isTextUpdated = false;

    public async void ButtonClick()
    {
        var config = SpeechConfig.FromSubscription(SubscriptionKey, Region);
        config.SpeechRecognitionLanguage = "zh-HK";

        using (var recognizer = new SpeechRecognizer(config))
        {
            lock (threadLocker)
            {
                waitingForReco = true;
            }
            var result = await recognizer.RecognizeOnceAsync().ConfigureAwait(false);

            string newMessage = string.Empty;
            if (result.Reason == ResultReason.RecognizedSpeech)
            {
                newMessage = result.Text;
            }
            else if (result.Reason == ResultReason.NoMatch)
            {
                newMessage = "Nomatch: Speech could not be recognized.";
            }
            else if (result.Reason == ResultReason.Canceled)
            {
                var cancellation = CancellationDetails.FromResult(result);
                newMessage = $"CANCELED: Reason={cancellation.Reason} ErrorDetails={cancellation.ErrorDetails}";
            }
            lock (threadLocker)
            {
                message = newMessage;
                waitingForReco = false;
                isTextUpdated = true;
            }
        }
    }

    void Start()
    {
        if (gameManager.Chatbox_Input == null)
        {
            UnityEngine.Debug.LogError("outputText is null");
        }
        if (startRecordButton == null)
        {
            UnityEngine.Debug.LogError("startRecordButton is null");
        }
        else
        {
            micPermissionGranted = true;
            message = "Click the button to start recording";
            startRecordButton.onClick.AddListener(ButtonClick);
        }
    }

    void Update()
    {
        lock (threadLocker)
        {
            if (startRecordButton != null)
            {
                startRecordButton.interactable = !waitingForReco && micPermissionGranted;
            }
            if (gameManager.Chatbox_Input != null && isTextUpdated)
            {
                gameManager.Chatbox_Input.text = message;
                isTextUpdated = false;

            }
        }
    }

}
