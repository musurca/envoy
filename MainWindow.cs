using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;

namespace WDS_Dispatches
{
    public partial class MainWindow : Form {
        private FileSystemWatcher _watcher;
        private System.Timers.Timer _fileTimer;

        private ScenarioData _scenarioData;
        private Dictionary<string, object> _curSender;
        private Dictionary<string, object> _curRecipient;
        private List<string> _recipientChain;

        private DispatchState _dispatchState;

        public MainWindow() {
            InitializeComponent();
            _scenarioData = new ScenarioData();
            _curRecipient = null;
            _curSender = null;
            _fileTimer = null;
            _dispatchState = null;
            _recipientChain = new List<string>();
        }

        private void MainWindow_Load(object sender, EventArgs e) {

        }

        private void DisableSending() {
            if (messageBody.InvokeRequired) {
                messageBody.Invoke((MethodInvoker)(() => messageBody.Enabled = false));
                btnSend.Invoke((MethodInvoker)(() => btnSend.Enabled = false));
            } else {
                messageBody.Enabled = false;
                btnSend.Enabled = false;
            }
        }

        private void EnableSending() {
            if (messageBody.InvokeRequired) {
                messageBody.Invoke((MethodInvoker)(() => messageBody.Enabled = true));
                btnSend.Invoke((MethodInvoker)(() => btnSend.Enabled = true));
            } else {
                messageBody.Enabled = true;
                btnSend.Enabled = true;
            }
        }

        private void UpdateSelection() {
            if (messageBody.InvokeRequired) {
                messageBody.Invoke((MethodInvoker)(() => messageBody.Clear()));
            } else {
                messageBody.Clear();
            }

            if (_dispatchState != null && _curSender != null) {
                // make sure sender can still send dispatches
                int dispatchesSent = _dispatchState.DispatchesSentBy(GetSender());
                bool canSendDispatches = dispatchesSent < _dispatchState.Settings.DispatchesPerLeader;
                if (!canSendDispatches) { // TODO max dispatches per leader
                    DisableSending();
                }
                if (barDispatchesLeft.InvokeRequired) {
                    barDispatchesLeft.Invoke(
                        (MethodInvoker)(
                            () => barDispatchesLeft.Maximum = _dispatchState.Settings.DispatchesPerLeader
                        )
                    );
                    barDispatchesLeft.Invoke(
                        (MethodInvoker)(
                            () => barDispatchesLeft.Value = barDispatchesLeft.Maximum - dispatchesSent
                        )
                    );
                } else {
                    barDispatchesLeft.Maximum = _dispatchState.Settings.DispatchesPerLeader;
                    barDispatchesLeft.Value = barDispatchesLeft.Maximum - dispatchesSent;
                }

                if (_curRecipient != null) {
                    Location recipLoc = (Location)_curRecipient["location"];
                    Location senderLoc = (Location)_curSender["location"];

                    if(!recipLoc.IsPresent() || !senderLoc.IsPresent()) {
                        canSendDispatches = false;
                        DisableSending();
                    }

                    string senderName = GetSender();
                    string receipName = GetRecipient();

                    Dispatch curDispatch = _dispatchState.FindDispatch(
                        senderName,
                        receipName
                    );
                    if (curDispatch != null) {
                        DisableSending();
                        if (messageBody.InvokeRequired) {
                            messageBody.Invoke((MethodInvoker)(
                                () => messageBody.Text = curDispatch.Message
                            ));
                        } else {
                            messageBody.Text = curDispatch.Message;
                        }
                    }

                    if (canSendDispatches && curDispatch == null) {
                        EnableSending();
                    }

                    // Populate message history
                    if (boxMessageHistory.InvokeRequired) {
                        boxMessageHistory.Invoke((MethodInvoker)(
                            () => boxMessageHistory.Clear()
                        ));
                    } else {
                        boxMessageHistory.Clear();
                    }

                    List<Dispatch> messageHistoryList = new List<Dispatch>();
                    for (int i = _dispatchState.CurrentTurn; i >= 1; i--) {
                        List<Dispatch> dispatchesReceived = _dispatchState.GetDispatchesReceived(i);

                        foreach (Dispatch d in dispatchesReceived) {
                            if (d.Recipient == receipName) {
                                messageHistoryList.Add(d);
                            }
                        }
                        if (messageHistoryList.Count > 0) {
                            string turnHeader = "----------- TURN " + i + " -----------\n\n";
                            foreach (Dispatch d in messageHistoryList) {
                                turnHeader += "FROM: " + d.Sender + " \n\n";
                                turnHeader += d.Message + "\n\n\n";
                            }
                            if (boxMessageHistory.InvokeRequired) {
                                boxMessageHistory.Invoke(
                                    (MethodInvoker)(
                                        () => boxMessageHistory.Text += turnHeader
                                    )
                                );
                            } else {
                                boxMessageHistory.Text += turnHeader;
                            }

                            messageHistoryList.Clear();
                        }
                    }

                    // Get recipient chain, if any
                    _recipientChain.Clear();
                    if (_dispatchState.Settings.UseChainOfCommand) {
                        bool senderFound = false;
                        string recip_parent = (string)_curRecipient["parent_node"];
                        while (recip_parent != "") {
                            if (recip_parent != senderName) {
                                _recipientChain.Add(recip_parent);
                                Dictionary<string, object> parent = _scenarioData.GetUnitDataByNodeName(recip_parent);
                                recip_parent = (string)parent["parent_node"];
                            } else {
                                senderFound = true;
                                break;
                            }
                        }
                        if (senderFound) {
                            _recipientChain.Reverse();
                        } else {
                            _recipientChain.Clear();
                            // Try it the other way
                            senderFound = false;
                            string sender_parent = (string)_curSender["parent_node"];
                            while (sender_parent != "") {
                                if (sender_parent != receipName) {
                                    _recipientChain.Add(sender_parent);
                                    Dictionary<string, object> parent = _scenarioData.GetUnitDataByNodeName(sender_parent);
                                    sender_parent = (string)parent["parent_node"];
                                } else {
                                    senderFound = true;
                                    break;
                                }
                            }

                            if (!senderFound) {
                                _recipientChain.Clear();
                            }
                        }
                    }

                    // Estimate delivery time
                    string final_delivery_eta = "";

                    Location a = senderLoc;
                    if (senderLoc.IsPresent() && recipLoc.IsPresent()) {
                        Location b;
                        int distance = 0;
                        if (_recipientChain.Count > 0) {
                            foreach (string recipient in _recipientChain) {
                                Dictionary<string, object> unitInfo = _scenarioData.GetUnitDataByNodeName(recipient);
                                b = (Location)unitInfo["location"];
                                if (b.IsPresent()) {
                                    distance += a.DistanceTo(b);
                                    a = b;
                                }
                            }
                        }
                        b = (Location)_curRecipient["location"];
                        distance += a.DistanceTo(b);

                        int minDelay = _dispatchState.Settings.MinimumDispatchDelay;
                        int time = distance / _dispatchState.Settings.DispatchSpeed + 1;
                        time = time < minDelay ? minDelay : time;

                        string interval = " turn";
                        if (time != 1) {
                            interval += "s";
                        }
                        final_delivery_eta = time + interval;
                    }

                    if (labelDispatchETA.InvokeRequired) {
                        labelDispatchETA.Invoke(
                            (MethodInvoker)(
                                () => labelDispatchETA.Text = final_delivery_eta
                            )
                        );
                    } else {
                        labelDispatchETA.Text = final_delivery_eta;
                    }

                    return;
                }
            }

            // Otherwise we can't send
            DisableSending();
            if (messageBody.InvokeRequired) {
                messageBody.Invoke((MethodInvoker)(
                    () => messageBody.Text = string.Empty
                ));
            } else {
                messageBody.Text = string.Empty;
            }
        }

        private string GetRecipient() {
            TreeNode selected = null;
            if (treeRecipient.InvokeRequired) {
                selected = (TreeNode)treeRecipient.Invoke(
                    new Func<Object>(() => treeRecipient.SelectedNode)
                );
            } else {
                selected = treeRecipient.SelectedNode;
            }

            if (selected != null) { 
                return selected.Text;
            }

            return "";
        }

        private string GetSender() {
            TreeNode selected = null;
            if (treeSender.InvokeRequired) {
                selected = (TreeNode)treeSender.Invoke(
                    new Func<Object>(() => treeSender.SelectedNode)
                );
            } else {
                selected = treeSender.SelectedNode;
            }

            if (selected != null) {
                return selected.Text;
            }

            return "";
        }

        private void SelectRecipient() {
            TreeNode selected = treeRecipient.SelectedNode;
            if (selected != null) {
                TreeNode senderSelected = treeSender.SelectedNode;
                if (senderSelected != null) {
                    // if we've selected same sender, clear sender
                    if (senderSelected.Text == selected.Text) {
                        _curSender = null;
                        treeSender.SelectedNode = null;
                        labelMessageSender.Text = "(no one)";
                    }
                }

                _curRecipient = _scenarioData.GetUnitDataByNodeName(selected.Text);
                Location loc = (Location)_curRecipient["location"];

                labelMessageRecipient.Text = (string)_curRecipient["message_name"] + " " + loc.ToString();

                UpdateSelection();
            } else {
                _curRecipient = null;
                labelMessageRecipient.Text = "(no one)";
            }
        }

        private void SelectSender() {
            TreeNode selected = treeSender.SelectedNode;
            if (selected != null) {
                TreeNode recipSelected = treeRecipient.SelectedNode;
                if (recipSelected != null) {
                    // if we've selected same recipient, clear recipient
                    if (recipSelected.Text == selected.Text) {
                        _curRecipient = null;
                        treeRecipient.SelectedNode = null;
                        labelMessageRecipient.Text = "(no one)";
                    }
                }

                _curSender = _scenarioData.GetUnitDataByNodeName(selected.Text);
                Location loc = (Location)_curSender["location"];

                labelMessageSender.Text = (string)_curSender["message_name"] + " " + loc.ToString();

                UpdateSelection();
            } else {
                _curSender = null;
                labelMessageSender.Text = "(no one)";
            }
        }

        private void treeRecipient_AfterSelect(object sender, TreeViewEventArgs e) {
            SelectRecipient();
        }

        private void treeSender_AfterSelect(object sender, TreeViewEventArgs e) {
            SelectSender();
        }

        private void ShowSettingsWindow(bool initialSetting = false) {
            using(SettingsWindow sw = new SettingsWindow()) { 
                sw.Text = "Settings for " + _scenarioData.GetScenarioName();
                if(initialSetting) {
                    sw.DisableCancel();
                } else {
                    sw.EnableCancel();
                }
                sw.SetDispatchState(_dispatchState);
                sw.ShowDialog();
            }
        }

        private void settingsToolStripMenuItem1_Click(object sender, EventArgs e) {
            ShowSettingsWindow();
        }

        private void UpdateScenarioLabels() {
            SetScenarioName(
                    _scenarioData.GetScenarioName() + ", " +
                    _scenarioData.GetScenarioDate() + ", " +
                    _scenarioData.GetScenarioTime()
                );

            if (labelTurnCount.InvokeRequired) {
                labelTurnCount.Invoke(
                    (MethodInvoker)(
                        () => labelTurnCount.Text = "Turn " + _scenarioData.GetCurrentTurn() + " of " + _scenarioData.GetMaxTurns()
                    )
                );
            } else {
                labelTurnCount.Text = "Turn " + _scenarioData.GetCurrentTurn() + " of " + _scenarioData.GetMaxTurns();
            }
        }

        private void PopulateContextMenu() {
            messageBodyContextMenu.Items.Clear();

            List<string> unitNames = new List<string>();
            List<Dictionary<string, object>> unitdata = _dispatchState.Scenario.GetAllUnits();
            foreach (Dictionary<string, object> unit in unitdata) {
                bool isFriendly = (bool)unit["friendly"];
                if (isFriendly) {
                    string unitname = (string)unit["node_name"];
                    string utype = (string)unit["type"];
                    if (utype == "Leader") {
                        unitNames.Add(unitname);
                    }
                }
            }

            List<string> objectives = _dispatchState.Scenario.GetObjectives();

            // March to...
            ToolStripItem[] march_objective_items = new ToolStripItem[objectives.Count()];
            for (int i = 0; i < objectives.Count(); i++) {
                march_objective_items[i] = new ToolStripMenuItem(objectives[i]);
                march_objective_items[i].Click += new System.EventHandler(this.marchToObjective_Click);
            }
            ToolStripMenuItem marchMenu = new ToolStripMenuItem(
                "March to...", 
                null, 
                march_objective_items
            );
            messageBodyContextMenu.Items.Add(marchMenu);

            // Attack...
            ToolStripItem[] base_attack_options = new ToolStripItem[2];
            ToolStripItem[] attack_options = new ToolStripItem[2];
            attack_options[0] = new ToolStripMenuItem("with an all-out assault");
            attack_options[1] = new ToolStripMenuItem("with a probe");
            attack_options[0].Click += new System.EventHandler(this.attackObjective_Click);
            attack_options[1].Click += new System.EventHandler(this.attackObjective_Click);
            base_attack_options[0] = new ToolStripMenuItem(
                "the enemy to your front", 
                null, 
                attack_options
            );
            List<string> attack_targets = new List<string>();
            for (int i = 0; i < objectives.Count(); i++) {
                attack_targets.Add(objectives[i]);
            }
            ToolStripItem[] attack_objective_items = new ToolStripItem[attack_targets.Count()];
            for(int i = 0; i < attack_targets.Count(); i++) {
                string target = attack_targets[i];
                attack_options = new ToolStripItem[2];
                attack_options[0] = new ToolStripMenuItem("with an all-out assault");
                attack_options[1] = new ToolStripMenuItem("with a probe");
                attack_options[0].Click += new System.EventHandler(this.attackObjective_Click);
                attack_options[1].Click += new System.EventHandler(this.attackObjective_Click);
                attack_objective_items[i] = new ToolStripMenuItem(target, null, attack_options);
            }
            base_attack_options[1] = new ToolStripMenuItem(
               "objectives",
               null,
               attack_objective_items
           );
            ToolStripMenuItem attackMenu = new ToolStripMenuItem(
                "Attack...", 
                null,
                base_attack_options
            );
            messageBodyContextMenu.Items.Add(attackMenu);

            // Defend...
            ToolStripItem[] defend_objective_items = new ToolStripItem[objectives.Count()];
            for (int i = 0; i < objectives.Count(); i++) {
                ToolStripItem[] defend_options = new ToolStripItem[2];
                defend_options[0] = new ToolStripMenuItem("but retreat if hard pressed");
                defend_options[1] = new ToolStripMenuItem("to the last man");
                defend_options[0].Click += new System.EventHandler(this.defendObjective_Click);
                defend_options[1].Click += new System.EventHandler(this.defendObjective_Click);
                defend_objective_items[i] = new ToolStripMenuItem(objectives[i], null, defend_options);
            }
            ToolStripMenuItem defendMenu = new ToolStripMenuItem(
                "Defend...",
                null,
                defend_objective_items
            );
            messageBodyContextMenu.Items.Add(defendMenu);

            // Support...
            ToolStripItem[] support_objective_items = new ToolStripItem[unitNames.Count()];
            for (int i = 0; i < unitNames.Count(); i++) {
                ToolStripItem[] support_options = new ToolStripItem[3];
                support_options[0] = new ToolStripMenuItem("on the left flank");
                support_options[1] = new ToolStripMenuItem("on the right flank");
                support_options[2] = new ToolStripMenuItem("as a second line");
                support_options[0].Click += new System.EventHandler(this.supportObjective_Click);
                support_options[1].Click += new System.EventHandler(this.supportObjective_Click);
                support_options[2].Click += new System.EventHandler(this.supportObjective_Click);
                support_objective_items[i] = new ToolStripMenuItem(unitNames[i], null, support_options);
            }
            ToolStripMenuItem supportMenu = new ToolStripMenuItem(
                "Support...",
                null,
                support_objective_items
            );
            messageBodyContextMenu.Items.Add(supportMenu);
        }

        private void loadToolStripMenuItem_Click(object sender, EventArgs e) {
            string battlePath = "";
            using (
                OpenFileDialog dialog = new OpenFileDialog()
            ) {
                dialog.InitialDirectory = @"C:\WDS";
                dialog.Filter = "Battle files (*.btl;*.bte)|*.btl;*.bte";
                dialog.FilterIndex = 0;
                dialog.RestoreDirectory = true;

                // Show the dialog and check if the user clicked OK
                if (dialog.ShowDialog() == DialogResult.OK) {
                    // Retrieve the selected folder path
                    battlePath = dialog.FileName;
                }
            }

            if(battlePath != "") {
                _dispatchState = DispatchState.Deserialize(battlePath, treeRecipient, treeSender);
                _scenarioData = _dispatchState.Scenario;
                if (_dispatchState.CurrentTurn < _scenarioData.GetCurrentTurn()) {
                    UpdateDispatchState();
                } else {
                    UpdateScenarioLabels();
                }

                if (_dispatchState.Settings == null) {
                    ShowSettingsWindow(true);
                } else if(!_dispatchState.Settings.Validate()) {
                    // old settings, need update
                    ShowSettingsWindow(true);
                }

                PopulateContextMenu();

                // Set default sender to overall commander
                treeSender.SelectedNode = treeSender.Nodes[0];
                treeRecipient.SelectedNode = null;
                SelectRecipient();
                SelectSender();

                editToolStripMenuItem.Enabled = true;
                settingsToolStripMenuItem1.Enabled = true;

                if (_fileTimer != null) {
                    _fileTimer.Stop();
                    _fileTimer.Dispose();
                }
                if (_watcher != null) {
                    _watcher.Dispose();
                }

                _watcher = new FileSystemWatcher(
                    Path.GetDirectoryName(battlePath), 
                    Path.GetFileName(battlePath)
                );
                _watcher.NotifyFilter = NotifyFilters.LastWrite;
                _watcher.Changed += FileChanged;
                _watcher.EnableRaisingEvents = true;

                _fileTimer = new System.Timers.Timer(250);
                _fileTimer.AutoReset = true;
                _fileTimer.Elapsed += TimerElapsed;
                _fileTimer.Start();
            }
        }

        private void UpdateDispatchState() {
            List<Dispatch> dispatches = _dispatchState.Update();

            if (dispatches != null) {
                UpdateScenarioLabels();
                UpdateSelection();

                if (dispatches.Count > 0) {
                    // Show new dispatches that have been received
                    ReviewDispatches rd = new ReviewDispatches();
                    rd.SetDispatchDelivery(_dispatchState.CurrentTurn, dispatches);

                    System.Threading.Thread rd_thread = new System.Threading.Thread(
                        () => { Application.Run(rd); }
                    );
                    rd_thread.Start();
                }
            }
        }

        private void TimerElapsed(object sender, ElapsedEventArgs e) {
            if (_scenarioData.HasUpdated()) {
                UpdateDispatchState();
            }
        }

        private void FileChanged(object sender, FileSystemEventArgs e) {
            _scenarioData.Refresh();
        }

        private void btnSend_Click(object sender, EventArgs e) {
            if(_dispatchState != null) {
                _dispatchState.SendDispatch(
                    GetSender(),
                    GetRecipient(),
                    messageBody.Text,
                    _recipientChain
                );

                UpdateSelection();
            }
        }

        private void undoLastDispatchToolStripMenuItem1_Click(object sender, EventArgs e) {
            if (_dispatchState != null) {
                Dispatch undo = _dispatchState.UndoLastDispatch();

                if (undo != null) {
                    UpdateSelection();
                    messageBody.Text = undo.Message;
                }
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e) {
            this.Close();
        }

        private void cutToolStripMenuItem_Click(object sender, EventArgs e) {
            if (messageBody.Enabled) {
                messageBody.Cut();
            } else {
                messageBody.Copy();
            }
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e) {
            messageBody.Copy();
        }

        private void pasteToolStripMenuItem_Click(object sender, EventArgs e) {
            if(messageBody.Enabled) {
                messageBody.Paste();
            }
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e) {
            using (About aboutForm = new About()) {
                aboutForm.ShowDialog();
            }
        }

        private void marchToObjective_Click(object sender, EventArgs e) {
            ToolStripMenuItem item = (ToolStripMenuItem)sender;

            messageBody.Text += "March to " + item.Text + ". ";
        }

        private void attackObjective_Click(object sender, EventArgs e) {
            ToolStripMenuItem item = (ToolStripMenuItem)sender;
            ToolStripMenuItem parent = (ToolStripMenuItem)item.OwnerItem;

            messageBody.Text += "Attack " + parent.Text + " " + item.Text + ". ";
        }

        private void defendObjective_Click(object sender, EventArgs e) {
            ToolStripMenuItem item = (ToolStripMenuItem)sender;
            ToolStripMenuItem parent = (ToolStripMenuItem)item.OwnerItem;

            messageBody.Text += "Defend " + parent.Text + " " + item.Text + ". ";
        }

        private void supportObjective_Click(object sender, EventArgs e) {
            ToolStripMenuItem item = (ToolStripMenuItem)sender;
            ToolStripMenuItem parent = (ToolStripMenuItem)item.OwnerItem;

            messageBody.Text += "Support " + parent.Text  + " " + item.Text + ". ";
        }

        private void howToUseToolStripMenuItem_Click(object sender, EventArgs e) {
            try {
                System.Diagnostics.Process.Start("Envoy_Manual_v10.pdf");
            } catch(Exception ex) {
                // do nothing
            }
        }
    }
}
