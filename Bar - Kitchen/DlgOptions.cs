using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Bar___Kitchen
{
    public partial class DlgOptions : Form
    {
        public OrderType type { get; set; }

        public DlgOptions(OrderType type)
        {
            InitializeComponent();
            if (type == OrderType.Bar) rbBar.Checked = true;
            else rbKitchen.Checked = true;
        }

        private void btOk_Click(object sender, EventArgs e)
        {
            if(rbBar.Checked == true) type = OrderType.Bar;
            else type = OrderType.Kitchen;
            this.DialogResult = DialogResult.OK;
        }
    }
}
