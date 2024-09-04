namespace Seedstarfield
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
            components = new System.ComponentModel.Container();
            button1 = new Button();
            textBox1 = new TextBox();
            timer1 = new System.Windows.Forms.Timer(components);
            button3 = new Button();
            label4 = new Label();
            Seed = new TextBox();
            randseed = new Button();
            build10 = new Button();
            ESMDropdown = new ComboBox();
            label1 = new Label();
            label2 = new Label();
            generatordropbox = new ComboBox();
            loadesm = new Button();
            SuspendLayout();
            // 
            // button1
            // 
            button1.Location = new Point(328, 198);
            button1.Name = "button1";
            button1.Size = new Size(97, 40);
            button1.TabIndex = 0;
            button1.Text = "Build";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_ClickAsync;
            // 
            // textBox1
            // 
            textBox1.Location = new Point(8, 242);
            textBox1.Multiline = true;
            textBox1.Name = "textBox1";
            textBox1.ReadOnly = true;
            textBox1.ScrollBars = ScrollBars.Vertical;
            textBox1.Size = new Size(417, 464);
            textBox1.TabIndex = 1;
            // 
            // timer1
            // 
            timer1.Enabled = true;
            timer1.Tick += timer1_Tick;
            // 
            // button3
            // 
            button3.Location = new Point(12, 213);
            button3.Name = "button3";
            button3.Size = new Size(113, 23);
            button3.TabIndex = 3;
            button3.Text = "Save Settings";
            button3.UseVisualStyleBackColor = true;
            button3.Click += button3_Click;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(12, 9);
            label4.Name = "label4";
            label4.Size = new Size(61, 15);
            label4.TabIndex = 10;
            label4.Text = "ESP Name";
            // 
            // Seed
            // 
            Seed.Location = new Point(79, 35);
            Seed.Name = "Seed";
            Seed.Size = new Size(100, 23);
            Seed.TabIndex = 13;
            Seed.Text = "1";
            // 
            // randseed
            // 
            randseed.Location = new Point(185, 35);
            randseed.Name = "randseed";
            randseed.Size = new Size(90, 23);
            randseed.TabIndex = 14;
            randseed.Text = "random seed";
            randseed.UseVisualStyleBackColor = true;
            randseed.Click += randseed_Click;
            // 
            // build10
            // 
            build10.Location = new Point(328, 140);
            build10.Name = "build10";
            build10.Size = new Size(97, 52);
            build10.TabIndex = 15;
            build10.Text = "Batch Build 10";
            build10.UseVisualStyleBackColor = true;
            build10.Click += build10_Click;
            // 
            // ESMDropdown
            // 
            ESMDropdown.FormattingEnabled = true;
            ESMDropdown.Location = new Point(79, 6);
            ESMDropdown.Name = "ESMDropdown";
            ESMDropdown.Size = new Size(196, 23);
            ESMDropdown.TabIndex = 16;
            ESMDropdown.SelectedIndexChanged += ESMDropdown_SelectedIndexChanged;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(12, 39);
            label1.Name = "label1";
            label1.Size = new Size(32, 15);
            label1.TabIndex = 17;
            label1.Text = "Seed";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(12, 70);
            label2.Name = "label2";
            label2.Size = new Size(59, 15);
            label2.TabIndex = 18;
            label2.Text = "Generator";
            // 
            // generatordropbox
            // 
            generatordropbox.FormattingEnabled = true;
            generatordropbox.Location = new Point(79, 67);
            generatordropbox.Name = "generatordropbox";
            generatordropbox.Size = new Size(196, 23);
            generatordropbox.TabIndex = 19;
            generatordropbox.SelectedIndexChanged += generatordropbox_SelectedIndexChanged;
            // 
            // loadesm
            // 
            loadesm.Location = new Point(281, 6);
            loadesm.Name = "loadesm";
            loadesm.Size = new Size(102, 23);
            loadesm.TabIndex = 20;
            loadesm.Text = "Load ESM List";
            loadesm.UseVisualStyleBackColor = true;
            loadesm.Click += loadesm_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(437, 718);
            Controls.Add(loadesm);
            Controls.Add(generatordropbox);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(ESMDropdown);
            Controls.Add(build10);
            Controls.Add(randseed);
            Controls.Add(Seed);
            Controls.Add(label4);
            Controls.Add(button3);
            Controls.Add(textBox1);
            Controls.Add(button1);
            Name = "Form1";
            Text = "StarTiller";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button button1;
        private TextBox textBox1;
        private System.Windows.Forms.Timer timer1;
        private Button button3;
        private Label label4;
        private TextBox Seed;
        private Button randseed;
        private Button build10;
        private ComboBox ESMDropdown;
        private Label label1;
        private Label label2;
        private ComboBox generatordropbox;
        public Button loadesm;
    }
}