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
    public partial class ReconnectBattleFile : Form {
        public string NewPath { get; set; }

        public ReconnectBattleFile(string badPath) {
            InitializeComponent();

            labelBadPath.Text = badPath;
        }

        private void btnOk_Click(object sender, EventArgs e) {
            using (
                OpenFileDialog dialog = new OpenFileDialog()
            ) {
                dialog.InitialDirectory = @"C:\WDS";
                dialog.Filter = "Battle files (*.btl;*.bte;*.btc;*.btt)|*.btl;*.bte;*.btc;*.btt";
                dialog.FilterIndex = 0;
                dialog.RestoreDirectory = true;

                // Show the dialog and check if the user clicked OK
                if (dialog.ShowDialog() == DialogResult.OK) {
                    NewPath = dialog.FileName;
                    DialogResult = DialogResult.OK;
                } else {
                    DialogResult = DialogResult.Cancel;
                }
            }
        }
    }
}
