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
    public class CsTransformer
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

            // ск-42 зона 10
            string projstring_initial = 
                "+proj=tmerc +lat_0=0 +lon_0=57 +k=1 +x_0=10500000 +y_0=0 +ellps=krass +towgs84=23.57,-140.95,-79.8,0,0.35,0.79,-0.22 +units=m +no_defs +axis=neu  +col_0=\"X\" +col_1=\"Y\" +col_2=\"H\"";
            // ск-42 зона 11
            string projstring_target = 
                "+proj=tmerc +lat_0=0 +lon_0=63 +k=1 +x_0=11500000 +y_0=0 +ellps=krass +towgs84=23.92,-141.27,-80.9,0,0.35,0.82,-0.12 +units=m +no_defs +axis=neu  +col_0=\"X\" +col_1=\"Y\" +col_2=\"H\""; 
            DotSpatial.Projections.ProjectionInfo crs_initial = 
                DotSpatial.Projections.ProjectionInfo.FromProj4String(projstring_initial);
            DotSpatial.Projections.ProjectionInfo crs_target = 
                DotSpatial.Projections.ProjectionInfo.FromProj4String(projstring_target);
            DotSpatial.Projections.ProjectionInfo crs_wgs = 
                DotSpatial.Projections.ProjectionInfo.FromEpsgCode(4326);

            double[] xy = new double[] {
                590169.2431,7352219.492,
                590123.2431,7352231.492,
                590252.1738,7352269.636
            };
            double[] z = {0, 0, 0};

            DotSpatial.Projections.Reproject.ReprojectPoints(xy, z, crs_initial, crs_target, 0, xy.Length / 2);

            int count = 0;
            var tr = doc.TransactionManager.StartTransaction();
            using (tr)
            {
                List<Point2d> pts = new List<Point2d>();
                foreach (SelectedObject so in psr.Value)
                {
                    Entity ent =
                    (Entity)tr.GetObject(
                    so.ObjectId,
                    OpenMode.ForRead
                    );

                    // coordinates transformation cases for all kinds of drawing objects
                    // Regular Polyline
                    Polyline pl = (Polyline)ent;
                    if (pl != null)
                    {
                        // Collect vertices
                        pts = new List<Point2d>();
                        for (int i = 0; i < pl.NumberOfVertices; i++)
                        {
                            pts.Add(pl.GetPoint2dAt(i));
                            pl.GetArcSegment2dAt(i);
                        }
                
                        // Open Polyline for edit
                        pl.UpgradeOpen();
                
                        // Write rounded coordinates back to Polyline
                        for (int i = 0; i < pl.NumberOfVertices; i++)
                        {
                            pl.SetPointAt(i, new Point2d(pts[i].X + 10, pts[i].Y + 10));
                        }
                    }

                    // Polyline3d
                    Polyline3d pl3d = (Polyline3d)ent;
                    if (pl3d != null)
                    {
                        foreach (ObjectId vId in pl3d)
                        {
                            PolylineVertex3d pV3d = (PolylineVertex3d)tr.GetObject(vId, OpenMode.ForWrite);
                            pV3d.Position = new Point3d(pV3d.Position.X + 10, pV3d.Position.Y + 10, pV3d.Position.Z);
                        }
                    }

                    // Polyline 2d
                    Polyline2d pl2d = (Polyline2d)ent;
                    if (pl2d != null)
                    {
                        foreach (ObjectId vId in pl2d)
                        {
                            Vertex2d pV2d = (Vertex2d)tr.GetObject(vId, OpenMode.ForWrite);
                            pV2d.Position = new Point3d(pV2d.Position.X + 10, pV2d.Position.Y + 10, 0);
                        }
                    }

                    // Hatch
                    Hatch ht = (Hatch)ent;
                    if (ht != null)
                    {
                        Vector3d v3 = new Vector3d(1,1,1);
                        // v3.
                        // ht.GetStretchPoints();
                        // ht.set
                        // ht.AppendLoop(HatchLoopTypes.External, );
                    }


                    // Autodesk.AutoCAD.DatabaseServices.Hatch;
                    // Autodesk.AutoCAD.DatabaseServices.Polyline3d;
                    // Autodesk.AutoCAD.DatabaseServices.MText;
                    // Autodesk.AutoCAD.DatabaseServices.DBText;
                    // Autodesk.AutoCAD.DatabaseServices.Line;
                    // Autodesk.AutoCAD.DatabaseServices.BlockReference;
                    // Autodesk.AutoCAD.DatabaseServices.DBPoint;
                    // Autodesk.AutoCAD.DatabaseServices.Polyline;

                    count ++;
                }

            tr.Commit();
            ed.WriteMessage("Преобразовано " + count.ToString() + " объектов");
            }
            
        }
    }
}
