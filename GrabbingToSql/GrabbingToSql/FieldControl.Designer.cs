namespace GrabbingToSql
{
    partial class FieldControl
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
            if (disposing && ( components != null ))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.dataGridOverview = new System.Windows.Forms.DataGridView();
            this.dataGridFillingHistory = new System.Windows.Forms.DataGridView();
            this.dataGridPeople = new System.Windows.Forms.DataGridView();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.tabPage3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridOverview)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridFillingHistory)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridPeople)).BeginInit();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(770, 487);
            this.tabControl1.TabIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.dataGridOverview);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(762, 461);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Overview";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.dataGridFillingHistory);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(762, 461);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Filing History";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.dataGridPeople);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(762, 461);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "People";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // dataGridOverview
            // 
            this.dataGridOverview.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridOverview.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridOverview.Location = new System.Drawing.Point(3, 3);
            this.dataGridOverview.Name = "dataGridOverview";
            this.dataGridOverview.Size = new System.Drawing.Size(756, 455);
            this.dataGridOverview.TabIndex = 0;
            // 
            // dataGridFillingHistory
            // 
            this.dataGridFillingHistory.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridFillingHistory.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridFillingHistory.Location = new System.Drawing.Point(3, 3);
            this.dataGridFillingHistory.Name = "dataGridFillingHistory";
            this.dataGridFillingHistory.Size = new System.Drawing.Size(756, 455);
            this.dataGridFillingHistory.TabIndex = 0;
            // 
            // dataGridPeople
            // 
            this.dataGridPeople.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridPeople.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridPeople.Location = new System.Drawing.Point(3, 3);
            this.dataGridPeople.Name = "dataGridPeople";
            this.dataGridPeople.Size = new System.Drawing.Size(756, 455);
            this.dataGridPeople.TabIndex = 0;
            // 
            // FieldControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tabControl1);
            this.Name = "FieldControl";
            this.Size = new System.Drawing.Size(770, 487);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.tabPage3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridOverview)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridFillingHistory)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridPeople)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.DataGridView dataGridOverview;
        private System.Windows.Forms.DataGridView dataGridFillingHistory;
        private System.Windows.Forms.DataGridView dataGridPeople;
    }
}
