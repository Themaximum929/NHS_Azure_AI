# NHS_Azure_AI
sources code for azure ai chatbot services and a simple chatbox for testing function with contains 3 files
1. Raw_code
2. unity_test
3. source_code

## Raw_code
The Raw_code file contains the script that controls the API calling to Azure. **Newest updated bversion** 
### Explainations of the most important part
```
            var chatCompletionsOptions = new ChatCompletionsOptions()
            {
                // DeploymentName = "FYP", // Use DeploymentName for "model" with non-Azure clients
                DeploymentName = "gpt-4",
                Messages = {
                },
                // Fine tune the temperature
                Temperature = (float)0.1,
                MaxTokens = 2000,
                NucleusSamplingFactor = (float)0.95,
                FrequencyPenalty = 0,
                PresencePenalty = 0,
            };
```
This part is to initialize the parameters of the gpt model parameters
```
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
```
This part is for the presets of the gpt role
```
            for (int i = 0; i < question.Count; i++)
            {
                chatCompletionsOptions.Messages.Add(new ChatRequestUserMessage(question[i]));
                if (i < answer.Count)
                {
                    chatCompletionsOptions.Messages.Add(new ChatRequestAssistantMessage(answer[i]));
                }
            }
```
This part is to save the chat history record for the this api call

## unity_test
I provided a simple chatbot in under the file unity_test such that you can try to test the accuracy 
### Reminder
As this is unity_test project has been deprecated for chatbot testing during June and July, parameters may not be the most recent updates but the behaviour of the respons is similar enough comparing to the newest version

## Source code
I provided the source code of unity_test such that you can try to modify the parameters to see the change of behaviours
