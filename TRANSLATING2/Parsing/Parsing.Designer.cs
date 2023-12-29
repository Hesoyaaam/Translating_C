using System.Drawing;
using System.Windows.Forms;

namespace TRANSLATING2.Parsing
{
    partial class Parsing
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
            button6 = new Button();
            button7 = new Button();
            button5 = new Button();
            txtPath = new TextBox();
            label1 = new Label();
            label2 = new Label();
            textSourceCode = new TextBox();
            msgBox = new TextBox();
            SuspendLayout();
            // 
            // button6
            // 
            button6.Location = new Point(64, 42);
            button6.Name = "button6";
            button6.Size = new Size(94, 29);
            button6.TabIndex = 6;
            button6.Text = "Browse Folder";
            button6.UseVisualStyleBackColor = true;
            button6.Click += btnBrowseFolder_Click;
            // 
            // button7
            // 
            button7.Location = new Point(680, 41);
            button7.Name = "button7";
            button7.Size = new Size(94, 29);
            button7.TabIndex = 7;
            button7.Text = "Check";
            button7.UseVisualStyleBackColor = true;
            button7.Click += btnCheck_Click;
            // 
            // button5
            // 
            button5.Location = new Point(875, 42);
            button5.Name = "button5";
            button5.Size = new Size(94, 29);
            button5.TabIndex = 15;
            button5.Text = "Help";
            button5.UseVisualStyleBackColor = true;
            button5.Click += btnHelp_Click;
            // 
            // txtPath
            // 
            txtPath.Location = new Point(192, 42);
            txtPath.Name = "txtPath";
            txtPath.Size = new Size(482, 27);
            txtPath.TabIndex = 3;
            txtPath.TextChanged += txtPath_TextChanged;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(469, 95);
            label1.Name = "label1";
            label1.Size = new Size(91, 20);
            label1.TabIndex = 4;
            label1.Text = "Model JSON";
            label1.Click += label1_Click;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(469, 315);
            label2.Name = "label2";
            label2.Size = new Size(93, 20);
            label2.TabIndex = 5;
            label2.Text = "Hasil Parsing";
            // 
            // textSourceCode
            // 
            textSourceCode.BorderStyle = BorderStyle.FixedSingle;
            textSourceCode.Cursor = Cursors.IBeam;
            textSourceCode.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Regular, GraphicsUnit.Point);
            textSourceCode.Location = new Point(64, 120);
            textSourceCode.Multiline = true;
            textSourceCode.Name = "textSourceCode";
            textSourceCode.ReadOnly = true;
            textSourceCode.ScrollBars = ScrollBars.Vertical;
            textSourceCode.Size = new Size(896, 189);
            textSourceCode.TabIndex = 4;
            textSourceCode.TextChanged += textSourceCode_TextChanged;
            // 
            // msgBox
            // 
            msgBox.BorderStyle = BorderStyle.FixedSingle;
            msgBox.Cursor = Cursors.IBeam;
            msgBox.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Regular, GraphicsUnit.Point);
            msgBox.Location = new Point(64, 338);
            msgBox.Multiline = true;
            msgBox.Name = "msgBox";
            msgBox.ReadOnly = true;
            msgBox.ScrollBars = ScrollBars.Vertical;
            msgBox.Size = new Size(896, 189);
            msgBox.TabIndex = 5;
            msgBox.TextChanged += msgBox_TextChanged;
            // 
            // Parsing
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(990, 610);
            Controls.Add(msgBox);
            Controls.Add(textSourceCode);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(txtPath);
            Controls.Add(button5);
            Controls.Add(button7);
            Controls.Add(button6);
            Name = "Parsing";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Parsing";
            Load += Parsing_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button button6;
        private Button button7;
        private Button button5;
        private TextBox txtPath;
        private Label label1;
        private Label label2;
        private TextBox textSourceCode;
        private TextBox msgBox;
    }
}