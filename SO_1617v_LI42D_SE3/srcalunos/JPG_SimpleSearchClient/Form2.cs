using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JPG_SimpleSearchClient
{
    public partial class Form2 : Form
    {
        public Form2(String message)
        {
            InitializeComponent();
            label1.Text = message;
        }
    }
}
