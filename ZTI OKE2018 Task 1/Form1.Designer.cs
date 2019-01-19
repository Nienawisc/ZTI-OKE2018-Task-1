namespace ZTI_OKE2018_Task_1
{
    partial class SparQLButton
    {
        /// <summary>
        /// Wymagana zmienna projektanta.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Wyczyść wszystkie używane zasoby.
        /// </summary>
        /// <param name="disposing">prawda, jeżeli zarządzane zasoby powinny zostać zlikwidowane; Fałsz w przeciwnym wypadku.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Kod generowany przez Projektanta formularzy systemu Windows

        /// <summary>
        /// Metoda wymagana do obsługi projektanta — nie należy modyfikować
        /// jej zawartości w edytorze kodu.
        /// </summary>
        private void InitializeComponent()
        {
			this.inputTextBox = new System.Windows.Forms.RichTextBox();
			this.outputTextBox = new System.Windows.Forms.RichTextBox();
			this.button1 = new System.Windows.Forms.Button();
			this.Stanford = new System.Windows.Forms.TextBox();
			this.JarFileLocation = new System.Windows.Forms.Button();
			this.FileLocation = new System.Windows.Forms.Button();
			this.InputFile = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.SparQL = new System.Windows.Forms.PictureBox();
			this.DebugButton = new System.Windows.Forms.Button();
			((System.ComponentModel.ISupportInitialize)(this.SparQL)).BeginInit();
			this.SuspendLayout();
			// 
			// inputTextBox
			// 
			this.inputTextBox.Location = new System.Drawing.Point(11, 118);
			this.inputTextBox.Margin = new System.Windows.Forms.Padding(2);
			this.inputTextBox.Name = "inputTextBox";
			this.inputTextBox.Size = new System.Drawing.Size(355, 125);
			this.inputTextBox.TabIndex = 0;
			this.inputTextBox.Text = "";
			// 
			// outputTextBox
			// 
			this.outputTextBox.Location = new System.Drawing.Point(11, 276);
			this.outputTextBox.Margin = new System.Windows.Forms.Padding(2);
			this.outputTextBox.Name = "outputTextBox";
			this.outputTextBox.ReadOnly = true;
			this.outputTextBox.Size = new System.Drawing.Size(355, 274);
			this.outputTextBox.TabIndex = 1;
			this.outputTextBox.Text = "";
			// 
			// button1
			// 
			this.button1.Location = new System.Drawing.Point(226, 64);
			this.button1.Margin = new System.Windows.Forms.Padding(2);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(140, 21);
			this.button1.TabIndex = 2;
			this.button1.Text = "Do Action";
			this.button1.UseVisualStyleBackColor = true;
			this.button1.Click += new System.EventHandler(this.button1_Click);
			// 
			// Stanford
			// 
			this.Stanford.Location = new System.Drawing.Point(121, 13);
			this.Stanford.Name = "Stanford";
			this.Stanford.ReadOnly = true;
			this.Stanford.Size = new System.Drawing.Size(212, 20);
			this.Stanford.TabIndex = 4;
			this.Stanford.Text = "Stanford .jar location";
			// 
			// JarFileLocation
			// 
			this.JarFileLocation.Location = new System.Drawing.Point(339, 10);
			this.JarFileLocation.Name = "JarFileLocation";
			this.JarFileLocation.Size = new System.Drawing.Size(27, 23);
			this.JarFileLocation.TabIndex = 6;
			this.JarFileLocation.Text = "...";
			this.JarFileLocation.UseVisualStyleBackColor = true;
			this.JarFileLocation.Click += new System.EventHandler(this.JarFileLocation_Click);
			// 
			// FileLocation
			// 
			this.FileLocation.Location = new System.Drawing.Point(339, 36);
			this.FileLocation.Name = "FileLocation";
			this.FileLocation.Size = new System.Drawing.Size(27, 23);
			this.FileLocation.TabIndex = 8;
			this.FileLocation.Text = "...";
			this.FileLocation.UseVisualStyleBackColor = true;
			this.FileLocation.Click += new System.EventHandler(this.FileLocation_Click);
			// 
			// InputFile
			// 
			this.InputFile.Location = new System.Drawing.Point(121, 39);
			this.InputFile.Name = "InputFile";
			this.InputFile.ReadOnly = true;
			this.InputFile.Size = new System.Drawing.Size(212, 20);
			this.InputFile.TabIndex = 7;
			this.InputFile.Text = ".ttl file location";
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(8, 16);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(107, 13);
			this.label1.TabIndex = 9;
			this.label1.Text = "Stanford .jar location:";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(8, 41);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(73, 13);
			this.label2.TabIndex = 10;
			this.label2.Text = "TTL Input file:";
			// 
			// SparQL
			// 
			this.SparQL.BackgroundImage = global::ZTI_OKE2018_Task_1.Properties.Resources.SparQL1;
			this.SparQL.Location = new System.Drawing.Point(121, 65);
			this.SparQL.Name = "SparQL";
			this.SparQL.Size = new System.Drawing.Size(100, 20);
			this.SparQL.TabIndex = 11;
			this.SparQL.TabStop = false;
			this.SparQL.Click += new System.EventHandler(this.SparQL_Click);
			// 
			// DebugButton
			// 
			this.DebugButton.Location = new System.Drawing.Point(11, 65);
			this.DebugButton.Name = "DebugButton";
			this.DebugButton.Size = new System.Drawing.Size(70, 20);
			this.DebugButton.TabIndex = 12;
			this.DebugButton.Text = "Debug ↓↓↓";
			this.DebugButton.UseVisualStyleBackColor = true;
			this.DebugButton.Visible = false;
			this.DebugButton.Click += new System.EventHandler(this.DebugButton_Click);
			// 
			// SparQLButton
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(384, 561);
			this.Controls.Add(this.DebugButton);
			this.Controls.Add(this.SparQL);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.FileLocation);
			this.Controls.Add(this.InputFile);
			this.Controls.Add(this.JarFileLocation);
			this.Controls.Add(this.Stanford);
			this.Controls.Add(this.button1);
			this.Controls.Add(this.outputTextBox);
			this.Controls.Add(this.inputTextBox);
			this.Margin = new System.Windows.Forms.Padding(2);
			this.MaximumSize = new System.Drawing.Size(400, 600);
			this.MinimumSize = new System.Drawing.Size(400, 140);
			this.Name = "SparQLButton";
			this.Text = "IsSparQL";
			((System.ComponentModel.ISupportInitialize)(this.SparQL)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RichTextBox inputTextBox;
        private System.Windows.Forms.RichTextBox outputTextBox;
        private System.Windows.Forms.Button button1;
		private System.Windows.Forms.TextBox Stanford;
		private System.Windows.Forms.Button JarFileLocation;
		private System.Windows.Forms.Button FileLocation;
		private System.Windows.Forms.TextBox InputFile;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.PictureBox SparQL;
		private System.Windows.Forms.Button DebugButton;
	}
}

