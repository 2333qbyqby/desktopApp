using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

namespace OpenAI
{
    public class ChatGPT : MonoBehaviour
    {
        [SerializeField] private TMP_InputField inputField;
        [SerializeField] private Button button;
        [Header("刷新按钮")]
        [SerializeField] private Button refreshButton;
        [SerializeField] private ScrollRect scroll;
        
        [SerializeField] private RectTransform sent;
        [SerializeField] private RectTransform received;

        private float height;
        private OpenAIApiProxy openai;

        private List<ChatMessage> messages = new List<ChatMessage>();
        private string prompt;
        private string model;
        private void Start()
        {
            InitChat();
            button.onClick.AddListener(SendReply);
            button.onClick.AddListener(InitChat);
        }
        private void AppendMessage(ChatMessage message)
        {
            scroll.content.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 0);

            var item = Instantiate(message.Role == "user" ? sent : received, scroll.content);
            item.GetComponent<TextMeshProUGUI>().text = message.Content;
            Debug.Log(message.Content);
            item.anchoredPosition = new Vector2(0, -height);
            LayoutRebuilder.ForceRebuildLayoutImmediate(item);
            height += item.sizeDelta.y;
            scroll.content.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
            scroll.verticalNormalizedPosition = 0;
        }

        private async void SendReply()
        {
            var newMessage = new ChatMessage()
            {
                Role = "user",
                Content = inputField.text
            };
            
            AppendMessage(newMessage);

            if (messages.Count == 0) newMessage.Content = prompt + "\n" + inputField.text; 
            
            messages.Add(newMessage);
            
            button.enabled = false;
            inputField.text = "";
            inputField.enabled = false;
            
            // Complete the instruction
            var completionResponse = await openai.CreateChatCompletion(new CreateChatCompletionRequest()
            {
                Model = model,
                Messages = messages
            });

            if (completionResponse.Choices != null && completionResponse.Choices.Count > 0)
            {
                var message = completionResponse.Choices[0].Message;
                message.Content = message.Content.Trim();
                
                messages.Add(message);
                AppendMessage(message);
            }
            else
            {
                Debug.LogWarning("No text was generated from this prompt.");
            }

            button.enabled = true;
            inputField.enabled = true;
        }

        //从json中读取数据，apikey,prompt,base_path
        public void InitChat()
        {
            string readData = SimpleJsonReadJson.ReadData("ChatGPTSetting.json");
            ChatgptSetting chatgptSetting = JsonUtility.FromJson<ChatgptSetting>(readData);
            openai = new OpenAIApiProxy(chatgptSetting.apiKey);
            openai.BASE_PATH = chatgptSetting.base_path;
            model = chatgptSetting.model;
            prompt = chatgptSetting.prompt;
        }

        public void SimpleCreateJson()
        {
            ChatgptSetting chatgptSetting = new ChatgptSetting();
            chatgptSetting.apiKey = "sk-tzX0DvrSRcp0s30t21F95589F5224d578f3c013c794a8754";
            chatgptSetting.prompt = "你是一位教授的助手，负责教授的起居，对教授有求必应";
            chatgptSetting.base_path = "https://elderman.top/v1";
            chatgptSetting.model = "gpt-3.5-turbo";
            string writeData = JsonUtility.ToJson(chatgptSetting);
            SimpleJsonReadJson.WriteData("ChatGPTSetting.json", writeData);
        }
    }
}
