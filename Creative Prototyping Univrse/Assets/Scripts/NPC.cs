using HuggingFace.API;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NPC : MonoBehaviour
{
    private Conversation conversation = new Conversation();
    [SerializeField] private string inputText;
    public string result;

    void Start()
    {
        SendExampleQuery();
    }

    void Update()
    {
        
    }

    private void SendExampleQuery()
    {
        HuggingFaceAPI.Conversation("Hello!", response => {
            string reply = conversation.GetLatestResponse();
            result = reply;
        }, error => {
            Debug.Log(error);
        }, conversation);
    }
}
