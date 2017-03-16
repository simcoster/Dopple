namespace DoppleGraph
{
    partial class Form2
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
            this.ShowProgramFlowLinks = new System.Windows.Forms.CheckBox();
            this.ShowDataFlowLinks = new System.Windows.Forms.CheckBox();
            this.ShowRightDataLinks = new System.Windows.Forms.CheckBox();
            this.ShowLeftDataLinks = new System.Windows.Forms.CheckBox();
            this.minIndex = new System.Windows.Forms.TextBox();
            this.maxIndex = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.FlowAffectingChb = new System.Windows.Forms.CheckBox();
            this.freeTextTextBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.SetWidthTxt = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.SetHightTxt = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.exportToXmlBtn = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // ShowProgramFlowLinks
            // 
            this.ShowProgramFlowLinks.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ShowProgramFlowLinks.AutoSize = true;
            this.ShowProgramFlowLinks.Location = new System.Drawing.Point(871, 12);
            this.ShowProgramFlowLinks.Name = "ShowProgramFlowLinks";
            this.ShowProgramFlowLinks.Size = new System.Drawing.Size(76, 17);
            this.ShowProgramFlowLinks.TabIndex = 0;
            this.ShowProgramFlowLinks.Text = "Flow Links";
            this.ShowProgramFlowLinks.UseVisualStyleBackColor = true;
            this.ShowProgramFlowLinks.CheckedChanged += new System.EventHandler(this.ShowFlowLinks_CheckedChanged);
            // 
            // ShowDataFlowLinks
            // 
            this.ShowDataFlowLinks.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ShowDataFlowLinks.AutoSize = true;
            this.ShowDataFlowLinks.Checked = true;
            this.ShowDataFlowLinks.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ShowDataFlowLinks.Location = new System.Drawing.Point(870, 35);
            this.ShowDataFlowLinks.Name = "ShowDataFlowLinks";
            this.ShowDataFlowLinks.Size = new System.Drawing.Size(77, 17);
            this.ShowDataFlowLinks.TabIndex = 1;
            this.ShowDataFlowLinks.Text = "Data Links";
            this.ShowDataFlowLinks.UseVisualStyleBackColor = true;
            this.ShowDataFlowLinks.CheckedChanged += new System.EventHandler(this.ShowDataFlowLinks_CheckedChanged);
            // 
            // ShowRightDataLinks
            // 
            this.ShowRightDataLinks.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ShowRightDataLinks.AutoSize = true;
            this.ShowRightDataLinks.Checked = true;
            this.ShowRightDataLinks.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ShowRightDataLinks.Location = new System.Drawing.Point(842, 77);
            this.ShowRightDataLinks.Name = "ShowRightDataLinks";
            this.ShowRightDataLinks.Size = new System.Drawing.Size(105, 17);
            this.ShowRightDataLinks.TabIndex = 2;
            this.ShowRightDataLinks.Text = "Right Data Links";
            this.ShowRightDataLinks.UseVisualStyleBackColor = true;
            this.ShowRightDataLinks.CheckedChanged += new System.EventHandler(this.ShowRightDataLinks_CheckedChanged);
            // 
            // ShowLeftDataLinks
            // 
            this.ShowLeftDataLinks.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ShowLeftDataLinks.AutoSize = true;
            this.ShowLeftDataLinks.Checked = true;
            this.ShowLeftDataLinks.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ShowLeftDataLinks.Location = new System.Drawing.Point(849, 100);
            this.ShowLeftDataLinks.Name = "ShowLeftDataLinks";
            this.ShowLeftDataLinks.Size = new System.Drawing.Size(98, 17);
            this.ShowLeftDataLinks.TabIndex = 3;
            this.ShowLeftDataLinks.Text = "Left Data Links";
            this.ShowLeftDataLinks.UseVisualStyleBackColor = true;
            this.ShowLeftDataLinks.CheckedChanged += new System.EventHandler(this.ShowLeftDataLinks_CheckedChanged);
            // 
            // minIndex
            // 
            this.minIndex.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.minIndex.Location = new System.Drawing.Point(851, 123);
            this.minIndex.Name = "minIndex";
            this.minIndex.Size = new System.Drawing.Size(37, 20);
            this.minIndex.TabIndex = 4;
            this.minIndex.TextChanged += new System.EventHandler(this.minIndex_TextChanged);
            // 
            // maxIndex
            // 
            this.maxIndex.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.maxIndex.Location = new System.Drawing.Point(910, 123);
            this.maxIndex.Name = "maxIndex";
            this.maxIndex.Size = new System.Drawing.Size(37, 20);
            this.maxIndex.TabIndex = 5;
            this.maxIndex.TextChanged += new System.EventHandler(this.maxIndex_TextChanged);
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(894, 126);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(10, 13);
            this.label1.TabIndex = 6;
            this.label1.Text = "-";
            // 
            // FlowAffectingChb
            // 
            this.FlowAffectingChb.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.FlowAffectingChb.AutoSize = true;
            this.FlowAffectingChb.Checked = true;
            this.FlowAffectingChb.CheckState = System.Windows.Forms.CheckState.Checked;
            this.FlowAffectingChb.Location = new System.Drawing.Point(826, 57);
            this.FlowAffectingChb.Name = "FlowAffectingChb";
            this.FlowAffectingChb.Size = new System.Drawing.Size(121, 17);
            this.FlowAffectingChb.TabIndex = 7;
            this.FlowAffectingChb.Text = "Flow Affecting Links";
            this.FlowAffectingChb.UseVisualStyleBackColor = true;
            this.FlowAffectingChb.CheckedChanged += new System.EventHandler(this.ShowFlowAffectingLinks_CheckedChanged);
            // 
            // freeTextTextBox
            // 
            this.freeTextTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.freeTextTextBox.Location = new System.Drawing.Point(842, 158);
            this.freeTextTextBox.Name = "freeTextTextBox";
            this.freeTextTextBox.Size = new System.Drawing.Size(100, 20);
            this.freeTextTextBox.TabIndex = 8;
            this.freeTextTextBox.TextChanged += new System.EventHandler(this.freeTextTextBox_TextChanged);
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(771, 161);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(65, 13);
            this.label2.TabIndex = 9;
            this.label2.Text = "Search Text";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(765, 126);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(80, 13);
            this.label3.TabIndex = 10;
            this.label3.Text = "Limit to Indexes";
            // 
            // SetWidthTxt
            // 
            this.SetWidthTxt.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.SetWidthTxt.Location = new System.Drawing.Point(842, 234);
            this.SetWidthTxt.Name = "SetWidthTxt";
            this.SetWidthTxt.Size = new System.Drawing.Size(100, 20);
            this.SetWidthTxt.TabIndex = 11;
            this.SetWidthTxt.Text = "4000";
            this.SetWidthTxt.TextChanged += new System.EventHandler(this.SetWidthTxt_TextChanged);
            // 
            // label4
            // 
            this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(782, 234);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(54, 13);
            this.label4.TabIndex = 12;
            this.label4.Text = "Set Width";
            // 
            // SetHightTxt
            // 
            this.SetHightTxt.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.SetHightTxt.Location = new System.Drawing.Point(842, 280);
            this.SetHightTxt.Name = "SetHightTxt";
            this.SetHightTxt.Size = new System.Drawing.Size(100, 20);
            this.SetHightTxt.TabIndex = 13;
            this.SetHightTxt.Text = "1000";
            this.SetHightTxt.TextChanged += new System.EventHandler(this.SetHightTxt_TextChanged);
            // 
            // label5
            // 
            this.label5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(779, 280);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(57, 13);
            this.label5.TabIndex = 14;
            this.label5.Text = "Set Height";
            // 
            // label6
            // 
            this.label6.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(756, 198);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(80, 13);
            this.label6.TabIndex = 16;
            this.label6.Text = "Search Method";
            // 
            // textBox1
            // 
            this.textBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox1.Location = new System.Drawing.Point(842, 195);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(100, 20);
            this.textBox1.TabIndex = 15;
            this.textBox1.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // exportToXmlBtn
            // 
            this.exportToXmlBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.exportToXmlBtn.Location = new System.Drawing.Point(842, 320);
            this.exportToXmlBtn.Name = "exportToXmlBtn";
            this.exportToXmlBtn.Size = new System.Drawing.Size(99, 33);
            this.exportToXmlBtn.TabIndex = 17;
            this.exportToXmlBtn.Text = "export to xml";
            this.exportToXmlBtn.UseVisualStyleBackColor = true;
            this.exportToXmlBtn.Click += new System.EventHandler(this.exportToXmlBtn_Click);
            // 
            // Form2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(954, 481);
            this.Controls.Add(this.exportToXmlBtn);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.SetHightTxt);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.SetWidthTxt);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.freeTextTextBox);
            this.Controls.Add(this.FlowAffectingChb);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.maxIndex);
            this.Controls.Add(this.minIndex);
            this.Controls.Add(this.ShowLeftDataLinks);
            this.Controls.Add(this.ShowRightDataLinks);
            this.Controls.Add(this.ShowDataFlowLinks);
            this.Controls.Add(this.ShowProgramFlowLinks);
            this.Name = "Form2";
            this.Text = "Form2";
            this.Load += new System.EventHandler(this.Form2_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox ShowProgramFlowLinks;
        private System.Windows.Forms.CheckBox ShowDataFlowLinks;
        private System.Windows.Forms.CheckBox ShowRightDataLinks;
        private System.Windows.Forms.CheckBox ShowLeftDataLinks;
        private System.Windows.Forms.TextBox minIndex;
        private System.Windows.Forms.TextBox maxIndex;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox FlowAffectingChb;
        private System.Windows.Forms.TextBox freeTextTextBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox SetWidthTxt;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox SetHightTxt;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button exportToXmlBtn;
    }
}