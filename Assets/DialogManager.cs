using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class DialogManager : MonoBehaviour
{
    // We keep up with all the dialogs, so they show up in order and not all at once
    public List<DialogDefinition> activeDialogQueue = new List<DialogDefinition>();
    public DialogDefinition activeDialog = null;

    public TMP_Text dialogText;

    private VoiceManager voiceManager;

    private void Awake()
    {
        voiceManager = this.gameObject.GetComponent<VoiceManager>();
    }

    void Start()
    {
        AddDialogToQueue(new DialogDefinition { message = "Hellluuuw!", showDelay = 1.4f, cooldown = 0.2f, voiceKey = "hello" });
        AddDialogToQueue(new DialogDefinition { message = "Howw are yuu doin?", showDelay = 2f, cooldown = 0.2f, voiceKey = "how are you doin" });
        AddDialogToQueue(new DialogDefinition { message = "I know my speking buble is not as cutt as myself", showDelay = 2.5f, cooldown = 0.6f, voiceKey = "speech bubble" });
        AddDialogToQueue(new DialogDefinition { message = "But am working on it, and soon..", showDelay = 1.5f, cooldown = 0.2f, voiceKey = "working on it" });
        AddDialogToQueue(new DialogDefinition { message = "..it will be animatedd annd stuffff!!!!!", showDelay = 1.5f, cooldown = 0.2f, voiceKey = "it will animate" });
        AddDialogToQueue(new DialogDefinition { message = "i coont wait to seeit!! :3", showDelay = 1.5f, cooldown = 2.5f, voiceKey = "i cant wait" });
        
        /*
        foreach(DialogDefinition def in activeDialogQueue)
        {
            SetDialogDelayToVoiceLength(def);
        }*/

        StartQueue();
    }

    private void SetDialogDelayToVoiceLength(DialogDefinition def)
    {
        def.showDelay = voiceManager.GetVoiceDefinitionFromList(def.voiceKey).voiceClip.length;
    }

    void Update()
    {
        
    }

    public void AddDialogToQueue(DialogDefinition def)
    {
        activeDialogQueue.Add(def);
    }

    public void RunDialog()
    {
        if(activeDialogQueue.Count > 0)
        {
            activeDialog = activeDialogQueue[0];
            voiceManager.PlayClip(activeDialog.voiceKey);
            StartCoroutine(TypeDialogMessage());
        }
    }

    private List<string> formattingdb = new List<string>()
    {
        "(C)<color= (0) </color>",
    };

    public IEnumerator TypeDialogMessage()
    {
        dialogText.text = "";
        float characterDelay = activeDialog.showDelay / activeDialog.message.Length;

        bool collectingToBuffer = false;
        string buffer = "";
        //string formatSuffix = "";
        //char lastChar = '%';

        foreach(char c in activeDialog.message)
        {
            if (collectingToBuffer)
            {
                yield return new WaitForSeconds(0);
            }
            else
            {
                yield return new WaitForSeconds(characterDelay);
            }

            if(c != '<' && c != '>' && collectingToBuffer == false)
            {
                dialogText.text = dialogText.text + c;
            }
            else if(c == '<')
            {
                if(collectingToBuffer == false)
                {
                    collectingToBuffer = true;
                }

                buffer += c;
            }
            else if (c == '>')
            {
                if (collectingToBuffer == true)
                {
                    buffer += c;
                    dialogText.text += buffer;
                    buffer = "";
                    collectingToBuffer = false;
                }
            }
            else if (collectingToBuffer)
            {
                buffer += c;
            }

        }

        yield return new WaitForSeconds(activeDialog.cooldown);

        dialogText.text = "";
        MoveQueue();
    }

    public void StartQueue()
    {
        if(activeDialogQueue.Count > 0)
        {
            RunDialog();
        }
    }

    public void MoveQueue()
    {
        if (activeDialogQueue != null && activeDialogQueue.Count > 0) activeDialogQueue.RemoveAt(0);
        RunDialog();
    }

    public string GetSuffixForFormattingOperation(string prefix)
    {
        foreach(string s in formattingdb)
        {
            if (s.StartsWith("(C)"))
            {
                //contains...
                if(prefix.Contains(s.Replace("(C)", string.Empty).Split(" (0) ")[0]))
                {
                    return s.Split(" (0) ")[1];
                }
            }
            else
            {
                //is equal to...

                if (prefix == s.Replace("(C)", string.Empty).Split(" (0) ")[0])
                {
                    return s.Split(" (0) ")[1];
                }
            }
        }

        return "</" + prefix.Replace('<', '\0');
    }

}

[System.Serializable]
public class DialogDefinition
{
    public string message; // the thing that character is going to say
    public float showDelay; // the time (in seconds) in which this message will fully "type" itself
    public float cooldown; // how long the message should stay on screen after being fully "typed"? (in seconds)

    public bool customVoiceKey = false;
    public string voiceKey; // voice-over string indetifier to determine which voice-over should be used here

    // todo: text effects

    // todo: dialog type
}
