using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.BoundaryRepresentation;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.Colors;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections;
using System.Threading;
using System.Windows.Forms;
using System.ComponentModel;

namespace AcadCsObjectsTransform
{
    public class CsTransformer
    {

        public string projStringInitial;
        public string projStringTarget;
        public int objectsCompleted = 0;

        public IEnumerable<int> Transform()
        //public int Transform(object sender, DoWorkEventArgs e)
        {
            //BackgroundWorker worker = sender as BackgroundWorker;
            int progressPercentage = 0;

            Document doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
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
            //if (psr.Status != PromptStatus.OK)
            //{
            //    ed.WriteMessage("Не найдено ни одного объекта для преобразования\n");
            //    return 0;
            //}

            int selectedObjectsCount = psr.Value.Count;

            

            
            /*

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

            */

            List<Point2d> plPts = new List<Point2d>();
            List<CircularArc2d> plArcs = new List<CircularArc2d>();
            List<ObjectId> arcIds = new List<ObjectId>();
            CircularArc2d arc = new CircularArc2d();
            double blg = new double();
            List<double> plBlgs = new List<double>();
            List<int> arcInds = new List<int>();
            Point3d pt1 = new Point3d();
            Point3d pt2 = new Point3d();
            double chord = new double();
            
            foreach (SelectedObject so in psr.Value)
            {
                var tr = doc.TransactionManager.StartTransaction();
                using (tr)
                {
                    Entity ent =
                        (Entity)tr.GetObject(
                        so.ObjectId,
                        OpenMode.ForRead
                        );

                    // coordinates transformation cases for all kinds of drawing objects
                    // Regular Polyline
                    Polyline pl = ent as Polyline;
                    if (pl != null)
                    {
                        // Collect vertices
                        plPts = new List<Point2d>();
                        plArcs = new List<CircularArc2d>();
                        plBlgs = new List<double>();
                        arcInds = new List<int>();
                        for (int i = 0; i < pl.NumberOfVertices; i++)
                        { plPts.Add(pl.GetPoint2dAt(i)); }


                        // Collect bulges
                        for (int i = 0; i < pl.NumberOfVertices; i++)
                        {
                            plBlgs.Add(pl.GetBulgeAt(i));
                        }
                        // collect arcs
                        for (int i = 0; i < pl.NumberOfVertices - 1; i++)
                        {
                            plArcs.Add(pl.GetArcSegment2dAt(i));
                            if (plArcs[i].EndAngle != Math.PI)
                            {
                                arcInds.Add(i);
                            }
                        }

                        pl.UpgradeOpen();
                        // Write coordinates back to Polyline
                        for (int i = 0; i < pl.NumberOfVertices; i++)
                        {
                            pl.SetPointAt(i, new Point2d(plPts[i].X + 10, plPts[i].Y + 10));
                        }

                        foreach (int i in arcInds)
                        {
                            if ((pl.Closed) & (i == pl.NumberOfVertices - 1))
                            {
                                pt1 = pl.GetPointAtParameter(i);
                                pt2 = pl.GetPointAtParameter(0);
                            }
                            else
                            {
                                pt1 = pl.GetPointAtParameter(i);
                                pt2 = pl.GetPointAtParameter(i + 1);
                            }

                            chord = Math.Sqrt(
                                Math.Pow((pt1.X - pt2.X), 2) +
                                Math.Pow((pt1.Y - pt2.Y), 2)
                                );
                            blg = Math.Tan(
                                Math.Asin(chord / (2 * 2 * plArcs[i].Radius)) / 2
                                );
                            if (plArcs[i].IsClockWise)
                            {
                                blg = -blg;
                            }
                            pl.SetBulgeAt(i, blg);
                        }

                        objectsCompleted++;
                    }

                    // Polyline3d
                    Polyline3d pl3d = ent as Polyline3d;
                    if (pl3d != null)
                    {
                        foreach (ObjectId vId in pl3d)
                        {
                            PolylineVertex3d pV3d = (PolylineVertex3d)tr.GetObject(vId, OpenMode.ForWrite);
                            pV3d.Position = new Point3d(pV3d.Position.X + 10, pV3d.Position.Y + 10, pV3d.Position.Z);
                        }
                        objectsCompleted++;
                    }

                    // Polyline 2d
                    Polyline2d pl2d = ent as Polyline2d;
                    if (pl2d != null)
                    {
                        foreach (ObjectId vId in pl2d)
                        {
                            Vertex2d pV2d = (Vertex2d)tr.GetObject(vId, OpenMode.ForWrite);
                            pV2d.Position = new Point3d(pV2d.Position.X + 10, pV2d.Position.Y + 10, 0);
                        }
                        objectsCompleted++;
                    }

                    // Line
                    Line ln = ent as Line;
                    if (ln != null)
                    {
                        ObjectId lnid = ln.Id;
                        ln = (Line)tr.GetObject(lnid, OpenMode.ForWrite);
                        ln.StartPoint = new Point3d(ln.StartPoint.X + 10, ln.StartPoint.Y + 10, ln.StartPoint.Z);
                        ln.EndPoint = new Point3d(ln.EndPoint.X + 10, ln.EndPoint.Y + 10, ln.EndPoint.Z);
                        objectsCompleted++;
                    }


                    // Hatch - will get back later
                    Hatch ht = ent as Hatch;
                    if (ht != null)
                    {
                        // Vector3d v3 = new Vector3d(1,1,1);
                        // v3.
                        //ht.GetStretchPoints();
                        // ht.set
                        // ht.AppendLoop(HatchLoopTypes.External, );
                        ObjectId htid = ht.Id;
                        Matrix3d matrixDisplacement = Matrix3d.Displacement(new Vector3d(10, 10, 0));
                        Matrix3d matrixScaling = Matrix3d.Scaling(0.5, new Point3d(ht.Origin.X + 10, ht.Origin.Y + 10, 0));
                        Matrix3d matrixRotation = Matrix3d.Rotation(
                            30,
                            new Vector3d(ht.Origin.X, ht.Origin.Y + 10, 0),
                            new Point3d(ht.Origin.X, ht.Origin.Y, 0)
                            );
                        ht = (Hatch)tr.GetObject(htid, OpenMode.ForWrite);
                        ht.TransformBy(matrixDisplacement);
                        ht.TransformBy(matrixScaling);
                        //ht.TransformBy(matrixRotation);
                        objectsCompleted++;
                    }


                    // Point
                    DBPoint pt = ent as DBPoint;
                    if (pt != null)
                    {
                        ObjectId ptid = pt.Id;
                        pt = (DBPoint)tr.GetObject(ptid, OpenMode.ForWrite);
                        pt.Position = new Point3d(pt.Position.X + 10, pt.Position.Y + 10, pt.Position.Z);
                        objectsCompleted++;
                    }

                    // Block
                    BlockReference bl = ent as BlockReference;
                    if (bl != null)
                    {
                        ObjectId blid = bl.Id;
                        bl = (BlockReference)tr.GetObject(blid, OpenMode.ForWrite);
                        bl.Position = new Point3d(bl.Position.X + 10, bl.Position.Y + 10, bl.Position.Z);
                        objectsCompleted++;
                    }

                    // DBText
                    DBText txt = ent as DBText;
                    if (txt != null)
                    {
                        ObjectId txtid = txt.Id;
                        txt = (DBText)tr.GetObject(txtid, OpenMode.ForWrite);
                        txt.Position = new Point3d(txt.Position.X + 10, txt.Position.Y + 10, txt.Position.Z);
                        objectsCompleted++;
                    }

                    // MText
                    MText mtxt = ent as MText;
                    if (mtxt != null)
                    {
                        ObjectId mtxtid = mtxt.Id;
                        mtxt = (MText)tr.GetObject(mtxtid, OpenMode.ForWrite);
                        mtxt.Location = new Point3d(mtxt.Location.X + 10, mtxt.Location.Y + 10, mtxt.Location.Z);
                        objectsCompleted++;
                    }


                    // Autodesk.AutoCAD.DatabaseServices.Polyline;
                    // Autodesk.AutoCAD.DatabaseServices.Polyline3d;
                    // Autodesk.AutoCAD.DatabaseServices.Polyline2d;
                    // Autodesk.AutoCAD.DatabaseServices.Line;
                    // Autodesk.AutoCAD.DatabaseServices.Hatch;
                    // Autodesk.AutoCAD.DatabaseServices.DBPoint;
                    // Autodesk.AutoCAD.DatabaseServices.BlockReference;
                    // Autodesk.AutoCAD.DatabaseServices.DBText;
                    // Autodesk.AutoCAD.DatabaseServices.MText;


                    
                    //worker.ReportProgress(progressPercentage);
                    //e.Result = progressPercentage;
                    //Thread.Sleep(100);
                    tr.Commit();
                    yield return progressPercentage = (objectsCompleted / selectedObjectsCount * 100);
                }
            //ed.WriteMessage("Преобразовано " + objectsCompleted.ToString() + " объектов\n");
            
        }
        //return objectsCompleted;
            //worker.ReportProgress(100);
            //worker.CancelAsync();
            //if (worker.CancellationPending == true)
            //{
            //    e.Cancel = true;
            //    return;
            //}
        }
    }
}
