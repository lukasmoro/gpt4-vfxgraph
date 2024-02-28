using OpenAI_API;
using OpenAI_API.Chat;
using OpenAI_API.Models;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;

public class OpenAIController : MonoBehaviour
{
    public TMP_Text textField;
    public TMP_InputField inputField;
    public Button inputButton;

    private OpenAIAPI api;
    private List<ChatMessage> messages;

    void Start()
    {
        // use .env instead !!!
        api = new OpenAIAPI("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"); 
        StartConversation();
        inputButton.onClick.AddListener(() => GetResponse());
    }

    private void StartConversation()
    {
        messages = new List<ChatMessage> {
        new ChatMessage(ChatMessageRole.System, 
            "{\"interiorData\":[" +
            "{\"intensity\": 1," +
            "\"drag\": 2," +
            "\"color\": {\"r\": 1.0, \"g\": 0.5, \"b\": 0.0, \"a\": 1.0}}" +
            "]}" +
            "From now on, interpret every message to populate this JSON's properties with new values & respond with the new JSON always starting with {\"interiorData\":[" +
            "{\"intensity\": & ending with the color property.")
    };

        inputField.text = "";
        string startString = "Describe in metaphors how you want the particles to behave!";
        textField.text = startString;
        Debug.Log(startString);
    }

    private async void GetResponse()
    {
        if (inputField.text.Length < 1)
        {
            return;
        }

        inputButton.enabled = false;

        ChatMessage userMessage = new ChatMessage();
        userMessage.Role = ChatMessageRole.User;
        userMessage.Content = inputField.text;
        
        if (userMessage.Content.Length > 100)
        {
            userMessage.Content = userMessage.Content.Substring(0, 100);
        }

        Debug.Log(string.Format("{0}: {1}", userMessage.rawRole, userMessage.Content));

        messages.Add(userMessage);

        textField.text = string.Format("You: {0}", userMessage.Content);

        inputField.text = "";

        var chatResult = await api.Chat.CreateChatCompletionAsync(new ChatRequest()
        {
            Model = Model.ChatGPTTurbo,
            Temperature = 0.9,
            MaxTokens = 64,
            Messages = messages
        });

        // Get the response message
        ChatMessage responseMessage = new ChatMessage();
        responseMessage.Role = chatResult.Choices[0].Message.Role;
        responseMessage.Content = chatResult.Choices[0].Message.Content;
        Debug.Log(string.Format("{0}: {1}", responseMessage.rawRole, responseMessage.Content));

        // Add the response to the list of messages
        messages.Add(responseMessage);

        // Update the text field with the response
        textField.text = string.Format("You: {0}\n\nGPT: {1}", userMessage.Content, responseMessage.Content);

        // Re-enable the OK button
        inputButton.enabled = true;

        // Get the response message
        responseMessage.Role = chatResult.Choices[0].Message.Role;
        responseMessage.Content = chatResult.Choices[0].Message.Content;
        Debug.Log(string.Format("{0}: {1}", responseMessage.rawRole, responseMessage.Content));
        Debug.Log(responseMessage.Content);

        // Save the response as JSON
        SaveResponseAsJson(responseMessage.Content);
    }

    private void SaveResponseAsJson(string content)
    {
        // Define the file path where you want to save the JSON file
        string filePath = "Assets/Scripts/response.json";

        try
        {
            // Remove the escape characters (\"), if any
            string cleanedContent = content.Replace("\\\"", "\"");

            // Write the cleaned JSON content to the file
            File.WriteAllText(filePath, cleanedContent);

            // Log a message indicating that the response has been saved
            Debug.Log("Response saved as JSON at: " + filePath);
        }
        catch (Exception e)
        {
            // Handle any potential exceptions that may occur during file writing
            Debug.LogError("Error saving response as JSON: " + e.Message);
        }
    }



}