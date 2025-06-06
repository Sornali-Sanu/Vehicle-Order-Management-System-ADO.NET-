namespace VehicleOrderMasterDetail
{
    partial class ReportForm
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
            this.ReportViwer = new CrystalDecisions.Windows.Forms.CrystalReportViewer();
            this.SuspendLayout();
            // 
            // ReportViwer
            // 
            this.ReportViwer.ActiveViewIndex = -1;
            this.ReportViwer.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.ReportViwer.Cursor = System.Windows.Forms.Cursors.Default;
            this.ReportViwer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ReportViwer.Location = new System.Drawing.Point(0, 0);
            this.ReportViwer.Name = "ReportViwer";
            this.ReportViwer.Size = new System.Drawing.Size(800, 450);
            this.ReportViwer.TabIndex = 0;
            this.ReportViwer.Load += new System.EventHandler(this.ReportViwer_Load);
            // 
            // ReportForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.ReportViwer);
            this.Name = "ReportForm";
            this.Text = "ReportForm";
            this.ResumeLayout(false);

        }

        #endregion

        private CrystalDecisions.Windows.Forms.CrystalReportViewer ReportViwer;
    }
}