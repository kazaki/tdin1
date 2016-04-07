using System;
using System.Collections;
using System.Runtime.Remoting;
using System.Windows.Forms;

namespace POS
{
    public partial class Form1 : Form
    {
        IOrdersList orderManager; //Objeto Remoto

        public Form1()
        {
            RemotingConfiguration.Configure("POS.exe.config", false);
            InitializeComponent();  
            orderManager = (IOrdersList)RemoteNew.New(typeof(IOrdersList));
            orderManager.connect();
        }

        CheckBox lastChecked;
        private void chk_Click(object sender, EventArgs e)
        {
            CheckBox activeCheckBox = sender as CheckBox;
            if (activeCheckBox != lastChecked && lastChecked != null) lastChecked.Checked = false;
            lastChecked = activeCheckBox.Checked ? activeCheckBox : null;
        }

        private void btAssingTable_Click(object sender, EventArgs e)
        {
            if (lastChecked != null)
            {
                orderManager.assignTable(Int32.Parse(lastChecked.Text) - 1);
                lastChecked.BackColor = System.Drawing.Color.Blue;
            }
            else MessageBox.Show("Please select a table.");
        }

        private void btPayTable_Click(object sender, EventArgs e)
        {
            if (lastChecked != null) {
                if (orderManager.payTable(Int32.Parse(lastChecked.Text) - 1)) lastChecked.BackColor = System.Drawing.SystemColors.ControlLight;
                else MessageBox.Show("Table hasn't requested the bill.");
            }
            else MessageBox.Show("Please select a table.");
        }

        private void btConsultTable_Click(object sender, EventArgs e)
        {
            if (lastChecked != null)
            {
                if(orderManager.requestBill(Int32.Parse(lastChecked.Text) - 1)) lastChecked.BackColor = System.Drawing.Color.Green;
                else MessageBox.Show("Table is not occupied.");
            }
            else MessageBox.Show("Please select a table.");
        }
    }



}
