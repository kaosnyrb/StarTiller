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
            button2 = new Button();
            button3 = new Button();
            GenLength_TextBox = new TextBox();
            label1 = new Label();
            label2 = new Label();
            MinBlocks_Text = new TextBox();
            label3 = new Label();
            FormIdOffset_Text = new TextBox();
            label4 = new Label();
            espname_text = new TextBox();
            SuspendLayout();
            // 
            // button1
            // 
            button1.Location = new Point(647, 340);
            button1.Name = "button1";
            button1.Size = new Size(75, 23);
            button1.TabIndex = 0;
            button1.Text = "Build";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_ClickAsync;
            // 
            // textBox1
            // 
            textBox1.Location = new Point(1, 369);
            textBox1.Multiline = true;
            textBox1.Name = "textBox1";
            textBox1.ReadOnly = true;
            textBox1.ScrollBars = ScrollBars.Vertical;
            textBox1.Size = new Size(812, 464);
            textBox1.TabIndex = 1;
            // 
            // timer1
            // 
            timer1.Enabled = true;
            timer1.Tick += timer1_Tick;
            // 
            // button2
            // 
            button2.Location = new Point(728, 340);
            button2.Name = "button2";
            button2.Size = new Size(75, 23);
            button2.TabIndex = 2;
            button2.Text = "Spriggit";
            button2.UseVisualStyleBackColor = true;
            button2.Click += button2_Click;
            // 
            // button3
            // 
            button3.Location = new Point(12, 340);
            button3.Name = "button3";
            button3.Size = new Size(113, 23);
            button3.TabIndex = 3;
            button3.Text = "Save Settings";
            button3.UseVisualStyleBackColor = true;
            button3.Click += button3_Click;
            // 
            // GenLength_TextBox
            // 
            GenLength_TextBox.Location = new Point(112, 6);
            GenLength_TextBox.Name = "GenLength_TextBox";
            GenLength_TextBox.Size = new Size(100, 23);
            GenLength_TextBox.TabIndex = 4;
            GenLength_TextBox.TextChanged += GenLength_TextBox_TextChanged;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(12, 9);
            label1.Name = "label1";
            label1.Size = new Size(65, 15);
            label1.TabIndex = 5;
            label1.Text = "GenLength";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(12, 38);
            label2.Name = "label2";
            label2.Size = new Size(65, 15);
            label2.TabIndex = 6;
            label2.Text = "Min Blocks";
            // 
            // MinBlocks_Text
            // 
            MinBlocks_Text.Location = new Point(112, 35);
            MinBlocks_Text.Name = "MinBlocks_Text";
            MinBlocks_Text.Size = new Size(100, 23);
            MinBlocks_Text.TabIndex = 7;
            MinBlocks_Text.TextChanged += MinBlocks_Text_TextChanged;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(12, 67);
            label3.Name = "label3";
            label3.Size = new Size(77, 15);
            label3.TabIndex = 8;
            label3.Text = "FormIdOffset";
            // 
            // FormIdOffset_Text
            // 
            FormIdOffset_Text.Location = new Point(112, 64);
            FormIdOffset_Text.Name = "FormIdOffset_Text";
            FormIdOffset_Text.Size = new Size(100, 23);
            FormIdOffset_Text.TabIndex = 9;
            FormIdOffset_Text.TextChanged += FormIdOffset_Text_TextChanged;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(12, 96);
            label4.Name = "label4";
            label4.Size = new Size(61, 15);
            label4.TabIndex = 10;
            label4.Text = "ESP Name";
            // 
            // espname_text
            // 
            espname_text.Location = new Point(112, 93);
            espname_text.Name = "espname_text";
            espname_text.Size = new Size(100, 23);
            espname_text.TabIndex = 11;
            espname_text.TextChanged += espname_text_TextChanged;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(815, 845);
            Controls.Add(espname_text);
            Controls.Add(label4);
            Controls.Add(FormIdOffset_Text);
            Controls.Add(label3);
            Controls.Add(MinBlocks_Text);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(GenLength_TextBox);
            Controls.Add(button3);
            Controls.Add(button2);
            Controls.Add(textBox1);
            Controls.Add(button1);
            Name = "Form1";
            Text = "Seedstarfield";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button button1;
        private TextBox textBox1;
        private System.Windows.Forms.Timer timer1;
        private Button button2;
        private Button button3;
        private TextBox GenLength_TextBox;
        private Label label1;
        private Label label2;
        private TextBox MinBlocks_Text;
        private Label label3;
        private TextBox FormIdOffset_Text;
        private Label label4;
        private TextBox espname_text;
    }
}