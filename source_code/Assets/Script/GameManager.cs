using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Windows.Speech;

public class GameManager : MonoBehaviour
{
    // Input field text
    public TMP_InputField Chatbox_Input;
    public GameObject chatPanel;

    // Chatbox field text
    public GameObject textObject;

    public Color playerMessage, info;

    public AI_algorithm AI_algorithm;
    public STT_Manager speechToTextManager;

    // Start is called before the first frame update
    void Start()
    {
    }


    // Update Input field
    void Update()
    {
        if (Chatbox_Input.text != "")
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                sendTextToAI();
            }
        }
        else
        {
            if (!Chatbox_Input.isFocused && Input.GetKeyDown(KeyCode.Return))
            {
                Chatbox_Input.ActivateInputField();
            }
        }
        if (!Chatbox_Input.isFocused)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                SendMessageToChat("You have Pressed Space", Message.MessageType.info);
                Debug.Log("Space");
            }
        }
    }

    // Send text to AI Input
    public void sendTextToAI()
    {
            SendMessageToChat("User: " + Chatbox_Input.text, Message.MessageType.playerMessage);
            Debug.Log("User: " + Chatbox_Input.text);

            StartCoroutine(AI_algorithm.AI_responseCoroutine(Chatbox_Input.text, (response) =>
            {
                SendMessageToChat(response, Message.MessageType.info);
                Debug.Log("AI: " + response);
            }));

            Chatbox_Input.text = "";
    }

    public void SendMessageToChat(string text, Message.MessageType messageType)
    {
        Debug.Log("SendMessageToChat Start");
        Message newMessage = new Message();
        newMessage.text = text;

        GameObject newText = Instantiate(textObject, chatPanel.transform);
        newMessage.textObject = newText.GetComponent<TMP_Text>();
        newMessage.textObject.text = newMessage.text;
        newMessage.textObject.color = MessageTypeColor(messageType);

        newText.SetActive(true);

        Debug.Log("SendMessageToChat End");
    }


    Color MessageTypeColor(Message.MessageType messageType)
    {
        Color color = info;

        switch (messageType)
        {
            case Message.MessageType.playerMessage:
                color = playerMessage;
                break;
        }

        return color;
    }
}


[System.Serializable]
public class Message
{
    public string text;
    public TMP_Text textObject;
    public MessageType messageType;

    public enum MessageType
    {
        playerMessage,
        info
    }
}
