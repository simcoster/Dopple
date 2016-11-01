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
            this.SuspendLayout();
            // 
            // ShowProgramFlowLinks
            // 
            this.ShowProgramFlowLinks.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ShowProgramFlowLinks.AutoSize = true;
            this.ShowProgramFlowLinks.Checked = true;
            this.ShowProgramFlowLinks.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ShowProgramFlowLinks.Location = new System.Drawing.Point(193, 12);
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
            this.ShowDataFlowLinks.Location = new System.Drawing.Point(192, 35);
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
            this.ShowRightDataLinks.Location = new System.Drawing.Point(164, 58);
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
            this.ShowLeftDataLinks.Location = new System.Drawing.Point(171, 81);
            this.ShowLeftDataLinks.Name = "ShowLeftDataLinks";
            this.ShowLeftDataLinks.Size = new System.Drawing.Size(98, 17);
            this.ShowLeftDataLinks.TabIndex = 3;
            this.ShowLeftDataLinks.Text = "Left Data Links";
            this.ShowLeftDataLinks.UseVisualStyleBackColor = true;
            this.ShowLeftDataLinks.CheckedChanged += new System.EventHandler(this.ShowLeftDataLinks_CheckedChanged);
            // 
            // Form2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(276, 277);
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
    }
}