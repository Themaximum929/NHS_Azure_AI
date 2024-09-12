# NHS_Azure_AI
sources code for azure ai chatbot services and a simple chatbox for testing function

## Raw_code
The Raw_code file contains the script that controls the API calling to Azure. **Newest updated bversion** 
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
This part is to initialize the parameters of the gpt behaviour
