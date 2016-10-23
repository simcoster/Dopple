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
            // Form2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(276, 277);
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
    }
}