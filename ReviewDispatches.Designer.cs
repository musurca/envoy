namespace WDS_Dispatches {
    partial class ReviewDispatches {
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
            this.messageBody = new System.Windows.Forms.RichTextBox();
            this.labelMessageSender = new System.Windows.Forms.Label();
            this.labelMessageSenderFrom = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.labelMessageRecipient = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnNext = new System.Windows.Forms.Button();
            this.btnPrev = new System.Windows.Forms.Button();
            this.labelXofY = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // messageBody
            // 
            this.messageBody.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.messageBody.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.messageBody.Location = new System.Drawing.Point(15, 79);
            this.messageBody.Name = "messageBody";
            this.messageBody.ReadOnly = true;
            this.messageBody.Size = new System.Drawing.Size(341, 160);
            this.messageBody.TabIndex = 0;
            this.messageBody.Text = "";
            // 
            // labelMessageSender
            // 
            this.labelMessageSender.AutoSize = true;
            this.labelMessageSender.Location = new System.Drawing.Point(51, 16);
            this.labelMessageSender.Name = "labelMessageSender";
            this.labelMessageSender.Size = new System.Drawing.Size(46, 13);
            this.labelMessageSender.TabIndex = 15;
            this.labelMessageSender.Text = "(no one)";
            // 
            // labelMessageSenderFrom
            // 
            this.labelMessageSenderFrom.AutoSize = true;
            this.labelMessageSenderFrom.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelMessageSenderFrom.Location = new System.Drawing.Point(12, 16);
            this.labelMessageSenderFrom.Name = "labelMessageSenderFrom";
            this.labelMessageSenderFrom.Size = new System.Drawing.Size(33, 13);
            this.labelMessageSenderFrom.TabIndex = 14;
            this.labelMessageSenderFrom.Text = "From:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(12, 48);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(23, 13);
            this.label2.TabIndex = 12;
            this.label2.Text = "To:";
            // 
            // labelMessageRecipient
            // 
            this.labelMessageRecipient.AutoSize = true;
            this.labelMessageRecipient.Location = new System.Drawing.Point(51, 48);
            this.labelMessageRecipient.Name = "labelMessageRecipient";
            this.labelMessageRecipient.Size = new System.Drawing.Size(46, 13);
            this.labelMessageRecipient.TabIndex = 13;
            this.labelMessageRecipient.Text = "(no one)";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.messageBody);
            this.groupBox1.Controls.Add(this.labelMessageSender);
            this.groupBox1.Controls.Add(this.labelMessageSenderFrom);
            this.groupBox1.Controls.Add(this.labelMessageRecipient);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(371, 257);
            this.groupBox1.TabIndex = 16;
            this.groupBox1.TabStop = false;
            // 
            // btnNext
            // 
            this.btnNext.Location = new System.Drawing.Point(308, 275);
            this.btnNext.Name = "btnNext";
            this.btnNext.Size = new System.Drawing.Size(75, 23);
            this.btnNext.TabIndex = 17;
            this.btnNext.Text = "Next >>";
            this.btnNext.UseVisualStyleBackColor = true;
            this.btnNext.Click += new System.EventHandler(this.btnNext_Click);
            // 
            // btnPrev
            // 
            this.btnPrev.Location = new System.Drawing.Point(12, 275);
            this.btnPrev.Name = "btnPrev";
            this.btnPrev.Size = new System.Drawing.Size(75, 23);
            this.btnPrev.TabIndex = 18;
            this.btnPrev.Text = "<< Previous";
            this.btnPrev.UseVisualStyleBackColor = true;
            this.btnPrev.Click += new System.EventHandler(this.btnPrev_Click);
            // 
            // labelXofY
            // 
            this.labelXofY.AutoSize = true;
            this.labelXofY.Location = new System.Drawing.Point(183, 280);
            this.labelXofY.Name = "labelXofY";
            this.labelXofY.Size = new System.Drawing.Size(36, 13);
            this.labelXofY.TabIndex = 19;
            this.labelXofY.Text = "0 / 10";
            // 
            // ReviewDispatches
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(396, 306);
            this.Controls.Add(this.labelXofY);
            this.Controls.Add(this.btnPrev);
            this.Controls.Add(this.btnNext);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "ReviewDispatches";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Dispatches delivered on turn";
            this.Load += new System.EventHandler(this.ReviewDispatches_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RichTextBox messageBody;
        private System.Windows.Forms.Label labelMessageSender;
        private System.Windows.Forms.Label labelMessageSenderFrom;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label labelMessageRecipient;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnNext;
        private System.Windows.Forms.Button btnPrev;
        private System.Windows.Forms.Label labelXofY;
    }
}