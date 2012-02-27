using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.Xml.Serialization;

namespace XiboClient.Log
{
    public partial class ClientInfo : Form
    {
        public delegate void StatusDelegate(string status);
        public delegate void AddLogMessage(string message, LogType logType);

        /// <summary>
        /// Set the schedule status
        /// </summary>
        public string ScheduleStatus
        {
            set
            {
                if (InvokeRequired)
                {
                    BeginInvoke(new StatusDelegate(SetScheduleStatus), value);
                }
                else
                {
                    SetScheduleStatus(value);
                }
            }
        }

        /// <summary>
        /// Set the required files status
        /// </summary>
        public string RequiredFilesStatus
        {
            set
            {
                if (InvokeRequired)
                {
                    BeginInvoke(new StatusDelegate(SetRequiredFilesStatus), value);
                }
                else
                {
                    SetRequiredFilesStatus(value);
                }
            }
        }

        /// <summary>
        /// Required Files Object
        /// </summary>
        public RequiredFiles RequiredFiles
        {
            set
            {
                _requiredFiles = value;
            }
        }
        private RequiredFiles _requiredFiles;

        /// <summary>
        /// Client Info Object
        /// </summary>
        public ClientInfo()
        {
            InitializeComponent();

            MaximizeBox = false;
            MinimizeBox = false;

            FormClosing += new FormClosingEventHandler(ClientInfo_FormClosing);
        }

        /// <summary>
        /// Set the schedule status
        /// </summary>
        /// <param name="status"></param>
        public void SetScheduleStatus(string status)
        {
            scheduleStatusLabel.Text = status;
        }

        /// <summary>
        /// Set the schedule status
        /// </summary>
        /// <param name="status"></param>
        public void SetRequiredFilesStatus(string status)
        {
            requiredFilesStatus.Text = status;
        }

        /// <summary>
        /// Set the required files textbox
        /// </summary>
        /// <param name="status"></param>
        public void SetRequiredFilesTextBox(string status)
        {
            requiredFilesTextBox.Text = status;
        }

        /// <summary>
        /// Adds a log message
        /// </summary>
        /// <param name="message"></param>
        public void AddToLogGrid(string message, LogType logType)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new AddLogMessage(AddToLogGrid), new object[] { message, logType });
                return;
            }

            int newRow = logDataGridView.Rows.Add();

            LogMessage logMessage;

            try
            {
                logMessage = new LogMessage(message);
            }
            catch
            {
                logMessage = new LogMessage("Unknown", message);
            }

            logDataGridView.Rows[newRow].Cells[0].Value = logMessage.LogDate.ToString();
            logDataGridView.Rows[newRow].Cells[1].Value = logType.ToString();
            logDataGridView.Rows[newRow].Cells[2].Value = logMessage._method;
            logDataGridView.Rows[newRow].Cells[3].Value = logMessage._message;
        }

        /// <summary>
        /// Update the required files text box
        /// </summary>
        public void UpdateRequiredFiles()
        {
            string requiredFilesTextBox = "";

            if (_requiredFiles != null)
            {
                foreach (RequiredFile requiredFile in _requiredFiles.RequiredFileList)
                {
                    string percentComplete = (requiredFile.Downloading) ? (requiredFile.ChunkOffset / requiredFile.Size).ToString() : "100";
                    requiredFilesTextBox = requiredFilesTextBox + requiredFile.FileType + ": " + requiredFile.Path + ". (" + percentComplete + "%)" + Environment.NewLine;
                }
            }

            if (InvokeRequired)
            {
                BeginInvoke(new StatusDelegate(SetRequiredFilesTextBox), requiredFilesTextBox);
            }
            else
            {
                SetRequiredFilesTextBox(requiredFilesTextBox);
            }
        }

        /// <summary>
        /// Form Closing Event (prevents the form from closing)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ClientInfo_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Prevent the form from closing
            e.Cancel = true;
            Hide();
        }
    }
}