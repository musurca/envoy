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
    public partial class CustomDispatchNode : Form {
        public string CustomName { get; set; }
        public Location CustomLocation { get; set; }
        
        public CustomDispatchNode(string customType, ScenarioData sd) {
            InitializeComponent();

            Width = 346;
            Height = 162;

            nameLabel.Text = $"Name of {customType}:";
            Text = $"Custom {customType}...";

            hexX.Maximum = sd.MapWidth - 1;
            hexY.Maximum = sd.MapHeight - 1;
            hexX.Minimum = hexY.Minimum = 0;
            hexX.Value = hexY.Value = 0;
        }

        private void btnOK_Click(object sender, EventArgs e) {
            CustomName = customName.Text.Trim();
            CustomLocation = new Location((int)hexX.Value, (int)hexY.Value);
            
            DialogResult = DialogResult.OK;
        }
    }
}
