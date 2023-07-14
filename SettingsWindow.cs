using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WDS_Dispatches
{
    public partial class SettingsWindow : Form
    {
        public string SelectedArmy { get; set; }

        private DispatchState _ds;
        private ScenarioSettings _settings;
        private List<string> _nameToArmyId;

        public SettingsWindow()
        {
            InitializeComponent();

            SetPresetItems();
        }

        public void SetPresetItems() {
            comboPreset.Items.Clear();
            string[] preset_names = ScenarioSettings.GetPresetNames();
            comboPreset.Items.AddRange(preset_names);
        }

        private void PopulateArmyBox(List<string> armies, string currentArmy) {
            _nameToArmyId = new List<string>();

            armyBox.BeginUpdate();
            armyBox.Items.Clear();
            
            // Replace underscores with whitespace to make army ID more readable
            foreach (string army in armies) {
                _nameToArmyId.Add(army);

                armyBox.Items.Add(army.Replace('_', ' '));
            }
            
            int selectedIndex = _nameToArmyId.IndexOf(currentArmy);
            armyBox.SelectedIndex = selectedIndex;
            armyBox.EndUpdate();
        }

        public void SetDispatchState(DispatchState ds) {
            _ds = ds;

            PopulateArmyBox(
                _ds.Scenario.GetNations(), 
                _ds.Scenario.GetNation()
            );

            if(ds.CurrentTurn > 1) {
                armyBox.Enabled = false;
            } else {
                armyBox.Enabled = true;
            }
            
            if (ds.Settings != null) {
                _settings = ds.Settings;
                // update settings version no matter what
                _settings.SettingsVersion = ScenarioSettings.CURRENT_SETTINGS_VERSION;
                MatchCurrentSettings();
            } else {
                _settings = new ScenarioSettings();
                ds.Settings = _settings;
            }
        }

        public void DisableCancel() {
            btnCancel.Enabled = false;
            btnCancel.Visible = false;
        }

        public void EnableCancel() {
            btnCancel.Enabled = true;
            btnCancel.Visible = true;
        }

        private void MatchCurrentSettings() {
            if(_settings != null) {
                numDispatchSpeed.Value          = _settings.DispatchSpeed;
                numMinDispatchDelay.Value       = _settings.MinimumDispatchDelay;
                numDispatchesPerLeader.Value    = _settings.DispatchesPerLeader;
                numChanceDispatchLost.Value     = _settings.ChanceDispatchLost;
                checkUseChainOfCommand.Checked  = _settings.UseChainOfCommand;
                numInterdictionChance.Value     = _settings.InterdictionChance;
            }
        }

        private void SaveCurrentSettingsTo(ScenarioSettings settings) {
            // Save settings
            settings.SettingsVersion        = ScenarioSettings.CURRENT_SETTINGS_VERSION;

            settings.DispatchSpeed          = (int)numDispatchSpeed.Value;
            settings.MinimumDispatchDelay   = (int)numMinDispatchDelay.Value;
            settings.DispatchesPerLeader    = (int)numDispatchesPerLeader.Value;
            settings.ChanceDispatchLost     = (int)numChanceDispatchLost.Value;
            settings.UseChainOfCommand      = checkUseChainOfCommand.Checked;
            settings.InterdictionChance     = (int)numInterdictionChance.Value;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            SaveCurrentSettingsTo(_settings);
            _ds.Settings = _settings;
            _ds.Serialize();

            // Translate the readable army name to its underlying army ID
            SelectedArmy = _nameToArmyId[armyBox.SelectedIndex];

            this.DialogResult = DialogResult.OK;
        }

        private void comboPreset_SelectedIndexChanged(object sender, EventArgs e) {
            string preset_name = comboPreset.SelectedItem.ToString();
            ScenarioSettings preset = ScenarioSettings.FromPreset(preset_name);
            // update settings version no matter what
            preset.SettingsVersion = ScenarioSettings.CURRENT_SETTINGS_VERSION;
            _settings = preset;
            MatchCurrentSettings();
        }

        private void btnSaveAsPreset_Click(object sender, EventArgs e) {
            EnterPresetName epn = new EnterPresetName();
            if (epn.ShowDialog() == DialogResult.OK) {
                ScenarioSettings settings = new ScenarioSettings();
                SaveCurrentSettingsTo(settings);

                settings.ToPreset(epn.PresetName);
                SetPresetItems();
            }
            epn.Dispose();
        }

        private void btnCancel_Click(object sender, EventArgs e) {
            this.DialogResult = DialogResult.Cancel;
        }
    }
}
