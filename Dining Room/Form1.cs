using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.Remoting;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections;

namespace Dining_Room
{
    public partial class Form1 : Form
    {

        IOrdersList orderManager;
        AlterEventRepeater evRepeater;
        public delegate void UpdateTabelaOrdersCallback(Order order); //Para conseguir alterar a interface com um processo exterior

        public Form1()
        {
            RemotingConfiguration.Configure("Dining Room.exe.config", false);
            InitializeComponent();

            orderManager = (IOrdersList)RemoteNew.New(typeof(IOrdersList));
            customInitialize();

            evRepeater = new AlterEventRepeater();
            evRepeater.alterEvent += new AlterDelegate(DoAlterations);
            orderManager.alterEvent += new AlterDelegate(evRepeater.Repeater);
        }

        /* The client is also a remote object. The Server calls remotely the AlterEvent handler Infinite lifetime */
        public override object InitializeLifetimeService() { return null; }

        /* Chamado após evento subscrito de alteração */
        public void DoAlterations(Operation op, Order order)
        {
            switch (op)
            {
                case Operation.New:
                    Invoke(new UpdateTabelaOrdersCallback(this.UpdateTabelaOrders), new object[] { order });
                    break;
                case Operation.Change:
                    break;
            }
        }

        /* Faz alterações à dataGridView1 */
        private void UpdateTabelaOrders(Order order)
        {
            this.dataGridView1.Rows.Add(order.Table.Id, order.Item.Name, order.Quantity, order.getStatus());
        }

        /* Add order */
        private void button1_Click(object sender, EventArgs e)
        {
            if (cbMenu.SelectedItem != null && cbTable.SelectedItem != null)
            {
                orderManager.addOrder(cbTable.SelectedIndex, (Int32)cbMenu.SelectedValue, Decimal.ToInt32(nudQuantity.Value));
            }
        }

        /* Desubscreve os eventos porque o programa fechou */
        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            orderManager.alterEvent -= new AlterDelegate(evRepeater.Repeater);
            evRepeater.alterEvent -= new AlterDelegate(DoAlterations);
        }
    }

    /* Mechanism for instanciating a remote object through its interface, using the config file */
    class RemoteNew
    {
        private static Hashtable types = null;

        private static void InitTypeTable()
        {
            types = new Hashtable();
            foreach (WellKnownClientTypeEntry entry in RemotingConfiguration.GetRegisteredWellKnownClientTypes())
                types.Add(entry.ObjectType, entry);
        }

        public static object New(Type type)
        {
            if (types == null)
                InitTypeTable();
            WellKnownClientTypeEntry entry = (WellKnownClientTypeEntry)types[type];
            if (entry == null)
                throw new RemotingException("Type not found!");
            return RemotingServices.Connect(type, entry.ObjectUrl);
        }
    }


}
