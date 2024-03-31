using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Forms;

namespace NatsukiLauncher
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            //artworkscroller.Scroll += Panel1_Scroll;
            artworkpanel2.MouseWheel += Panel1_MouseWheel;

            CheckForFirstTime();
            LoadSettings();
            InitNotifyIcon();
            CheckForLaunch();
        }

        private void SetStartup(bool set)
        {
            RegistryKey rk = Registry.CurrentUser.OpenSubKey
                ("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);

            if (set)
                rk.SetValue(Application.ProductName, Application.ExecutablePath);
            else
                rk.DeleteValue(Application.ProductName, false);

        }

        public bool launcherStaysOpen = false;

        public void LoadSettings()
        {
            string[] lastSave = File.ReadAllLines(Environment.CurrentDirectory + "\\build\\launcher.config");
            
            if (lastSave[0].Split('=')[1] == "1") // run on boot
            {
                SetStartup(true);
                checkBox1.Checked = true;
            }
            else
            {
                SetStartup(false);
                checkBox1.Checked = false;
            }

            if (lastSave[1].Split('=')[1] == "1") // stay open
            {
                launcherStaysOpen = true;
                checkBox2.Checked = true;
            }
            else
            {
                launcherStaysOpen = false;
                checkBox2.Checked = false;
            }

            if (lastSave[2].Split('=')[1] == "<defaultUser>")
            {
                textBox1.Text = Environment.UserName;
                UpdateConfig();
            }
            else
            {
                textBox1.Text = lastSave[2].Split('=')[1];
            }
        }

        public bool firstTime = false;
        public void CheckForFirstTime()
        {
            string firstTime = File.ReadAllText(Environment.CurrentDirectory + "\\build\\persist\\FirstTime.txt");

            if (firstTime.Contains("1"))
            {
                this.firstTime = true;
                File.WriteAllText(Environment.CurrentDirectory + "\\build\\persist\\FirstTime.txt", "0");
            }
        }

        public void CheckForLaunch()
        {
            string analogLaunch = File.ReadAllText(Environment.CurrentDirectory + "\\build\\persist\\AnalogLaunch.txt");

            if(analogLaunch.Contains("1"))
            {
                LaunchClicked();
            }

            File.WriteAllText(Environment.CurrentDirectory + "\\build\\persist\\AnalogLaunch.txt", "0");
        }

        public static NotifyIcon notifyIconInstance = null;

        public void InitNotifyIcon()
        {
            NotifyIcon nico = new NotifyIcon();
            nico.Icon= this.Icon;
            nico.Text = "Natsuki Takahashi";
            nico.Click += (sender, e) =>
            {
                contextMenuStrip1.Show(Cursor.Position);
            };

            nico.ContextMenuStrip = contextMenuStrip1;
            nico.Visible = true;
            nico.ContextMenuStrip.Enabled = true;
            nico.BalloonTipTitle = "Welcome!";
            nico.BalloonTipText = "Meet Natsuki Takahashi!";
            notifyIconInstance = nico;
        }

        private void button1_MouseEnter(object sender, EventArgs e)
        {
            if (isLaunching)
                return;

            button1.BackgroundImage = NatsukiLauncher.Properties.Resources.kitsuneLaunchHover;
            HideSelectionCursor();
        }

        private void button1_MouseLeave(object sender, EventArgs e)
        {
            if (isLaunching)
                return;

            if (hasLaunched)
                return;

            button1.BackgroundImage = NatsukiLauncher.Properties.Resources.kitsuneLaunch;
            //HideSelectionCursor();
        }

        private void button2_MouseMove(object sender, MouseEventArgs e)
        {
            MoveSelectionCursor(e.Location, (Control)sender);
        }

        public void MoveSelectionCursor(Point pos, Control control)
        {
            if(isLaunching)
                return;

            if (hasLaunched)
                return;

            label1.Show();
            label1.Location = new Point(label1.Location.X, pos.Y + control.Location.Y);
        }

        public void HideSelectionCursor()
        {
            label1.Hide();
        }

        private void button3_MouseMove(object sender, MouseEventArgs e)
        {
            MoveSelectionCursor(e.Location, (Control)sender);
        }

        private void button4_MouseMove(object sender, MouseEventArgs e)
        {
            MoveSelectionCursor(e.Location, (Control)sender);
        }

        private void button5_MouseMove(object sender, MouseEventArgs e)
        {
            MoveSelectionCursor(e.Location, (Control)sender);
        }

        private void button2_MouseLeave(object sender, EventArgs e)
        {
            HideSelectionCursor();
        }

        private void button3_MouseLeave(object sender, EventArgs e)
        {
            HideSelectionCursor();
        }

        private void button4_MouseLeave(object sender, EventArgs e)
        {
            HideSelectionCursor();
        }

        private void button5_MouseLeave(object sender, EventArgs e)
        {
            HideSelectionCursor();

            pictureBox2.Hide();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            LaunchClicked();
        }

        public void LaunchClicked()
        {
            if (managedProcess != null && hasLaunched)
            {
                managedProcess.Kill();
                Application.Restart();
                return;
            }

            button1.BackgroundImage = NatsukiLauncher.Properties.Resources.kitsuneLaunch__1_;

            Launch();
            if(firstTime) { notifyIconInstance.ShowBalloonTip(3); }
            
        }

        public bool isLaunching = false;
        public bool hasLaunched = false;

        public Process managedProcess;

        public async void Launch()
        {
            if (managedProcess != null)
                return;

            if (!Directory.Exists(Environment.CurrentDirectory + "\\build\\main\\"))
            {
                richTextBox1.Text += "Main build is missing! Please reinstall this software!" + Environment.NewLine;
                button1.BackgroundImage = NatsukiLauncher.Properties.Resources.kitsuneLaunch__1___2_;
                return;
            }

            if (!File.Exists(Environment.CurrentDirectory + "\\build\\main\\NatsukiTakahashi.exe"))
            {
                richTextBox1.Text += "The source file is missing! Please reinstall this software!" + Environment.NewLine;
                button1.BackgroundImage = NatsukiLauncher.Properties.Resources.kitsuneLaunch__1___2_;
                return;
            }

            button1.BackgroundImage = NatsukiLauncher.Properties.Resources.kitsuneLaunch__1_;

            artworkpanel2.Hide();
            configpanel2.Hide();
            aboutpanel2.Hide();

            isLaunching = true;
            panel1.Show();
            panel1.Location = new Point(12, 165);
            richTextBox1.Clear();

            richTextBox1.Text += "Calling for Natsuki..." + Environment.NewLine;
            Process prcs = new Process();
            prcs.EnableRaisingEvents = true;
            prcs.Exited += (sender, e) =>
            {
                if (prcs.ExitCode != 0)
                {
                    this.Show();
                    this.WindowState = FormWindowState.Normal;
                    this.Opacity = 100;
                    MessageBox.Show("Uh-Oh, It looks like something crashed. Please report this bug to the developers.");
                    isLaunching = false;
                    hasLaunched = false;
                    Application.Restart();
                }
                else
                {
                    this.Show();
                    this.WindowState = FormWindowState.Normal;
                    this.Opacity = 100;
                    richTextBox1.Text += "Exiting...";
                    isLaunching = false;
                    hasLaunched = false;
                    Application.Exit();
                }
            };

            prcs = System.Diagnostics.Process.Start(Environment.CurrentDirectory + "\\build\\main\\NatsukiTakahashi.exe");


            managedProcess = prcs;

            richTextBox1.Text += "Please wait, while we're trying to wake up Natsuki :3" + Environment.NewLine;
            richTextBox1.Text += "This will take around 3 more calls." + Environment.NewLine;
            richTextBox1.Text += "..." + Environment.NewLine;
            richTextBox1.Text += "..." + Environment.NewLine;
            richTextBox1.Text += "..." + Environment.NewLine;

            await Task.Delay(5000);

            button1.BackgroundImage = NatsukiLauncher.Properties.Resources.running;
            richTextBox1.Text += "Loading in the background..." + Environment.NewLine;
            hasLaunched = true;
            isLaunching = false;

            richTextBox1.Text += "..." + Environment.NewLine;
            richTextBox1.Text += "This Process will now minimize to tray" + Environment.NewLine;
            richTextBox1.Text += "Please use tray to exit, to avoid data loss!" + Environment.NewLine;
            await Task.Delay(1000);

            button1.BackgroundImage = NatsukiLauncher.Properties.Resources.running;

            if (!launcherStaysOpen)
            {
                this.WindowState = FormWindowState.Minimized;
                this.Hide();
                this.Opacity = 0;
            }

            //prcs.WaitForExit();

        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            notifyIconInstance.Visible = false;
            notifyIconInstance.Dispose();

            try
            {
                if (managedProcess != null)
                {
                    managedProcess.Kill();
                }
            }
            catch { }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (managedProcess != null)
            {
                managedProcess.Kill();
            }
        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {

        }

        private void tellAFunJokeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("TBI");
        }

        private void goToSleepToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(managedProcess != null)
            {
                managedProcess.Kill();
            }

            Application.Exit();
        }

        private void forceRestartToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bool oldStatus = this.Visible;

            //restart with launcher
            this.Hide();

            
            DialogResult dr = new DialogResult();
            dr = MessageBox.Show("Do you want to restart and launch again?", "Force Restart", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if(dr == DialogResult.Yes)
            {
                this.Show();
                richTextBox1.Clear();
                richTextBox1.Text += "RESTARTING...";
                if(managedProcess != null)
                {
                    managedProcess.Kill();
                }

                File.WriteAllText(Environment.CurrentDirectory + "\\build\\persist\\AnalogLaunch.txt", "1");

                Application.Restart();
            }
            else
            {
                this.Visible = oldStatus;
            }


        }

        private void runMenuMangerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("TBI");
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            label12.Parent = pictureBox1;
            label11.Parent = pictureBox1;
            label10.Parent = pictureBox1;

            initialArtwPos = artworkscroller.Location;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            LoadArtwork();
            artworkpanel2.Show();
            artworkpanel2.Location = new Point(12, 162);
        }

        private void label5_Click(object sender, EventArgs e)
        {
            artworkpanel2.Hide();
            UnLoadArtwork();
        }

        private void label8_Click(object sender, EventArgs e)
        {
            configpanel2.Hide();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            UpdateConfig();
        }

        public void UpdateConfig()
        {
            bool run = checkBox1.Checked;
            bool open = checkBox2.Checked;
            string name = textBox1.Text;

            string[] fileSave =
            {
                "run=" + (run ? "1" : "0"),
                "open=" + (open ? "1" : "0"),
                "name=" + (name)
            };

            File.WriteAllLines(Environment.CurrentDirectory + "\\build\\launcher.config", fileSave);
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            UpdateConfig();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            UpdateConfig();
        }
        
        private void button3_Click(object sender, EventArgs e)
        {
            configpanel2.Location = new Point(11, 163);
            configpanel2.Show();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            aboutpanel2.Location = new Point(12, 163);
            aboutpanel2.Show();
        }

        private void label11_Click(object sender, EventArgs e)
        {
            aboutpanel2.Hide();
        }

        private void button5_MouseEnter(object sender, EventArgs e)
        {
            pictureBox2.Show();
        }

        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            pictureBox2.Hide();
        }

        private void button1_BackgroundImageChanged(object sender, EventArgs e)
        {
            
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void artworkpanel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private int artworkSpacing = 5;
        private int imagesInRow = 3;
        private int imageHeight = 50;
        private Point initialArtwPos;

        public void LoadArtwork()
        {
            string[] files = Directory.GetFiles(Environment.CurrentDirectory + "\\build\\artwork\\");

            int currentIndex = 0;
            int currentRow = 0;
            int rowCounter = 0;

            foreach (string file in files)
            {
                if (file.EndsWith(".png") || file.EndsWith(".jpg"))
                {
                    currentIndex += 1;
                    rowCounter += 1;

                    try
                    {
                        PictureBox newImage = new PictureBox();
                        newImage.Visible = false;

                        newImage.Click += (sender, e) =>
                        {
                            ImageViewer imgview = new ImageViewer();
                            imgview.filePath = file;
                            imgview.ShowDialog();
                        };

                        int pictureWidth = (artworkscroller.Width - ((imagesInRow + 1) * artworkSpacing)) / imagesInRow;
                        int imageRowValue = (rowCounter - 1) * (pictureWidth + artworkSpacing);

                        newImage.Location = new Point(imageRowValue, currentRow * (imageHeight + artworkSpacing));
                        newImage.SizeMode = PictureBoxSizeMode.Zoom;
                        newImage.BackColor = Color.White;
                        newImage.Load(file);

                        artworkscroller.Controls.Add(newImage);
                        newImage.Show();

                        newImage.Cursor = Cursors.Hand;

                        if (rowCounter >= imagesInRow)
                        {
                            currentRow++;
                            rowCounter = 0;
                        }
                    }
                    catch
                    {
                        continue;
                    }
                }
            }

            //MessageBox.Show(currentRow.ToString());

            artworkscroller.Size = new Size(artworkscroller.Width, (currentRow + 1) * (imageHeight + artworkSpacing));

            vScrollBar1.Maximum = artworkscroller.Height;
        }


        public void UnLoadArtwork()
        {
            foreach(Control c in artworkscroller.Controls)
            {
                if(c != artworkscroller && c.GetType() == typeof(PictureBox))
                {
                    try {
                        ((PictureBox)c).Image = null;
                        c.Hide();
                        ((PictureBox)c).Dispose();
                        artworkscroller.Controls.Remove(c); } catch { /*fuck*/ }
                }
            }
        }

        private void vScrollBar1_Scroll(object sender, ScrollEventArgs e)
        {
            artworkscroller.Location = new Point(artworkscroller.Location.X, initialArtwPos.Y - vScrollBar1.Value);
        }

        private void Panel1_Scroll(object sender, ScrollEventArgs e)
        {
            //MessageBox.Show("scroll!");
        }

        private void Panel1_MouseWheel(object sender, MouseEventArgs e)
        {
            //MessageBox.Show("scroll!");

            if(e.Delta > 0)
            {
                if (vScrollBar1.Value - 10 < vScrollBar1.Minimum)
                {
                    vScrollBar1.Value = vScrollBar1.Minimum;
                }
                else
                {
                    vScrollBar1.Value = vScrollBar1.Value - 10;
                }
            }
            else
            {
                if (vScrollBar1.Value + 10 > vScrollBar1.Maximum)
                {
                    vScrollBar1.Value = vScrollBar1.Maximum;
                }
                else
                {
                    vScrollBar1.Value = vScrollBar1.Value + 10;
                }
            }

            artworkscroller.Location = new Point(artworkscroller.Location.X, initialArtwPos.Y - vScrollBar1.Value);
        }

        private void artworkpanel2_Scroll(object sender, ScrollEventArgs e)
        {
            /*
            if (e.Type == ScrollEventType.SmallDecrement || e.Type == ScrollEventType.LargeDecrement)
            {
                //MessageBox.Show("Panel scrolled up. Scroll position: " + panel1.VerticalScroll.Value);

                if (vScrollBar1.Value + 10 > vScrollBar1.Maximum)
                {
                    vScrollBar1.Value = vScrollBar1.Maximum;
                }
                else
                {
                    vScrollBar1.Value = vScrollBar1.Value + 10;
                }

            }
            else if (e.Type == ScrollEventType.SmallIncrement || e.Type == ScrollEventType.LargeIncrement)
            {
                //MessageBox.Show("Panel scrolled down. Scroll position: " + panel1.VerticalScroll.Value);

                if (vScrollBar1.Value - 10 < vScrollBar1.Minimum)
                {
                    vScrollBar1.Value = vScrollBar1.Minimum;
                }
                else
                {
                    vScrollBar1.Value = vScrollBar1.Value - 10;
                }
            }
            */
        }
    }
}
