namespace DoppleGraph
{
    partial class NodePairingGraph
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
            this.ImageGraphMethodlbl = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.SourceGraphMethodLbl = new System.Windows.Forms.Label();
            this.ScoreLbl = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // SmallGraphMethodlbl
            // 
            this.ImageGraphMethodlbl.AutoSize = true;
            this.ImageGraphMethodlbl.Location = new System.Drawing.Point(121, 21);
            this.ImageGraphMethodlbl.Name = "SmallGraphMethodlbl";
            this.ImageGraphMethodlbl.Size = new System.Drawing.Size(35, 13);
            this.ImageGraphMethodlbl.TabIndex = 1;
            this.ImageGraphMethodlbl.Text = "fsafsa";
            this.ImageGraphMethodlbl.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(175, 51);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(38, 13);
            this.label4.TabIndex = 2;
            this.label4.Text = "Score:";
            // 
            // BigGraphMethodLbl
            // 
            this.SourceGraphMethodLbl.AutoSize = true;
            this.SourceGraphMethodLbl.Location = new System.Drawing.Point(237, 21);
            this.SourceGraphMethodLbl.Name = "BigGraphMethodLbl";
            this.SourceGraphMethodLbl.Size = new System.Drawing.Size(35, 13);
            this.SourceGraphMethodLbl.TabIndex = 3;
            this.SourceGraphMethodLbl.Text = "fsafsa";
            this.SourceGraphMethodLbl.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // ScoreLbl
            // 
            this.ScoreLbl.AutoSize = true;
            this.ScoreLbl.Location = new System.Drawing.Point(237, 51);
            this.ScoreLbl.Name = "ScoreLbl";
            this.ScoreLbl.Size = new System.Drawing.Size(35, 13);
            this.ScoreLbl.TabIndex = 4;
            this.ScoreLbl.Text = "label1";
            // 
            // NodePairingGraph
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Controls.Add(this.ScoreLbl);
            this.Controls.Add(this.SourceGraphMethodLbl);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.ImageGraphMethodlbl);
            this.Name = "NodePairingGraph";
            this.Text = "NodePairingGraph";
            this.Load += new System.EventHandler(this.NodePairingGraph_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label ImageGraphMethodlbl;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label SourceGraphMethodLbl;
        private System.Windows.Forms.Label ScoreLbl;
    }
}