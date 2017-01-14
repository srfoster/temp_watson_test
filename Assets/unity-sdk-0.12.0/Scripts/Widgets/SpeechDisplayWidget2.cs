/**
* Copyright 2015 IBM Corp. All Rights Reserved.
*
* Licensed under the Apache License, Version 2.0 (the "License");
* you may not use this file except in compliance with the License.
* You may obtain a copy of the License at
*
*      http://www.apache.org/licenses/LICENSE-2.0
*
* Unless required by applicable law or agreed to in writing, software
* distributed under the License is distributed on an "AS IS" BASIS,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the License for the specific language governing permissions and
* limitations under the License.
*
*/


using IBM.Watson.DeveloperCloud.DataTypes;
using IBM.Watson.DeveloperCloud.Services.SpeechToText.v1;
using IBM.Watson.DeveloperCloud.Services.Conversation.v1;
using IBM.Watson.DeveloperCloud.Utilities;


using UnityEngine;
using UnityEngine.UI;

#pragma warning disable 414

namespace IBM.Watson.DeveloperCloud.Widgets
{
  /// <summary>
  /// Simple class for displaying the SpeechToText result data in the UI.
  /// </summary>
  public class SpeechDisplayWidget2 : Widget
  {
		public GameObject sphere;
		public GameObject cube;
		public Transform player_transform;

    #region Inputs
    [SerializeField]
    private Input m_SpeechInput = new Input("SpeechInput", typeof(SpeechToTextData), "OnSpeechInput");
    #endregion

    #region Widget interface
    /// <exclude />
    protected override string GetName()
    {
      return "SpeechDisplay";
    }
    #endregion

    #region Private Data

    [SerializeField]
    private bool m_ContinuousText = false;
    [SerializeField]
    private Text m_Output = null;
    [SerializeField]
    private InputField m_OutputAsInputField = null;
    [SerializeField]
    private Text m_OutputStatus = null;
    [SerializeField]
    private float m_MinConfidenceToShow = 0.5f;

    private string m_PreviousOutputTextWithStatus = "";
    private string m_PreviousOutputText = "";
    private float m_ThresholdTimeFromLastInput = 3.0f; //3 secs as threshold time. After 3 secs from last OnSpeechInput, we are considering input as new input
    private float m_TimeAtLastInterim = 0.0f;

    #endregion

		private Conversation m_Conversation = new Conversation();

		private string m_WorkspaceID;

		public void Start(){
			base.Start ();
			m_WorkspaceID = Config.Instance.GetVariableValue("ConversationV1_ID");
		}


    #region Event Handlers
    private void OnSpeechInput(Data data)
    {
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

						if (entity.value == "sphere") {
							Instantiate (sphere, player_transform.position, Quaternion.identity);
						}
						if (entity.value == "cube") {
							Instantiate (cube, player_transform.position, Quaternion.identity);
						}
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
    #endregion


	

  }
}
