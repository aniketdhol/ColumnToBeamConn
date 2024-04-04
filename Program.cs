using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tekla.Structures;
using Tekla.Structures.Model;
using Tekla.Structures.Geometry3d;
using Tekla.Structures.Model.UI;
using Tekla.Structures.Solid;
using System.Collections;

namespace ColumnToBeamConn
{
    internal class Program
    {
        static void Main(string[] args)
        {

            Model model = new Model();
            //// Connection trial 1
            if (model.GetConnectionStatus())
            {
                Picker picker = new Picker();
                Beam Column = picker.PickObject(Picker.PickObjectEnum.PICK_ONE_PART, "Pick a Column") as Beam;
                Beam Beam = picker.PickObject(Picker.PickObjectEnum.PICK_ONE_PART, "Pick a Beam") as Beam;
         
                CoordinateSystem localCoordinateSystem = FindMyCoordinateSystem(Column, Beam);
                //CoordinateSystem localCoordinateSystem = Column.GetCoordinateSystem();

                WorkPlaneHandler workPlaneHandler = model.GetWorkPlaneHandler();
                TransformationPlane currentTransformationPlane = workPlaneHandler.GetCurrentTransformationPlane();
                TransformationPlane desiredTransformationPlane = new TransformationPlane(localCoordinateSystem);
                workPlaneHandler.SetCurrentTransformationPlane(desiredTransformationPlane);
                try
                {
                    DrawCoordinateSystem("CDS");
                    double w = 177.8;
                    double h = 406.4;
                    ContourPoint point1 = new ContourPoint(new Point(w / 2, (-h / 2 - 12.7), 0), null);
                    ContourPoint point2 = new ContourPoint(new Point(w / 2, (h / 2+ 12.7), 0), null);
                    ContourPoint point3 = new ContourPoint(new Point(-w / 2, (h / 2+ 12.7) , 0), null);
                    ContourPoint point4 = new ContourPoint(new Point(-w / 2, (-h / 2- 12.7) , 0), null);

                    ContourPlate CP = new ContourPlate();  // Create a plate object for Fixing a plate

                    CP.AddContourPoint(point1);
                    CP.AddContourPoint(point2);
                    CP.AddContourPoint(point3);
                    CP.AddContourPoint(point4);
                    CP.Finish = "NO PAINT";
                    CP.Profile.ProfileString = "PL10";
                    CP.Material.MaterialString = "A36";
                    CP.Position.Depth = Position.DepthEnum.FRONT;

                    bool Result = false;
                    Result = CP.Insert();

                    Plane fittingPlane = new Plane();               // Create a plane object for Fixing a bolt
                    fittingPlane.Origin = new Point(0, 0, 10);
                    fittingPlane.AxisX = new Vector(1, 0, 0);
                    fittingPlane.AxisY = new Vector(0, 1, 0);


                    Fitting fitting = new Fitting();   // Create a fitting object
                    fitting.Plane = fittingPlane;
                    fitting.Father = Beam;
                    fitting.Insert();


                    BoltArray bolt = new BoltArray(); // Create a bolt object 
                    bolt.PartToBeBolted = CP;
                    bolt.PartToBoltTo = Column;

                    bolt.FirstPosition = new Point(0, h / 2, 0);   // Create a object for Fixing a bolt 
                    bolt.SecondPosition = new Point(0, -h / 2, 0); // Create a object for Fixing a bolt 

                    bolt.BoltSize = 19.05;                             // Create function for bolt dia 
                    bolt.Tolerance = 3.00;                         // Create function for bolt tolerance 
                    bolt.BoltStandard = "A325N_TC";                    // Create function for bolt grade 
                    bolt.BoltType = BoltGroup.BoltTypeEnum.BOLT_TYPE_SITE;    // Create function for bolt type 
                    bolt.CutLength = 76.2;                                      // Create function for bolt bolt cutlength 
                    

                    bolt.Length = 100;                                           // Create function for bolt length
                    bolt.ExtraLength = 6.35;                                        // Create function for bolt Extralength
                    bolt.ThreadInMaterial = BoltGroup.BoltThreadInMaterialEnum.THREAD_IN_MATERIAL_YES;   // Create function for bolt threadMaterial (yes or no) 

                    bolt.Position.Depth = Position.DepthEnum.MIDDLE;                     // Create function for bolt position.depth 
                    bolt.Position.Plane = Position.PlaneEnum.MIDDLE;                      // Create function for bolt position.plane
                    bolt.Position.Rotation = Position.RotationEnum.FRONT;                   // Create function for bolt position.rotation

                    bolt.Bolt = true;
                    bolt.Washer1 = true;
                    bolt.Washer2 = false;
                    bolt.Washer3 = false;
                    bolt.Nut1 = true;
                    bolt.Nut2 = false;

                    bolt.Hole1 = true;
                    bolt.Hole2 = true;
                    bolt.Hole3 = true;
                    bolt.Hole4 = true;
                    bolt.Hole5 = true;

                    bolt.StartPointOffset.Dx = 76.2; //first Hole Distance 
                    bolt.AddBoltDistX(76.2); //     pitch distance
                    bolt.AddBoltDistX(76.2); //     pitch distance
                    bolt.AddBoltDistX(76.2); //     pitch distance

                    bolt.AddBoltDistY(88.9); //     pitch guage distance
                   // bolt.AddBoltDistY(44.45);

                    if (!bolt.Insert())
                        Console.WriteLine("BoltArray Insert failed!");

                    Weld weld = new Weld();

                    weld.SizeAbove = 6.35;
                    weld.SizeBelow = 6.35;
                    weld.ShopWeld = true;
                    weld.MainObject = Beam;
                    weld.SecondaryObject = CP;
                    weld.TypeBelow = BaseWeld.WeldTypeEnum.WELD_TYPE_FILLET;
                    weld.TypeAbove = BaseWeld.WeldTypeEnum.WELD_TYPE_FILLET;
                   // weld.EffectiveThroatAbove = 6.35;
                   // weld.EffectiveThroatBelow = 6.35;
                    weld.AroundWeld = false;
                    weld.Position = Weld.WeldPositionEnum.WELD_POSITION_PLUS_Y;
                    // weld.Placement = BaseWeld.WeldPlacementTypeEnum.PLACEMENT_AUTO;
                    //Weld.SizeAbove = 6.35;

                    Weld weld2 = new Weld();

                    weld2.SizeAbove = 6.35;
                    weld2.SizeBelow = 6.35;
                    weld2.ShopWeld = true;
                    weld2.MainObject = Beam;
                    weld2.SecondaryObject = CP;
                    weld2.TypeBelow = BaseWeld.WeldTypeEnum.WELD_TYPE_FILLET;
                    weld2.TypeAbove = BaseWeld.WeldTypeEnum.WELD_TYPE_FILLET;
                    weld2.AroundWeld = false;
                    weld2.Position = Weld.WeldPositionEnum.WELD_POSITION_PLUS_X;

                    Weld weld3 = new Weld();
                    weld3.SizeAbove = 6.35;
                    weld3.SizeBelow = 6.35;
                    weld3.ShopWeld = true;
                    weld3.MainObject = Beam;
                    weld3.SecondaryObject = CP;
                    weld3.TypeAbove = BaseWeld.WeldTypeEnum.WELD_TYPE_FILLET;
                    weld3.TypeBelow = BaseWeld.WeldTypeEnum.WELD_TYPE_FILLET;
                    weld3.AroundWeld = false;
                    weld3.Position = Weld.WeldPositionEnum.WELD_POSITION_MINUS_Y;
                    
                    
                    weld.Insert();
                    weld2.Insert();
                    weld3.Insert();

                    //PolygonWeld polygonWeld = new PolygonWeld();
                    //polygonWeld.MainObject = CP;
                    //polygonWeld.SecondaryObject = Beam;
                    //Point WeldP1 = new Point(9000, 11850, 0);
                    //Point WeldP2 = new Point(9000, 12000, 0);
                    //Point WeldP3 = new Point(9150, 12000, 0);

                    //polygonWeld.Polygon.Points.Add(WeldP1);
                    //polygonWeld.Polygon.Points.Add(WeldP2);
                    //polygonWeld.Polygon.Points.Add(WeldP3);

                    //polygonWeld.TypeAbove = PolygonWeld.WeldTypeEnum.WELD_TYPE_SQUARE_GROOVE_SQUARE_BUTT;

                    //polygonWeld.Insert();

                    //polygonWeld.TypeBelow = PolygonWeld.WeldTypeEnum.WELD_TYPE_SLOT;

                    //polygonWeld.Modify();



                    //Beam Beamcp = picker.PickObject(Picker.PickObjectEnum.PICK_ONE_PART, "Pick a Beam_cp") as Beam;
                    //Beam Beam = picker.PickObject(Picker.PickObjectEnum.PICK_ONE_PART, "Pick a Beam") as Beam;

                    ////Weld.ConnectAssemblies = true;


                    //Vector xVector = new Vector(1, 0, 0);
                    //Vector yVector = new Vector(0, 1, 0);



                    //Weld Weld2 = new Weld();
                    //Weld2.MainObject = CP;
                    //Weld2.SecondaryObject = Beam;
                    //Weld2.Position = Weld.WeldPositionEnum.WELD_POSITION_PLUS_Y;
                    //Weld2.SizeAbove = 7.9375;
                    //Weld2.SizeBelow = 7.9375;


                    //Weld2.Insert();

                    weld.Modify();
                    //Weld2.Modify();

                    //Weld.LengthAbove = 0;
                    //Weld.TypeBelow = BaseWeld.WeldTypeEnum.WELD_TYPE_FILLET;
                    //Weld.SizeAbove = 6.35;





                    model.CommitChanges();
                }
                finally
                {
                    workPlaneHandler.SetCurrentTransformationPlane(currentTransformationPlane);
                }
            }
            else
            {
                Console.WriteLine("Tekla is not connected");
            }
            Console.ReadLine();
        }

        private static void DrawCoordinateSystem(string v)
        {
            double value = 1000;
            Vector xVector = new Vector(1, 0, 0);
            Vector yVector = new Vector(0, 1, 0);
            LineSegment xline = new LineSegment(new Point(), (new Point() + xVector * value));
            LineSegment yline = new LineSegment(new Point(), (new Point() + yVector * value));
            new GraphicsDrawer().DrawText(new Point(), v, new Color());
            new GraphicsDrawer().DrawLineSegment(xline.Point1, xline.Point2, new Color(1, 0, 0));
            new GraphicsDrawer().DrawText(xline.Point2, "X", new Color(1, 0, 0));
            new GraphicsDrawer().DrawLineSegment(yline.Point1, yline.Point2, new Color(0, 1, 0));
            new GraphicsDrawer().DrawText(yline.Point2, "Y", new Color(0, 1, 0));
        }

        public static CoordinateSystem FindMyCoordinateSystem(Beam Column, Beam beam)
        {
            bool isOnTheFlange = false;

            ///find centerLIne of Beam
            ArrayList beamPoints = beam.GetCenterLine(true);
            Point firstPt = beamPoints[0] as Point;
            Point secondpt = beamPoints[1] as Point;
            Line beamCenterLine = new Line(firstPt, secondpt);
            Vector beamDirection = beamCenterLine.Direction;
            Vector webDirection = Column.GetCoordinateSystem().AxisX.Cross(Column.GetCoordinateSystem().AxisY).GetNormal();
            Vector flangeDirection = Column.GetCoordinateSystem().AxisY.GetNormal();


            if (Math.Round(webDirection.GetAngleBetween(beamDirection), 2) == 3.14 / 2)
            {
                isOnTheFlange = true;
            }

            ///find web plane inside plane of column
            FaceEnumerator faceenumerator = Column.GetSolid().GetFaceEnumerator();
            List<Face> lstFace = new List<Face>();
            while (faceenumerator.MoveNext())
            {
                var current = faceenumerator.Current;
                lstFace.Add(current);
            }

            ///parallla to beam faces
            List<Face> parllaltoBeam = new List<Face>();
            foreach (Face face in lstFace)
            {
                if (Tekla.Structures.Geometry3d.Parallel.VectorToVector(isOnTheFlange ? flangeDirection : webDirection, face.Normal))
                {
                    parllaltoBeam.Add(face);
                }
            }
            Point clmcp = GetCenterOfPart(Column);
            Point bmcp = GetCenterOfPart(beam);
            Vector columntoBeam = new Vector(bmcp - clmcp);

            List<Face> threeFaceList = new List<Face>();
            foreach (Face face in parllaltoBeam)
            {
                if (face.Normal.Dot(columntoBeam) > 0)
                {
                    threeFaceList.Add(face);
                }
            }
            List<double> perimeters = new List<double>();
            foreach (Face item in threeFaceList)
            {
                double curretntpremeter = FindPerimeterofFace(item);
                perimeters.Add(curretntpremeter);
            }

            Face webFace = FindMaxFacePerimeter(perimeters, threeFaceList);
            List<Point> webFacePts = FindPointsofFace(webFace);
            GeometricPlane webgeometricPlane = new GeometricPlane();
            webgeometricPlane.Normal = webFace.Normal;
            webgeometricPlane.Origin = webFacePts.FirstOrDefault();

            Point IntersectionPoint = Intersection.LineToPlane(beamCenterLine, webgeometricPlane);
            new GraphicsDrawer().DrawText(IntersectionPoint, "<<<<====Origin", new Color());

            //CoordinateSystem coordinateSystem = new CoordinateSystem();
            //coordinateSystem.Origin = IntersectionPoint;
            //coordinateSystem.AxisX = Column.GetCoordinateSystem().AxisY;
            //coordinateSystem.AxisY = beam.GetCoordinateSystem().AxisY;

            CoordinateSystem coordinateSystem = new CoordinateSystem();
            coordinateSystem.Origin = IntersectionPoint;
            coordinateSystem.AxisX = Column.GetCoordinateSystem().AxisX.GetNormal().Cross(webFace.Normal.GetNormal());
            coordinateSystem.AxisY = Column.GetCoordinateSystem().AxisX.GetNormal();

            return coordinateSystem;
        }
        private static Face FindMaxFacePerimeter(List<double> perimeters, List<Face> threeFaceList)
        {
            double max = 0;
            int maxperimeterIndex = 0;
            for (int i = 0; i < perimeters.Count; i++)
            {
                if (perimeters[i] > max)
                {
                    max = perimeters[i];
                    maxperimeterIndex = i;
                }
            }
            return threeFaceList[maxperimeterIndex];
        }

        public static double FindPerimeterofFace(Face face)
        {
            double perimeter = 0;
            List<Point> mypoints = new List<Point>();
            mypoints = FindPointsofFace(face);
            perimeter = FindPerimeterFromPoint(mypoints);
            return perimeter;

        }

        private static List<Point> FindPointsofFace(Face face)
        {
            LoopEnumerator loopEnum = face.GetLoopEnumerator();
            Loop myloop = null;
            while (loopEnum.MoveNext())
            {
                myloop = loopEnum.Current;
            }
            VertexEnumerator myvertexEnum = myloop.GetVertexEnumerator();
            List<Point> mypoints = new List<Point>();
            while (myvertexEnum.MoveNext())
            {
                Point currentPt = myvertexEnum.Current;
                mypoints.Add(currentPt);
            }
            return mypoints;
        }

        private static double FindPerimeterFromPoint(List<Point> mypoints)
        {
            double myPertimeter = 0;
            for (int i = 0; i < mypoints.Count; i++)
            {
                Point pt1 = mypoints[i];
                int nextIndext = 0;
                if (i == mypoints.Count - 1)
                {
                    nextIndext = 0;
                }
                else
                {
                    nextIndext = i + 1;
                }
                Point pt2 = mypoints[nextIndext];
                double currentDistance = Distance.PointToPoint(pt1, pt2);
                myPertimeter = myPertimeter + currentDistance;
            }
            return myPertimeter;
        }

        public static Point GetCenterOfPart(Beam part)
        {
            Solid solid = part.GetSolid();
            Point pt1 = solid.MinimumPoint;
            Point pt2 = solid.MaximumPoint;

            Point resultpt = new Point();
            resultpt.X = 0.5 * (pt1.X + pt2.X);
            resultpt.Y = 0.5 * (pt1.Y + pt2.Y);
            resultpt.Z = 0.5 * (pt1.Z + pt2.Z);

            return resultpt;
        }
    }
    
}

