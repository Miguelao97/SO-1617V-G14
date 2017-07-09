namespace JPG_SimpleSearchClient
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
            this.execButton = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.filesList = new System.Windows.Forms.ListBox();
            this.args = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // execButton
            // 
            this.execButton.Location = new System.Drawing.Point(12, 41);
            this.execButton.Name = "execButton";
            this.execButton.Size = new System.Drawing.Size(141, 51);
            this.execButton.TabIndex = 0;
            this.execButton.Text = "Search";
            this.execButton.UseVisualStyleBackColor = true;
            this.execButton.Click += new System.EventHandler(this.execButton_Click);
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.BackColor = System.Drawing.SystemColors.MenuBar;
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel1.Location = new System.Drawing.Point(325, 124);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(499, 324);
            this.panel1.TabIndex = 2;
            // 
            // filesList
            // 
            this.filesList.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.filesList.FormattingEnabled = true;
            this.filesList.ItemHeight = 16;
            this.filesList.Location = new System.Drawing.Point(12, 124);
            this.filesList.Name = "filesList";
            this.filesList.Size = new System.Drawing.Size(307, 324);
            this.filesList.TabIndex = 5;
            this.filesList.SelectedIndexChanged += new System.EventHandler(this.filesList_SelectedIndexChanged);
            // 
            // args
            // 
            this.args.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.args.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.args.Location = new System.Drawing.Point(6, 30);
            this.args.Name = "args";
            this.args.Size = new System.Drawing.Size(630, 30);
            this.args.TabIndex = 0;
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.args);
            this.groupBox1.Location = new System.Drawing.Point(176, 18);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(648, 100);
            this.groupBox1.TabIndex = 6;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Search expression";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(836, 470);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.execButton);
            this.Controls.Add(this.filesList);
            this.Controls.Add(this.panel1);
            this.Name = "Form1";
            this.Text = "Photo Searcher";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button execButton;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ListBox filesList;
        private System.Windows.Forms.TextBox args;
        private System.Windows.Forms.GroupBox groupBox1;
    }
}

