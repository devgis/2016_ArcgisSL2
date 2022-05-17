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
using ESRI.ArcGIS.Client.Tasks;

namespace TrackPlayback
{
    public partial class TrackPage : ChildWindow
    {
        GraphicsLayer stopsGraphicsLayer;
        GraphicsLayer routeGraphicsLayer;
        RouteTask routeTask;

        Graphic lastRoute = null;

        public TrackPage()
        {
            InitializeComponent();

            stopsGraphicsLayer = MyMap.Layers["MyStopsGraphicsLayer"] as GraphicsLayer;
            routeGraphicsLayer = MyMap.Layers["MyRouteGraphicsLayer"] as GraphicsLayer;
            routeTask = LayoutRoot.Resources["MyRouteTask"] as RouteTask;

        }

        private void playback_Click_1(object sender, RoutedEventArgs e)
        {
            if (this.lastRoute == null)
                return;
            ElementLayer eleLayer = MyMap.Layers["MoveCarLayer"] as ElementLayer;
            ESRI.ArcGIS.Client.Geometry.Polyline pline = lastRoute.Geometry as ESRI.ArcGIS.Client.Geometry.Polyline;

            Flag iele = new Flag();
            iele.BindMap = MyMap;
            iele.BindLayer = eleLayer;
            iele.Interval = 1;
            iele.Route = this.lastRoute;
            iele.Start();
        }

        private void MyMap_MouseClick_1(object sender, Map.MouseEventArgs e)
        {
            Graphic stop = new Graphic() { Geometry = e.MapPoint };

            stopsGraphicsLayer.Graphics.Add(stop);

            if (stopsGraphicsLayer.Graphics.Count > 1)
            {
                if (routeTask.IsBusy)
                {
                    routeTask.CancelAsync();
                    stopsGraphicsLayer.Graphics.RemoveAt(stopsGraphicsLayer.Graphics.Count - 1);
                }
                routeTask.SolveAsync(new RouteParameters()
                {
                    Stops = stopsGraphicsLayer,
                    UseTimeWindows = false,
                    OutSpatialReference = MyMap.SpatialReference
                });
            }

        }

        private void MyRouteTask_Failed(object sender, TaskFailedEventArgs e)
        {
            string errorMessage = "Routing error: ";
            errorMessage += e.Error.Message;
            foreach (string detail in (e.Error as ServiceException).Details)
                errorMessage += "," + detail;

            MessageBox.Show(errorMessage);

            stopsGraphicsLayer.Graphics.RemoveAt(stopsGraphicsLayer.Graphics.Count - 1);

        }

        private void MyRouteTask_SolveCompleted(object sender, RouteEventArgs e)
        {
           
            routeGraphicsLayer.Graphics.Clear();

            RouteResult routeResult = e.RouteResults[0];

            Graphic lastRoute = routeResult.Route;

            decimal totalTime = (decimal)lastRoute.Attributes["Total_TravelTime"];
            TimeSpan totalTimeSpan = TimeSpan.FromMinutes(Decimal.ToDouble(totalTime));
            TotalTimeTextBlock.Text = totalTimeSpan.Minutes.ToString();

            decimal totalDistance = (decimal)lastRoute.Attributes["Shape_Length"];
            // convert meters into miles
            double totalDistanceMiles = Decimal.ToDouble(totalDistance) * 0.0006213700922;
            TotalDistanceTextBlock.Text = totalDistanceMiles.ToString("#0.0");

            routeGraphicsLayer.Graphics.Add(lastRoute);
            this.lastRoute = lastRoute;

        }
    }
}
