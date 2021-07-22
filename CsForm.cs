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
using DotSpatial.Projections;

namespace AcadCsObjectsTransform
{
    public partial class CsForm : Form
    {
        CsTransformer transformer;
        public CsForm()
        {
            InitializeComponent();
            startButton.Click += new EventHandler(this.startButton_Click);
            GsToComboBox();
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
                double initialXOffsetValue = double.Parse(
                    initialXOffsetTextBox.Text.Replace(",", ".")
                    );
                double initialYOffsetValue = double.Parse(
                    initialYOffsetTextBox.Text.Replace(",", ".")
                    );
                double targetXOffsetValue = double.Parse(
                    targetXOffsetTextBox.Text.Replace(",", ".")
                    );
                double targetYOffsetValue = double.Parse(
                    targetYOffsetTextBox.Text.Replace(",", ".")
                    );

                progressBar.Value = 0;
                startButton.Enabled = false;

                // projections info
                transformer.crsInitial = ProjectionInfo.FromProj4String(csDict[initialCsComboBox.Text]);
                transformer.crsTarget = ProjectionInfo.FromProj4String(csDict[targetCsComboBox.Text]);

                // transform objects and update progress bar
                foreach (int progressPercentage in transformer.Transform())
                {
                    if (progressPercentage != -1)
                    {                    
                        progressBar.Value = progressPercentage;
                        progressBar.Update();
                    }
                    else
                    {
                        MessageBox.Show(
                        "Не найдено объектов для преобразования",
                            "",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        startButton.Enabled = true;
                        return;
                    }
                }

                int objectsTransformedCount = transformer.objectsCompleted;
                MessageBox.Show(
                    string.Format("Преобразовано {0} объектов", objectsTransformedCount.ToString()),
                    "",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                startButton.Enabled = true;
            }
            catch (FormatException)
            {
                progressBar.Value = 0;
                MessageBox.Show("Неверное значение смещения", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                progressBar.Value = 0;
                MessageBox.Show(string.Format("Неопределенная ошибка приложения {0}", ex.GetType()), "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
