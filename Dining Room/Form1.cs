using System;
using System.Runtime.Remoting;
using System.Windows.Forms;
using System.Collections;

namespace Dining_Room
{
    public partial class Form1 : Form
    {
        private BindingSource bsOrders; //BindingSource para o dataGridView1
        IOrdersList orderManager; //Objeto Remoto
        AlterEventRepeater evRepeater; //Subscritor dos eventos
        public delegate void UpdateTabelaOrdersCallback(Order order, Operation op); //Para conseguir alterar a interface com um processo exterior

        public Form1()
        {
            RemotingConfiguration.Configure("Dining Room.exe.config", false);
            InitializeComponent();
            orderManager = (IOrdersList)RemoteNew.New(typeof(IOrdersList));
            bsOrders = new BindingSource();
            customInitialize();

            /*Inicia o subscritor dos eventos e subescreve-os */
            evRepeater = new AlterEventRepeater();
            evRepeater.alterEvent += new AlterDelegate(DoAlterations);
            orderManager.alterEvent += new AlterDelegate(evRepeater.Repeater);
        }

        /* The client is also a remote object. The Server calls remotely the AlterEvent handler Infinite lifetime */
        public override object InitializeLifetimeService() { return null; }

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
                bsOrders.Add(order);
            }
            else if (op == Operation.Change)
            {
                if(bsOrders.IndexOf(order) < 0) return;
                bsOrders[bsOrders.IndexOf(order)] = order;
                /* Procura linha pra alterar cor */
                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    if (row.DataBoundItem.Equals(order))
                    {
                        switch (order.Status)

                        {
                            case OrderStatus.Prepararion:
                                row.DefaultCellStyle.BackColor = System.Drawing.Color.Orange;
                                break;
                            case OrderStatus.Ready:
                                row.DefaultCellStyle.BackColor = System.Drawing.Color.LawnGreen;
                                break;
                            case OrderStatus.Delivered:
                                bsOrders.Remove(order);
                                break;
                        }
                        break;
                    }
                }

            }
        }

        /* Add order */
        private void button1_Click(object sender, EventArgs e)
        {
            if (cbMenu.SelectedItem != null && cbTable.SelectedItem != null)
            {
                try
                {
                    int result = orderManager.addOrder(cbTable.SelectedIndex, (Int32)cbMenu.SelectedValue, Decimal.ToInt32(nudQuantity.Value));
                    if (result == 1) MessageBox.Show("Unable to add order. The table is in payment process.");
                    else if(result == 2) MessageBox.Show("Unable to add order. Table is not ocupied.");
                }
                catch (Exception)
                {
                    MessageBox.Show("Sorry, an error occurred adding your order.");
                }
            }
        }

        private void btClearOrder_Click(object sender, EventArgs e)
        {
            Order o;
            Int32 selectedRowCount = dataGridView1.Rows.GetRowCount(DataGridViewElementStates.Selected);
            if (selectedRowCount > 0)
            {
                for (int i = 0; i < selectedRowCount; i++)
                {
                    o = (Order)dataGridView1.SelectedRows[i].DataBoundItem;
                    if (o.Status == OrderStatus.Ready)
                    {
                        orderManager.changeOrderStatus(o, OrderStatus.Delivered);
                    }
                    else if (o.Status == OrderStatus.Pending)
                    {
                        orderManager.deleteOrder(o);
                    }
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
