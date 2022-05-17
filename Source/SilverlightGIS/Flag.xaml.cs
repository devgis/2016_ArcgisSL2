using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

using ESRI.ArcGIS.Client;
using System.Windows.Media.Imaging;
using ESRI.ArcGIS.Client.Geometry;

namespace TrackPlayback
{
    public partial class Flag : UserControl
    {
        private Map _pMap = null;
        private int count = 0;
        private ESRI.ArcGIS.Client.Geometry.PointCollection pCol = null;
        private Storyboard sb = new Storyboard();
        DoubleAnimation dba;
        DoubleAnimation dba1;
        private double lastTan;

        private Graphic _Route = null;
        public Graphic Route
        {
            get { return _Route; }
            set { _Route = value; }
        }

        public Flag()
        {
            InitializeComponent();
        }

        public string ImageSource
        {
            set {
                flagImage.Source = new BitmapImage(new Uri(value, UriKind.Relative));
            }
        }
        public Map BindMap
        {
            set { _pMap = value; }
            get { return _pMap; }
        }
        public static readonly DependencyProperty Xproperty =
            DependencyProperty.Register("X", typeof(double), typeof(Flag), new PropertyMetadata(OnXchanged));
        public double X
        {
            get { return (double)base.GetValue(Xproperty); }
            set { base.SetValue(Xproperty, value); ResetEnvelop(); }
        }
        public static void OnXchanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            (sender as Flag).X = (double)e.NewValue;
        }
        public static readonly DependencyProperty Yproperty =
            DependencyProperty.Register("Y", typeof(double), typeof(Flag), new PropertyMetadata(OnYChanged));
        public double Y
        {
            get { return (double)base.GetValue(Yproperty); }
            set
            {
                base.SetValue(Yproperty, value);
                ResetEnvelop();
            }
            
        }
        private static void OnYChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            (sender as Flag).Y=(double)e.NewValue;
        }
        private ElementLayer _bindLayer = null;
        public ElementLayer BindLayer
        {
            get { return _bindLayer; }
            set { _bindLayer = value; }
        }
        private void ResetEnvelop()
        {
            Flag ele = (Flag)_bindLayer.Children[0];
            ElementLayer.SetEnvelope(ele, new ESRI.ArcGIS.Client.Geometry.Envelope(X, Y, X, Y));
            if (_pMap != null)
            {
                ESRI.ArcGIS.Client.Geometry.Polygon gon = new ESRI.ArcGIS.Client.Geometry.Polygon();
                ESRI.ArcGIS.Client.Geometry.PointCollection con = new ESRI.ArcGIS.Client.Geometry.PointCollection();
            }
        }

        private double _Interval = 1;
        /// <summary>
        /// 移动时间间隔 默认1
        /// </summary>
        public double Interval
        {
            get { return _Interval; }
            set { _Interval = value; }
        }
        public void Start()
        {
            if (_pMap != null)
            {
                _pMap.ZoomTo(_Route.Geometry);
            }
            if (_bindLayer.Children.Count > 0) _bindLayer.Children.RemoveAt(0);
            _bindLayer.Children.Add(this);
            ESRI.ArcGIS.Client.Geometry.Polyline line = _Route.Geometry as ESRI.ArcGIS.Client.Geometry.Polyline;
            pCol = line.Paths[0];
            MapPoint pt1 = pCol[0];
            MapPoint pt2 = pCol[1];
            double angle = CalulateXYAnagel(pt1.X, pt1.Y, pt2.X, pt2.Y);
            RotateItemCanvas.Angle = angle;
            ElementLayer.SetEnvelope(this, pt1.Extent);
            if (dba == null)
            {
                dba = new DoubleAnimation();
                Storyboard.SetTarget(dba, this);
                Storyboard.SetTargetProperty(dba, new PropertyPath("X"));
                sb.Children.Add(dba);
                dba1 = new DoubleAnimation();
                Storyboard.SetTarget(dba1, this);
                Storyboard.SetTargetProperty(dba1, new PropertyPath("Y"));
                sb.Children.Add(dba1);
            }
            sb.Completed+=sb_Completed;
            dba.From = pt1.X;
            dba.To = pt2.X;
            dba.Duration = new Duration(TimeSpan.FromSeconds(_Interval));
            dba1.From = pt1.Y;
            dba1.To = pt2.Y;
            dba1.Duration = new Duration(TimeSpan.FromSeconds(_Interval));
            sb.Begin();
        }
        private void sb_Completed(object sender, EventArgs e)
        {
            if (count < pCol.Count - 2)
            {
                count++;
                MapPoint pt1 = pCol[count];
                MapPoint pt2 = pCol[count + 1];
                DoubleAnimation db = (DoubleAnimation)(sender as Storyboard).Children[0];
                db.From = pt1.X;
                db.To = pt2.X;
                double angle = CalulateXYAnagel(pt1.X, pt1.Y, pt2.X, pt2.Y);
                RotateItemCanvas.Angle = angle;
                DoubleAnimation db1 = (DoubleAnimation)(sender as Storyboard).Children[1];
                db1.From = pt1.Y;
                db1.To = pt2.Y;
                sb.Begin();
            }
        }
        /// <summary>
        /// 停止
        /// </summary>
        public void Stop()
        {
            sb.Stop();
        }
        /// <summary>
        /// 暂停
        /// </summary>
        public void Pause()
        {
            sb.Pause();
        }
        /// <summary>
        /// 重新开始
        /// </summary>
        public void Resume()
        {
            sb.Resume();
        }
        /// <summary>
        /// x轴正方向为0度，顺时针为正
        /// </summary>
        /// <param name="startx"></param>
        /// <param name="starty"></param>
        /// <param name="endx"></param>
        /// <param name="endy"></param>
        /// <returns></returns>
        public double CalulateXYAnagel(double startx, double starty, double endx, double endy)
        {
            
            double tan = Math.Atan(Math.Abs((endy - starty) / (endx - startx))) * 180 / Math.PI;
            double returnTan=0;
            if (endx > startx && endy > starty)//第一象限
            {
                returnTan= - tan;
            }
            else if (endx > startx && endy < starty)//第二象限
            {
                returnTan= tan;
            }
            else if (endx < startx && endy > starty)//第三象限
            {
                returnTan= tan - 180;
            }
            else if (endx > startx && endy == starty)//x轴正方向
            {
                returnTan = 0;
            }
            else if (endx < startx && endy == starty)//x轴负方向
            {
                returnTan = 180;
            }
            else if (endx == startx && endy > starty)//y轴正方向
            {
                returnTan = -90;
            }
            else if (endx == startx && endy < starty)//y轴正方向
            {
                returnTan = 90;
            }
            else
            {
                returnTan = 180 - tan;
            }
            return returnTan;
        }
    }
}
