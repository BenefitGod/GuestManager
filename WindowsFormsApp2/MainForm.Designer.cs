namespace WindowsFormsApp2
{
    partial class MainForm
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
            this.button2 = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.button5 = new System.Windows.Forms.Button();
            this.button6 = new System.Windows.Forms.Button();
            this.button7 = new System.Windows.Forms.Button();
            this.button8 = new System.Windows.Forms.Button();
            this.button9 = new System.Windows.Forms.Button();
            this.button10 = new System.Windows.Forms.Button();
            this.button11 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // button2
            // 
            this.button2.AccessibleName = "resident";
            this.button2.Location = new System.Drawing.Point(21, 535);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(183, 29);
            this.button2.TabIndex = 1;
            this.button2.Text = "Гости";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button1
            // 
            this.button1.AccessibleName = "room";
            this.button1.Location = new System.Drawing.Point(21, 570);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(183, 33);
            this.button1.TabIndex = 2;
            this.button1.Text = "Номера";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button3
            // 
            this.button3.AccessibleName = "booking";
            this.button3.Location = new System.Drawing.Point(21, 501);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(183, 28);
            this.button3.TabIndex = 3;
            this.button3.Text = "Бронирование";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // button4
            // 
            this.button4.AccessibleName = "staff";
            this.button4.Location = new System.Drawing.Point(473, 501);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(183, 28);
            this.button4.TabIndex = 6;
            this.button4.Text = "Персонал";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // button5
            // 
            this.button5.AccessibleName = "room_type";
            this.button5.Location = new System.Drawing.Point(473, 570);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(183, 33);
            this.button5.TabIndex = 5;
            this.button5.Text = "Типы номеров";
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Click += new System.EventHandler(this.button5_Click);
            // 
            // button6
            // 
            this.button6.AccessibleName = "service";
            this.button6.Location = new System.Drawing.Point(473, 535);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(183, 29);
            this.button6.TabIndex = 4;
            this.button6.Text = "Услуги";
            this.button6.UseVisualStyleBackColor = true;
            this.button6.Click += new System.EventHandler(this.button6_Click);
            // 
            // button7
            // 
            this.button7.AccessibleName = "payment";
            this.button7.Location = new System.Drawing.Point(940, 501);
            this.button7.Name = "button7";
            this.button7.Size = new System.Drawing.Size(183, 28);
            this.button7.TabIndex = 9;
            this.button7.Text = "Оплата";
            this.button7.UseVisualStyleBackColor = true;
            this.button7.Click += new System.EventHandler(this.button7_Click);
            // 
            // button8
            // 
            this.button8.AccessibleName = "book_service";
            this.button8.Location = new System.Drawing.Point(940, 570);
            this.button8.Name = "button8";
            this.button8.Size = new System.Drawing.Size(183, 33);
            this.button8.TabIndex = 8;
            this.button8.Text = "Бронирование Услуг";
            this.button8.UseVisualStyleBackColor = true;
            this.button8.Click += new System.EventHandler(this.button8_Click);
            // 
            // button9
            // 
            this.button9.AccessibleName = "feedback";
            this.button9.Location = new System.Drawing.Point(940, 535);
            this.button9.Name = "button9";
            this.button9.Size = new System.Drawing.Size(183, 29);
            this.button9.TabIndex = 7;
            this.button9.Text = "Отзывы";
            this.button9.UseVisualStyleBackColor = true;
            this.button9.Click += new System.EventHandler(this.button9_Click);
            // 
            // button10
            // 
            this.button10.AccessibleName = "status";
            this.button10.Location = new System.Drawing.Point(239, 536);
            this.button10.Name = "button10";
            this.button10.Size = new System.Drawing.Size(183, 28);
            this.button10.TabIndex = 12;
            this.button10.Text = "Статус";
            this.button10.UseVisualStyleBackColor = true;
            this.button10.Click += new System.EventHandler(this.button10_Click);
            // 
            // button11
            // 
            this.button11.AccessibleName = "post";
            this.button11.Location = new System.Drawing.Point(700, 531);
            this.button11.Name = "button11";
            this.button11.Size = new System.Drawing.Size(183, 33);
            this.button11.TabIndex = 11;
            this.button11.Text = "Должности";
            this.button11.UseVisualStyleBackColor = true;
            this.button11.Click += new System.EventHandler(this.button11_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1163, 688);
            this.Controls.Add(this.button10);
            this.Controls.Add(this.button11);
            this.Controls.Add(this.button7);
            this.Controls.Add(this.button8);
            this.Controls.Add(this.button9);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.button5);
            this.Controls.Add(this.button6);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.button2);
            this.Name = "MainForm";
            this.Text = "MainForm";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.Button button6;
        private System.Windows.Forms.Button button7;
        private System.Windows.Forms.Button button8;
        private System.Windows.Forms.Button button9;
        private System.Windows.Forms.Button button10;
        private System.Windows.Forms.Button button11;
    }
}