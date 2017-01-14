using System.Collections;
using System.Collections.Generic;
using IBM.Watson.DeveloperCloud.Services.Conversation.v1;
using IBM.Watson.DeveloperCloud.DataTypes;
using IBM.Watson.DeveloperCloud.Services.SpeechToText.v1;
using IBM.Watson.DeveloperCloud.Services.TextToSpeech.v1;
using IBM.Watson.DeveloperCloud.Utilities;
using IBM.Watson.DeveloperCloud.Logging;
using UnityEngine;
using UnityEngine.UI;

namespace IBM.Watson.DeveloperCloud.Widgets
{
  public class VoiceSpawner : Widget {

 

    #region Inputs
    [SerializeField]

    private Input m_SpeechInput = new Input("SpeechInput", typeof(SpeechToTextData), "OnSpeechInput");
    //private Input m_TextInput = new Input("Text", typeof(TextToSpeechData), "OnTextInput");
    #endregion

    private Conversation m_Conversation = new Conversation();
    private string m_WorkspaceID;

    public void Start(){
      Config.Instance.GetVariableValue("ConversationV1_ID");
    }

    public GameObject gameManager;

    #region Event Handlers

    private void OnSpeechInput(Data data)
    {
      UnityEngine.Debug.Log("OnSpeechInput!!!");
      SpeechRecognitionEvent result = ((SpeechToTextData)data).Results;
      if (result != null && result.results.Length > 0)
      {
        foreach (var res in result.results)
        {
          foreach (var alt in res.alternatives)
          {
            if (res.final && alt.confidence > 0)
            {
              string text = alt.transcript;
              UnityEngine.Debug.Log("Result: " + text + " Confidence: " + alt.confidence);
              m_Conversation.Message(OnMessage, m_WorkspaceID, text);
            }
          }
        }
      }
    }
    #endregion


    void OnMessage(MessageResponse resp, string customData)
      {
        UnityEngine.Debug.Log("OnMessage");
          if (resp != null && (resp.intents.Length > 0 || resp.entities.Length > 0))
          {
              string intent = resp.intents[0].intent;
              UnityEngine.Debug.Log("Intent: " + intent);
              Material currentMat = null;
              if (intent == "create")
              {
                  bool createdObject = false;
                  foreach (EntityResponse entity in resp.entities)
                  {
                      UnityEngine.Debug.Log("entityType: " + entity.entity + " , value: " + entity.value);
                    /*
                      if (entity.entity == "material")
                      {
                          currentMat = gameManager.GetMaterial(entity.value);
                      } else if (entity.entity == "object")
                      {
                          gameManager.CreateObject(entity.value, currentMat);
                          createdObject = true;
                          currentMat = null;
                      } 
                    */
                  }

              }
          } else
          {
              UnityEngine.Debug.Log("Failed to invoke OnMessage();");
          }
      }


     protected override string GetName(){
       return "VoiceSpawner";
     }      
    
  }
}
