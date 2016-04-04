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
        public delegate void UpdateTabelaOrdersCallback(Order order); //Para conseguir alterar a interface com um processo exterior

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
            if (order.Item.Type == myType) bsOrders.Add(order);
        }

        private void optionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DlgOptions dlg = new DlgOptions(myType);
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                if (this.myType != dlg.type)
                {
                    this.myType = dlg.type;
                    this.Text = "POS: " + this.myType.ToString() + " Profile";
                    toolStripStatusLabel1.Text = "Your Profile: " + this.myType.ToString();
                    bsOrders.Clear();
                    IList<Order> orders = new List<Order> { };
                    try
                    {
                        orders = orderManager.getOrders(); //Obtem todas as encomendas que já estejam no server
                    }
                    catch { }
                    foreach (Order order in orders) this.UpdateTabelaOrders(order);
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
