using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.IO;

namespace AcadCsObjectsTransform
{
    public partial class CsForm : Form
    {
        BackgroundWorker worker;
        CsTransformer transformer;
        public CsForm()
        {
            InitializeComponent();
            startButton.Click += new EventHandler(this.startButton_Click);
            GsToComboBox();

            worker = new BackgroundWorker();
            worker.WorkerReportsProgress = true;
            worker.WorkerSupportsCancellation = true;
            worker.ProgressChanged += worker_ProgressChanged;
            worker.DoWork += worker_DoWork;
            worker.RunWorkerCompleted += worker_RunWorkerCompleted;
        }


        Dictionary<string, string> csDict = new Dictionary<string, string>();
        private void GsToComboBox()
        {
            
            // read geosystems.xml
            XmlDocument gsDoc = new XmlDocument();
            try
            {
                gsDoc.Load("geosystems.xml");
            }
            catch (FileNotFoundException)
            {
                MessageBox.Show("Файл geosystems.xml не найден", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            XmlElement gsRoot = gsDoc.DocumentElement;

            string cs = "";
            string projString = "";

            //find all systems and add to dict
            foreach (XmlNode gsNode in gsRoot)
            {
                if (gsNode.Attributes.Count > 0)
                {
                    try
                    {
                        cs = gsNode.Attributes.GetNamedItem("name").Value;
                        cs += ": " + gsNode.Attributes.GetNamedItem("folder").Value;
                        
                        foreach (XmlNode csNode in gsNode.ChildNodes)
                        {
                            if (csNode.Attributes.GetNamedItem("receiver").Value == "вега")
                            {
                                projString = csNode.InnerText;
                                csDict.Add(cs, projString);
                            }
                        }
                    }
                    catch { }
                }
            }

            // add systems names to combo
            foreach (KeyValuePair<string,string> keyValue in csDict)
            {
                initialCsComboBox.Items.Add(keyValue.Key);
                targetCsComboBox.Items.Add(keyValue.Key);
            }

        }

        double initialXOffsetValue = 0.0;
        double initialYOffsetValue = 0.0;
        double targetXOffsetValue = 0.0;
        double targetYOffsetValue = 0.0;
        int objectsTransformedCount = 0;

        private void startButton_Click(object sender, EventArgs e)
        {
            transformer = new CsTransformer();
            // cs errors
            if (!csDict.ContainsKey(initialCsComboBox.Text))
            {
                MessageBox.Show("Неверная исходная система координат", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (!csDict.ContainsKey(targetCsComboBox.Text))
            {
                MessageBox.Show("Неверная целевая система координат", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            // check for offset errors and start transformation
            try
            {
                initialXOffsetValue = double.Parse(
                    initialXOffsetTextBox.Text.Replace(",", ".")
                    );
                initialYOffsetValue = double.Parse(
                    initialYOffsetTextBox.Text.Replace(",", ".")
                    );
                targetXOffsetValue = double.Parse(
                    targetXOffsetTextBox.Text.Replace(",", ".")
                    );
                targetYOffsetValue = double.Parse(
                    targetYOffsetTextBox.Text.Replace(",", ".")
                    );

                transformer.projStringInitial = csDict[initialCsComboBox.Text];
                transformer.projStringTarget = csDict[targetCsComboBox.Text];
                progressBar.Value = 0;
                //if (worker.IsBusy != true)
                //{
                //  startButton.Enabled = false;
                //  worker.RunWorkerAsync();
                //}

                startButton.Enabled = false;

                foreach (int progressPercentage in transformer.Transform())
                {
                    progressBar.Value = progressPercentage;
                    progressBar.Update();
                }
                objectsTransformedCount = transformer.objectsCompleted;

                if (objectsTransformedCount > 0)
                {
                    MessageBox.Show(
                        string.Format("Преобразовано {0} объектов", objectsTransformedCount.ToString()),
                        "",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    startButton.Enabled = true;
                }
                else
                {
                    MessageBox.Show(
                        "Не найдено объектов для преобразования",
                        "",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    progressBar.Value = 0;
                    startButton.Enabled = true;
                }


            }
            catch (FormatException)
            {
                progressBar.Value = 0;
                MessageBox.Show("Неверное значение смещения", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            worker = sender as BackgroundWorker;
            //transformer.Transform(worker, e);
        }

        void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled == true)
            {
                startButton.Enabled = true;
                MessageBox.Show("Преобразование координат завершено", "",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                //return;
            }
            else
            {
                progressBar.Value = 100;
                startButton.Enabled = true;
                MessageBox.Show("Преобразование координат завершено", "",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                //return;
            }
        }

        void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            worker = sender as BackgroundWorker;
            progressBar.Value = e.ProgressPercentage;
            
        }

    }
}
