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
    public partial class EnterPresetName : Form {
        public string PresetName { get; set; }
        
        public EnterPresetName() {
            InitializeComponent();
            presetName.SelectAll();
            presetName.Focus();
        }

        private void btnSave_Click(object sender, EventArgs e) {
            PresetName = presetName.Text;
            this.DialogResult = DialogResult.OK;
        }

        private void btnCancel_Click(object sender, EventArgs e) {
            this.DialogResult = DialogResult.Cancel;
        }
    }
}
