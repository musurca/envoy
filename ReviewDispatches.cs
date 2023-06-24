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
    public partial class ReviewDispatches : Form {

        private int _currentDispatchIndex;
        private List<Dispatch> _dispatches;

        public ReviewDispatches() {
            InitializeComponent();
        }

        public void NextDispatch() {
            _currentDispatchIndex = Math.Min(
                _dispatches.Count - 1, 
                _currentDispatchIndex + 1
            );
            RefreshText();
        }

        public void PrevDispatch() {
            _currentDispatchIndex = Math.Max(
                0, 
                _currentDispatchIndex - 1
            );
            RefreshText();
        }

        public void RefreshText() {
            Dispatch d = _dispatches[_currentDispatchIndex];
            labelMessageSender.Text = d.Sender;
            labelMessageRecipient.Text = d.Recipient;
            messageBody.Text = d.Message.Trim();

            labelXofY.Text = (_currentDispatchIndex + 1) + " / " + _dispatches.Count;

            // Enable/disable next/prev buttons based on index
            if (_currentDispatchIndex + 1 == _dispatches.Count) {
                btnNext.Enabled = false;
            } else {
                btnNext.Enabled = true;
            }
            if (_currentDispatchIndex == 0) {
                btnPrev.Enabled = false;
            } else {
                btnPrev.Enabled = true;
            }
        }

        public void SetDispatchDelivery(int turn_num, List<Dispatch> dispatches) {
            _dispatches = dispatches;
            _currentDispatchIndex = -1;

            if(_dispatches.Count == 1) {
                // hide the next/prev buttons
                btnNext.Visible = false;
                btnPrev.Visible = false;
            }

            this.Text = "Dispatches delivered on turn " + turn_num;
            NextDispatch();
        }

        private void ReviewDispatches_Load(object sender, EventArgs e) {
            Activate();
            System.Media.SystemSounds.Asterisk.Play();
        }

        private void btnNext_Click(object sender, EventArgs e) {
            NextDispatch();
        }

        private void btnPrev_Click(object sender, EventArgs e) {
            PrevDispatch();
        }
    }
}
