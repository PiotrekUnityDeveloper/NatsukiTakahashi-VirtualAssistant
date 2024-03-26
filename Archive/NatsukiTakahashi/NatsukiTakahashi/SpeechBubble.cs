using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Media;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NatsukiTakahashi
{
    public partial class SpeechBubble : Form
    {
        public SpeechBubble()
        {
            InitializeComponent();
        }

        // temp. variable defining the dialog definition message
        public string targetMessage = "";
        // currently used dialog
        public DialogDefinition dialog;

        private void SpeechBubble_Load(object sender, EventArgs e)
        {
            this.TransparencyKey = Color.FromArgb(64, 64, 64);
            label1.Parent = pictureBox1;
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        public void InitializeBubbleSpeech(DialogDefinition dialog)
        {
            this.dialog = dialog;

            // move to the char.
            this.Location = new Point(Form1.instance.Location.X - (this.Width - 45), Form1.instance.Location.Y - 60);

            SpeakByDialogContent(dialog.message, true);
            TypeDialogue(this.dialog);
        }

        private WaveOut waveOut;
        
        // TODO: rewrite
        public async void SpeakByDialogContent(string name, bool searchByDialog)
        {
            if (searchByDialog)
            {
                //string dialog = this.dialog.message;

                //dialog = dialog.Replace(' ', '-');
                //Clipboard.SetText(Form1.resourcePath + "\\voice\\" + Form1.voiceSkin + @"\\" + this.dialog.fileName);
                //SoundPlayer soundPlayer = new SoundPlayer(Form1.resourcePath + "\\voice\\" + Form1.voiceSkin + "\\" + Form1.voiceLang + "\\" + this.dialog.voiceFileName);
                //soundPlayer.Play();
                var reader = new Mp3FileReader(Form1.resourcePath + "\\voice\\" + Form1.voiceSkin + "\\" + Form1.voiceLang + "\\" + this.dialog.voiceFileName);
                var waveOut = new WaveOut();
                waveOut.Init(reader);
                this.waveOut = waveOut;
                //waveOut.Volume = Form1.voiceVolume;
                waveOut.Play();
                await Task.Delay(reader.TotalTime.Milliseconds);
            }
        }

        public async void TypeDialogue(DialogDefinition dialogDef)
        {
            label1.Text = "";
            string cleanDialogSource = "";

            if (dialogDef.loadFromFile)
            {
                int charDelay = 50; //default value
                int dialogDisplay = dialogDef.cooldownDelay; //default value
                string dialogSource = File.ReadAllText(Form1.resourcePath + "\\dialogue\\" + Form1.dialogueSkin + "\\" + Form1.dialogueLang + "\\" + this.dialog.fileName);

                string pattern = @"\{([^{}]*)\}";

                // Replace text within curly braces with an empty string
                cleanDialogSource = Regex.Replace(dialogSource, pattern, "");

                Console.WriteLine(dialogSource);

                // TODO
                // i want all the text inside two { } brackets to be discarded including the brackets itself.
                // but whatever is inside the brackets should be treated as a variable deifinition.

                // for example, if there is a "charDelay:20" inside the brackets, you should set the charDelay to 20
                // the same goes for dialogDisplay

                
                var matches = Regex.Matches(dialogSource, @"\{([^{}]*)\}");

                foreach (Match match in matches)
                {
                    // Get the content within the curly braces
                    string content = match.Groups[1].Value;

                    // Split the content by ':' to get variable name and value
                    string[] parts = content.Split(':');

                    if (parts.Length == 2)
                    {
                        string variableName = parts[0].Trim();
                        string variableValue = parts[1].Trim();

                        // Check the variable name and update corresponding variable
                        switch (variableName)
                        {
                            case "charDelay":
                                if (int.TryParse(variableValue, out int delay))
                                {
                                    charDelay = delay;
                                }
                                break;
                            case "dialogDisplay":
                                if (int.TryParse(variableValue, out int display))
                                {
                                    dialogDisplay = display;
                                }
                                break;
                                // Add cases for other variables if needed
                        }
                    }
                }

                //afterDialog.Interval = dialogDisplay;

                // END
            }

            


            if (dialogDef.loadFromFile)
            {
                targetMessage = null;
                targetMessage = cleanDialogSource;
            }
            else
            {
                targetMessage = dialogDef.message;
            }

            iteration = 0;
            timer1.Interval = dialogDef.characterDelay;
            timer1.Start();
        }

        // used by timer1
        private int iteration = 0;

        // timer1 - Responsible for typewriting the character's message
        private void timer1_Tick(object sender, EventArgs e)
        {
            // typewriting the character's response
            if(iteration < targetMessage.Length)
            {
                label1.Text = label1.Text + targetMessage[iteration];
            }

            // if the message is complete, stop the timer, and proceed
            if(label1.Text == targetMessage)
            {
                WaitAfterLastDialog();
                timer1.Stop();
            }

            iteration++;
        }

        private async void WaitAfterLastDialog()
        {
            // After the dialog was typed and spoken by the character, leave the speech bubble on the screen
            // for an addictional X seconds.

            //afterDialog.Start();
            await Task.Delay(dialog.cooldownDelay);
            //Console.WriteLine("cooldown ended");
            DialogManager.ProceedWithDialog();
            this.SendToBack();
            await Task.Delay(50);
            this.Hide();
            await Task.Delay(80);
            waveOut.Dispose();
            this.Close();
        }

        // Ensures, the bubble will always be beside the character, in case the user drags it.
        public void MoveToCharacter()
        {
            this.Location = new Point(Form1.instance.Location.X - (this.Width - 45), Form1.instance.Location.Y - 60);
        }

        private int dialogCountdown = 10;

        private void afterDialog_Tick(object sender, EventArgs e)
        {
            dialogCountdown -= 1;

            if(dialogCountdown <= 0)
            {
                DialogManager.ProceedWithDialog();
            }
        }
    }

    public class DialogDefinition
    {
        // core
        public string message;
        public int characterDelay; // delay between the characters showing up
        public int cooldownDelay; // time to wait after the message was read

        // TODO: Advanced Properties

        /// Response origin
        ///   defines the origin of the response, and if it should be read from a file.
        ///   useful for implementing multiple languages for the interactions.

        public bool loadFromFile;
        public string fileName;
        public string voiceFileName;

        /// Response language
        ///   defines the language of the response, this can be changed by an external script.
        ///   useful, if there is a settings system in which a language can be selected.
        ///   note: the language is being initialized at dialog initialization. Please modify the
        ///   language before it is processed
           
        // public string language = "en";

        /// Dialog type
        ///   defines the type of the dialog. Examples:
        ///   Normal Dialog - normal dialog in which the character says something
        ///   Question Dialog - dialog with a selection which is shown after the character stop talking.
        ///   Ghost Dialog - dialog which is stopped in the middle of being typed/spoken by the character.

        public DialogType dialogType = DialogType.Normal;

        /// Bubble type
        ///   defines the type of the bubble, which basically means its appearence. Like in manga,
        ///   the bubble can be a dialog, a thought, a scream, anything basically
           
        public BubbleType bubbleType = BubbleType.dialog;

    }
}

public enum DialogType
{
    Normal,
    Question,
    Ghost,
}

public enum BubbleType
{
    dialog, thought, scream, flash, blackflash, blast, wavy, rect, spiky
}