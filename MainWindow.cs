using System;
using System.CodeDom;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Lifetime;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;

namespace WDS_Dispatches
{
    public partial class MainWindow : Form {
        private FileSystemWatcher _watcher;
        private System.Timers.Timer _fileTimer;

        string _dispatchFilename;

        private ScenarioData _scenarioData;
        private object _selectionThreadLock = new object();
        private Dictionary<string, object> _curSender;
        private Dictionary<string, object> _curRecipient;
        private List<string> _recipientChain;

        private bool        _customSenderSet;
        private string      _customSenderName;
        private Location    _customSenderLocation;
        private bool        _customRecipientSet;
        private string      _customRecipientName;
        private Location    _customRecipientLocation;

        private object _dispatchHistoryThreadLock = new object();
        private Dictionary<string, Dispatch> _dispatchHistoryDict;

        private object _dispatchStateThreadLock = new object();
        private DispatchState _dispatchState;

        public MainWindow() {
            InitializeComponent();
            _scenarioData = new ScenarioData();
            _curRecipient = null;
            _curSender = null;
            _fileTimer = null;
            _dispatchState = null;
            _recipientChain = new List<string>();

            _customSenderSet = false;
            _customRecipientSet = false;

            historyFromLabel.Enabled = false;
            historyToLabel.Enabled = false;

            _dispatchFilename = "";
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

        private void SetDispatchETA(string eta) {
            if (labelDispatchETA.InvokeRequired) {
                labelDispatchETA.Invoke(
                    (MethodInvoker)(
                        () => labelDispatchETA.Text = eta
                    )
                );
            } else {
                labelDispatchETA.Text = eta;
            }
        }

        private void SetDispatchesRemaining(int sent_dispatches, int max_dispatches) {
            int cur_dispatches = max_dispatches - sent_dispatches;
            string label = "";
            if(max_dispatches > 0) {
                label = $"{cur_dispatches} left";
            }

            if (barDispatchLabel.InvokeRequired) {
                barDispatchLabel.Invoke(
                    (MethodInvoker)(
                        () => barDispatchLabel.Text = label
                    )
                );
            } else {
                barDispatchLabel.Text = label;
            }
        }

        private void SetMessageBodyText(string text) {
            if (messageBody.InvokeRequired) {
                messageBody.Invoke((MethodInvoker)(
                    () => messageBody.Text = text
                ));
            } else {
                messageBody.Text = text;
            }
        }

        private void ClearMessageHistory() {
            if (boxMessageHistory.InvokeRequired) {
                boxMessageHistory.Invoke((MethodInvoker)(
                    () => boxMessageHistory.Clear()
                ));
            } else {
                boxMessageHistory.Clear();
            }
        }

        private void AppendMessageHistory(string text) {
            if (boxMessageHistory.InvokeRequired) {
                boxMessageHistory.Invoke(
                    (MethodInvoker)(
                        () => boxMessageHistory.Text += text
                    )
                );
            } else {
                boxMessageHistory.Text += text;
            }
        }

        private void UpdateSelection() {
            lock (_selectionThreadLock) {
                if (messageBody.InvokeRequired) {
                    messageBody.Invoke((MethodInvoker)(() => messageBody.Clear()));
                } else {
                    messageBody.Clear();
                }

                SetDispatchETA("");

                if (_dispatchState != null && _curSender != null) {
                    // make sure sender can still send dispatches
                    int dispatchesSent = _dispatchState.DispatchesSentBy(GetSender());
                    bool canSendDispatches = dispatchesSent < _dispatchState.Settings.DispatchesPerLeader;
                    if (!canSendDispatches) { // TODO max dispatches per leader
                        DisableSending();
                    }
                    SetDispatchesRemaining(
                        dispatchesSent,
                        _dispatchState.Settings.DispatchesPerLeader
                    );

                    if (_curRecipient != null) {
                        Location recipLoc = (Location)_curRecipient["location"];
                        Location senderLoc = (Location)_curSender["location"];

                        if (!recipLoc.IsPresent() || !senderLoc.IsPresent()) {
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
                            SetMessageBodyText(curDispatch.Message);
                        }

                        if (canSendDispatches && curDispatch == null) {
                            EnableSending();
                        }

                        // Get recipient chain, if any
                        if (
                            _dispatchState.Settings.UseChainOfCommand &&
                            !_customRecipientSet &&
                            !_customSenderSet
                        ) {
                            _scenarioData.BuildRecipientChain(
                                ref _recipientChain,
                                _curSender,
                                _curRecipient
                            );
                        } else {
                            _recipientChain.Clear();
                        }

                        // Estimate delivery time
                        string final_delivery_eta = "";
                        int distance = _scenarioData.FindDistance(
                            _recipientChain,
                            _curSender,
                            _curRecipient
                        );

                        if (distance >= 0) {
                            int time = _dispatchState.CalculateETA(distance);
                            string interval = "";
                            if (time != 1) {
                                interval += "s";
                            }
                            final_delivery_eta = $"{time} turn{interval}";
                        }

                        SetDispatchETA(final_delivery_eta);

                        return;
                    }
                } else {
                    SetDispatchesRemaining(0, 0);
                }

                // Otherwise we can't send
                DisableSending();
                SetMessageBodyText("");
            }
        }

        private string GetRecipient() {
            if (!_customRecipientSet) {
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
            } else {
                return _customRecipientName;
            }

            return "";
        }

        private string GetSender() {
            if (!_customSenderSet) {
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
            } else {
                return _customSenderName;
            }

            return "";
        }

        private void SelectRecipient() {
            TreeNode selected = treeRecipient.SelectedNode;
            if (selected != null) {
                _customRecipientSet = false;

                TreeNode senderSelected = treeSender.SelectedNode;
                if (senderSelected != null) {
                    // if we've selected same sender, clear sender
                    if (senderSelected.Text == selected.Text) {
                        TreeViewUtil.treeUnselect(treeSender);
                        _curSender = null;
                        labelMessageSender.Text = "(no one)";
                    }
                }

                _curRecipient = _scenarioData.GetUnitDataByNodeName(selected.Text);
                if (_curRecipient != null) {
                    Location loc = (Location)_curRecipient["location"];
                    labelMessageRecipient.Text = $"{(string)_curRecipient["message_name"]} {loc}";
                } else {
                    labelMessageRecipient.Text = selected.Text;
                }
            } else {
                if (_customRecipientSet) {
                    _curRecipient = new Dictionary<string, object> {
                        { "node_name", _customRecipientName },
                        { "location", _customRecipientLocation },
                        { "parent_node", "" }
                    };
                    labelMessageRecipient.Text = $"{_customRecipientName} {_customRecipientLocation}"; 
                } else {
                    _curRecipient = null;
                    labelMessageRecipient.Text = "(no one)";
                }
            }
            UpdateSelection();
        }

        private void SelectSender() {
            TreeNode selected = treeSender.SelectedNode;
            if (selected != null) {
                _customSenderSet = false;

                TreeNode recipSelected = treeRecipient.SelectedNode;
                if (recipSelected != null) {
                    // if we've selected same recipient, clear recipient
                    if (recipSelected.Text == selected.Text) {
                        TreeViewUtil.treeUnselect(treeRecipient);
                        _curRecipient = null;
                        labelMessageRecipient.Text = "(no one)";
                    }
                }

                _curSender = _scenarioData.GetUnitDataByNodeName(selected.Text);
                if (_curSender != null) {
                    Location loc = (Location)_curSender["location"];

                    labelMessageSender.Text = $"{(string)_curSender["message_name"]} {loc}";
                } else {
                    labelMessageSender.Text = selected.Text;
                }
            } else {
                if (_customSenderSet) {
                    _curSender = new Dictionary<string, object> {
                        { "node_name", _customSenderName },
                        { "location", _customSenderLocation },
                        { "parent_node", "" }
                    };
                    labelMessageSender.Text = $"{_customSenderName} {_customSenderLocation}";
                } else {
                    _curSender = null;
                    labelMessageSender.Text = "(no one)";
                }
            }

            UpdateSelection();
        }

        private void treeRecipient_AfterSelect(object sender, TreeViewEventArgs e) {
            if (treeRecipient.SelectedNode != null) {
                SelectRecipient();
            }
        }

        private void treeSender_AfterSelect(object sender, TreeViewEventArgs e) {
            if (treeSender.SelectedNode != null) {
                SelectSender();
            }
        }

        private void ShowSettingsWindow(bool initialSetting = false) {
            using(SettingsWindow sw = new SettingsWindow()) { 
                sw.Text = $"Settings for {_scenarioData.GetScenarioName()}";
                if(initialSetting) {
                    sw.DisableCancel();
                } else {
                    sw.EnableCancel();
                }
                sw.SetDispatchState(_dispatchState);
                if(sw.ShowDialog() == DialogResult.OK) {
                    if (_scenarioData.GetNation() != sw.SelectedArmy) {
                        _dispatchState.ChangeNation(sw.SelectedArmy);
                        _scenarioData.PopulateUI();
                        PopulateDispatchHistory();

                        TreeViewUtil.treeSelect(treeSender, treeSender.Nodes[0]);
                        TreeViewUtil.treeUnselect(treeRecipient);

                        ClearCustomRecipient();
                        ClearCustomSender();
                    }
                    SaveDispatch();
                }
            }
        }

        private void settingsToolStripMenuItem1_Click(object sender, EventArgs e) {
            ShowSettingsWindow();
        }

        private void SetTurnLabel(string text) {
            if (labelTurnCount.InvokeRequired) {
                labelTurnCount.Invoke(
                    (MethodInvoker)(
                        () => labelTurnCount.Text = text
                    )
                );
            } else {
                labelTurnCount.Text = text;
            }
        }

        private void UpdateScenarioLabels() {
            SetScenarioName(
                    _scenarioData.GetScenarioName() + ", " +
                    _scenarioData.GetScenarioDate() + ", " +
                    _scenarioData.GetScenarioTime()
                );

            SetTurnLabel($"Turn {_scenarioData.GetCurrentTurn()} of {_scenarioData.GetMaxTurns()}");
        }

        private void ClearOOBTree() {
            treeRecipient.BeginUpdate();
            treeRecipient.Nodes.Clear();
            treeRecipient.EndUpdate();
            treeSender.BeginUpdate();
            treeSender.Nodes.Clear();
            treeSender.EndUpdate();
        }

        private void ClearDispatchHistory() {
            receivedHistoryTree.BeginUpdate();
            receivedHistoryTree.Nodes.Clear();
            receivedHistoryTree.EndUpdate();
            sentHistoryTree.BeginUpdate();
            sentHistoryTree.Nodes.Clear();
            sentHistoryTree.EndUpdate();

            historyRecipientLabel.Text = "";
            historySenderLabel.Text = "";
            ClearMessageHistory();
        }

        private void ResetWindowState() {
            ClearOOBTree();
            ClearDispatchHistory();
            
            SelectRecipient();
            SelectSender();

            editToolStripMenuItem.Enabled = false;
            settingsToolStripMenuItem1.Enabled = false;
            btnCustomRecip.Enabled = false;
            btnCustomSender.Enabled = false;
            receivedHistoryTree.Enabled = false;
            sentHistoryTree.Enabled = false;
            historyFromLabel.Enabled = false;
            historyToLabel.Enabled = false;

            SetScenarioName("");
            SetTurnLabel("");
        }

        private void PopulateContextMenu() {
            messageBodyContextMenu.Items.Clear();

            List<string> unitNames = new List<string>();
            List<Dictionary<string, object>> unitdata = _dispatchState.Scenario.GetAllUnits();
            foreach (Dictionary<string, object> unit in unitdata) { 
                if (_dispatchState.Scenario.IsUnitFriendly(unit)) {
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

        private void PopulateDispatchHistory() {
            lock (_dispatchHistoryThreadLock) {
                _dispatchHistoryDict = new Dictionary<string, Dispatch>();

                // Build received history from confirmed received messages
                TreeViewUtil.treeBeginUpdate(receivedHistoryTree);
                TreeViewUtil.treeClear(receivedHistoryTree);
                foreach (int turn in _dispatchState.DispatchesReceived.Keys) {
                    List<Dispatch> turnDispatches = _dispatchState.DispatchesReceived[turn];
                    if (turnDispatches.Count > 0) {
                        string turnHeader = $"Turn {turn}";
                        TreeNode turnNode = TreeViewUtil.treeAdd(receivedHistoryTree, turnHeader);
                        foreach (Dispatch dispatch in turnDispatches) {
                            string messageId = $"{dispatch.Recipient} <- {dispatch.Sender}";
                            TreeViewUtil.treeNodeAdd(receivedHistoryTree, turnNode, messageId);
                            _dispatchHistoryDict.Add($"{turnHeader}: {messageId}", dispatch);
                        }
                    }
                }
                TreeViewUtil.treeEndUpdate(receivedHistoryTree);

                // Get sent history
                SortedDictionary<int, List<Dispatch>> sentDispatches = _dispatchState.GetDispatchesSent();
                TreeViewUtil.treeBeginUpdate(sentHistoryTree);
                TreeViewUtil.treeClear(sentHistoryTree);
                foreach (int turn in sentDispatches.Keys) {
                    List<Dispatch> turnDispatches = sentDispatches[turn];
                    if (turnDispatches.Count > 0) {
                        string turnHeader = $"Turn {turn}";
                        TreeNode turnNode = TreeViewUtil.treeAdd(sentHistoryTree, turnHeader);
                        foreach (Dispatch dispatch in turnDispatches) {
                            string messageId = $"{dispatch.Sender} -> {dispatch.Recipient}";
                            TreeViewUtil.treeNodeAdd(sentHistoryTree, turnNode, messageId);
                            _dispatchHistoryDict.Add($"{turnHeader}: {messageId}", dispatch);
                        }
                    }
                }
                TreeViewUtil.treeEndUpdate(sentHistoryTree);
            }
        }

        private void UpdateDispatchHistory(int turn, List<Dispatch> turnDispatches) {
            lock (_dispatchHistoryThreadLock) {
                TreeViewUtil.treeBeginUpdate(receivedHistoryTree);
                TreeViewUtil.treeClear(receivedHistoryTree);
                if (turnDispatches.Count > 0) {
                    string turnHeader = $"Turn {turn}";
                    TreeNode turnNode = TreeViewUtil.treeAdd(receivedHistoryTree, turnHeader);
                    foreach (Dispatch dispatch in turnDispatches) {
                        string messageId = $"{dispatch.Recipient} <- {dispatch.Sender}";
                        TreeViewUtil.treeNodeAdd(receivedHistoryTree, turnNode, messageId);
                        _dispatchHistoryDict.Add($"{turnHeader}: {messageId}", dispatch);
                    }
                }
                TreeViewUtil.treeEndUpdate(receivedHistoryTree);
            }
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e) {
            string battlePath = "";
            using (
                OpenFileDialog dialog = new OpenFileDialog()
            ) {
                dialog.InitialDirectory = @"C:\WDS";
                dialog.Filter = "Battle files (*.btl;*.bte;*.btc;*.btt)|*.btl;*.bte;*.btc;*.btt";
                dialog.FilterIndex = 0;
                dialog.RestoreDirectory = true;

                // Show the dialog and check if the user clicked OK
                if (dialog.ShowDialog() == DialogResult.OK) {
                    // Retrieve the selected folder path
                    battlePath = dialog.FileName;
                }
            }

            if (battlePath != "") {
                _dispatchState = DispatchState.Deserialize(battlePath, treeRecipient, treeSender);
                if (_dispatchState.Scenario.LoadedCorrectly()) {
                    _scenarioData = _dispatchState.Scenario;
                    ClearOOBTree();
                    ClearDispatchHistory();
                    UpdateScenarioLabels();

                    string currentNation = _scenarioData.GetNation();

                    if (_dispatchState.Settings == null) {
                        ShowSettingsWindow(true);

                        // Get a name for the Envoy save
                        bool saveFileChosen = false;
                        while (!saveFileChosen) {
                            using (
                                SaveFileDialog dialog = new SaveFileDialog()
                            ) {
                                dialog.InitialDirectory = Path.GetDirectoryName(battlePath);
                                dialog.Filter = "Envoy saves (*.dispatch)|*.dispatch";
                                dialog.FilterIndex = 0;
                                // Show the dialog and check if the user clicked OK
                                if (dialog.ShowDialog() == DialogResult.OK) {
                                    _dispatchFilename = dialog.FileName;
                                    saveFileChosen = true;
                                }
                            }
                        }

                        SaveDispatch();
                    } else if (!_dispatchState.Settings.Validate()) {
                        // old settings, need update
                        ShowSettingsWindow(true);
                    }

                    if (_scenarioData.GetNation() == currentNation) {
                        _scenarioData.PopulateUI();
                    }

                    if (_dispatchState.CurrentTurn < _scenarioData.GetCurrentTurn()) {
                        UpdateDispatchState();
                    }

                    PopulateContextMenu();

                    // Set default sender to overall commander
                    TreeViewUtil.treeSelect(treeSender, treeSender.Nodes[0]);
                    TreeViewUtil.treeUnselect(treeRecipient);
                    ClearCustomRecipient();
                    ClearCustomSender();

                    editToolStripMenuItem.Enabled = true;
                    settingsToolStripMenuItem1.Enabled = true;
                    btnCustomRecip.Enabled = true;
                    btnCustomSender.Enabled = true;
                    sentHistoryTree.Enabled = true;
                    receivedHistoryTree.Enabled = true;

                    PopulateDispatchHistory();

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
                } else {
                    ResetWindowState();

                    _customRecipientSet = false;
                    _customSenderSet = false;

                    if (_fileTimer != null) {
                        _fileTimer.Stop();
                        _fileTimer.Dispose();
                    }
                    if (_watcher != null) {
                        _watcher.Dispose();
                    }

                    _dispatchState = null;
                    _dispatchFilename = "";
                }
            }
        }

        private void loadToolStripMenuItem_Click(object sender, EventArgs e) {
            string battlePath = "";
            using (
                OpenFileDialog dialog = new OpenFileDialog()
            ) {
                dialog.InitialDirectory = @"C:\WDS";
                dialog.Filter = "Envoy saves (*.dispatch;*.dispatch_pem)|*.dispatch;*.dispatch_pem";
                dialog.FilterIndex = 0;
                dialog.RestoreDirectory = true;

                // Show the dialog and check if the user clicked OK
                if (dialog.ShowDialog() == DialogResult.OK) {
                    // Retrieve the selected folder path
                    battlePath = dialog.FileName;
                }
            }

            if(battlePath != "") {
                bool dispatchLoaded;
                try {
                    _dispatchState = DispatchState.Deserialize(battlePath, treeRecipient, treeSender);
                    dispatchLoaded = true;
                } catch {
                    dispatchLoaded = false;
                }
                
                if (dispatchLoaded && _dispatchState.Scenario.LoadedCorrectly()) {
                    _scenarioData = _dispatchState.Scenario;
                    ClearOOBTree();
                    ClearDispatchHistory();
                    UpdateScenarioLabels();

                    string currentNation = _scenarioData.GetNation();

                    if (_dispatchState.Settings == null) {
                        ShowSettingsWindow(true);
                    } else if (!_dispatchState.Settings.Validate()) {
                        // old settings, need update
                        ShowSettingsWindow(true);
                    }

                    if (_scenarioData.GetNation() == currentNation) {
                        _scenarioData.PopulateUI();
                    }

                    PopulateDispatchHistory();

                    if (_dispatchState.CurrentTurn < _scenarioData.GetCurrentTurn()) {
                        UpdateDispatchState();
                    }

                    PopulateContextMenu();

                    // Set default sender to overall commander
                    TreeViewUtil.treeSelect(treeSender, treeSender.Nodes[0]);
                    TreeViewUtil.treeUnselect(treeRecipient);
                    ClearCustomRecipient();
                    ClearCustomSender();

                    editToolStripMenuItem.Enabled = true;
                    settingsToolStripMenuItem1.Enabled = true;
                    btnCustomRecip.Enabled = true;
                    btnCustomSender.Enabled = true;
                    sentHistoryTree.Enabled = true;
                    receivedHistoryTree.Enabled = true;

                    if (_fileTimer != null) {
                        _fileTimer.Stop();
                        _fileTimer.Dispose();
                    }
                    if (_watcher != null) {
                        _watcher.Dispose();
                    }

                    string scenFile = _scenarioData.GetFilenameFullPath();
                    _watcher = new FileSystemWatcher(
                        Path.GetDirectoryName(scenFile),
                        Path.GetFileName(scenFile)
                    );
                    _watcher.NotifyFilter = NotifyFilters.LastWrite;
                    _watcher.Changed += FileChanged;
                    _watcher.EnableRaisingEvents = true;

                    _fileTimer = new System.Timers.Timer(250);
                    _fileTimer.AutoReset = true;
                    _fileTimer.Elapsed += TimerElapsed;
                    _fileTimer.Start();
                } else {
                    ResetWindowState();

                    _customRecipientSet = false;
                    _customSenderSet = false;

                    if (_fileTimer != null) {
                        _fileTimer.Stop();
                        _fileTimer.Dispose();
                    }
                    if (_watcher != null) {
                        _watcher.Dispose();
                    }

                    _dispatchState = null;
                }
            }

            _dispatchFilename = battlePath;
            SaveDispatch();
        }

        private void SaveDispatch() {
            if (_dispatchState != null && _dispatchFilename != "") {
                lock (_dispatchStateThreadLock) {
                    _dispatchState.Serialize(_dispatchFilename);
                }
            }
        }

        private void UpdateDispatchState() {
            List<Dispatch> dispatches = _dispatchState.Update();
            SortedDictionary<int, List<Dispatch>> dispatchesByTurn = new SortedDictionary<int, List<Dispatch>>();
            foreach (Dispatch d in dispatches) {
                int arrivalTurn = d.TurnSent + d.TurnsInTransit;
                if (!dispatchesByTurn.ContainsKey(arrivalTurn)) {
                    dispatchesByTurn.Add(arrivalTurn, new List<Dispatch>());
                }
                dispatchesByTurn[arrivalTurn].Add(d);
            }

            if (dispatches != null) {
                UpdateScenarioLabels();
                UpdateSelection();

                if (dispatches.Count > 0) {
                    // Show new dispatches that have been received
                    foreach (int turn in dispatchesByTurn.Keys) {
                        ReviewDispatches rd = new ReviewDispatches();
                        rd.SetDispatchDelivery(turn, dispatchesByTurn[turn]);

                        System.Threading.Thread rd_thread = new System.Threading.Thread(
                            () => { System.Windows.Forms.Application.Run(rd); }
                        );
                        rd_thread.Start();

                        UpdateDispatchHistory(turn, dispatchesByTurn[turn]);
                    }
                }
            }

            SaveDispatch();
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
                Location customOrigin = _customSenderSet ? _customSenderLocation : null;
                Location customDestination = _customRecipientSet ? _customRecipientLocation : null;

                _dispatchState.SendDispatch(
                    GetSender(),
                    GetRecipient(),
                    messageBody.Text,
                    _recipientChain,
                    false,
                    customOrigin,
                    customDestination                    
                );

                SaveDispatch();

                UpdateSelection();
                PopulateDispatchHistory();
            }
        }

        private void undoLastDispatchToolStripMenuItem1_Click(object sender, EventArgs e) {
            if (_dispatchState != null) {
                Dispatch undo = _dispatchState.UndoLastDispatch();
                
                if (undo != null) {
                    SaveDispatch();

                    UpdateSelection();
                    messageBody.Text = undo.Message;

                    PopulateDispatchHistory();
                }
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e) {
            this.Close();
        }

        private void cutToolStripMenuItem_Click(object sender, EventArgs e) {
            if (messageBody.SelectionLength > 0) {
                if (messageBody.Enabled) {
                    messageBody.Cut();
                } else {
                    messageBody.Copy();
                }
            } else if(boxMessageHistory.SelectionLength > 0) {
                boxMessageHistory.Copy();
            }
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e) {
            if (messageBody.SelectionLength > 0) {
                messageBody.Copy();
            } else if (boxMessageHistory.SelectionLength > 0) {
                boxMessageHistory.Copy();
            }
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
                System.Diagnostics.Process.Start("manual\\Envoy_Manual_v11.pdf");
            } catch(Exception) {
                // do nothing
            }
        }

        private void SetCustomRecipient(string name, Location loc) {
            _customRecipientSet = true;
            _customRecipientName = name;
            _customRecipientLocation = loc;
            
            TreeViewUtil.treeUnselect(treeRecipient);
            SelectRecipient();
        }

        private void SetCustomSender(string name, Location loc) {
            _customSenderSet = true;
            _customSenderName = name;
            _customSenderLocation = loc;

            TreeViewUtil.treeUnselect(treeSender);
            SelectSender();
        }

        private void ClearCustomRecipient() {
            _customRecipientSet = false;
            SelectRecipient();
        }

        private void ClearCustomSender() {
            _customSenderSet = false;
            SelectSender();
        }

        private void btnCustomRecip_Click(object sender, EventArgs e) {
            CustomDispatchNode cdn = new CustomDispatchNode("recipient", _scenarioData);
            if(cdn.ShowDialog() == DialogResult.OK) {
                SetCustomRecipient(cdn.CustomName, cdn.CustomLocation);
            }
        }

        private void btnCustomSender_Click(object sender, EventArgs e) {
            CustomDispatchNode cdn = new CustomDispatchNode("sender", _scenarioData);
            if (cdn.ShowDialog() == DialogResult.OK) {
                SetCustomSender(cdn.CustomName, cdn.CustomLocation);
            }
        }

        private void receivedHistoryTree_AfterSelect(object sender, TreeViewEventArgs e) {
            lock (_dispatchHistoryThreadLock) {
                if (_dispatchHistoryDict == null) { return; }

                TreeNode selectedDispatch = receivedHistoryTree.SelectedNode;
                if (selectedDispatch != null) {
                    if (selectedDispatch.Level == 1) {
                        string id = $"{selectedDispatch.Parent.Text}: {selectedDispatch.Text}";
                        if (_dispatchHistoryDict.ContainsKey(id)) {
                            Dispatch d = _dispatchHistoryDict[id];
                            ClearMessageHistory();
                            AppendMessageHistory(d.Message);

                            historyFromLabel.Enabled = true;
                            historyToLabel.Enabled = true;
                            historyRecipientLabel.Text = d.Recipient;
                            historySenderLabel.Text = d.Sender;
                        }
                    }
                }
            }
        }

        private void sentHistoryTree_AfterSelect(object sender, TreeViewEventArgs e) {
            lock (_dispatchHistoryThreadLock) {
                if (_dispatchHistoryDict == null) { return; }

                TreeNode selectedDispatch = sentHistoryTree.SelectedNode;
                if (selectedDispatch != null) {
                    if (selectedDispatch.Level == 1) {
                        string id = $"{selectedDispatch.Parent.Text}: {selectedDispatch.Text}";
                        if (_dispatchHistoryDict.ContainsKey(id)) {
                            Dispatch d = _dispatchHistoryDict[id];
                            ClearMessageHistory();
                            AppendMessageHistory(d.Message);

                            historyFromLabel.Enabled = true;
                            historyToLabel.Enabled = true;
                            historyRecipientLabel.Text = d.Recipient;
                            historySenderLabel.Text = d.Sender;
                        }
                    }
                }
            }
        }

        private void copyToolStripMenuItem_Click_1(object sender, EventArgs e) {
            boxMessageHistory.Copy();
        }
    }
}
