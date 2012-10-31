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
        ScrobblingSettings settings;

        public ConfigForm(ScrobblingSettings s)
        {
            InitializeComponent();

            settings = s;
            settingsBindingSource.DataSource = s;
            UpdateAuthStatusText(!string.IsNullOrEmpty(s.SessionKey));
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
            string sessionKey = string.Empty;
            try
            {
                sessionKey = Auth.Authenticate(username, password);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
            UpdateAuthStatusText(!string.IsNullOrEmpty(sessionKey));

            settings.SessionKey = sessionKey;
        }

        void UpdateAuthStatusText(bool loggedIn)
        {
            if (loggedIn)
            {
                label_authStatus.ForeColor = Color.Green;
                label_authStatus.Text = "Logged in";
            }
            else
            {
                label_authStatus.ForeColor = Color.Black;//FromArgb(192, 0, 0);
                label_authStatus.Text = "Not logged in";
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
    }
}
