using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;

using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JPG_SimpleSearchClient
{
    public partial class Form1 : Form
    {
        private JPGService service;
        private PictureBox picture;
        ToolTip toolTip = new ToolTip();

        //mensagens de erro
        private static string con_error = "Erro de ligação.Por favor reinicie a aplicação!";
        private static string tag_error = "Erro de leitura da metadata!";

        public Form1()
        {
            InitializeComponent();

            service = new JPGService();
            service.Connect();

            picture = new PictureBox();
            picture.Dock = DockStyle.Fill;
            picture.SizeMode = PictureBoxSizeMode.Zoom;
            toolTip.AutoPopDelay = 32000;
            panel1.Controls.Add(picture); 
        }

        private void disableActions()
        {
            filesList.Enabled = false;
            execButton.Enabled = false;
        }

        private void enableActions()
        {
            filesList.Enabled = true;
            execButton.Enabled = true;
        }

        private void handleError()
        {
            try
            {
                service.Reconnect();
            }
            catch (Exception)
            {
                Form dialog1 = new Form2(con_error);
                dialog1.ShowDialog();
            }
        }

        private void execButton_Click(object sender, EventArgs e)
        {
            filesList.Items.Clear();
            
            DoSearch(args.Text);
        }
 

        private void DoSearch(String args)
        {
            disableActions();
            service.SearchPhotos(args).
                ContinueWith(t =>
                {
                    if (t.IsFaulted )
                    {
                        handleError();     
                    }
                    else 
                    {
                        // add names to user interface in UI thread
                        foreach (String name in t.Result)
                            filesList.Items.Add(name);
                    }
                    enableActions();
                }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private String buildTagList(Image img)
        {
            StringBuilder sb = new StringBuilder();
            foreach(PropertyItem pi in img.PropertyItems)
            {
                try {
                    String tagS = ExifUtils.TagToString(pi.Id, pi.Type, pi.Len, pi.Value);
                    if (tagS != String.Empty)
                    {
                        sb.Append(tagS);
                        sb.AppendLine();
                    }
                }
                catch(Exception)
                {
                    Form dialog1 = new Form2(tag_error + ": " + pi.Id);
                    dialog1.ShowDialog();
                }
            }
            return sb.ToString();
        }

        private void DoGet(String args)
        {
            disableActions();
            service.GetPhoto(args).
                ContinueWith(t =>
                {
                    if (t.IsFaulted)
                    {
                        handleError();
                    }
                    else
                    {
                        // add image to control in UI thread
                        Image img = Image.FromStream(t.Result);
                        toolTip.RemoveAll();

                        toolTip.SetToolTip(picture, buildTagList(img));
                        picture.Image = img;
                        enableActions();
                    }
                }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void filesList_SelectedIndexChanged(object sender, EventArgs e)
        {
            String file = (String)filesList.SelectedItem;
            DoGet(file);
        }
    }
}
