using System;

namespace TRANSLATING2
{
    partial class Form1
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
            this.richTextBox2 = new System.Windows.Forms.RichTextBox();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.btnHitung = new System.Windows.Forms.Button();
            this.btnUpload = new System.Windows.Forms.Button();
            this.btnClear = new System.Windows.Forms.Button();
            this.btnExport = new System.Windows.Forms.Button();
            this.btnHelp = new System.Windows.Forms.Button();
            this.btnParsing = new System.Windows.Forms.Button();
            this.btnVizualize = new System.Windows.Forms.Button();
            this.btnSimulate = new System.Windows.Forms.Button();
            this.btnCCODE = new System.Windows.Forms.Button();
            this.btnCJSON = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // richTextBox2
            // 
            this.richTextBox2.Location = new System.Drawing.Point(609, 75);
            this.richTextBox2.Name = "richTextBox2";
            this.richTextBox2.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            this.richTextBox2.Size = new System.Drawing.Size(483, 526);
            this.richTextBox2.TabIndex = 14;
            this.richTextBox2.Text = "";
            // 
            // richTextBox1
            // 
            this.richTextBox1.Location = new System.Drawing.Point(120, 76);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            this.richTextBox1.Size = new System.Drawing.Size(483, 525);
            this.richTextBox1.TabIndex = 13;
            this.richTextBox1.Text = "";
            // 
            // btnHitung
            // 
            this.btnHitung.Location = new System.Drawing.Point(14, 304);
            this.btnHitung.Name = "btnHitung";
            this.btnHitung.Size = new System.Drawing.Size(100, 40);
            this.btnHitung.TabIndex = 12;
            this.btnHitung.Text = "Translate";
            this.btnHitung.UseVisualStyleBackColor = true;
            this.btnHitung.Click += new System.EventHandler(this.btnHitung_Click);
            // 
            // btnUpload
            // 
            this.btnUpload.Location = new System.Drawing.Point(14, 76);
            this.btnUpload.Name = "btnUpload";
            this.btnUpload.Size = new System.Drawing.Size(100, 40);
            this.btnUpload.TabIndex = 11;
            this.btnUpload.Text = "Browse";
            this.btnUpload.UseVisualStyleBackColor = true;
            this.btnUpload.Click += new System.EventHandler(this.btnUpload_Click);
            // 
            // btnClear
            // 
            this.btnClear.Location = new System.Drawing.Point(14, 561);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(100, 40);
            this.btnClear.TabIndex = 15;
            this.btnClear.Text = "Clear";
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // btnExport
            // 
            this.btnExport.Location = new System.Drawing.Point(332, 617);
            this.btnExport.Name = "btnExport";
            this.btnExport.Size = new System.Drawing.Size(100, 40);
            this.btnExport.TabIndex = 16;
            this.btnExport.Text = "Export";
            this.btnExport.UseVisualStyleBackColor = true;
            this.btnExport.Click += new System.EventHandler(this.btnExport_Click);
            // 
            // btnHelp
            // 
            this.btnHelp.Location = new System.Drawing.Point(14, 617);
            this.btnHelp.Name = "btnHelp";
            this.btnHelp.Size = new System.Drawing.Size(100, 40);
            this.btnHelp.TabIndex = 17;
            this.btnHelp.Text = "Help";
            this.btnHelp.UseVisualStyleBackColor = true;
            this.btnHelp.Click += new System.EventHandler(this.btnHelp_Click);
            // 
            // btnParsing
            // 
            this.btnParsing.Location = new System.Drawing.Point(14, 131);
            this.btnParsing.Name = "btnParsing";
            this.btnParsing.Size = new System.Drawing.Size(100, 40);
            this.btnParsing.TabIndex = 18;
            this.btnParsing.Text = "Parsing";
            this.btnParsing.UseVisualStyleBackColor = true;
            // 
            // btnVizualize
            // 
            this.btnVizualize.Location = new System.Drawing.Point(14, 246);
            this.btnVizualize.Name = "btnVizualize";
            this.btnVizualize.Size = new System.Drawing.Size(100, 40);
            this.btnVizualize.TabIndex = 19;
            this.btnVizualize.Text = "Visualize";
            this.btnVizualize.UseVisualStyleBackColor = true;
            // 
            // btnSimulate
            // 
            this.btnSimulate.Location = new System.Drawing.Point(14, 187);
            this.btnSimulate.Name = "btnSimulate";
            this.btnSimulate.Size = new System.Drawing.Size(100, 40);
            this.btnSimulate.TabIndex = 20;
            this.btnSimulate.Text = "Simulate";
            this.btnSimulate.UseVisualStyleBackColor = true;
            // 
            // btnCCODE
            // 
            this.btnCCODE.Location = new System.Drawing.Point(226, 617);
            this.btnCCODE.Name = "btnCCODE";
            this.btnCCODE.Size = new System.Drawing.Size(100, 40);
            this.btnCCODE.TabIndex = 21;
            this.btnCCODE.Text = "Copy Code";
            this.btnCCODE.UseVisualStyleBackColor = true;
            this.btnCCODE.Click += new System.EventHandler(this.btnCCODE_Click);
            // 
            // btnCJSON
            // 
            this.btnCJSON.Location = new System.Drawing.Point(120, 617);
            this.btnCJSON.Name = "btnCJSON";
            this.btnCJSON.Size = new System.Drawing.Size(100, 40);
            this.btnCJSON.TabIndex = 22;
            this.btnCJSON.Text = "Copy JSON";
            this.btnCJSON.UseVisualStyleBackColor = true;
            this.btnCJSON.Click += new System.EventHandler(this.btnCJSON_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(448, 27);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(325, 25);
            this.label1.TabIndex = 23;
            this.label1.Text = "From xtUML JSOM MODEL to C";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1114, 667);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnCJSON);
            this.Controls.Add(this.btnCCODE);
            this.Controls.Add(this.btnSimulate);
            this.Controls.Add(this.btnVizualize);
            this.Controls.Add(this.btnParsing);
            this.Controls.Add(this.btnHelp);
            this.Controls.Add(this.btnExport);
            this.Controls.Add(this.btnClear);
            this.Controls.Add(this.richTextBox2);
            this.Controls.Add(this.richTextBox1);
            this.Controls.Add(this.btnHitung);
            this.Controls.Add(this.btnUpload);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.RichTextBox richTextBox2;
        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.Button btnHitung;
        private System.Windows.Forms.Button btnUpload;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.Button btnExport;
        private System.Windows.Forms.Button btnHelp;
        private System.Windows.Forms.Button btnParsing;
        private System.Windows.Forms.Button btnVizualize;
        private System.Windows.Forms.Button btnSimulate;
        private System.Windows.Forms.Button btnCCODE;
        private System.Windows.Forms.Button btnCJSON;
        private System.Windows.Forms.Label label1;
    }
}

