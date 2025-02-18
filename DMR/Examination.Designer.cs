namespace DMR
{
    partial class Examination
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
            this.label1 = new System.Windows.Forms.Label();
            this.actionButton = new System.Windows.Forms.Button();
            this.gbQuestions = new System.Windows.Forms.GroupBox();
            this.rbAns4 = new System.Windows.Forms.RadioButton();
            this.rbAns3 = new System.Windows.Forms.RadioButton();
            this.rbAns2 = new System.Windows.Forms.RadioButton();
            this.rbAns1 = new System.Windows.Forms.RadioButton();
            this.lblQuestion = new System.Windows.Forms.Label();
            this.gbQuestions.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label1.Location = new System.Drawing.Point(20, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(774, 42);
            this.label1.TabIndex = 0;
            this.label1.Text = "Для доступа к редактору калибровок необходимо пройти небольшой тест на знание мат" +
    "части (или умения поиска и осмысления информации).";
            // 
            // actionButton
            // 
            this.actionButton.BackColor = System.Drawing.SystemColors.Control;
            this.actionButton.Location = new System.Drawing.Point(642, 401);
            this.actionButton.Name = "actionButton";
            this.actionButton.Size = new System.Drawing.Size(152, 26);
            this.actionButton.TabIndex = 2;
            this.actionButton.Text = "Далее";
            this.actionButton.UseVisualStyleBackColor = false;
            this.actionButton.Click += new System.EventHandler(this.actionButton_Click);
            // 
            // gbQuestions
            // 
            this.gbQuestions.Controls.Add(this.rbAns4);
            this.gbQuestions.Controls.Add(this.rbAns3);
            this.gbQuestions.Controls.Add(this.rbAns2);
            this.gbQuestions.Controls.Add(this.rbAns1);
            this.gbQuestions.Controls.Add(this.lblQuestion);
            this.gbQuestions.Location = new System.Drawing.Point(23, 58);
            this.gbQuestions.Name = "gbQuestions";
            this.gbQuestions.Size = new System.Drawing.Size(771, 337);
            this.gbQuestions.TabIndex = 3;
            this.gbQuestions.TabStop = false;
            // 
            // rbAns4
            // 
            this.rbAns4.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.rbAns4.Location = new System.Drawing.Point(17, 250);
            this.rbAns4.Name = "rbAns4";
            this.rbAns4.Size = new System.Drawing.Size(748, 60);
            this.rbAns4.TabIndex = 4;
            this.rbAns4.TabStop = true;
            this.rbAns4.Text = "radioButton1";
            this.rbAns4.UseVisualStyleBackColor = true;
            // 
            // rbAns3
            // 
            this.rbAns3.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.rbAns3.Location = new System.Drawing.Point(17, 190);
            this.rbAns3.Name = "rbAns3";
            this.rbAns3.Size = new System.Drawing.Size(748, 60);
            this.rbAns3.TabIndex = 3;
            this.rbAns3.TabStop = true;
            this.rbAns3.Text = "radioButton1";
            this.rbAns3.UseVisualStyleBackColor = true;
            // 
            // rbAns2
            // 
            this.rbAns2.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.rbAns2.Location = new System.Drawing.Point(17, 130);
            this.rbAns2.Name = "rbAns2";
            this.rbAns2.Size = new System.Drawing.Size(748, 60);
            this.rbAns2.TabIndex = 2;
            this.rbAns2.TabStop = true;
            this.rbAns2.Text = "radioButton1";
            this.rbAns2.UseVisualStyleBackColor = true;
            // 
            // rbAns1
            // 
            this.rbAns1.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.rbAns1.Location = new System.Drawing.Point(17, 70);
            this.rbAns1.Name = "rbAns1";
            this.rbAns1.Size = new System.Drawing.Size(748, 60);
            this.rbAns1.TabIndex = 1;
            this.rbAns1.TabStop = true;
            this.rbAns1.Text = "radioButton1";
            this.rbAns1.UseVisualStyleBackColor = true;
            // 
            // lblQuestion
            // 
            this.lblQuestion.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lblQuestion.Location = new System.Drawing.Point(14, 18);
            this.lblQuestion.Name = "lblQuestion";
            this.lblQuestion.Size = new System.Drawing.Size(738, 68);
            this.lblQuestion.TabIndex = 0;
            // 
            // Examination
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(826, 437);
            this.Controls.Add(this.gbQuestions);
            this.Controls.Add(this.actionButton);
            this.Controls.Add(this.label1);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Examination";
            this.Text = "Проверочный тест";
            this.Load += new System.EventHandler(this.Examination_Load);
            this.gbQuestions.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button actionButton;
        private System.Windows.Forms.GroupBox gbQuestions;
        private System.Windows.Forms.RadioButton rbAns4;
        private System.Windows.Forms.RadioButton rbAns3;
        private System.Windows.Forms.RadioButton rbAns2;
        private System.Windows.Forms.RadioButton rbAns1;
        private System.Windows.Forms.Label lblQuestion;
    }
}