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

        RestaurantHandler restaurant;

        public Form1()
        {

            RemotingConfiguration.Configure("Dining Room.exe.config", false);

            restaurant = (RestaurantHandler)RemoteNew.New(typeof(RestaurantHandler));

            InitializeComponent();
            customInitialize();
        }

        private void button1_Click(object sender, EventArgs e)
        {

            restaurant.printTables();
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
