using Autodesk.AutoCAD.Runtime;
using System;
using System.IO;


namespace AcadCsObjectsTransform
{
    public class AcadStartForm
    {
        [STAThread]
        [CommandMethod("CsTransform", CommandFlags.UsePickSet)]
        public void StartForm()
        {
            CsForm form = new CsForm();
            System.Windows.Forms.Application.EnableVisualStyles();
            System.Windows.Forms.Application.Run(form);
        }
    }
}
