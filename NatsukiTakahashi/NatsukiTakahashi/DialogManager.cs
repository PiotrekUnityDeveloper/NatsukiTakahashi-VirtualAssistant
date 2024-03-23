using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NatsukiTakahashi
{
    internal class DialogManager
    {
        private static List<DialogDefinition> dialogsInQueue = new List<DialogDefinition>();
        public static void AddDialogToQueue(DialogDefinition dialogdef)
        {
            dialogsInQueue.Add(dialogdef);
        }

        public static void ProceedWithDialog()
        {
            if (dialogsInQueue.Count == 0)
                return;

            Form1.InitializeDialog(dialogsInQueue[0]);
            dialogsInQueue.RemoveAt(0);
        }
    }
}
