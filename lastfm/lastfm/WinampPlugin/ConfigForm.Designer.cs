﻿namespace lastfm
{
    partial class ConfigForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConfigForm));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label_authStatus = new System.Windows.Forms.Label();
            this.checkBox_unmaskPassword = new System.Windows.Forms.CheckBox();
            this.button_authenticate = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.textBox_password = new System.Windows.Forms.TextBox();
            this.textBox_username = new System.Windows.Forms.TextBox();
            this.groupBox_scrobbling = new System.Windows.Forms.GroupBox();
            this.numericUpDown1 = new System.Windows.Forms.NumericUpDown();
            this.settingsBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.label7 = new System.Windows.Forms.Label();
            this.numericUpDown_scrobblePercent = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.checkBox_scrobbleRadio = new System.Windows.Forms.CheckBox();
            this.checkBox_updateNowPlaying = new System.Windows.Forms.CheckBox();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.button_ok = new System.Windows.Forms.Button();
            this.button_cancel = new System.Windows.Forms.Button();
            this.checkBox_enableScrobbling = new System.Windows.Forms.CheckBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.button_log = new System.Windows.Forms.Button();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.groupBox1.SuspendLayout();
            this.groupBox_scrobbling.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.settingsBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_scrobblePercent)).BeginInit();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Controls.Add(this.label_authStatus);
            this.groupBox1.Controls.Add(this.checkBox_unmaskPassword);
            this.groupBox1.Controls.Add(this.button_authenticate);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.textBox_password);
            this.groupBox1.Controls.Add(this.textBox_username);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // label_authStatus
            // 
            resources.ApplyResources(this.label_authStatus, "label_authStatus");
            this.label_authStatus.Name = "label_authStatus";
            // 
            // checkBox_unmaskPassword
            // 
            resources.ApplyResources(this.checkBox_unmaskPassword, "checkBox_unmaskPassword");
            this.checkBox_unmaskPassword.Name = "checkBox_unmaskPassword";
            this.checkBox_unmaskPassword.UseVisualStyleBackColor = true;
            this.checkBox_unmaskPassword.CheckedChanged += new System.EventHandler(this.checkBox_unmaskPassword_CheckedChanged);
            // 
            // button_authenticate
            // 
            resources.ApplyResources(this.button_authenticate, "button_authenticate");
            this.button_authenticate.Name = "button_authenticate";
            this.button_authenticate.UseVisualStyleBackColor = true;
            this.button_authenticate.Click += new System.EventHandler(this.button_authenticate_Click);
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // textBox_password
            // 
            resources.ApplyResources(this.textBox_password, "textBox_password");
            this.textBox_password.Name = "textBox_password";
            // 
            // textBox_username
            // 
            resources.ApplyResources(this.textBox_username, "textBox_username");
            this.textBox_username.Name = "textBox_username";
            // 
            // groupBox_scrobbling
            // 
            resources.ApplyResources(this.groupBox_scrobbling, "groupBox_scrobbling");
            this.groupBox_scrobbling.Controls.Add(this.numericUpDown1);
            this.groupBox_scrobbling.Controls.Add(this.label7);
            this.groupBox_scrobbling.Controls.Add(this.numericUpDown_scrobblePercent);
            this.groupBox_scrobbling.Controls.Add(this.label3);
            this.groupBox_scrobbling.Controls.Add(this.label4);
            this.groupBox_scrobbling.Controls.Add(this.checkBox_scrobbleRadio);
            this.groupBox_scrobbling.Name = "groupBox_scrobbling";
            this.groupBox_scrobbling.TabStop = false;
            // 
            // numericUpDown1
            // 
            resources.ApplyResources(this.numericUpDown1, "numericUpDown1");
            this.numericUpDown1.DataBindings.Add(new System.Windows.Forms.Binding("Value", this.settingsBindingSource, "DefaultScrobbleTime", true, System.Windows.Forms.DataSourceUpdateMode.OnValidation, "0"));
            this.numericUpDown1.Name = "numericUpDown1";
            this.toolTip1.SetToolTip(this.numericUpDown1, resources.GetString("numericUpDown1.ToolTip"));
            this.numericUpDown1.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
            // 
            // label7
            // 
            resources.ApplyResources(this.label7, "label7");
            this.label7.Name = "label7";
            this.toolTip1.SetToolTip(this.label7, resources.GetString("label7.ToolTip"));
            // 
            // numericUpDown_scrobblePercent
            // 
            resources.ApplyResources(this.numericUpDown_scrobblePercent, "numericUpDown_scrobblePercent");
            this.numericUpDown_scrobblePercent.DataBindings.Add(new System.Windows.Forms.Binding("Value", this.settingsBindingSource, "ScrobbleSongtimePercent", true));
            this.numericUpDown_scrobblePercent.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDown_scrobblePercent.Name = "numericUpDown_scrobblePercent";
            this.toolTip1.SetToolTip(this.numericUpDown_scrobblePercent, resources.GetString("numericUpDown_scrobblePercent.ToolTip"));
            this.numericUpDown_scrobblePercent.Value = new decimal(new int[] {
            70,
            0,
            0,
            0});
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            this.toolTip1.SetToolTip(this.label4, resources.GetString("label4.ToolTip"));
            // 
            // checkBox_scrobbleRadio
            // 
            resources.ApplyResources(this.checkBox_scrobbleRadio, "checkBox_scrobbleRadio");
            this.checkBox_scrobbleRadio.DataBindings.Add(new System.Windows.Forms.Binding("Checked", this.settingsBindingSource, "ScrobbleRadio", true));
            this.checkBox_scrobbleRadio.Name = "checkBox_scrobbleRadio";
            this.toolTip1.SetToolTip(this.checkBox_scrobbleRadio, resources.GetString("checkBox_scrobbleRadio.ToolTip"));
            this.checkBox_scrobbleRadio.UseVisualStyleBackColor = true;
            // 
            // checkBox_updateNowPlaying
            // 
            resources.ApplyResources(this.checkBox_updateNowPlaying, "checkBox_updateNowPlaying");
            this.checkBox_updateNowPlaying.DataBindings.Add(new System.Windows.Forms.Binding("Checked", this.settingsBindingSource, "UpdateNowPlaying", true));
            this.checkBox_updateNowPlaying.Name = "checkBox_updateNowPlaying";
            this.checkBox_updateNowPlaying.UseVisualStyleBackColor = true;
            // 
            // checkBox1
            // 
            resources.ApplyResources(this.checkBox1, "checkBox1");
            this.checkBox1.DataBindings.Add(new System.Windows.Forms.Binding("Checked", this.settingsBindingSource, "DisplayErrorMessages", true));
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.UseVisualStyleBackColor = true;
            // 
            // button_ok
            // 
            resources.ApplyResources(this.button_ok, "button_ok");
            this.button_ok.Name = "button_ok";
            this.button_ok.UseVisualStyleBackColor = true;
            this.button_ok.Click += new System.EventHandler(this.button_ok_Click);
            // 
            // button_cancel
            // 
            resources.ApplyResources(this.button_cancel, "button_cancel");
            this.button_cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button_cancel.Name = "button_cancel";
            this.button_cancel.UseVisualStyleBackColor = true;
            this.button_cancel.Click += new System.EventHandler(this.button_cancel_Click);
            // 
            // checkBox_enableScrobbling
            // 
            this.checkBox_enableScrobbling.DataBindings.Add(new System.Windows.Forms.Binding("Checked", this.settingsBindingSource, "ScrobblingEnabled", true));
            resources.ApplyResources(this.checkBox_enableScrobbling, "checkBox_enableScrobbling");
            this.checkBox_enableScrobbling.Name = "checkBox_enableScrobbling";
            this.checkBox_enableScrobbling.CheckedChanged += new System.EventHandler(this.checkBox2_CheckedChanged);
            // 
            // groupBox2
            // 
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.Controls.Add(this.button_log);
            this.groupBox2.Controls.Add(this.checkBox_updateNowPlaying);
            this.groupBox2.Controls.Add(this.checkBox1);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            // 
            // button_log
            // 
            resources.ApplyResources(this.button_log, "button_log");
            this.button_log.Name = "button_log";
            this.button_log.UseVisualStyleBackColor = true;
            this.button_log.Click += new System.EventHandler(this.button_log_Click);
            // 
            // ConfigForm
            // 
            this.AcceptButton = this.button_authenticate;
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.AutoValidate = System.Windows.Forms.AutoValidate.EnableAllowFocusChange;
            this.CancelButton = this.button_cancel;
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.button_cancel);
            this.Controls.Add(this.button_ok);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.checkBox_enableScrobbling);
            this.Controls.Add(this.groupBox_scrobbling);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.Name = "ConfigForm";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ConfigForm_FormClosing);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox_scrobbling.ResumeLayout(false);
            this.groupBox_scrobbling.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.settingsBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_scrobblePercent)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox checkBox_unmaskPassword;
        private System.Windows.Forms.Button button_authenticate;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBox_password;
        private System.Windows.Forms.TextBox textBox_username;
        private System.Windows.Forms.GroupBox groupBox_scrobbling;
        private System.Windows.Forms.NumericUpDown numericUpDown_scrobblePercent;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.CheckBox checkBox_updateNowPlaying;
        private System.Windows.Forms.CheckBox checkBox_scrobbleRadio;
        private System.Windows.Forms.Label label_authStatus;
        private System.Windows.Forms.BindingSource settingsBindingSource;
        private System.Windows.Forms.Button button_ok;
        private System.Windows.Forms.Button button_cancel;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.CheckBox checkBox_enableScrobbling;
        private System.Windows.Forms.NumericUpDown numericUpDown1;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button button_log;
    }
}