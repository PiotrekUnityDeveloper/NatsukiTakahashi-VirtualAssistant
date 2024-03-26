using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NatsukiTakahashi
{
    public partial class Form1 : Form
    {
        public static string resourcePath;
        public static Form1 instance;

        public static string dialogueSkin = "def";
        public static string voiceSkin = "def";
        public static string characterSkin = "def";
        public static string bubbleSkin = "def";

        public static string dialogueLang = "en";
        public static string voiceLang = "jp";

        public static float voiceVolume = 1.0f;

        public Form1()
        {
            InitializeComponent();
            resourcePath = Environment.CurrentDirectory + "";
            instance = this;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.TransparencyKey = Color.FromArgb(30, 30 ,30);
            InitializeFirstTimeConversation();
        }

        // Define event
        public event EventHandler FormMoved;

        // Method to raise the event when the form is moved
        public void OnFormMoved(EventArgs e)
        {
            FormMoved?.Invoke(this, e);
        }

        private async void InitializeFirstTimeConversation()
        {
            await Task.Delay(2000);

            DialogManager.AddDialogToQueue(new DialogDefinition
            {
                message = "first time init 1.txt",
                characterDelay = 200,
                cooldownDelay = 2000,
                dialogType = DialogType.Normal,
                bubbleType = BubbleType.dialog,
                voiceFileName = "first-time-init-1.mp3",
                loadFromFile = true,
                fileName = "first-time-init-1.txt",
            });

            DialogManager.AddDialogToQueue(new DialogDefinition
            {
                message = "first time init 1.txt",
                characterDelay = 200,
                cooldownDelay = 2000,
                dialogType = DialogType.Normal,
                bubbleType = BubbleType.dialog,
                voiceFileName = "first-time-init-1-2.mp3",
                loadFromFile = true,
                fileName = "first-time-init-1-2.txt",
            });

            DialogManager.AddDialogToQueue(new DialogDefinition
            {
                message = "first time init 1.txt",
                characterDelay = 75,
                cooldownDelay = 3000,
                dialogType = DialogType.Normal,
                bubbleType = BubbleType.dialog,
                voiceFileName = "first-time-init-1-3.mp3",
                loadFromFile = true,
                fileName = "first-time-init-1-3.txt",
            });

            DialogManager.AddDialogToQueue(new DialogDefinition
            {
                message = "first time init 1.txt",
                characterDelay = 75,
                cooldownDelay = 3000,
                dialogType = DialogType.Normal,
                bubbleType = BubbleType.dialog,
                voiceFileName = "first-time-init-1-4.mp3",
                loadFromFile = true,
                fileName = "first-time-init-1-4.txt",
            });

            DialogManager.AddDialogToQueue(new DialogDefinition
            {
                message = "first time init 1.txt",
                characterDelay = 75,
                cooldownDelay = 3000,
                dialogType = DialogType.Normal,
                bubbleType = BubbleType.dialog,
                voiceFileName = "first-time-init-1-5.mp3",
                loadFromFile = true,
                fileName = "first-time-init-1-5.txt",
            });

            DialogManager.AddDialogToQueue(new DialogDefinition
            {
                message = "first time init 1.txt",
                characterDelay = 75,
                cooldownDelay = 3000,
                dialogType = DialogType.Normal,
                bubbleType = BubbleType.dialog,
                voiceFileName = "first-time-init-1-6.mp3",
                loadFromFile = true,
                fileName = "first-time-init-1-6.txt",
            });

            DialogManager.ProceedWithDialog();
        }

        private static SpeechBubble activeDialog = null;

        public static void InitializeDialog(DialogDefinition dialogDef)
        {
            SpeechBubble speech = new SpeechBubble();
            activeDialog = speech;
            speech.Show();
            speech.InitializeBubbleSpeech(dialogDef);
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        [DllImportAttribute("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {

        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        private void pictureBox1_Move(object sender, EventArgs e)
        {

        }

        private void Form1_Move(object sender, EventArgs e)
        {
            if(activeDialog != null)
            {
                activeDialog.MoveToCharacter();
            }
        }
    }
}
