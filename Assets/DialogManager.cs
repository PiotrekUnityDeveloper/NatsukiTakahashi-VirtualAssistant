using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class DialogManager : MonoBehaviour
{
    // We keep up with all the dialogs, so they show up in order and not all at once
    public List<DialogDefinition> activeDialogQueue = new List<DialogDefinition>();
    public DialogDefinition activeDialog = null; // current dialog

    public TMP_Text dialogText; // Text displaying the dialog content
    public GameObject speechBubble; // Speech bubble object (namely, the dialogText's background)
    public bool speechBubbleVisible = false; // is SpeechBubble Visible?

    public string currentLang = "en";

    public static string[] knownLanguages =
    {
        "en", "jp"
    };

    private VoiceManager voiceManager;

    private void Awake()
    {
        voiceManager = this.gameObject.GetComponent<VoiceManager>();
    }

    void Start()
    {
        StartCoroutine(DelayFirstInteraction());
    }

    private IEnumerator DelayFirstInteraction()
    {
        yield return new WaitForSeconds(5f); //wait 5 seconds before talking
        //add as many as you want
        ///AddDialogToQueue(new DialogDefinition { message = "Hellluuuw!", showDelay = 1.4f, cooldown = 0.2f, voiceKey = "hello" });
        ///AddDialogToQueue(new DialogDefinition { message = "Howw are yuu doin?", showDelay = 2f, cooldown = 0.2f, voiceKey = "how are you doin" });
        ///AddDialogToQueue(new DialogDefinition { message = "I know im not yet ready to be able to help", showDelay = 3.8f, cooldown = 1.0f, voiceKey = "speech bubble" });
        ///AddDialogToQueue(new DialogDefinition { message = "But am working on it, and soon..", showDelay = 2.0f, cooldown = 1.8f, voiceKey = "working on it" });
        ///AddDialogToQueue(new DialogDefinition { message = "..i will be functional and helpful!", showDelay = 1.3f, cooldown = 1.2f, voiceKey = "it will animate" });
        ///AddDialogToQueue(new DialogDefinition { message = "i coont wait to seeit!! :3", showDelay = 1.7f, cooldown = 3.9f, voiceKey = "i cant wait" });
        
        // load the dialogs from the queue

        LoadDialogsToQueue(GetDialogQueueFromFile("firsttime.queue"));

        /*
        foreach(DialogDefinition def in activeDialogQueue)
        {
            SetDialogDelayToVoiceLength(def);
        }*/

        //start the queue!
        StartQueue();
    }

    // Make the 'typing' animation take the same amount of time as the voiceover length
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

    public void LoadDialogsToQueue(DialogDefinition[] dialogs)
    {
        activeDialogQueue.AddRange(dialogs);
    }

    public void RunDialog()
    {
        if(activeDialogQueue.Count > 0)
        {
            if (!speechBubbleVisible) // if speech bubble is not visible, show it!
            {
                speechBubble.GetComponent<Animator>().SetTrigger("show");
                speechBubbleVisible = true;
            }

            activeDialog = activeDialogQueue[0]; //set current dialog as the active one

            if (string.IsNullOrWhiteSpace(activeDialog.message))
            {
                SetPrimaryDialogMessageToTranslated(activeDialog, currentLang);
            }

            voiceManager.PlayClip(activeDialog.voiceKey); //play the dialog's voice-over
            StartCoroutine(TypeDialogMessage()); //"type" the dialog
        }
        else //if theres no more dialogs in queue, hide the speech bubble
        {
            speechBubble.GetComponent<Animator>().SetTrigger("hide");
            speechBubbleVisible = false;
        }
    }

    // NOT USED ANYMORE
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

        foreach(char c in activeDialog.message) // iterate thru all the characters in the message
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
                dialogText.text = dialogText.text + c; //we can "type" the character in, as the buffer is inactive!
            }
            else if(c == '<') // If there is a < character, then assume it's part of the formatting and pause the typing, until the formatting key is over
            {
                if(collectingToBuffer == false)
                {
                    collectingToBuffer = true; // we're now collecting new characters to a buffer, so the formatting (like <color="red">) is not shown
                }

                buffer += c;
            }
            else if (c == '>') // This is the end of the formatting! we can now paste anything we stored in our buffer directly to the message!
            {
                if (collectingToBuffer == true)
                {
                    buffer += c;
                    dialogText.text += buffer;
                    buffer = "";
                    collectingToBuffer = false; // We're now pasting the text from the buffer.
                }
            }
            else if (collectingToBuffer)
            {
                buffer += c; // buffer is active, store the character
            }

        }

        yield return new WaitForSeconds(activeDialog.cooldown); // wait for X seconds after finishing the typing. Specified in DialogDefinition

        dialogText.text = ""; //get ready for the next dialog in queue!
        MoveQueue();
    }

    public void StartQueue() // Run the currently active dialog, without moving the queue, like MoveQueue() does
        // Can be used to repeat the same dialog (sentence)
    {
        if(activeDialogQueue.Count > 0)
        {
            RunDialog();
        }
    }

    public void MoveQueue() // Run the next dialog in queue
    {
        if (activeDialogQueue != null && activeDialogQueue.Count > 0) activeDialogQueue.RemoveAt(0); //remove the previous dialog from the queue
        RunDialog(); // run the next one!
    }

    // NOT USED ANYMORE
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

    public DialogDefinition[] GetDialogQueueFromFile(string queueName)
    {
        string[] dialogs;

        if (queueName.EndsWith(".queue"))
        {
            dialogs = File.ReadAllLines(Environment.CurrentDirectory + "\\res\\queue\\" + queueName);
        }
        else
        {
            dialogs = File.ReadAllLines(Environment.CurrentDirectory + "\\res\\queue\\" + queueName + ".queue");
        }

        List<DialogDefinition> dialogsInQueue = new List<DialogDefinition>();

        foreach(string s in dialogs)
        {
            dialogsInQueue.Add(GetDialogFromFile(Environment.CurrentDirectory + "\\res\\dialog\\natsuki\\" + queueName.Replace(".queue", string.Empty) + "\\" + s));
        }

        return dialogsInQueue.ToArray();
    }

    public DialogDefinition GetDialogFromFile(string filePath)
    {
        DialogDefinition dialogDef0 = new DialogDefinition();

        if (string.Equals(Path.GetExtension(filePath), ".dialog", System.StringComparison.OrdinalIgnoreCase))
        {
            string[] dialogData = File.ReadAllLines(filePath);

            dialogDef0.translatedMessages = new List<TranslatedMessage>();

            foreach (string s in dialogData)
            {
                //handle translations
                foreach(string s1 in knownLanguages)
                {
                    if(s.StartsWith(s1 + "="))
                    {
                        dialogDef0.translatedMessages.Add(new TranslatedMessage { langPrefix = s1, translatedMessage = 
                            s.Replace(s1 + "=", string.Empty, StringComparison.OrdinalIgnoreCase)
                        });
                    }
                }

                if (s.StartsWith("voicekey=", System.StringComparison.OrdinalIgnoreCase))
                {
                    if (string.Equals(s, "voicekey=(auto)", System.StringComparison.OrdinalIgnoreCase) ||
                        string.Equals(s, "voicekey=auto", System.StringComparison.OrdinalIgnoreCase))
                    {
                        dialogDef0.voiceKey = GetVoiceKeyByDialogData(Path.GetFileName(filePath));
                    }
                    else
                    {
                        dialogDef0.voiceKey = s.Replace("voicekey=", string.Empty, StringComparison.OrdinalIgnoreCase);
                    }
                }

                if (s.StartsWith("chard=", System.StringComparison.OrdinalIgnoreCase))
                {
                    if (string.Equals("chard=(auto)", s, System.StringComparison.OrdinalIgnoreCase) ||
                        string.Equals("chard=auto", s, System.StringComparison.OrdinalIgnoreCase))
                    {
                        dialogDef0.showDelay = VoiceManager.GetClipFromFile(Environment.CurrentDirectory + "\\res\\voice\\natsuki\\" + dialogDef0.voiceKey).length;
                    }
                    else
                    {
                        dialogDef0.showDelay = float.Parse(s.Replace("chard=", string.Empty, StringComparison.OrdinalIgnoreCase));
                    }
                }

                if(s.StartsWith("cooldown=", StringComparison.OrdinalIgnoreCase))
                {
                    dialogDef0.cooldown = float.Parse(s.Replace("cooldown=", string.Empty, StringComparison.OrdinalIgnoreCase));
                }
            }

            return dialogDef0;
        }
        else
        {
            return null;
        }
    }

    public string GetVoiceKeyByDialogData(string dialogFileName)
    {
        if(dialogFileName.EndsWith(".dialog", System.StringComparison.OrdinalIgnoreCase))
        {
            return dialogFileName.Replace(".dialog", string.Empty) + ".wav";
        }
        else
        {
            return dialogFileName + ".wav";
        }
    }

    public void SetPrimaryDialogMessageToTranslated(DialogDefinition dialog0, string lang)
    {
        if(dialog0.translatedMessages != null &&
            dialog0.translatedMessages.Count > 0)
        {
            foreach(TranslatedMessage tmsg in dialog0.translatedMessages)
            {
                if(tmsg.langPrefix == lang)
                {
                    dialog0.message = tmsg.translatedMessage;
                }
            }
        }
    }

}

[System.Serializable]
public class DialogDefinition //Our dialog object
{
    public string message; // the thing that character is going to say
    //NOTE: message supports text formatting!!! :D
    // some basic formattings:
    // <b>text</b> - Bold Text
    // <i>text</i> - Italic Text
    // <color="green"> - Coloured Text
    // please refer to: https://docs.unity3d.com/Packages/com.unity.textmeshpro@4.0/manual/RichText.html for more information

    public List<TranslatedMessage> translatedMessages;

    public float showDelay; // the time (in seconds) in which this message will fully "type" itself
    public float cooldown; // how long the message should stay on screen after being fully "typed"? (in seconds)

    public bool customVoiceKey = false;
    public string voiceKey; // voice-over string indetifier to determine which voice-over should be used here

    // todo: text effects

    // todo: dialog type
}

public class TranslatedMessage
{
    public string langPrefix { get; set; }
    public string translatedMessage { get; set; }
}
