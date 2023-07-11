using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WDS_Dispatches {
    public partial class AdvChangeArmy : Form {

        public string SelectedArmy { get; set; }
        
        public AdvChangeArmy(List<string> armies, string currentArmy) {
            InitializeComponent();

            int selectedIndex = armies.IndexOf(currentArmy);
            
            armyBox.BeginUpdate();
            armyBox.Items.AddRange(armies.ToArray());
            armyBox.SelectedIndex = selectedIndex;
            armyBox.EndUpdate();
            
            SelectedArmy = currentArmy;
        }

        private void btnSave_Click(object sender, EventArgs e) {
            SelectedArmy = armyBox.SelectedItem.ToString();
            this.DialogResult = DialogResult.OK;
        }

        private void btnCancel_Click(object sender, EventArgs e) {
            this.DialogResult = DialogResult.Cancel;
        }
    }
}
