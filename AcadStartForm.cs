using Autodesk.AutoCAD.Runtime;
using System;


namespace AcadCsObjectsTransform
{
    public class AcadStartForm
    {
        [STAThread]
        [CommandMethod("CsTransform", CommandFlags.UsePickSet)]
        public void StartForm()
        {
            System.Windows.Forms.Application.EnableVisualStyles();
            System.Windows.Forms.Application.Run(new CsForm());
        }
    }
}
