namespace DMR
{
    partial class AppSettings
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
            this.btnApply = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.gbCommunication = new System.Windows.Forms.GroupBox();
            this.lblPollDescription = new System.Windows.Forms.Label();
            this.numPolling = new System.Windows.Forms.NumericUpDown();
            this.lblPolling = new System.Windows.Forms.Label();
            this.lblWarning = new System.Windows.Forms.Label();
            this.btnRestart = new System.Windows.Forms.Button();
            this.gbInternet = new System.Windows.Forms.GroupBox();
            this.lblURIDescription = new System.Windows.Forms.Label();
            this.tbURI = new System.Windows.Forms.TextBox();
            this.lblURI = new System.Windows.Forms.Label();
            this.gbCommunication.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numPolling)).BeginInit();
            this.gbInternet.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnApply
            // 
            this.btnApply.BackColor = System.Drawing.SystemColors.Control;
            this.btnApply.Location = new System.Drawing.Point(26, 305);
            this.btnApply.Name = "btnApply";
            this.btnApply.Size = new System.Drawing.Size(96, 23);
            this.btnApply.TabIndex = 0;
            this.btnApply.Text = "Применить";
            this.btnApply.UseVisualStyleBackColor = false;
            this.btnApply.Click += new System.EventHandler(this.btnApply_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.BackColor = System.Drawing.SystemColors.Control;
            this.btnCancel.Location = new System.Drawing.Point(345, 305);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(96, 23);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "Отмена";
            this.btnCancel.UseVisualStyleBackColor = false;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // gbCommunication
            // 
            this.gbCommunication.Controls.Add(this.lblPollDescription);
            this.gbCommunication.Controls.Add(this.numPolling);
            this.gbCommunication.Controls.Add(this.lblPolling);
            this.gbCommunication.Location = new System.Drawing.Point(13, 13);
            this.gbCommunication.Name = "gbCommunication";
            this.gbCommunication.Size = new System.Drawing.Size(447, 119);
            this.gbCommunication.TabIndex = 2;
            this.gbCommunication.TabStop = false;
            this.gbCommunication.Text = "Соединение";
            // 
            // lblPollDescription
            // 
            this.lblPollDescription.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lblPollDescription.Location = new System.Drawing.Point(10, 56);
            this.lblPollDescription.Name = "lblPollDescription";
            this.lblPollDescription.Size = new System.Drawing.Size(364, 28);
            this.lblPollDescription.TabIndex = 5;
            this.lblPollDescription.Text = "Частота запросов поиска подключенной рации";
            // 
            // numPolling
            // 
            this.numPolling.BackColor = System.Drawing.Color.White;
            this.numPolling.Increment = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.numPolling.InterceptArrowKeys = false;
            this.numPolling.Location = new System.Drawing.Point(181, 24);
            this.numPolling.Maximum = new decimal(new int[] {
            2000,
            0,
            0,
            0});
            this.numPolling.Minimum = new decimal(new int[] {
            200,
            0,
            0,
            0});
            this.numPolling.Name = "numPolling";
            this.numPolling.ReadOnly = true;
            this.numPolling.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.numPolling.Size = new System.Drawing.Size(120, 22);
            this.numPolling.TabIndex = 4;
            this.numPolling.Value = new decimal(new int[] {
            500,
            0,
            0,
            0});
            // 
            // lblPolling
            // 
            this.lblPolling.AutoSize = true;
            this.lblPolling.Location = new System.Drawing.Point(10, 27);
            this.lblPolling.Name = "lblPolling";
            this.lblPolling.Size = new System.Drawing.Size(165, 16);
            this.lblPolling.TabIndex = 3;
            this.lblPolling.Text = "Интервал поллинга, мс: ";
            // 
            // lblWarning
            // 
            this.lblWarning.AutoSize = true;
            this.lblWarning.ForeColor = System.Drawing.Color.Red;
            this.lblWarning.Location = new System.Drawing.Point(23, 275);
            this.lblWarning.Name = "lblWarning";
            this.lblWarning.Size = new System.Drawing.Size(395, 16);
            this.lblWarning.TabIndex = 3;
            this.lblWarning.Text = "Изменения вступят в силу после перезапуска приложения";
            // 
            // btnRestart
            // 
            this.btnRestart.BackColor = System.Drawing.Color.Red;
            this.btnRestart.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnRestart.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btnRestart.ForeColor = System.Drawing.Color.White;
            this.btnRestart.Location = new System.Drawing.Point(177, 305);
            this.btnRestart.Name = "btnRestart";
            this.btnRestart.Size = new System.Drawing.Size(126, 35);
            this.btnRestart.TabIndex = 4;
            this.btnRestart.Text = "Рестарт";
            this.btnRestart.UseVisualStyleBackColor = false;
            this.btnRestart.Visible = false;
            this.btnRestart.Click += new System.EventHandler(this.btnRestart_Click);
            // 
            // gbInternet
            // 
            this.gbInternet.Controls.Add(this.lblURIDescription);
            this.gbInternet.Controls.Add(this.tbURI);
            this.gbInternet.Controls.Add(this.lblURI);
            this.gbInternet.Location = new System.Drawing.Point(14, 142);
            this.gbInternet.Name = "gbInternet";
            this.gbInternet.Size = new System.Drawing.Size(446, 115);
            this.gbInternet.TabIndex = 5;
            this.gbInternet.TabStop = false;
            this.gbInternet.Text = "Интернет";
            // 
            // lblURIDescription
            // 
            this.lblURIDescription.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lblURIDescription.Location = new System.Drawing.Point(9, 50);
            this.lblURIDescription.Name = "lblURIDescription";
            this.lblURIDescription.Size = new System.Drawing.Size(418, 46);
            this.lblURIDescription.TabIndex = 2;
            this.lblURIDescription.Text = "label1";
            // 
            // tbURI
            // 
            this.tbURI.Location = new System.Drawing.Point(163, 22);
            this.tbURI.Name = "tbURI";
            this.tbURI.Size = new System.Drawing.Size(267, 22);
            this.tbURI.TabIndex = 1;
            this.tbURI.WordWrap = false;
            // 
            // lblURI
            // 
            this.lblURI.Location = new System.Drawing.Point(9, 24);
            this.lblURI.Name = "lblURI";
            this.lblURI.Size = new System.Drawing.Size(160, 26);
            this.lblURI.TabIndex = 0;
            this.lblURI.Text = "Адрес поиска данных: ";
            // 
            // AppSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(479, 352);
            this.Controls.Add(this.gbInternet);
            this.Controls.Add(this.btnRestart);
            this.Controls.Add(this.lblWarning);
            this.Controls.Add(this.gbCommunication);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnApply);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AppSettings";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "AppSettings";
            this.Load += new System.EventHandler(this.AppSettings_Load);
            this.gbCommunication.ResumeLayout(false);
            this.gbCommunication.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numPolling)).EndInit();
            this.gbInternet.ResumeLayout(false);
            this.gbInternet.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnApply;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.GroupBox gbCommunication;
        private System.Windows.Forms.Label lblPolling;
        private System.Windows.Forms.NumericUpDown numPolling;
        private System.Windows.Forms.Label lblPollDescription;
        private System.Windows.Forms.Label lblWarning;
        private System.Windows.Forms.Button btnRestart;
        private System.Windows.Forms.GroupBox gbInternet;
        private System.Windows.Forms.Label lblURI;
        private System.Windows.Forms.TextBox tbURI;
        private System.Windows.Forms.Label lblURIDescription;
    }
}