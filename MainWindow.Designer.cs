using System.Windows.Forms;

namespace WDS_Dispatches
{
    partial class MainWindow
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

            if(_watcher != null) {
                _watcher.Dispose();
            }
        }

        public void SetScenarioName(string name) {
            if (labelScenarioName.InvokeRequired) {
                labelScenarioName.Invoke(
                    (MethodInvoker)(() => labelScenarioName.Text = name)
                );
            } else {
                labelScenarioName.Text = name;
            }
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainWindow));
            this.treeRecipient = new System.Windows.Forms.TreeView();
            this.messageBody = new System.Windows.Forms.RichTextBox();
            this.messageBodyContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.labelScenario = new System.Windows.Forms.Label();
            this.labelScenarioName = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.barDispatchesLeft = new System.Windows.Forms.ProgressBar();
            this.label2 = new System.Windows.Forms.Label();
            this.btnSend = new System.Windows.Forms.Button();
            this.labelMessageRecipient = new System.Windows.Forms.Label();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabRecipient = new System.Windows.Forms.TabPage();
            this.tabSender = new System.Windows.Forms.TabPage();
            this.treeSender = new System.Windows.Forms.TreeView();
            this.tabControl2 = new System.Windows.Forms.TabControl();
            this.tabSendDispatch = new System.Windows.Forms.TabPage();
            this.labelDispatchETA = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.labelMessageSender = new System.Windows.Forms.Label();
            this.labelMessageSenderFrom = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.tabMessageHistory = new System.Windows.Forms.TabPage();
            this.boxMessageHistory = new System.Windows.Forms.RichTextBox();
            this.menuMain = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyEditToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pasteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.undoLastDispatchToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.settingsToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.howToUseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.labelTurnCount = new System.Windows.Forms.Label();
            this.btnCustomRecip = new System.Windows.Forms.Button();
            this.btnCustomSender = new System.Windows.Forms.Button();
            this.tabControl1.SuspendLayout();
            this.tabRecipient.SuspendLayout();
            this.tabSender.SuspendLayout();
            this.tabControl2.SuspendLayout();
            this.tabSendDispatch.SuspendLayout();
            this.tabMessageHistory.SuspendLayout();
            this.menuMain.SuspendLayout();
            this.SuspendLayout();
            // 
            // treeRecipient
            // 
            this.treeRecipient.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.treeRecipient.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.treeRecipient.Location = new System.Drawing.Point(6, 6);
            this.treeRecipient.Name = "treeRecipient";
            this.treeRecipient.Size = new System.Drawing.Size(335, 325);
            this.treeRecipient.TabIndex = 1;
            this.treeRecipient.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeRecipient_AfterSelect);
            // 
            // messageBody
            // 
            this.messageBody.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.messageBody.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.messageBody.ContextMenuStrip = this.messageBodyContextMenu;
            this.messageBody.DetectUrls = false;
            this.messageBody.Enabled = false;
            this.messageBody.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.messageBody.Location = new System.Drawing.Point(9, 84);
            this.messageBody.Name = "messageBody";
            this.messageBody.Size = new System.Drawing.Size(487, 247);
            this.messageBody.TabIndex = 2;
            this.messageBody.Text = "";
            // 
            // messageBodyContextMenu
            // 
            this.messageBodyContextMenu.Name = "contextMenuStrip1";
            this.messageBodyContextMenu.Size = new System.Drawing.Size(61, 4);
            // 
            // labelScenario
            // 
            this.labelScenario.AutoSize = true;
            this.labelScenario.ForeColor = System.Drawing.Color.Black;
            this.labelScenario.Location = new System.Drawing.Point(189, 37);
            this.labelScenario.Name = "labelScenario";
            this.labelScenario.Size = new System.Drawing.Size(0, 13);
            this.labelScenario.TabIndex = 2;
            // 
            // labelScenarioName
            // 
            this.labelScenarioName.AutoSize = true;
            this.labelScenarioName.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelScenarioName.Location = new System.Drawing.Point(12, 37);
            this.labelScenarioName.Name = "labelScenarioName";
            this.labelScenarioName.Size = new System.Drawing.Size(124, 13);
            this.labelScenarioName.TabIndex = 3;
            this.labelScenarioName.Text = "Please load a battle.";
            this.labelScenarioName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 70);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(0, 13);
            this.label1.TabIndex = 4;
            // 
            // barDispatchesLeft
            // 
            this.barDispatchesLeft.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.barDispatchesLeft.Location = new System.Drawing.Point(381, 9);
            this.barDispatchesLeft.MarqueeAnimationSpeed = 1;
            this.barDispatchesLeft.Maximum = 5;
            this.barDispatchesLeft.Name = "barDispatchesLeft";
            this.barDispatchesLeft.Size = new System.Drawing.Size(118, 16);
            this.barDispatchesLeft.Step = 1;
            this.barDispatchesLeft.TabIndex = 5;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(6, 41);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(23, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "To:";
            // 
            // btnSend
            // 
            this.btnSend.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSend.Enabled = false;
            this.btnSend.Location = new System.Drawing.Point(424, 337);
            this.btnSend.Name = "btnSend";
            this.btnSend.Size = new System.Drawing.Size(75, 23);
            this.btnSend.TabIndex = 3;
            this.btnSend.Text = "Send";
            this.btnSend.UseVisualStyleBackColor = true;
            this.btnSend.Click += new System.EventHandler(this.btnSend_Click);
            // 
            // labelMessageRecipient
            // 
            this.labelMessageRecipient.AutoSize = true;
            this.labelMessageRecipient.Location = new System.Drawing.Point(45, 41);
            this.labelMessageRecipient.Name = "labelMessageRecipient";
            this.labelMessageRecipient.Size = new System.Drawing.Size(46, 13);
            this.labelMessageRecipient.TabIndex = 7;
            this.labelMessageRecipient.Text = "(no one)";
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.tabControl1.Controls.Add(this.tabRecipient);
            this.tabControl1.Controls.Add(this.tabSender);
            this.tabControl1.Location = new System.Drawing.Point(12, 70);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(355, 392);
            this.tabControl1.TabIndex = 8;
            // 
            // tabRecipient
            // 
            this.tabRecipient.BackColor = System.Drawing.Color.Gainsboro;
            this.tabRecipient.Controls.Add(this.btnCustomRecip);
            this.tabRecipient.Controls.Add(this.treeRecipient);
            this.tabRecipient.Location = new System.Drawing.Point(4, 22);
            this.tabRecipient.Name = "tabRecipient";
            this.tabRecipient.Padding = new System.Windows.Forms.Padding(3);
            this.tabRecipient.Size = new System.Drawing.Size(347, 366);
            this.tabRecipient.TabIndex = 0;
            this.tabRecipient.Text = "To";
            // 
            // tabSender
            // 
            this.tabSender.BackColor = System.Drawing.Color.Gainsboro;
            this.tabSender.Controls.Add(this.btnCustomSender);
            this.tabSender.Controls.Add(this.treeSender);
            this.tabSender.Location = new System.Drawing.Point(4, 22);
            this.tabSender.Name = "tabSender";
            this.tabSender.Padding = new System.Windows.Forms.Padding(3);
            this.tabSender.Size = new System.Drawing.Size(347, 366);
            this.tabSender.TabIndex = 1;
            this.tabSender.Text = "From";
            // 
            // treeSender
            // 
            this.treeSender.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.treeSender.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.treeSender.Location = new System.Drawing.Point(6, 6);
            this.treeSender.Name = "treeSender";
            this.treeSender.Size = new System.Drawing.Size(335, 325);
            this.treeSender.TabIndex = 1;
            this.treeSender.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeSender_AfterSelect);
            // 
            // tabControl2
            // 
            this.tabControl2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl2.Controls.Add(this.tabSendDispatch);
            this.tabControl2.Controls.Add(this.tabMessageHistory);
            this.tabControl2.Location = new System.Drawing.Point(373, 70);
            this.tabControl2.Name = "tabControl2";
            this.tabControl2.SelectedIndex = 0;
            this.tabControl2.Size = new System.Drawing.Size(513, 392);
            this.tabControl2.TabIndex = 9;
            // 
            // tabSendDispatch
            // 
            this.tabSendDispatch.BackColor = System.Drawing.Color.Gainsboro;
            this.tabSendDispatch.Controls.Add(this.labelDispatchETA);
            this.tabSendDispatch.Controls.Add(this.label4);
            this.tabSendDispatch.Controls.Add(this.labelMessageSender);
            this.tabSendDispatch.Controls.Add(this.labelMessageSenderFrom);
            this.tabSendDispatch.Controls.Add(this.label3);
            this.tabSendDispatch.Controls.Add(this.btnSend);
            this.tabSendDispatch.Controls.Add(this.messageBody);
            this.tabSendDispatch.Controls.Add(this.barDispatchesLeft);
            this.tabSendDispatch.Controls.Add(this.label2);
            this.tabSendDispatch.Controls.Add(this.labelMessageRecipient);
            this.tabSendDispatch.Location = new System.Drawing.Point(4, 22);
            this.tabSendDispatch.Name = "tabSendDispatch";
            this.tabSendDispatch.Padding = new System.Windows.Forms.Padding(3);
            this.tabSendDispatch.Size = new System.Drawing.Size(505, 366);
            this.tabSendDispatch.TabIndex = 0;
            this.tabSendDispatch.Text = "Send Dispatch";
            // 
            // labelDispatchETA
            // 
            this.labelDispatchETA.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.labelDispatchETA.AutoSize = true;
            this.labelDispatchETA.Location = new System.Drawing.Point(52, 342);
            this.labelDispatchETA.Name = "labelDispatchETA";
            this.labelDispatchETA.Size = new System.Drawing.Size(39, 13);
            this.labelDispatchETA.TabIndex = 13;
            this.labelDispatchETA.Text = "0 turns";
            // 
            // label4
            // 
            this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(6, 342);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(31, 13);
            this.label4.TabIndex = 12;
            this.label4.Text = "ETA:";
            // 
            // labelMessageSender
            // 
            this.labelMessageSender.AutoSize = true;
            this.labelMessageSender.Location = new System.Drawing.Point(45, 9);
            this.labelMessageSender.Name = "labelMessageSender";
            this.labelMessageSender.Size = new System.Drawing.Size(46, 13);
            this.labelMessageSender.TabIndex = 11;
            this.labelMessageSender.Text = "(no one)";
            // 
            // labelMessageSenderFrom
            // 
            this.labelMessageSenderFrom.AutoSize = true;
            this.labelMessageSenderFrom.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelMessageSenderFrom.Location = new System.Drawing.Point(6, 9);
            this.labelMessageSenderFrom.Name = "labelMessageSenderFrom";
            this.labelMessageSenderFrom.Size = new System.Drawing.Size(33, 13);
            this.labelMessageSenderFrom.TabIndex = 10;
            this.labelMessageSenderFrom.Text = "From:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(6, 68);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(53, 13);
            this.label3.TabIndex = 9;
            this.label3.Text = "Message:";
            // 
            // tabMessageHistory
            // 
            this.tabMessageHistory.BackColor = System.Drawing.Color.Gainsboro;
            this.tabMessageHistory.Controls.Add(this.boxMessageHistory);
            this.tabMessageHistory.Location = new System.Drawing.Point(4, 22);
            this.tabMessageHistory.Name = "tabMessageHistory";
            this.tabMessageHistory.Padding = new System.Windows.Forms.Padding(3);
            this.tabMessageHistory.Size = new System.Drawing.Size(505, 366);
            this.tabMessageHistory.TabIndex = 1;
            this.tabMessageHistory.Text = "Message History";
            // 
            // boxMessageHistory
            // 
            this.boxMessageHistory.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.boxMessageHistory.DetectUrls = false;
            this.boxMessageHistory.Location = new System.Drawing.Point(6, 6);
            this.boxMessageHistory.Name = "boxMessageHistory";
            this.boxMessageHistory.ReadOnly = true;
            this.boxMessageHistory.Size = new System.Drawing.Size(493, 354);
            this.boxMessageHistory.TabIndex = 0;
            this.boxMessageHistory.Text = "";
            // 
            // menuMain
            // 
            this.menuMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.editToolStripMenuItem,
            this.settingsToolStripMenuItem1,
            this.helpToolStripMenuItem});
            this.menuMain.Location = new System.Drawing.Point(0, 0);
            this.menuMain.Name = "menuMain";
            this.menuMain.Size = new System.Drawing.Size(898, 24);
            this.menuMain.TabIndex = 11;
            this.menuMain.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.loadToolStripMenuItem,
            this.toolStripSeparator2,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "&File";
            // 
            // loadToolStripMenuItem
            // 
            this.loadToolStripMenuItem.Name = "loadToolStripMenuItem";
            this.loadToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.L)));
            this.loadToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.loadToolStripMenuItem.Text = "&Load...";
            this.loadToolStripMenuItem.Click += new System.EventHandler(this.loadToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(177, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // editToolStripMenuItem
            // 
            this.editToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cutToolStripMenuItem,
            this.copyEditToolStripMenuItem,
            this.pasteToolStripMenuItem,
            this.toolStripSeparator1,
            this.undoLastDispatchToolStripMenuItem1});
            this.editToolStripMenuItem.Enabled = false;
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            this.editToolStripMenuItem.Size = new System.Drawing.Size(39, 20);
            this.editToolStripMenuItem.Text = "&Edit";
            // 
            // cutToolStripMenuItem
            // 
            this.cutToolStripMenuItem.Name = "cutToolStripMenuItem";
            this.cutToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.X)));
            this.cutToolStripMenuItem.Size = new System.Drawing.Size(217, 22);
            this.cutToolStripMenuItem.Text = "Cut";
            this.cutToolStripMenuItem.Click += new System.EventHandler(this.cutToolStripMenuItem_Click);
            // 
            // copyEditToolStripMenuItem
            // 
            this.copyEditToolStripMenuItem.Name = "copyEditToolStripMenuItem";
            this.copyEditToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
            this.copyEditToolStripMenuItem.Size = new System.Drawing.Size(217, 22);
            this.copyEditToolStripMenuItem.Text = "Copy";
            this.copyEditToolStripMenuItem.Click += new System.EventHandler(this.copyToolStripMenuItem_Click);
            // 
            // pasteToolStripMenuItem
            // 
            this.pasteToolStripMenuItem.Name = "pasteToolStripMenuItem";
            this.pasteToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.V)));
            this.pasteToolStripMenuItem.Size = new System.Drawing.Size(217, 22);
            this.pasteToolStripMenuItem.Text = "Paste";
            this.pasteToolStripMenuItem.Click += new System.EventHandler(this.pasteToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(214, 6);
            // 
            // undoLastDispatchToolStripMenuItem1
            // 
            this.undoLastDispatchToolStripMenuItem1.Name = "undoLastDispatchToolStripMenuItem1";
            this.undoLastDispatchToolStripMenuItem1.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Z)));
            this.undoLastDispatchToolStripMenuItem1.Size = new System.Drawing.Size(217, 22);
            this.undoLastDispatchToolStripMenuItem1.Text = "&Undo Last Dispatch";
            this.undoLastDispatchToolStripMenuItem1.Click += new System.EventHandler(this.undoLastDispatchToolStripMenuItem1_Click);
            // 
            // settingsToolStripMenuItem1
            // 
            this.settingsToolStripMenuItem1.Enabled = false;
            this.settingsToolStripMenuItem1.Name = "settingsToolStripMenuItem1";
            this.settingsToolStripMenuItem1.Size = new System.Drawing.Size(70, 20);
            this.settingsToolStripMenuItem1.Text = "&Settings...";
            this.settingsToolStripMenuItem1.Click += new System.EventHandler(this.settingsToolStripMenuItem1_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutToolStripMenuItem,
            this.howToUseToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.helpToolStripMenuItem.Text = "&Help";
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.aboutToolStripMenuItem.Text = "&About...";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // howToUseToolStripMenuItem
            // 
            this.howToUseToolStripMenuItem.Name = "howToUseToolStripMenuItem";
            this.howToUseToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.howToUseToolStripMenuItem.Text = "&Manual...";
            this.howToUseToolStripMenuItem.Click += new System.EventHandler(this.howToUseToolStripMenuItem_Click);
            // 
            // labelTurnCount
            // 
            this.labelTurnCount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelTurnCount.AutoSize = true;
            this.labelTurnCount.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelTurnCount.Location = new System.Drawing.Point(788, 37);
            this.labelTurnCount.Name = "labelTurnCount";
            this.labelTurnCount.Size = new System.Drawing.Size(94, 13);
            this.labelTurnCount.TabIndex = 12;
            this.labelTurnCount.Text = "(File -> Load...)";
            this.labelTurnCount.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // btnCustomRecip
            // 
            this.btnCustomRecip.Enabled = false;
            this.btnCustomRecip.Location = new System.Drawing.Point(6, 337);
            this.btnCustomRecip.Name = "btnCustomRecip";
            this.btnCustomRecip.Size = new System.Drawing.Size(335, 23);
            this.btnCustomRecip.TabIndex = 4;
            this.btnCustomRecip.Text = "Custom recipient...";
            this.btnCustomRecip.UseVisualStyleBackColor = true;
            this.btnCustomRecip.Click += new System.EventHandler(this.btnCustomRecip_Click);
            // 
            // btnCustomSender
            // 
            this.btnCustomSender.Enabled = false;
            this.btnCustomSender.Location = new System.Drawing.Point(6, 337);
            this.btnCustomSender.Name = "btnCustomSender";
            this.btnCustomSender.Size = new System.Drawing.Size(335, 23);
            this.btnCustomSender.TabIndex = 2;
            this.btnCustomSender.Text = "Custom sender...";
            this.btnCustomSender.UseVisualStyleBackColor = true;
            this.btnCustomSender.Click += new System.EventHandler(this.btnCustomSender_Click);
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(898, 473);
            this.Controls.Add(this.labelTurnCount);
            this.Controls.Add(this.tabControl2);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.labelScenarioName);
            this.Controls.Add(this.labelScenario);
            this.Controls.Add(this.menuMain);
            this.HelpButton = true;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuMain;
            this.MinimumSize = new System.Drawing.Size(850, 443);
            this.Name = "MainWindow";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Envoy";
            this.Load += new System.EventHandler(this.MainWindow_Load);
            this.tabControl1.ResumeLayout(false);
            this.tabRecipient.ResumeLayout(false);
            this.tabSender.ResumeLayout(false);
            this.tabControl2.ResumeLayout(false);
            this.tabSendDispatch.ResumeLayout(false);
            this.tabSendDispatch.PerformLayout();
            this.tabMessageHistory.ResumeLayout(false);
            this.menuMain.ResumeLayout(false);
            this.menuMain.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private void CopyToolStripMenuItem_Click(object sender, System.EventArgs e) {
            throw new System.NotImplementedException();
        }

        #endregion

        private System.Windows.Forms.TreeView treeRecipient;
        private System.Windows.Forms.RichTextBox messageBody;
        private System.Windows.Forms.Label labelScenario;
        private System.Windows.Forms.Label labelScenarioName;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ProgressBar barDispatchesLeft;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label labelMessageRecipient;
        private System.Windows.Forms.Button btnSend;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabRecipient;
        private System.Windows.Forms.TabPage tabSender;
        private System.Windows.Forms.TabControl tabControl2;
        private System.Windows.Forms.TabPage tabSendDispatch;
        private System.Windows.Forms.TabPage tabMessageHistory;
        private System.Windows.Forms.TreeView treeSender;
        private System.Windows.Forms.MenuStrip menuMain;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
        private System.Windows.Forms.Label labelMessageSender;
        private System.Windows.Forms.Label labelMessageSenderFrom;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label labelTurnCount;
        private System.Windows.Forms.ToolStripMenuItem copyEditToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem settingsToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem pasteToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem undoLastDispatchToolStripMenuItem1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem howToUseToolStripMenuItem;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label labelDispatchETA;
        private System.Windows.Forms.ToolStripMenuItem cutToolStripMenuItem;
        private System.Windows.Forms.RichTextBox boxMessageHistory;
        private ContextMenuStrip messageBodyContextMenu;
        private Button btnCustomRecip;
        private Button btnCustomSender;
    }
}

