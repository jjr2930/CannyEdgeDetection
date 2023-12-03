namespace CannyEdgeDetection
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            menuStrip1 = new MenuStrip();
            fileToolStripMenuItem = new ToolStripMenuItem();
            LowThresholdUpDown = new NumericUpDown();
            LowThresholdLabel = new Label();
            HighThresholdLabel = new Label();
            HighThresholdUpDown = new NumericUpDown();
            BlurSizeLabel = new Label();
            BlurSizeUpDown = new NumericUpDown();
            BlurSigmaLabel = new Label();
            BlurSigmaUpdown = new NumericUpDown();
            ImageContainer = new Panel();
            menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)LowThresholdUpDown).BeginInit();
            ((System.ComponentModel.ISupportInitialize)HighThresholdUpDown).BeginInit();
            ((System.ComponentModel.ISupportInitialize)BlurSizeUpDown).BeginInit();
            ((System.ComponentModel.ISupportInitialize)BlurSigmaUpdown).BeginInit();
            SuspendLayout();
            // 
            // menuStrip1
            // 
            menuStrip1.ImageScalingSize = new Size(24, 24);
            menuStrip1.Items.AddRange(new ToolStripItem[] { fileToolStripMenuItem });
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Size = new Size(1002, 33);
            menuStrip1.TabIndex = 0;
            menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            fileToolStripMenuItem.Size = new Size(73, 29);
            fileToolStripMenuItem.Text = "Open";
            fileToolStripMenuItem.Click += fileToolStripMenuItem_Click;
            // 
            // LowThresholdUpDown
            // 
            LowThresholdUpDown.Location = new Point(151, 36);
            LowThresholdUpDown.Maximum = new decimal(new int[] { 255, 0, 0, 0 });
            LowThresholdUpDown.Name = "LowThresholdUpDown";
            LowThresholdUpDown.Size = new Size(180, 31);
            LowThresholdUpDown.TabIndex = 1;
            LowThresholdUpDown.Value = new decimal(new int[] { 50, 0, 0, 0 });
            LowThresholdUpDown.ValueChanged += LowThresholdUpDown_ValueChanged;
            // 
            // LowThresholdLabel
            // 
            LowThresholdLabel.AutoSize = true;
            LowThresholdLabel.Location = new Point(15, 38);
            LowThresholdLabel.Name = "LowThresholdLabel";
            LowThresholdLabel.Size = new Size(131, 25);
            LowThresholdLabel.TabIndex = 2;
            LowThresholdLabel.Text = "Low Threshold";
            // 
            // HighThresholdLabel
            // 
            HighThresholdLabel.AutoSize = true;
            HighThresholdLabel.Location = new Point(359, 40);
            HighThresholdLabel.Name = "HighThresholdLabel";
            HighThresholdLabel.Size = new Size(136, 25);
            HighThresholdLabel.TabIndex = 4;
            HighThresholdLabel.Text = "High Threshold";
            // 
            // HighThresholdUpDown
            // 
            HighThresholdUpDown.Location = new Point(495, 38);
            HighThresholdUpDown.Maximum = new decimal(new int[] { 255, 0, 0, 0 });
            HighThresholdUpDown.Name = "HighThresholdUpDown";
            HighThresholdUpDown.Size = new Size(180, 31);
            HighThresholdUpDown.TabIndex = 3;
            HighThresholdUpDown.Value = new decimal(new int[] { 150, 0, 0, 0 });
            HighThresholdUpDown.ValueChanged += HighThresholdUpDown_ValueChanged;
            // 
            // BlurSizeLabel
            // 
            BlurSizeLabel.AutoSize = true;
            BlurSizeLabel.Location = new Point(15, 70);
            BlurSizeLabel.Name = "BlurSizeLabel";
            BlurSizeLabel.Size = new Size(81, 25);
            BlurSizeLabel.TabIndex = 6;
            BlurSizeLabel.Text = "Blur Size";
            // 
            // BlurSizeUpDown
            // 
            BlurSizeUpDown.Increment = new decimal(new int[] { 2, 0, 0, 0 });
            BlurSizeUpDown.Location = new Point(151, 68);
            BlurSizeUpDown.Maximum = new decimal(new int[] { 15, 0, 0, 0 });
            BlurSizeUpDown.Minimum = new decimal(new int[] { 3, 0, 0, 0 });
            BlurSizeUpDown.Name = "BlurSizeUpDown";
            BlurSizeUpDown.Size = new Size(180, 31);
            BlurSizeUpDown.TabIndex = 5;
            BlurSizeUpDown.Value = new decimal(new int[] { 3, 0, 0, 0 });
            BlurSizeUpDown.ValueChanged += BlurSizeUpDown_ValueChanged;
            // 
            // BlurSigmaLabel
            // 
            BlurSigmaLabel.AutoSize = true;
            BlurSigmaLabel.Location = new Point(359, 74);
            BlurSigmaLabel.Name = "BlurSigmaLabel";
            BlurSigmaLabel.Size = new Size(99, 25);
            BlurSigmaLabel.TabIndex = 8;
            BlurSigmaLabel.Text = "Blur Sigma";
            // 
            // BlurSigmaUpdown
            // 
            BlurSigmaUpdown.DecimalPlaces = 3;
            BlurSigmaUpdown.Increment = new decimal(new int[] { 1, 0, 0, 65536 });
            BlurSigmaUpdown.Location = new Point(495, 72);
            BlurSigmaUpdown.Minimum = new decimal(new int[] { 1, 0, 0, 65536 });
            BlurSigmaUpdown.Name = "BlurSigmaUpdown";
            BlurSigmaUpdown.Size = new Size(180, 31);
            BlurSigmaUpdown.TabIndex = 7;
            BlurSigmaUpdown.Value = new decimal(new int[] { 3, 0, 0, 0 });
            BlurSigmaUpdown.ValueChanged += BlurSigmaUpdown_ValueChanged;
            // 
            // ImageContainer
            // 
            ImageContainer.AutoScroll = true;
            ImageContainer.Location = new Point(15, 116);
            ImageContainer.Name = "ImageContainer";
            ImageContainer.Size = new Size(975, 574);
            ImageContainer.TabIndex = 9;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1002, 712);
            Controls.Add(ImageContainer);
            Controls.Add(BlurSigmaLabel);
            Controls.Add(BlurSigmaUpdown);
            Controls.Add(BlurSizeLabel);
            Controls.Add(BlurSizeUpDown);
            Controls.Add(HighThresholdLabel);
            Controls.Add(HighThresholdUpDown);
            Controls.Add(LowThresholdLabel);
            Controls.Add(LowThresholdUpDown);
            Controls.Add(menuStrip1);
            MainMenuStrip = menuStrip1;
            Name = "Form1";
            Text = "Form1";
            Load += Form1_Load;
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)LowThresholdUpDown).EndInit();
            ((System.ComponentModel.ISupportInitialize)HighThresholdUpDown).EndInit();
            ((System.ComponentModel.ISupportInitialize)BlurSizeUpDown).EndInit();
            ((System.ComponentModel.ISupportInitialize)BlurSigmaUpdown).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private MenuStrip menuStrip1;
        private ToolStripMenuItem fileToolStripMenuItem;
        private NumericUpDown LowThresholdUpDown;
        private Label LowThresholdLabel;
        private Label HighThresholdLabel;
        private NumericUpDown HighThresholdUpDown;
        private Label BlurSizeLabel;
        private NumericUpDown BlurSizeUpDown;
        private Label BlurSigmaLabel;
        private NumericUpDown BlurSigmaUpdown;
        private Panel ImageContainer;
    }
}