namespace WDS_Dispatches {
    partial class CustomDispatchNode {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.customName = new System.Windows.Forms.TextBox();
            this.nameLabel = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.hexX = new System.Windows.Forms.NumericUpDown();
            this.hexY = new System.Windows.Forms.NumericUpDown();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.hexX)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.hexY)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // customName
            // 
            this.customName.Location = new System.Drawing.Point(224, 29);
            this.customName.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.customName.Name = "customName";
            this.customName.Size = new System.Drawing.Size(350, 31);
            this.customName.TabIndex = 1;
            // 
            // nameLabel
            // 
            this.nameLabel.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.nameLabel.Location = new System.Drawing.Point(12, 35);
            this.nameLabel.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.nameLabel.Name = "nameLabel";
            this.nameLabel.Size = new System.Drawing.Size(200, 25);
            this.nameLabel.TabIndex = 1;
            this.nameLabel.Text = "Name of recipient:";
            this.nameLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label1
            // 
            this.label1.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.label1.Location = new System.Drawing.Point(12, 94);
            this.label1.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(200, 25);
            this.label1.TabIndex = 2;
            this.label1.Text = "Location:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label2
            // 
            this.label2.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.label2.Location = new System.Drawing.Point(214, 94);
            this.label2.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(382, 25);
            this.label2.TabIndex = 3;
            this.label2.Text = "(                           ,                             )";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // hexX
            // 
            this.hexX.Location = new System.Drawing.Point(242, 89);
            this.hexX.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.hexX.Name = "hexX";
            this.hexX.Size = new System.Drawing.Size(146, 31);
            this.hexX.TabIndex = 2;
            this.hexX.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // hexY
            // 
            this.hexY.Location = new System.Drawing.Point(416, 89);
            this.hexY.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.hexY.Name = "hexY";
            this.hexY.Size = new System.Drawing.Size(146, 31);
            this.hexY.TabIndex = 3;
            this.hexY.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.customName);
            this.groupBox1.Controls.Add(this.hexY);
            this.groupBox1.Controls.Add(this.nameLabel);
            this.groupBox1.Controls.Add(this.hexX);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Location = new System.Drawing.Point(26, 6);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.groupBox1.Size = new System.Drawing.Size(612, 150);
            this.groupBox1.TabIndex = 6;
            this.groupBox1.TabStop = false;
            // 
            // btnOK
            // 
            this.btnOK.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.btnOK.Location = new System.Drawing.Point(484, 171);
            this.btnOK.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(150, 44);
            this.btnOK.TabIndex = 3;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.btnCancel.Location = new System.Drawing.Point(24, 171);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(150, 44);
            this.btnCancel.TabIndex = 7;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // CustomDispatchNode
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(661, 237);
            this.ControlBox = false;
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.groupBox1);
            this.ForeColor = System.Drawing.SystemColors.ControlDark;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CustomDispatchNode";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Custom recipient...";
            ((System.ComponentModel.ISupportInitialize)(this.hexX)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.hexY)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TextBox customName;
        private System.Windows.Forms.Label nameLabel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown hexX;
        private System.Windows.Forms.NumericUpDown hexY;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
    }
}