using Azure;
using Azure.AI.OpenAI;
using Azure.Core;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;

public class AI_algorithm : MonoBehaviour
{
    public CaseInfoDB caseInfoDB; // Reference to CaseInfoDB
    public SceneController sceneController; // Reference to SceneController to get selected case index
    private Patient currentPatient;

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

        // Other information for CMSManager
        public double Temp { get; set; }
        public string Temp_method { get; set; }
        public int Pulse { get; set; }
        public int BP1 { get; set; }
        public int BP2 { get; set; }
        public int SP02 { get; set; }
        public int RR { get; set; }
        public int GCS { get; set; }
        public double Hstix { get; set; }
    }

    private Coroutine currentCoroutine;
    private OpenAIClient client;

    List<string> question = new List<string>();
    List<string> answer = new List<string>();

    private void Start()
    {
        client = new OpenAIClient(
            new Uri("https://OPENAI-NHS.openai.azure.com/"),
            new AzureKeyCredential("0c33aca6ff6d49e3b02d452556f028e5")
        );
    }

    public void InitializeCurrentPatient()
    {
        if (sceneController != null && caseInfoDB != null)
        {
            int selectedCaseIndex = sceneController.selectedCaseIndex;
            if (caseInfoDB.caseAttributes.ContainsKey(selectedCaseIndex))
            {
                var selectedCase = caseInfoDB.caseAttributes[selectedCaseIndex];
                currentPatient = new Patient
                {
                    Disease = selectedCase.Disease,
                    Name = selectedCase.Name,
                    Age = int.Parse(selectedCase.Age),
                    Sex = selectedCase.Sex,
                    MedicalHistory = selectedCase.MedicalHistory,
                    Allergy = selectedCase.Allergy,
                    FTOCC = selectedCase.FTOCC,
                    Symptoms = selectedCase.Symptoms,
                    Complains = selectedCase.Complains,
                    Temp = double.Parse(selectedCase.Temp),
                    Pulse = int.Parse(selectedCase.HR),
                    BP1 = int.Parse(selectedCase.BP1),
                    BP2 = int.Parse(selectedCase.BP2),
                    SP02 = int.Parse(selectedCase.SP02),
                    RR = int.Parse(selectedCase.RR),
                    GCS = int.Parse(selectedCase.GCS),
                    Hstix = double.Parse(selectedCase.Hstix)
                };

                Debug.Log($"Loaded patient {currentPatient.Name} with disease {currentPatient.Disease}");
            }
            else
            {
                Debug.LogError("Selected case index not found in CaseInfoDB.");
            }
        }
        else
        {
            Debug.LogError("SceneController or CaseInfoDB is not assigned.");
        }
    }

    public IEnumerator AI_responseCoroutine(string input, Action<string> callback)
    {
        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
        }

        Task<string> task = AI_response(input);

        float timeout = 30f; // Wait for a maximum of 30 seconds
        float timeElapsed = 0f;

        while (!task.IsCompleted && timeElapsed < timeout)
        {
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        if (timeElapsed >= timeout)
        {
            // Handle timeout
            Debug.LogError("AI response timed out.");
            callback("Error: Request timed out.");
            yield break;
        }

        if (task.Exception != null)
        {
            // Handle the exception if the task failed
            Debug.LogError("Task failed with exception: " + task.Exception);
            callback("Error: " + task.Exception.Message);
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
        try
        {
            var chatCompletionsOptions = new ChatCompletionsOptions()
            {
                // DeploymentName = "FYP", // Use DeploymentName for "model" with non-Azure clients
                DeploymentName = "gpt-4",
                Messages = {
                    new ChatRequestSystemMessage($"你係 {currentPatient.Name}，患上{currentPatient.Disease}而你不知道。你扮演一位係醫院進入分診室嘅病人以便護士了解你的病況。護士會問你問題, 包括個人私隱問題, 請以香港格式回答例如身份證號碼係 R1234566 (請隨機作一個相同格式的號碼)。以下對話請用繁體中文回答，講嘢講得口語化的，每次最多淨係講一個症狀，只回答問題，不要問題以外的東西，最多一句，例如\" 我反反覆覆每兩日就發一次燒，而家發咗6日。\"。根據病情做一個劇本式嘅回覆。而且講咗個主因先再講其他症狀。{currentPatient.Name} 你有以下嘅基本資料:" +
                    $"Name: {currentPatient.Name}, Age/Sex: {currentPatient.Age}/{currentPatient.Sex}, Medical History: {currentPatient.MedicalHistory}, Allergy: {currentPatient.Allergy}, FTOCC: {currentPatient.FTOCC}, Symptoms: {currentPatient.Symptoms}, Complains: {currentPatient.Complains}。"),

                    new ChatRequestUserMessage("早晨"),
                    new ChatRequestAssistantMessage("早晨護士。"),
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
                // Fine tune the temperature
                Temperature = (float)0.1,
                MaxTokens = 2000,
                NucleusSamplingFactor = (float)0.95,
                FrequencyPenalty = 0,
                PresencePenalty = 0,
            };

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

            Debug.Log($"Completions Tokens: {response.Value.Usage.CompletionTokens}");
            Debug.Log($"Prompt Tokens: {response.Value.Usage.PromptTokens}");
            Debug.Log($"Total Tokens: {response.Value.Usage.TotalTokens}");

            if (response?.Value?.Choices != null && response.Value.Choices.Count > 0)
            {
                ChatResponseMessage responseMessage = response.Value.Choices[0].Message;
                string temp = $"[{responseMessage.Role.ToString().ToUpperInvariant()}]: {responseMessage.Content}";
                answer.Add(temp);
                string[] temp2 = temp.Split("[ASSISTANT]: ");
                string temp3 = temp2[temp2.Length - 1];
                return temp3;
            }
            else
            {
                answer.Add("");
                return "No response from AI.";
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"An exception occurred: {ex.Message}");
            return $"Error: An exception occurred: {ex.Message}";
        }
    }
}

