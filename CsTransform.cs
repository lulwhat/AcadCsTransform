using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.BoundaryRepresentation;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.Colors;
using System;
using System.Collections.Generic;
using System.Collections;

namespace AcadCsObjectsTransform
{
    public class CsTransform
    {
        [CommandMethod("CsTransform", CommandFlags.UsePickSet)]
        static public void Transform()
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            Editor ed = doc.Editor;

            // Create filter for selection
            TypedValue[] filterList = new TypedValue[] 
            { 
                new TypedValue(-4, "<OR"),
                new TypedValue(0, "LWPOLYLINE"),
                new TypedValue(0, "POLYLINE"),
                new TypedValue(0, "POLYLINE2D"),
                new TypedValue(0, "LINE"),
                new TypedValue(0, "POINT"),
                new TypedValue(0, "INSERT"),
                new TypedValue(0, "TEXT"),
                new TypedValue(0, "MTEXT"),
                new TypedValue(0, "HATCH"),
                new TypedValue(-4, "OR>")
            };
            SelectionFilter filter = new SelectionFilter(filterList);

            // Select all listed objects
            PromptSelectionResult psr = ed.SelectAll(filter);
            if (psr.Status != PromptStatus.OK)
            {
                ed.WriteMessage("Не найдено ни одного объекта для преобразования\n");
                return;
            }

            int count = 0;
            var tr = doc.TransactionManager.StartTransaction();
            using (tr)
            {
                foreach (SelectedObject so in psr.Value)
                {
                    Entity ent =
                    (Entity)tr.GetObject(
                    so.ObjectId,
                    OpenMode.ForRead
                    );

                    // Autodesk.AutoCAD.DatabaseServices.Hatch;
                    // Autodesk.AutoCAD.DatabaseServices.Polyline3d;
                    // Autodesk.AutoCAD.DatabaseServices.MText;
                    // Autodesk.AutoCAD.DatabaseServices.DBText;
                    // Autodesk.AutoCAD.DatabaseServices.Line;
                    // Autodesk.AutoCAD.DatabaseServices.BlockReference;
                    // Autodesk.AutoCAD.DatabaseServices.DBPoint;
                    // Autodesk.AutoCAD.DatabaseServices.Polyline;

                    ed.WriteMessage(ent.GetType().ToString() + "\n");
                    count ++;
                }

            // var obj = tr.GetObject(ent.ObjectId, OpenMode.ForRead);
            // Polyline pl = (Polyline)obj;
            tr.Commit();
            ed.WriteMessage("Found " + count.ToString() + " objects");
            }
            
        }
    }
}
