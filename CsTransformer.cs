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
using System.ComponentModel;
using System.Linq.Expressions;
using DotSpatial.Projections;

namespace AcadCsObjectsTransform
{
    public class CsTransformer
    {
        public int objectsCompleted = 0;
        public ProjectionInfo crsInitial;
        public ProjectionInfo crsTarget;

        public IEnumerable<int> Transform()
        {
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
            if (psr.Status != PromptStatus.OK)
            {
               yield return -1;
            }

            int selectedObjectsNumber = psr.Value.Count;

            // compute scaling between new and old coordinates
            // dbt - distance between 2 points before transformation
            // dat - distance between 2 points after transformation
            double[] xy = new double[] { 0, 0, 10, 10 };
            double[] z = new double[] { 0, 0 };
            double dbt = Math.Sqrt(
                Math.Pow(xy[0] - xy[2], 2) +
                Math.Pow(xy[1] - xy[3], 2)
                );
            Reproject.ReprojectPoints(xy, z, crsInitial, crsTarget, 0, 2);
            double dat = Math.Sqrt(
                Math.Pow(xy[0] - xy[2], 2) +
                Math.Pow(xy[1] - xy[3], 2)
                );
            double scalingFactor = dat / dbt;

            // compute new cs rotation
            // set (10,10) point in initial cs. get it's tilt angle
            // transform the point and get new tilt angle. then find the angles diff
            double initCsAngleDeg = Math.Acos(10 / Math.Sqrt(200));
            xy = new double[] { 0, 0, 10, 10 };
            z = new double[] { 0, 0 };
            Reproject.ReprojectPoints(xy, z, crsInitial, crsTarget, 0, 2);
            double targetAngleDeg = Math.Acos(
                (xy[2] - xy[0]) / Math.Sqrt(
                    Math.Pow(xy[0] - xy[2], 2) +
                    Math.Pow(xy[1] - xy[3], 2)
                    )
                );
            double csRotation = targetAngleDeg - initCsAngleDeg;


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
            List<double> reprojectedXYZ = new List<double>();
            
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
                        {
                            plPts.Add(pl.GetPoint2dAt(i));
                        }


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
                            reprojectedXYZ = reprojectPoint(plPts[i].X, plPts[i].Y, pl.Elevation);
                            pl.SetPointAt(i, new Point2d(
                                reprojectedXYZ[0], reprojectedXYZ[1]
                                ));
                        }

                        // cases for arc parts of polyline
                        /*foreach (int i in arcInds)
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
                                Math.Asin(chord * scalingFactor / (2 * plArcs[i].Radius)) / 2
                                );
                            if (plArcs[i].IsClockWise)
                            {
                                blg = -blg;
                            }
                            pl.SetBulgeAt(i, blg);
                        }*/

                        objectsCompleted++;
                    }

                    // Polyline3d
                    Polyline3d pl3d = ent as Polyline3d;
                    if (pl3d != null)
                    {
                        foreach (ObjectId vId in pl3d)
                        {
                            PolylineVertex3d pV3d = (PolylineVertex3d)tr.GetObject(vId, OpenMode.ForWrite);
                            reprojectedXYZ = reprojectPoint(pV3d.Position.X, pV3d.Position.Y, pV3d.Position.Z);
                            pV3d.Position = new Point3d(
                                reprojectedXYZ[0], reprojectedXYZ[1], reprojectedXYZ[2]
                                );
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
                            reprojectedXYZ = reprojectPoint(pV2d.Position.X, pV2d.Position.Y, pl2d.Elevation);
                            pV2d.Position = new Point3d(
                                reprojectedXYZ[0], reprojectedXYZ[1], reprojectedXYZ[2]);
                        }

                        objectsCompleted++;
                    }

                    // Line
                    Line ln = ent as Line;
                    if (ln != null)
                    {
                        ObjectId lnid = ln.Id;
                        ln = (Line)tr.GetObject(lnid, OpenMode.ForWrite);
                        reprojectedXYZ = reprojectPoint(ln.StartPoint.X, ln.StartPoint.Y, ln.StartPoint.Z);
                        ln.StartPoint = new Point3d(
                            reprojectedXYZ[0], reprojectedXYZ[1], reprojectedXYZ[2]
                            );
                        reprojectedXYZ = reprojectPoint(ln.EndPoint.X, ln.EndPoint.Y, ln.EndPoint.Z);
                        ln.EndPoint = new Point3d(
                            reprojectedXYZ[0], reprojectedXYZ[1], reprojectedXYZ[2]
                            );

                        objectsCompleted++;
                    }

                    // Hatch - will get back later
                    Hatch ht = ent as Hatch;
                    if (ht != null)
                    {
                        ObjectId htid = ht.Id;
                        ht = (Hatch)tr.GetObject(htid, OpenMode.ForWrite);

                        reprojectedXYZ = reprojectPoint(0, 0, 0);
                        Matrix3d htMatrixDisplacement = Matrix3d.Displacement(new Vector3d(
                            reprojectedXYZ[0], reprojectedXYZ[1], reprojectedXYZ[2]
                            ));
                        Matrix3d htMatrixScaling = Matrix3d.Scaling(
                            scalingFactor,
                            new Point3d(0, 0, ht.Elevation)
                            );
                        Matrix3d htMatrixRotation = Matrix3d.Rotation(
                            csRotation,
                            new Vector3d(0, 0, 1),
                            new Point3d(0, 0, ht.Elevation)
                            );

                        ht.TransformBy(htMatrixRotation);
                        ht.TransformBy(htMatrixScaling);
                        ht.TransformBy(htMatrixDisplacement);

                        objectsCompleted++;
                    }


                    // Point
                    DBPoint pt = ent as DBPoint;
                    if (pt != null)
                    {
                        ObjectId ptid = pt.Id;
                        pt = (DBPoint)tr.GetObject(ptid, OpenMode.ForWrite);
                        reprojectedXYZ = reprojectPoint(pt.Position.X, pt.Position.Y, pt.Position.Z);
                        pt.Position = new Point3d(
                            reprojectedXYZ[0], reprojectedXYZ[1], reprojectedXYZ[2]
                            );

                        objectsCompleted++;
                    }

                    // Block
                    BlockReference bl = ent as BlockReference;
                    if (bl != null)
                    {
                        ObjectId blid = bl.Id;
                        bl = (BlockReference)tr.GetObject(blid, OpenMode.ForWrite);
                        reprojectedXYZ = reprojectPoint(bl.Position.X, bl.Position.Y, bl.Position.Z);
                        bl.Position = new Point3d(
                            reprojectedXYZ[0], reprojectedXYZ[1], reprojectedXYZ[2]
                            );

                        if (scalingFactor > 2 | scalingFactor < 0.5)
                        {
                            Matrix3d blMatrixScaling = Matrix3d.Scaling(
                                 scalingFactor,
                                 new Point3d(bl.Position.X, bl.Position.Y, bl.Position.Z)
                                );
                            bl.TransformBy(blMatrixScaling);
                        }

                        objectsCompleted++;
                    }

                    // DBText
                    DBText txt = ent as DBText;
                    if (txt != null)
                    {
                        ObjectId txtid = txt.Id;
                        txt = (DBText)tr.GetObject(txtid, OpenMode.ForWrite);
                        reprojectedXYZ = reprojectPoint(txt.Position.X, txt.Position.Y, txt.Position.Z);
                        txt.Position = new Point3d(
                            reprojectedXYZ[0], reprojectedXYZ[1], reprojectedXYZ[2]
                            );

                        if (scalingFactor > 2 | scalingFactor < 0.5)
                        {
                            txt.Height = txt.Height * scalingFactor;
                        }

                        if (txt.Rotation != 0)
                        {
                            txt.Rotation += csRotation;
                        }

                        objectsCompleted++;
                    }

                    // MText
                    MText mtxt = ent as MText;
                    if (mtxt != null)
                    {
                        ObjectId mtxtid = mtxt.Id;
                        mtxt = (MText)tr.GetObject(mtxtid, OpenMode.ForWrite);
                        reprojectedXYZ = reprojectPoint(mtxt.Location.X, mtxt.Location.Y, mtxt.Location.Z);
                        mtxt.Location = new Point3d(
                            reprojectedXYZ[0], reprojectedXYZ[1], reprojectedXYZ[2]
                            );

                        if (scalingFactor > 2 | scalingFactor < 0.5)
                        {
                            Matrix3d mtxtMatrixScaling = Matrix3d.Scaling(
                                 scalingFactor,
                                 new Point3d(mtxt.Location.X, mtxt.Location.Y, mtxt.Location.Z)
                                );
                            mtxt.TransformBy(mtxtMatrixScaling);
                        }

                        if (mtxt.Rotation != 0)
                        {
                            mtxt.Rotation += csRotation;
                        }

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
  
                    tr.Commit();
                    yield return objectsCompleted / selectedObjectsNumber * 100;
                }
            }
        }
        public List<double> reprojectPoint(double ptX, double ptY, double ptZ)
        {
            double[] xy = new double[2] { ptX, ptY };
            double[] z = new double[1] { ptZ };
            List<double> reprojectedPtList = new List<double>();

            Reproject.ReprojectPoints(xy, z, crsInitial, crsTarget, 0, 1);

            reprojectedPtList.Add(xy[0]);
            reprojectedPtList.Add(xy[1]);
            reprojectedPtList.Add(z[0]);
            return reprojectedPtList;
        }
    }
}
