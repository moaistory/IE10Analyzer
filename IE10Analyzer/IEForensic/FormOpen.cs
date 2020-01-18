using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Management;

namespace IE10Analyzer
{

    public partial class FormOpen : Form
    {
        //[DllImport("ExtractFiles.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        //public static extern bool FileCopy([MarshalAs(UnmanagedType.LPWStr)]String lpSrcName, [MarshalAs(UnmanagedType.LPWStr)]String lpDstName);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate bool FileCopy([MarshalAs(UnmanagedType.LPWStr)]String lpSrcName, [MarshalAs(UnmanagedType.LPWStr)]String lpDstName);

        [DllImport("kernel32", SetLastError = true, CharSet = CharSet.Ansi)]
        static extern IntPtr LoadLibrary([MarshalAs(UnmanagedType.LPStr)]string lpFileName);

        [DllImport("kernel32", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
        static extern IntPtr GetProcAddress(IntPtr hModule, string procName);

        [DllImport("kernel32", SetLastError = true)]
        static extern bool FreeLibrary(IntPtr hModule);

        private String filePath = "";
        private String workingDirectory = "";
        private BackgroundWorker backgroudWoker;
        private FormWait formWait;
        private FormMain formMain;
        public FormOpen(FormMain formMain)
        {

            InitializeComponent();
            this.formMain = formMain;
            comboBoxBrowser.SelectedIndex = 0;
        }

        public bool fileOpen(string path)
        {
            try
            {
                HexReader hexReader = new HexReader(path);
                long signature = hexReader.readLong(4);
                if (signature != 6736818458095L)
                {
                    MessageBox.Show("Please open the correct file", "This file is not ESE database", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.filePath = "";
                    this.textBoxFilePath.Text = this.filePath;
                    hexReader.close();
                    return false ;
                }
                this.filePath = path;
                this.textBoxFilePath.Text = this.filePath;
                int status = hexReader.readInt(0x34);
                if (status == 2)
                {
                    this.labelFileStatus.Text = "File status : Dirty";
                    radioButtonUsingAPI.Enabled = false;
                    radioButtonParsing.Checked = true;
                }
                else
                {
                    this.labelFileStatus.Text = "File status : Clean";
                    radioButtonUsingAPI.Enabled = true;
                    radioButtonUsingAPI.Checked = true;
                    DateTime currTime = DateTime.Now;
                    String time = currTime.ToString("yyyy") + "_" + currTime.ToString("MM") + "_" + currTime.ToString("dd") + "__" + currTime.ToString("HH_mm_ss");
                    workingDirectory = Path.GetTempPath() + @"IE10Analyzer_" + time + @"\" + Path.GetFileName(textBoxFilePath.Text);
                    textBoxWorkingDirectory.Text = workingDirectory;

                }


                hexReader.close();
                return true;
            }
            catch
            {
                MessageBox.Show("Can not open the file", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        private void buttonFilePath_Click(object sender, EventArgs e)
        {
            OpenFileDialog open = new OpenFileDialog();
            open.Title = "Select ESE database file";


            if (open.ShowDialog() == DialogResult.OK)
            {
                fileOpen(open.FileName);
            }
        }

        private void radioButtonExtract_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonExtract.Checked == true)
            {
                buttonFilePath.Enabled = false;
                comboBoxBrowser.Enabled = true;
                comboBoxBrowser.SelectedIndex = 0;
                this.filePath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + @"\AppData\Local\Microsoft\Windows\WebCache\WebCacheV01.dat";
                if (!File.Exists(this.filePath))
                {
                    MessageBox.Show("Internet Explorer 10 later version or Edge browser is not installed", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.radioButtonOpen.Checked = true;
                }
                else
                {
                    this.textBoxFilePath.Text = this.filePath;
                    this.labelFileStatus.Text = "File status : Dirty";
                    radioButtonUsingAPI.Enabled = false;
                    radioButtonParsing.Checked = true;
                    buttonWorkingDirectory.Enabled = true;
                    DateTime currTime = DateTime.Now;
                    String time = currTime.ToString("yyyy") + "_" + currTime.ToString("MM") + "_" + currTime.ToString("dd") + "__" + currTime.ToString("HH_mm_ss");
                    workingDirectory = Path.GetTempPath() + @"IE10Analyzer_" + time + @"\" + Path.GetFileName(textBoxFilePath.Text);
                    textBoxWorkingDirectory.Text = workingDirectory;
                    groupBoxWorking.Text = "Working directory and extracted file name";
                }
            }
        }

        public void radioButtonOpen_Check()
        {
            radioButtonOpen.Checked = true;
        }

        private void radioButtonOpen_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonOpen.Checked == true)
            {
                this.filePath = "";
                this.textBoxFilePath.Text = this.filePath;
                this.labelFileStatus.Text = "File status :";
                buttonFilePath.Enabled = true;
                radioButtonUsingAPI.Enabled = true;
                radioButtonUsingAPI.Checked = true;
                buttonWorkingDirectory.Enabled = true;
                comboBoxBrowser.Enabled = false;
                groupBoxWorking.Text = "Working directory and copied file name";
            }
        }

        private void radioButtonParsing_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonParsing.Checked == true)
            {
                if (radioButtonOpen.Checked)
                {
                    buttonWorkingDirectory.Enabled = false;
                    textBoxWorkingDirectory.Text = "";
                    this.workingDirectory = "";
                }
                else
                {
                    buttonWorkingDirectory.Enabled = true;
                    DateTime currTime = DateTime.Now;
                    String time = currTime.ToString("yyyy") + "_" + currTime.ToString("MM") + "_" + currTime.ToString("dd") + "__" + currTime.ToString("HH_mm_ss");
                    workingDirectory = Path.GetTempPath() + @"IE10Analyzer_" + time + @"\" + Path.GetFileName(textBoxFilePath.Text);
                    textBoxWorkingDirectory.Text = workingDirectory;
                }
            }
        }

        private void radioButtonRecovery_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonRecovery.Checked == true)
            {
                if (radioButtonOpen.Checked)
                {
                    buttonWorkingDirectory.Enabled = false;
                    textBoxWorkingDirectory.Text = "";
                    this.workingDirectory = "";
                }
                else
                {
                    buttonWorkingDirectory.Enabled = true;
                    DateTime currTime = DateTime.Now;
                    String time = currTime.ToString("yyyy") + "_" + currTime.ToString("MM") + "_" + currTime.ToString("dd") + "__" + currTime.ToString("HH_mm_ss");
                    workingDirectory = Path.GetTempPath() + @"IE10Analyzer_" + time + @"\" + Path.GetFileName(textBoxFilePath.Text);
                    textBoxWorkingDirectory.Text = workingDirectory;
                }
            }
            
        }

        private void radioButtonCarving_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonCarving.Checked == true)
            {
                if (radioButtonOpen.Checked)
                {
                    buttonWorkingDirectory.Enabled = false;
                    textBoxWorkingDirectory.Text = "";
                    this.workingDirectory = "";
                }
                else
                {
                    buttonWorkingDirectory.Enabled = true;
                    DateTime currTime = DateTime.Now;
                    String time = currTime.ToString("yyyy") + "_" + currTime.ToString("MM") + "_" + currTime.ToString("dd") + "__" + currTime.ToString("HH_mm_ss");
                    workingDirectory = Path.GetTempPath() + @"IE10Analyzer_" + time + @"\" + Path.GetFileName(textBoxFilePath.Text);
                    textBoxWorkingDirectory.Text = workingDirectory;
                }
            }

        }
        private void radioButtonUsingAPI_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonUsingAPI.Checked == true)
            {
                if (radioButtonOpen.Checked)
                {
                    buttonWorkingDirectory.Enabled = true;
                    DateTime currTime = DateTime.Now;
                    String time = currTime.ToString("yyyy") + "_" + currTime.ToString("MM") + "_" + currTime.ToString("dd") + "__" + currTime.ToString("HH_mm_ss");
                    workingDirectory = Path.GetTempPath() + @"IE10Analyzer_" + time + @"\" + Path.GetFileName(textBoxFilePath.Text);
                    textBoxWorkingDirectory.Text = workingDirectory;
                }
                else
                {
                    buttonWorkingDirectory.Enabled = false;
                    textBoxWorkingDirectory.Text = "";
                    this.workingDirectory = "";
                }
            }
        }

        private void buttonWorkingDirectory_Click(object sender, EventArgs e)
        {
            SaveFileDialog sf = new SaveFileDialog();
            
            sf.FileName = Path.GetFileName(textBoxFilePath.Text);

            if (sf.ShowDialog() == DialogResult.OK)
            {
                this.workingDirectory = sf.FileName;
                textBoxWorkingDirectory.Text = this.workingDirectory;
                if (File.Exists(workingDirectory))
                {
                    File.Delete(workingDirectory);
                }
            }
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void extractFile()
        {
            formWait = new FormWait();
            formWait.setLabel("extracting file");
            formWait.FormClosing += new FormClosingEventHandler(waitFormClosingEventHandler);
            formWait.setStyle(ProgressBarStyle.Marquee);
            formWait.end("");
            backgroudWoker = new BackgroundWorker();
            backgroudWoker.DoWork += new DoWorkEventHandler(extractFileDoWork);
            backgroudWoker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(extractFileRunWorkerCompletedEventHandler);
            backgroudWoker.WorkerSupportsCancellation = true;
            backgroudWoker.RunWorkerAsync();
            formWait.ShowDialog();   
        }

        private void analyzeFile()
        {
            this.Visible = false;
            if (radioButtonUsingAPI.Checked == true)
            {
                Directory.CreateDirectory(Path.GetDirectoryName(workingDirectory));
                formMain.openAPI(this.filePath, this.workingDirectory);
            }
            else if (radioButtonParsing.Checked == true)
            {
                if (radioButtonExtract.Checked)
                {
                    formMain.openParsing(this.workingDirectory);
                }
                else
                {
                    formMain.openParsing(this.filePath);
                }
            }
            else if (radioButtonRecovery.Checked == true)
            {
                if (radioButtonExtract.Checked)
                {
                    formMain.openRecovery(this.workingDirectory);
                }
                else
                {
                    formMain.openRecovery(this.filePath);
                }
            }
            else if (radioButtonCarving.Checked == true)
            {
                if (radioButtonExtract.Checked)
                {
                    formMain.openCaving(this.workingDirectory);
                }
                else
                {
                    formMain.openCaving(this.filePath);
                }
            }
            this.Close();
        }



        public void extractFileDoWork(object sender, DoWorkEventArgs e)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(workingDirectory));
            if (!File.Exists("ExtractFiles.dll"))
            {
                using (Stream input = Assembly.GetExecutingAssembly().GetManifestResourceStream("IEForensic.Resources.ExtractFiles.dll"))
                using (Stream output = File.Create("ExtractFiles.dll"))
                {
                    byte[] buffer = new byte[8192];

                    int bytesRead;
                    while ((bytesRead = input.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        output.Write(buffer, 0, bytesRead);
                    }
                }
            }
            
            IntPtr hLib = LoadLibrary("ExtractFiles.dll");
            IntPtr ctorPtr = GetProcAddress(hLib, "FileCopy");
            FileCopy constructorFn = (FileCopy)Marshal.GetDelegateForFunctionPointer(ctorPtr, typeof(FileCopy));
            bool result = constructorFn(filePath, workingDirectory);
            FreeLibrary(hLib);

            if (File.Exists("ExtractFiles.dll"))
            {
                File.Delete("ExtractFiles.dll");
            }
            e.Result = result;
        }

        public void waitFormClosingEventHandler(object sender, FormClosingEventArgs e)
        {            
            if (e.CloseReason == CloseReason.UserClosing)
            {
                Process[] processList = Process.GetProcessesByName("ExtractFile");
                if (processList.Length > 0)
                {
                    foreach (Process process in processList)
                    {
                        process.Kill();
                    }
                    MessageBox.Show("Cancel extracting file", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            
        }

        public void extractFileRunWorkerCompletedEventHandler(object sender, RunWorkerCompletedEventArgs e)
        {
            bool result = (bool)e.Result;
            formWait.Visible = false;
            formWait.Close();

            if (result == false)
            {
                MessageBox.Show("Cannot extract file, Run this program as an administrator", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                analyzeFile();
            }

        }

        private void buttonOpen_Click(object sender, EventArgs e)
        {
            if (textBoxFilePath.Text.Length == 0)
            {
                MessageBox.Show("File path is blank", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (!File.Exists(textBoxFilePath.Text))
            {
                MessageBox.Show("File does not exist", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (radioButtonExtract.Checked == true)
            {
                extractFile();
            }
            else
            {
                analyzeFile();
            }
            
        }

        private void comboBoxBrowser_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxBrowser.SelectedIndex == 0)
            {
                this.filePath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + @"\AppData\Local\Microsoft\Windows\WebCache\WebCacheV01.dat";
            }
            else
            {
                this.filePath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + @"\AppData\Local\Microsoft\Windows\WebCache\WebCacheV01.dat";
            }
            this.textBoxFilePath.Text = this.filePath;
        }

        private void FormOpen_FormClosed(object sender, FormClosedEventArgs e)
        {

        }
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == (Keys.Escape))
            {
                this.Close();
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        
    }
}
