namespace WDS_Dispatches
{
    partial class SettingsWindow
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SettingsWindow));
            this.comboPreset = new System.Windows.Forms.ComboBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.numInterdictionChance = new System.Windows.Forms.NumericUpDown();
            this.label7 = new System.Windows.Forms.Label();
            this.checkUseChainOfCommand = new System.Windows.Forms.CheckBox();
            this.numChanceDispatchLost = new System.Windows.Forms.NumericUpDown();
            this.label5 = new System.Windows.Forms.Label();
            this.numDispatchesPerLeader = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.numMinDispatchDelay = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.numDispatchSpeed = new System.Windows.Forms.NumericUpDown();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.btnSaveAsPreset = new System.Windows.Forms.Button();
            this.tipInterdiction = new System.Windows.Forms.ToolTip(this.components);
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numInterdictionChance)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numChanceDispatchLost)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numDispatchesPerLeader)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMinDispatchDelay)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numDispatchSpeed)).BeginInit();
            this.SuspendLayout();
            // 
            // comboPreset
            // 
            this.comboPreset.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboPreset.FormattingEnabled = true;
            this.comboPreset.Items.AddRange(new object[] {
            "Pre-Modern",
            "Modern"});
            this.comboPreset.Location = new System.Drawing.Point(218, 14);
            this.comboPreset.Name = "comboPreset";
            this.comboPreset.Size = new System.Drawing.Size(121, 21);
            this.comboPreset.TabIndex = 0;
            this.comboPreset.SelectedIndexChanged += new System.EventHandler(this.comboPreset_SelectedIndexChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.numInterdictionChance);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.checkUseChainOfCommand);
            this.groupBox1.Controls.Add(this.numChanceDispatchLost);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.numDispatchesPerLeader);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.numMinDispatchDelay);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.numDispatchSpeed);
            this.groupBox1.Location = new System.Drawing.Point(13, 40);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(326, 218);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            // 
            // numInterdictionChance
            // 
            this.numInterdictionChance.Location = new System.Drawing.Point(228, 180);
            this.numInterdictionChance.Name = "numInterdictionChance";
            this.numInterdictionChance.Size = new System.Drawing.Size(46, 20);
            this.numInterdictionChance.TabIndex = 13;
            this.numInterdictionChance.Value = new decimal(new int[] {
            30,
            0,
            0,
            0});
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(15, 182);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(188, 13);
            this.label7.TabIndex = 12;
            this.label7.Text = "Interdiction chance in enemy ZOC (%):";
            this.tipInterdiction.SetToolTip(this.label7, resources.GetString("label7.ToolTip"));
            // 
            // checkUseChainOfCommand
            // 
            this.checkUseChainOfCommand.AutoSize = true;
            this.checkUseChainOfCommand.Checked = true;
            this.checkUseChainOfCommand.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkUseChainOfCommand.Location = new System.Drawing.Point(33, 19);
            this.checkUseChainOfCommand.Name = "checkUseChainOfCommand";
            this.checkUseChainOfCommand.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.checkUseChainOfCommand.Size = new System.Drawing.Size(222, 17);
            this.checkUseChainOfCommand.TabIndex = 11;
            this.checkUseChainOfCommand.Text = "Messages travel along chain of command";
            this.tipInterdiction.SetToolTip(this.checkUseChainOfCommand, resources.GetString("checkUseChainOfCommand.ToolTip"));
            this.checkUseChainOfCommand.UseVisualStyleBackColor = true;
            // 
            // numChanceDispatchLost
            // 
            this.numChanceDispatchLost.Location = new System.Drawing.Point(228, 146);
            this.numChanceDispatchLost.Name = "numChanceDispatchLost";
            this.numChanceDispatchLost.Size = new System.Drawing.Size(46, 20);
            this.numChanceDispatchLost.TabIndex = 9;
            this.numChanceDispatchLost.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(44, 148);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(159, 13);
            this.label5.TabIndex = 8;
            this.label5.Text = "Chance dispatch is lost (%/turn):";
            this.tipInterdiction.SetToolTip(this.label5, "Every turn, there is a chance that the dispatch will\r\nbe lost due to accident or " +
        "enemy action. Set this to\r\n0 to disable.");
            // 
            // numDispatchesPerLeader
            // 
            this.numDispatchesPerLeader.Location = new System.Drawing.Point(228, 51);
            this.numDispatchesPerLeader.Maximum = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.numDispatchesPerLeader.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numDispatchesPerLeader.Name = "numDispatchesPerLeader";
            this.numDispatchesPerLeader.Size = new System.Drawing.Size(46, 20);
            this.numDispatchesPerLeader.TabIndex = 6;
            this.numDispatchesPerLeader.Value = new decimal(new int[] {
            3,
            0,
            0,
            0});
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(51, 53);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(152, 13);
            this.label4.TabIndex = 5;
            this.label4.Text = "Dispatches per leader (#/turn):";
            this.tipInterdiction.SetToolTip(this.label4, "The number of dispatches that a leader can send\r\nper turn.");
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 129);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(0, 13);
            this.label3.TabIndex = 4;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(49, 115);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(154, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Minimum dispatch delay (turns):";
            this.tipInterdiction.SetToolTip(this.label2, "The minimum time in turns it takes to deliver the\r\ndispatch, regardless of distan" +
        "ce and speed.");
            // 
            // numMinDispatchDelay
            // 
            this.numMinDispatchDelay.Location = new System.Drawing.Point(228, 113);
            this.numMinDispatchDelay.Maximum = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.numMinDispatchDelay.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numMinDispatchDelay.Name = "numMinDispatchDelay";
            this.numMinDispatchDelay.Size = new System.Drawing.Size(46, 20);
            this.numMinDispatchDelay.TabIndex = 2;
            this.numMinDispatchDelay.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(59, 85);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(144, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Dispatch speed (hexes/turn):";
            this.tipInterdiction.SetToolTip(this.label1, "The speed in hexes that the dispatch moves every\r\nturn, directly in a line toward" +
        "s its destination.");
            // 
            // numDispatchSpeed
            // 
            this.numDispatchSpeed.Location = new System.Drawing.Point(228, 83);
            this.numDispatchSpeed.Maximum = new decimal(new int[] {
            500,
            0,
            0,
            0});
            this.numDispatchSpeed.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numDispatchSpeed.Name = "numDispatchSpeed";
            this.numDispatchSpeed.Size = new System.Drawing.Size(46, 20);
            this.numDispatchSpeed.TabIndex = 0;
            this.numDispatchSpeed.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(264, 264);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 2;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(12, 264);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 3;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(151, 17);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(65, 13);
            this.label6.TabIndex = 4;
            this.label6.Text = "From preset:";
            // 
            // btnSaveAsPreset
            // 
            this.btnSaveAsPreset.Location = new System.Drawing.Point(12, 12);
            this.btnSaveAsPreset.Name = "btnSaveAsPreset";
            this.btnSaveAsPreset.Size = new System.Drawing.Size(113, 23);
            this.btnSaveAsPreset.TabIndex = 5;
            this.btnSaveAsPreset.Text = "Save as preset...";
            this.btnSaveAsPreset.UseVisualStyleBackColor = true;
            this.btnSaveAsPreset.Click += new System.EventHandler(this.btnSaveAsPreset_Click);
            // 
            // tipInterdiction
            // 
            this.tipInterdiction.IsBalloon = true;
            // 
            // SettingsWindow
            // 
            this.AcceptButton = this.btnSave;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(343, 292);
            this.ControlBox = false;
            this.Controls.Add(this.btnSaveAsPreset);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.comboPreset);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SettingsWindow";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Settings";
            this.TopMost = true;
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numInterdictionChance)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numChanceDispatchLost)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numDispatchesPerLeader)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMinDispatchDelay)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numDispatchSpeed)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox comboPreset;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown numMinDispatchDelay;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown numDispatchSpeed;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.NumericUpDown numDispatchesPerLeader;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.NumericUpDown numChanceDispatchLost;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button btnSaveAsPreset;
        private System.Windows.Forms.CheckBox checkUseChainOfCommand;
        private System.Windows.Forms.NumericUpDown numInterdictionChance;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.ToolTip tipInterdiction;
    }
}