using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using lastfm.Services;

namespace lastfm
{
    public partial class ConfigForm : Form
    {
        enum AuthStatus { LoggedIn, NotLoggedIn, LoggingIn }


        ScrobblingSettings settings;

        public ConfigForm(ScrobblingSettings s)
        {
            InitializeComponent();

            settings = s;
            settingsBindingSource.DataSource = s;
            UpdateAuthStatusText(!string.IsNullOrEmpty(s.SessionKey) ? AuthStatus.LoggedIn : AuthStatus.NotLoggedIn);
            LfmServiceProxy.LastfmErrorOccured += HandleAuthError;
            LfmServiceProxy.NetworkErrorOccured += HandleAuthError;
            LfmServiceProxy.ErrorOccured += LfmServiceProxy_ErrorOccured;
        }

        void LfmServiceProxy_ErrorOccured(object sender, LfmServiceProxy.ErrorEventArgs e)
        {
            HandleAuthError(e.Error.Message);
        }

        void HandleAuthError(object sender, LfmServiceProxy.RequestErrorEventArgs e)
        {
            HandleAuthError(e.Message);
        }

        void HandleAuthError(string message)
        {
            UpdateAuthStatusText(AuthStatus.NotLoggedIn);
            var res = MessageBox.Show(message, "Authentication error", MessageBoxButtons.OKCancel);
        }

        private void checkBox_unmaskPassword_CheckedChanged(object sender, EventArgs e)
        {
            textBox_password.PasswordChar = checkBox_unmaskPassword.Checked ? '\0' : '●';
        }

        private void button_authenticate_Click(object sender, EventArgs e)
        {
            var username = textBox_username.Text;
            var password = textBox_password.Text;

            //if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            //    MessageBox.Show("Empty username or password", "Error");

            //Auth.Username = username;
            //Auth.Password = password;
            //string sessionKey = string.Empty;
            try
            {
                Auth.Authenticate(username, password, (string sessionKey) =>
                    {
                        settings.SessionKey = sessionKey;
                        LfmServiceProxy.SessionKey = sessionKey;
                        UpdateAuthStatusText(AuthStatus.LoggedIn);
                    });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }

            UpdateAuthStatusText(AuthStatus.LoggingIn);
        }

        void UpdateAuthStatusText(AuthStatus s)
        {
            if (s == AuthStatus.LoggedIn)
            {
                label_authStatus.ForeColor = Color.Green;
                label_authStatus.Text = "Logged in";
            }
            else if (s == AuthStatus.NotLoggedIn)
            {
                label_authStatus.ForeColor = Color.FromArgb(192, 0, 0);
                label_authStatus.Text = "Not logged in";
            }
            else
            {
                label_authStatus.ForeColor = Color.Black;
                label_authStatus.Text = "Logging in...";
            }
        }

        private void button_cancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void button_ok_Click(object sender, EventArgs e)
        {
            var s = settingsBindingSource.DataSource as ScrobblingSettings;
            s.SaveToFile();
            Close();
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            var cb = sender as CheckBox;
            groupBox_scrobbling.Enabled = cb.Checked;
            //cb.Enabled = true;
            //checkBox_scrobbleRadio.Enabled = true;
        }

        private void button_log_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(Logger.LogFileName);
        }

        private void ConfigForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            LfmServiceProxy.ErrorOccured -= LfmServiceProxy_ErrorOccured;
            LfmServiceProxy.LastfmErrorOccured -= HandleAuthError;
            LfmServiceProxy.NetworkErrorOccured -= HandleAuthError;
        }
    }
}
