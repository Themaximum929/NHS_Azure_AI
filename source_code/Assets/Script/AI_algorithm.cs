using Azure;
using Azure.AI.OpenAI;
using Azure.Core;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using System;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using System.Collections.Generic;
using UnityEngine.UIElements;
using JetBrains.Annotations;

public class Patient
{
    public string Disease { get; set; }
    public string Name { get; set; }
    public int Age { get; set; }
    public string Sex { get; set; }
    public string MedicalHistory { get; set; }
    public string Allergy { get; set; }
    public string FTOCC { get; set; }
    public string Symptoms { get; set; }
    public string Complains { get; set; }
}

public class AI_algorithm : MonoBehaviour
{
    private Coroutine currentCoroutine;
    private OpenAIClient client;

    List<string> question = new List<string>();
    List<string> answer = new List<string>();

    private void Start()
    {
        client = new OpenAIClient(
            //new Uri("https://fypopenaiservices.openai.azure.com/"),
            //new AzureKeyCredential("b5b116cbee5f40f29606a021a52299ae"));
            new Uri("https://OPENAI-NHS.openai.azure.com/"),
            new AzureKeyCredential("0c33aca6ff6d49e3b02d452556f028e5"));
    }

    public IEnumerator AI_responseCoroutine(string input, Action<string> callback)
    {


        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
        }

        Task<string> task = AI_response(input);
        yield return new UnityEngine.WaitUntil(() => task.IsCompleted);

        if (task.Exception != null)
        {
            // Handle the exception if the task failed
            Debug.LogError("Task failed with exception: " + task.Exception);
        }
        else
        {
            // If the task completed successfully, invoke the callback with the result
            callback(task.Result);
        }
        currentCoroutine = null;
    }
    public async Task<string> AI_response(string input)
    {
        // Patient information
        Patient currentPatient = new Patient()
        {
            Disease = "瘧疾",
            Name = "Alex Tinubu",
            Age = 32,
            Sex = "M",
            MedicalHistory = "身體健康，無煙無酒",
            Allergy = "無",
            FTOCC = "16天前去過非洲",
            Symptoms = "有咳有痰，反胃，想嘔，頭痛，發冷，肌肉痛，骨痛",
            Complains = "反反覆覆每兩日就發一次燒，發咗6日 (去醫院嘅主因)。比蚊叮個位有啲痕。兩個鐘前有食必理痛。今日有做3合1COVID測試呈陰性。"
        };
        /*
        Patient currentPatient = new Patient()
        {
            Disease = "ST-elevation myocardial infarction",
            Name = "Chan Chun Ting",
            Age = 68,
            Sex = "M",
            MedicalHistory = "Hypertension, Diabetes, Hyperlipidemia",
            Allergy = "Augmentin",
            FTOCC = "All negative",
            Symptoms = "Mild shortness of breath.  Rapid weak pulse.  Headache, sweating, dizziness, sleepy.  Chest pain, angina pain, back pain, neck pain.",
            Complains = "Playing mahjong with friends.  Sudden severe, sharp pain like tearing feeling.  Mild shortness of breath developed."
        };*/

        try
        {
            var chatCompletionsOptions = new ChatCompletionsOptions()
            {
                // DeploymentName = "FYP", // Use DeploymentName for "model" with non-Azure clients
                DeploymentName = "gpt-4",
                Messages = {
                    //new ChatRequestSystemMessage($"你係 {currentPatient.Name}，患上{currentPatient.Disease}而你不知道。你扮演一位係醫院進入分診室嘅病人以便護士了解你的病況。護士會問你問題, 包括個人私隱問題, 請以香港格式回答例如身份證號碼係 R1234566 (請隨機作一個相同格式的號碼)。以下對話請用繁體中文回答，講嘢講得口語化的，每次淨係講一個症狀，最多一句，例如\" 我反反覆覆每兩日就發一次燒，而家發咗6日。\"。同一個對話就唔好再講有咳嗽，痰。下次再問先再講出嚟。{currentPatient.Name} 你有以下嘅基本資料:" +
                    //$"Name: {currentPatient.Name}, Age/Sex: {currentPatient.Age}/{currentPatient.Sex}, Medical History: {currentPatient.MedicalHistory}, Allergy: {currentPatient.Allergy}, FTOCC: {currentPatient.FTOCC}, Symptoms: {currentPatient.Symptoms}, Complains: {currentPatient.Complains}"),
                    new ChatRequestSystemMessage($"你係 {currentPatient.Name}，患上{currentPatient.Disease}而你不知道。你扮演一位係醫院進入分診室嘅病人以便護士了解你的病況。護士會問你問題, 包括個人私隱問題, 請以香港格式回答例如身份證號碼係 R1234566 (請隨機作一個相同格式的號碼)。以下對話請用繁體中文回答，講嘢講得口語化的，每次淨係講一個症狀，最多一句，例如\" 我反反覆覆每兩日就發一次燒，而家發咗6日。\"。根據病情做一個劇本式嘅回覆。而且講咗個主因先再講其他症狀。{currentPatient.Name} 你有以下嘅基本資料:" +
                    $"Name: {currentPatient.Name}, Age/Sex: {currentPatient.Age}/{currentPatient.Sex}, Medical History: {currentPatient.MedicalHistory}, Allergy: {currentPatient.Allergy}, FTOCC: {currentPatient.FTOCC}, Symptoms: {currentPatient.Symptoms}, Complains: {currentPatient.Complains}。"),

                    new ChatRequestUserMessage("今日點解嚟睇醫生呀?"),
                    new ChatRequestAssistantMessage("我發燒咗好多日啦，食埋必理痛到冇乜效。"),
                    new ChatRequestUserMessage("有冇食必理痛或者其他藥呀?"),
                    new ChatRequestAssistantMessage("有啊，食咗必理痛係可以舒緩到個症狀嘅，但係過多幾個鐘個體溫又會升返上去。"),
                    new ChatRequestUserMessage("你返類型嘅工作架，會唔會接觸好多人呀?"),
                    new ChatRequestAssistantMessage("無返工呀。而家仲搵緊工探索緊世界。"),
                    new ChatRequestUserMessage("你隻手咩事呀，點解呢個位有紅腫嘅？"),
                    new ChatRequestAssistantMessage("哦。呢個位係幾個禮拜前嘅非洲旅行俾蚊叮既。初時有做防護措施架，之後都放棄咗。"),
                    new ChatRequestUserMessage("你仲有冇其他唔舒服嘅位呀?"),
                    new ChatRequestAssistantMessage("有啊。有啲咳同痰。有陣時仲有啲想嘔，頭痛同發冷。"),
                },
                Temperature = (float)0.5,
                MaxTokens = 800,
                NucleusSamplingFactor = (float)0.95,
                FrequencyPenalty = 0,
                PresencePenalty = 0,
            };

            // Add the chat log to the chatCompletionsOptions
            for (int i = 0; i < question.Count; i++)
            {
                chatCompletionsOptions.Messages.Add(new ChatRequestUserMessage(question[i]));
                if (i < answer.Count)
                {
                    chatCompletionsOptions.Messages.Add(new ChatRequestAssistantMessage(answer[i]));
                }
            }

            chatCompletionsOptions.Messages.Add(new ChatRequestUserMessage(input));
            question.Add(input);

            Response<ChatCompletions> response = await client.GetChatCompletionsAsync(chatCompletionsOptions);

            if (response?.Value?.Choices != null && response.Value.Choices.Count > 0)
            {
                // Formatting the response
                ChatResponseMessage responseMessage = response.Value.Choices[0].Message;
                string temp = $"[{responseMessage.Role.ToString().ToUpperInvariant()}]: {responseMessage.Content}";
                answer.Add(temp);
                string[] temp2 = temp.Split("[ASSISTANT]: ");
                string temp3 = temp2[temp2.Length - 1];
                return temp3;
            }
            else
            {   
                // Handle the case where response is null or Choices are empty
                answer.Add("");
                ChatResponseMessage responseMessage = response.Value.Choices[0].Message;    
                string temp = $"[{responseMessage.Role.ToString().ToUpperInvariant()}]: {responseMessage.Content}";
                string[] temp2 = temp.Split("[ASSISTANT]: ");
                string temp3 = temp2[temp2.Length - 1];
                return temp3;
            }
        }
        catch (Exception ex)
        {
            // Handle any other exceptions that may occur
            Debug.LogError($"An exception occurred: {ex.Message}");
            return $"Error: An exception occurred: {ex.Message}";
        }
    }
}

