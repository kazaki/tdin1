using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.Remoting;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Bar___Kitchen
{
    public partial class Form1 : Form
    {
        OrderType myType; //Localização: Bar ou cozinha
        private BindingSource bsOrders; //BindingSource para o dataGridView1
        IOrdersList orderManager; //Objeto Remoto
        AlterEventRepeater evRepeater; //Subscritor dos eventos
        public delegate void UpdateTabelaOrdersCallback(Order order, Operation op); //Para conseguir alterar a interface com um processo exterior

        public Form1()
        {
            RemotingConfiguration.Configure("Bar - Kitchen.exe.config", false);
            InitializeComponent();
            orderManager = (IOrdersList)RemoteNew.New(typeof(IOrdersList));
            bsOrders = new BindingSource();

            DlgOptions dlg = new DlgOptions(OrderType.Bar);
            while (dlg.ShowDialog() != DialogResult.OK)
            {
                MessageBox.Show("You have to select a profile before proceeding!", "Select a Prolfile", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            this.myType = dlg.type;
            this.Text = "POS: " + this.myType.ToString() + " Profile";
            toolStripStatusLabel1.Text = "Your Profile: " + this.myType.ToString();
            customInitialize();

            /*Inicia o subscritor dos eventos e subescreve-os */
            evRepeater = new AlterEventRepeater();
            evRepeater.alterEvent += new AlterDelegate(DoAlterations);
            orderManager.alterEvent += new AlterDelegate(evRepeater.Repeater);
        }

        /* Chamado após evento subscrito de alteração */
        public void DoAlterations(Operation op, Order order)
        {
            Invoke(new UpdateTabelaOrdersCallback(this.UpdateTabelaOrders), new object[] { order, op });
        }

        /* Faz alterações à dataGridView1 */
        private void UpdateTabelaOrders(Order order, Operation op)
        {
            if (op == Operation.New)
            {
                if (order.Item.Type == myType && order.Status != OrderStatus.Ready) bsOrders.Add(order);
            }
            else if (op == Operation.Change)
            {
                int i = bsOrders.IndexOf(order);
                if (i > -1)
                {
                    if (order.Status >= OrderStatus.Ready) bsOrders.RemoveAt(i);
                    else bsOrders[i] = order;
                }
            }
        }

        /* Listener do evento de clicar no botão na lista */
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            var senderGrid = (DataGridView)sender;
            if (senderGrid.Columns[e.ColumnIndex] is DataGridViewButtonColumn &&
                e.RowIndex >= 0)
            {
                Order selectedOrder = (Order)senderGrid.Rows[e.RowIndex].DataBoundItem;
                orderManager.changeOrderStatus(selectedOrder, selectedOrder.NextStatus);
                //if (selectedOrder.Status == OrderStatus.Ready) bsOrders.Remove(selectedOrder); //Como está concluido remove da lista
                //else senderGrid.Rows[e.RowIndex].Cells[3].Value = selectedOrder.Status;
            }
        }

        /* ToolStrip */
        private void optionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DlgOptions dlg = new DlgOptions(myType);
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                if (this.myType != dlg.type)
                {
                    this.myType = dlg.type;
                    this.Text = "POS: " + this.myType.ToString();
                    toolStripStatusLabel1.Text = "Your Profile: " + this.myType.ToString();
                    bsOrders.Clear();
                    IList<Order> orders = new List<Order> { };
                    try
                    {
                        orders = orderManager.getOrders(); //Obtem todas as encomendas que já estejam no server
                    }
                    catch { }
                    foreach (Order order in orders) this.UpdateTabelaOrders(order, Operation.New);
                }
            }

        }

        /* Desubscreve os eventos porque o programa fechou */
        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                orderManager.alterEvent -= new AlterDelegate(evRepeater.Repeater);
                evRepeater.alterEvent -= new AlterDelegate(DoAlterations);
            }
            catch (Exception) { }
        }

    }

}
