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
using ESRI.ArcGIS.Client.Geometry;
using ESRI.ArcGIS.Client.Symbols;
using ESRI.ArcGIS.Client.Tasks;
using SilverlightGIS.Information;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using TrackPlayback;
using SilverlightGIS.Searchs;
using System.Data;
using SilverlightGIS.MyService;

namespace SilverlightGIS
{
    public partial class MainPage : UserControl
    {
        public string AnjianLayerURL = "http://192.168.1.100:6080/arcgis/rest/services/NJ/MapServer/4";
        public string JingJuLayerURL = "http://192.168.1.100:6080/arcgis/rest/services/NJ/MapServer/0";
        public static string UserName = string.Empty;
        List<ThematicItem> ThematicItemList = new List<ThematicItem>();
        List<List<SolidColorBrush>> ColorList = new List<List<SolidColorBrush>>();
        int _colorShadeIndex = 0;
        int _thematicListIndex = 0;
        FeatureSet _featureSet = null;
        int _classType = 0; // EqualInterval = 1; Quantile = 0;
        int _classCount = 6;
        int _lastGeneratedClassCount = 0;
        bool _legendGridCollapsed;
        bool _classGridCollapsed;
        public MainPage()
        {
            InitializeComponent();
            GraphicsLayer layer = MainMap.Layers["QueryResultLayer"] as GraphicsLayer;
            //layer.SpatialReference = MainMap.SpatialReference;
            //MyNavgation.Map = MainMap;
            //MyScaleLine.Map = MainMap;
            //MyOverviewMap.Map = MainMap;
            this.Loaded += MainPage_Loaded;
            

        }

        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            
        }
        Random rd = new Random();
        private void SetRangeValues()
        {
            
            GraphicsLayer graphicsLayer = MainMap.Layers["QueryResultLayer"] as GraphicsLayer;

            // if necessary, update ColorList based on current number of classes.
            if (_lastGeneratedClassCount != _classCount) CreateColorList();

            // Field on which to generate a classification scheme.  
            ThematicItem thematicItem = new ThematicItem() { Name = "区域按键数量", Description = "区域按键数量 ", CalcField = "" }; //ThematicItemList[_thematicListIndex];

            // Calculate value for classification scheme
            bool useCalculatedValue = !string.IsNullOrEmpty(thematicItem.CalcField);

            // Store a list of values to classify
            List<double> valueList = new List<double>();

            // Get range, min, max, etc. from features
            for (int i = 0; i < _featureSet.Features.Count; i++)
            {
                Graphic graphicFeature = _featureSet.Features[i];

                double graphicValue = Convert.ToDouble(graphicFeature.Attributes[thematicItem.Name]);
                string graphicName = graphicFeature.Attributes["地区"].ToString(); //Name

                //if (useCalculatedValue)
                //{
                //    double calcVal = Convert.ToDouble(graphicFeature.Attributes[thematicItem.CalcField]);
                //    graphicValue = Math.Round(graphicValue / calcVal * 100, 2);
                //}
                double calcVal = Convert.ToDouble(graphicFeature.Attributes[thematicItem.CalcField]);
                graphicValue = Math.Round(graphicValue / calcVal * 100, 2);

                if (i == 0)
                {
                    thematicItem.Min = graphicValue;
                    thematicItem.Max = graphicValue;
                    thematicItem.MinName = graphicName;
                    thematicItem.MaxName = graphicName;
                }
                else
                {
                    if (graphicValue < thematicItem.Min) { thematicItem.Min = graphicValue; thematicItem.MinName = graphicName; }
                    if (graphicValue > thematicItem.Max) { thematicItem.Max = graphicValue; thematicItem.MaxName = graphicName; }
                }

                valueList.Add(graphicValue);
            }

            // Set up range start values
            thematicItem.RangeStarts = new List<double>();

            double totalRange = thematicItem.Max - thematicItem.Min;
            double portion = totalRange / _classCount;

            thematicItem.RangeStarts.Add(thematicItem.Min);
            double startRangeValue = thematicItem.Min;

            //for (int i = 1; i < _classCount; i++)
            //{
            //    startRangeValue += portion;
            //    thematicItem.RangeStarts.Add(startRangeValue);
            //}
            thematicItem.RangeStarts.Add(50);
            thematicItem.RangeStarts.Add(100);
            thematicItem.RangeStarts.Add(200);
            thematicItem.RangeStarts.Add(400);
            thematicItem.RangeStarts.Add(600);
            thematicItem.RangeStarts.Add(800);

            //// Equal Interval
            //if (_classType == 1)
            //{
            //    for (int i = 1; i < _classCount; i++)
            //    {
            //        startRangeValue += portion;
            //        thematicItem.RangeStarts.Add(startRangeValue);
            //    }
            //}
            //// Quantile
            //else
            //{
            //    // Enumerator of all values in ascending order
            //    IEnumerable<double> valueEnumerator =
            //    from aValue in valueList
            //    orderby aValue //"ascending" is default
            //    select aValue;

            //    int increment = Convert.ToInt32(Math.Ceiling(_featureSet.Features.Count / _classCount));
            //    for (int i = increment; i < valueList.Count; i += increment)
            //    {
            //        double value = valueEnumerator.ElementAt(i);
            //        thematicItem.RangeStarts.Add(value);
            //    }
            //}

            // Create graphic features and set symbol using the class range which contains the value 
            List<SolidColorBrush> brushList = ColorList[_colorShadeIndex];
            if (_featureSet != null && _featureSet.Features.Count > 0)
            {
                // Clear previous graphic features
                graphicsLayer.Graphics.Clear();

                for (int i = 0; i < _featureSet.Features.Count; i++)
                {
                    Graphic graphicFeature = _featureSet.Features[i];

                    //double graphicValue = Convert.ToDouble(graphicFeature.Attributes[thematicItem.Name]);
                    //if (useCalculatedValue)
                    //{
                    //    double calcVal = Convert.ToDouble(graphicFeature.Attributes[thematicItem.CalcField]);
                    //    graphicValue = Math.Round(graphicValue / calcVal * 100, 2);
                    //}
                    double graphicValue = rd.NextDouble() * 1000;
                    int brushIndex = GetRangeIndex(graphicValue, thematicItem.RangeStarts);

                    SimpleFillSymbol symbol = new SimpleFillSymbol()
                    {
                        Fill = brushList[brushIndex],
                        BorderBrush = new SolidColorBrush(Colors.Transparent),
                        BorderThickness = 1
                    };

                    Graphic graphic = new Graphic();
                    graphic.Geometry = graphicFeature.Geometry;
                    graphic.Attributes.Add("Name", graphicFeature.Attributes["地区"].ToString());
                    graphic.Attributes.Add("Description", thematicItem.Description);
                    graphic.Attributes.Add("Value", graphicFeature.Attributes["案件数量"]);//graphicValue
                    graphic.Symbol = symbol;

                    graphicsLayer.Graphics.Add(graphic);
                }


                // Create new legend with ranges and swatches.
                LegendStackPanel.Children.Clear();

                ListBox legendList = new ListBox();
                LegendTitle.Text = thematicItem.Description;

                for (int c = 0; c < _classCount; c++)
                {
                    Rectangle swatchRect = new Rectangle()
                    {
                        Width = 20,
                        Height = 20,
                        Stroke = new SolidColorBrush(Colors.Black),
                        Fill = brushList[c]
                    };

                    TextBlock classTextBlock = new TextBlock();

                    // First classification
                    if (c == 0)
                        classTextBlock.Text = String.Format("  Less than {0}", Math.Round(thematicItem.RangeStarts[1], 2));
                    // Last classification
                    else if (c == _classCount - 1)
                        classTextBlock.Text = String.Format("  {0} and above", Math.Round(thematicItem.RangeStarts[c], 2));
                    // Middle classifications
                    else
                        classTextBlock.Text = String.Format("  {0} to {1}", Math.Round(thematicItem.RangeStarts[c], 2), Math.Round(thematicItem.RangeStarts[c + 1], 2));

                    StackPanel classStackPanel = new StackPanel();
                    classStackPanel.Orientation = Orientation.Horizontal;
                    classStackPanel.Children.Add(swatchRect);
                    classStackPanel.Children.Add(classTextBlock);

                    legendList.Items.Add(classStackPanel);
                }

                //TextBlock minTextBlock = new TextBlock();
                //StackPanel minStackPanel = new StackPanel();
                //minStackPanel.Orientation = Orientation.Horizontal;
                //minTextBlock.Text = String.Format("Min: {0} ({1})", thematicItem.Min, thematicItem.MinName);
                //minStackPanel.Children.Add(minTextBlock);
                //legendList.Items.Add(minStackPanel);

                //TextBlock maxTextBlock = new TextBlock();
                //StackPanel maxStackPanel = new StackPanel();
                //maxStackPanel.Orientation = Orientation.Horizontal;
                //maxTextBlock.Text = String.Format("Max: {0} ({1})", thematicItem.Max, thematicItem.MaxName);
                //maxStackPanel.Children.Add(maxTextBlock);
                //legendList.Items.Add(maxStackPanel);

                LegendStackPanel.Children.Add(legendList);
            }
        }

        private void CreateColorList()
        {
            ColorList = new List<List<SolidColorBrush>>();

            List<SolidColorBrush> BlueShades = new List<SolidColorBrush>();
            List<SolidColorBrush> RedShades = new List<SolidColorBrush>();
            List<SolidColorBrush> GreenShades = new List<SolidColorBrush>();
            List<SolidColorBrush> YellowShades = new List<SolidColorBrush>();
            List<SolidColorBrush> MagentaShades = new List<SolidColorBrush>();
            List<SolidColorBrush> CyanShades = new List<SolidColorBrush>();

            int rgbFactor = 255 / _classCount;

            for (int j = 0; j < 256; j = j + rgbFactor)
            {
                BlueShades.Add(new SolidColorBrush(Color.FromArgb(192, (byte)j, (byte)j, 255)));
                RedShades.Add(new SolidColorBrush(Color.FromArgb(192, 255, (byte)j, (byte)j)));
                GreenShades.Add(new SolidColorBrush(Color.FromArgb(192, (byte)j, 255, (byte)j)));
                YellowShades.Add(new SolidColorBrush(Color.FromArgb(192, 255, 255, (byte)j)));
                MagentaShades.Add(new SolidColorBrush(Color.FromArgb(192, 255, (byte)j, 255)));
                CyanShades.Add(new SolidColorBrush(Color.FromArgb(192, (byte)j, 255, 255)));
            }

            ColorList.Add(BlueShades);
            ColorList.Add(RedShades);
            ColorList.Add(GreenShades);
            ColorList.Add(YellowShades);
            ColorList.Add(MagentaShades);
            ColorList.Add(CyanShades);

            foreach (List<SolidColorBrush> brushList in ColorList)
            {
                brushList.Reverse();
            }

            List<SolidColorBrush> MixedShades = new List<SolidColorBrush>();
            if (_classCount > 5) MixedShades.Add(new SolidColorBrush(Color.FromArgb(192, 0, 255, 255)));
            if (_classCount > 4) MixedShades.Add(new SolidColorBrush(Color.FromArgb(192, 255, 0, 255)));
            if (_classCount > 3) MixedShades.Add(new SolidColorBrush(Color.FromArgb(192, 255, 255, 0)));
            MixedShades.Add(new SolidColorBrush(Color.FromArgb(192, 0, 255, 0)));
            MixedShades.Add(new SolidColorBrush(Color.FromArgb(192, 0, 0, 255)));
            MixedShades.Add(new SolidColorBrush(Color.FromArgb(192, 255, 0, 0)));
            ColorList.Add(MixedShades);

            _lastGeneratedClassCount = _classCount;
        }

        private int GetRangeIndex(double val, List<double> ranges)
        {
            int index = _classCount - 1;
            for (int r = 0; r < _classCount - 1; r++)
            {
                if (val >= ranges[r] && val < ranges[r + 1]) index = r;
            }
            return index;
        }


        private void btSearchMap_Click(object sender, RoutedEventArgs e)
        {

            //新建一个Find task
            FindTask findTask = new FindTask("http://192.168.1.100:6080/arcgis/rest/services/NanJing/MapServer");

            //异步执行，绑定事件
            findTask.ExecuteCompleted += FindTask_ExecuteCompleted;
            findTask.Failed += FindTask_Failed; ;

            //初始化FindParameters参数
            FindParameters findParameters = new FindParameters();
            findParameters.LayerIds.AddRange(new int[] { 0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20 }); //查找的图层
            findParameters.SearchFields.AddRange(new string[] { "NAME" }); //查找的字段范围
            findParameters.ReturnGeometry = true;
            //findParameters.SearchText = tbSearchWords.Text; //查找的“属性值”

            //设置查询的LayerDefinitions
            ESRI.ArcGIS.Client.LayerDefinition myDefinition = new ESRI.ArcGIS.Client.LayerDefinition();
            //myDefinition.LayerID = 7;
            //设置LayerDefinition，属性字段“Name”属于ID为0的图层
            //LayerDefinition的设置语句和Query中的Where语句一样
            myDefinition.Definition = "NAME = 'XXX'";

            //创建一个ObservableCollection，add设置的LayerDefinition
            System.Collections.ObjectModel.ObservableCollection<LayerDefinition> myObservableCollection =
               new System.Collections.ObjectModel.ObservableCollection<LayerDefinition>();
            myObservableCollection.Add(myDefinition);
            //findParameters.LayerDefinitions = myObservableCollection; //设置查询的LayerDefinitions

            //异步执行
            findTask.ExecuteAsync(findParameters);
        }

        private void FindTask_Failed(object sender, TaskFailedEventArgs e)
        {
            MessageBox.Show("查询结果为空！");
        }
        private static ESRI.ArcGIS.Client.Projection.WebMercator mercator =
            new ESRI.ArcGIS.Client.Projection.WebMercator();
        private void FindTask_ExecuteCompleted(object sender, FindEventArgs e)
        {
            //MessageBox.Show(e.FindResults.Count.ToString());
            if (e.FindResults.Count > 0)
            {
                GraphicsLayer layer = MainMap.Layers["QueryResultLayer"] as GraphicsLayer;
                layer.Graphics.Clear();

                Graphic graphic = new Graphic()
                {
                    Geometry=new ESRI.ArcGIS.Client.Geometry.MapPoint(e.FindResults[0].Feature.Geometry.Extent.GetCenter().X, e.FindResults[0].Feature.Geometry.Extent.GetCenter().Y),
                    //Geometry = mercator.FromGeographic(e.FindResults[0].Feature.Geometry.Extent.GetCenter()), //new MapPoint((double)listGPoint[i].X, (double)listGPoint[i].Y)
                    Symbol = LayoutRoot.Resources["Symbol_Point_Select"] as Symbol //LayoutRoot.Resources["Symbol_Point"] as Symbol
                };
                //graphic.Geometry.SpatialReference = MainMap.SpatialReference;
                layer.Graphics.Add(graphic);

                /*
                MainMap.Extent = new Envelope() {
                    XMax = e.FindResults[0].Feature.Geometry.Extent.GetCenter().X * (1.1),
                    XMin = e.FindResults[0].Feature.Geometry.Extent.GetCenter().X * (0.9),
                    YMax = e.FindResults[0].Feature.Geometry.Extent.GetCenter().Y * (1.1),
                    YMin = e.FindResults[0].Feature.Geometry.Extent.GetCenter().Y * (0.9)
                };
                */
                //MainMap.Extent = new Envelope()
                //{
                //    XMax = e.FindResults[0].Feature.Geometry.Extent.GetCenter().X * (1.1),
                //    XMin = e.FindResults[0].Feature.Geometry.Extent.GetCenter().X * (0.9),
                //    YMax = e.FindResults[0].Feature.Geometry.Extent.GetCenter().Y * (1.1),
                //    YMin = e.FindResults[0].Feature.Geometry.Extent.GetCenter().Y * (0.9)
                //};

                //MainMap.PanTo(graphic.Geometry);
                MainMap.PanTo(graphic.Geometry);
            }
            else
            {
                MessageBox.Show("查询结果为空！");
            }
        }

        private void btInfoList_Click(object sender, RoutedEventArgs e)
        {
            InfoList frmInfoList = new InfoList();
            frmInfoList.Show();
        }

        private void btClear_Click(object sender, RoutedEventArgs e)
        {
            GraphicsLayer layer = MainMap.Layers["QueryResultLayer"] as GraphicsLayer;
            layer.Graphics.Clear();
        }

        private void btAbout_Click(object sender, RoutedEventArgs e)
        {
            Other.About frmAbout = new Other.About();
            frmAbout.Show();
        }

        private void btChPassword_Click(object sender, RoutedEventArgs e)
        {
            Modify frmModify = new Modify();
            frmModify.Show();
        }

        private void btXuanran_Click(object sender, RoutedEventArgs e)
        {
            ESRI.ArcGIS.Client.Tasks.Query query = new ESRI.ArcGIS.Client.Tasks.Query()
            {
                Where = "1>0",
                OutSpatialReference = MainMap.SpatialReference,
                ReturnGeometry = true
            };
            query.OutFields.Add("*");

            QueryTask queryTask =
                new QueryTask(AnjianLayerURL);

            queryTask.ExecuteCompleted += (evtsender, args) =>
            {
                if (args.FeatureSet == null)
                    return;
                _featureSet = args.FeatureSet;
                SetRangeValues();
                //RenderButton.IsEnabled = true;
            };
            queryTask.Failed += QueryTask_Failed;

            queryTask.ExecuteAsync(query);
        }

        private void QueryTask_Failed(object sender, TaskFailedEventArgs e)
        {
            MessageBox.Show(e.Error.Message);
        }

        private void MainMap_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            MapPoint p= MainMap.ScreenToMap(e.GetPosition(LayoutRoot));
            //MessageBox.Show(string.Format("x:{0},y:{1}",p.X,p.Y));
        }

        private void btTrack_Click(object sender, RoutedEventArgs e)
        {
            //TrackPage frmTrackPage = new TrackPage();
            //frmTrackPage.Width = 800;
            //frmTrackPage.Height = 600;
            //frmTrackPage.Show();

            SelectTrack selectTrack = new SelectTrack();
            selectTrack.Closed += (s, ar) =>
            {
                string where = string.Format("Author='{0}' and InfoTime between '{1}' and '{2}'",
                    selectTrack.PoliceID,selectTrack.dtStart.ToString(),selectTrack.dtEnd.ToString());
                //查询 
                var client = new MyService.DBServiceClient();
                client.GetInfoListCompleted += (s2, ar2) =>
                {
                    if (ar2.Result != null && ar2.Result.Count > 0)
                    {
                        GraphicsLayer layer = MainMap.Layers["QueryResultLayer"] as GraphicsLayer; //取得图层
                        layer.Graphics.Clear(); //清除

                        //线样式
                        SimpleLineSymbol lineSymbol = new SimpleLineSymbol();
                        lineSymbol.Color = new SolidColorBrush(Colors.Brown);
                        lineSymbol.Width = 1;
                        lineSymbol.Style = SimpleLineSymbol.LineStyle.Solid;


                        ESRI.ArcGIS.Client.Geometry.Polyline polyline = new ESRI.ArcGIS.Client.Geometry.Polyline();
                        Graphic gLine = new Graphic();
                        gLine.Geometry = polyline;
                        polyline.SpatialReference = MainMap.SpatialReference;

                        ESRI.ArcGIS.Client.Geometry.PointCollection pcollect = new ESRI.ArcGIS.Client.Geometry.PointCollection();
                        //

                        foreach (Info info in ar2.Result)
                        {
                            

                            //添加线点
                            MapPoint mapP = new MapPoint();
                            mapP.SpatialReference = MainMap.SpatialReference;
                            mapP.X = info.POSX;
                            mapP.Y = info.POSY;
                            pcollect.Add(mapP);

                            //绘点 

                            Graphic graphicLabel = new Graphic()
                            {
                                Geometry = mapP,
                                Symbol = new SimpleMarkerSymbol()
                                {
                                    Color = new SolidColorBrush(Colors.Green),
                                    Style = SimpleMarkerSymbol.SimpleMarkerStyle.Circle
                                }
                            };
                            layer.Graphics.Add(graphicLabel); //添加结果至GraphicLayer

                            //标记 
                            graphicLabel = new Graphic()
                            {
                                Geometry = mapP,
                                Symbol = new TextSymbol()
                                {
                                    Text = info.Content,
                                    FontSize = 12,
                                    Foreground = new SolidColorBrush(Colors.Black)
                                }
                            };
                            layer.Graphics.Add(graphicLabel); //添加结果至GraphicLayer
                        }

                        polyline.Paths.Add(pcollect);
                        gLine.Symbol = lineSymbol;


                        //绘线
                        layer.Graphics.Add(gLine);
                        //MessageBox.Show("取得结果！");

                        MainMap.Extent = gLine.Geometry.Extent;
                    }
                };
                client.GetInfoListAsync(where);
            };
            selectTrack.Width = 800;
            selectTrack.Height = 600;
            selectTrack.Show();
        }

        private void SelectTrack_Closed(object sender, EventArgs e)
        {
            MessageBox.Show(sender.ToString());
        }

        private void btBookMark_Click(object sender, RoutedEventArgs e)
        {
            if (this.MyBookMark.Visibility == Visibility.Visible)
            {
                this.MyBookMark.Visibility = Visibility.Collapsed;
            }
            else
            {
                this.MyBookMark.Visibility = Visibility.Visible;
            }
        }

        private void btPolygnSearch_Click(object sender, RoutedEventArgs e)
        {

            PolygonSearch frmPolygonSearch = new PolygonSearch();
            frmPolygonSearch.Closed += (evtsender, args) =>
            {
                //MessageBox.Show(frmPolygonSearch.ZoneName);
                //查询案件图层
                ESRI.ArcGIS.Client.Tasks.Query query = new ESRI.ArcGIS.Client.Tasks.Query()
                {
                    Where = "地区='"+frmPolygonSearch.ZoneName+"'",
                    OutSpatialReference = MainMap.SpatialReference,
                    ReturnGeometry = true
                };
                query.OutFields.Add("*");

                QueryTask queryTask =
                    new QueryTask(AnjianLayerURL);

                queryTask.ExecuteCompleted += (s, ar) =>
                {
                    if (ar.FeatureSet == null)
                        return;
                    if (ar.FeatureSet.Count<Graphic>() <= 0)
                    {
                        MessageBox.Show("没有符合条件的区域！");
                    }
                    else
                    {
                        Graphic g = ar.FeatureSet.First<Graphic>();
                        if (g != null)
                        {
                            MessageBox.Show(string.Format("案件类型：{0} 数量：{1}", g.Attributes["主要案件类"].ToString(), g.Attributes["案件数目"].ToString()));
                        }
                        
                    }
                };
                queryTask.Failed += QueryTask_Failed;

                queryTask.ExecuteAsync(query);
            };
            frmPolygonSearch.Show();
        }

        private void btLineSearch_Click(object sender, RoutedEventArgs e)
        {
            //LineSearch frmLineSearch = new LineSearch();
            //frmLineSearch.Show();
            Draw drawtool = new Draw(MainMap);
            drawtool.DrawMode = DrawMode.Polyline;
            drawtool.DrawComplete += Drawtool_DrawComplete;
            drawtool.IsEnabled = true;
        }

        private void Drawtool_DrawComplete(object sender, DrawEventArgs e)
        {
            //缓冲区分析
            string geoURL = "http://192.168.1.100:6080/arcgis/rest/services/Utilities/Geometry/GeometryServer";



            GeometryService geometryService = new GeometryService(geoURL);
            geometryService.BufferCompleted += GeometryService_BufferCompleted;
            geometryService.Failed += GeometryService_Failed;

            // If buffer spatial reference is GCS and unit is linear, geometry service will do geodesic buffering
            BufferParameters bufferParams = new BufferParameters()
            {
                //Unit = LinearUnit.Meter,
                BufferSpatialReference = new SpatialReference(4326),
                OutSpatialReference = MainMap.SpatialReference
            };
            bufferParams.Features.Add(new Graphic() {  Geometry= e.Geometry});
            bufferParams.Distances.Add(0.05); //500米

            geometryService.BufferAsync(bufferParams);
        }
        Graphic LineZone = null;
        private void GeometryService_BufferCompleted(object sender, GraphicsEventArgs e)
        {
            if (e.Results.Count > 0)
            {
                LineZone = e.Results[0];

                //ESRI.ArcGIS.Client.Tasks.Query query = new ESRI.ArcGIS.Client.Tasks.Query()
                //{
                //    Where = "1>0",
                //    OutSpatialReference = MainMap.SpatialReference,
                //    ReturnGeometry = true
                //};
                //query.OutFields.Add("*");

                //空间查询
                Query query = new Query();
                query.ReturnGeometry = true;
                query.OutFields.AddRange(new List<string>() { "NAME" });
                query.Geometry = LineZone.Geometry;
                query.OutSpatialReference = MainMap.SpatialReference;
                //queryTask.ExecuteAsync(query);

                QueryTask queryTask =
                    new QueryTask("http://192.168.1.100:6080/arcgis/rest/services/NJ/MapServer/0"); //查询警局图层

                queryTask.ExecuteCompleted += (evtsender, args) =>
                {
                    if (args.FeatureSet == null)
                        return;
                    else
                    {
                       string s="共查到" + args.FeatureSet.Count<Graphic>() + "个警局！分别是：";
                        foreach (Graphic g in args.FeatureSet)
                        {
                            s += g.Attributes["Name"].ToString()+",";
                        }
                        s = s.TrimEnd(',');
                        MessageBox.Show(s);
                    }
                    //ESRI.ArcGIS.Client.Geometry.Polygon ply = LineZone.Geometry as ESRI.ArcGIS.Client.Geometry.Polygon;
                    //foreach (Graphic gr in args.FeatureSet)
                    //{
                    //    ply.
                    //}
                    
                    //RenderButton.IsEnabled = true;
                };
                queryTask.Failed += QueryTask_Failed;

                queryTask.ExecuteAsync(query);
            }
            else
            {
                MessageBox.Show("获取周边警局失败！");
            }
        }

        private void GeometryService_Failed(object sender, TaskFailedEventArgs e)
        {
            MessageBox.Show("获取周边警局发生错误！");
        }

        private void btPointSearch_Click(object sender, RoutedEventArgs e)
        {
            PointSearch frmPointSearch = new PointSearch();
            frmPointSearch.Closed += (evtsender, args) =>
            {
                switch (frmPointSearch.SearchType)
                {
                    case 0:
                        //查询
                        ESRI.ArcGIS.Client.Tasks.Query query = new ESRI.ArcGIS.Client.Tasks.Query()
                        {
                            Where = "1>0",
                            OutSpatialReference = MainMap.SpatialReference,
                            ReturnGeometry = true
                        };
                        query.OutFields.Add("*");

                        QueryTask queryTask =
                            new QueryTask(AnjianLayerURL);

                        queryTask.ExecuteCompleted += (s, ags) =>
                        {
                            if (ags.FeatureSet == null)
                                return;
                            if (ags.FeatureSet.Count<Graphic>() > 0)
                            {
                                string sresult = "";
                                foreach (Graphic gra in ags.FeatureSet)
                                {
                                    sresult += (gra.Attributes["地区"].ToString()+",");
                                }
                                sresult = sresult.TrimEnd(',');
                                MessageBox.Show(sresult);
                            }
                            else
                            {
                                MessageBox.Show("没有复合条件的结果！");
                            }
                        };
                        queryTask.Failed += QueryTask_Failed;

                        queryTask.ExecuteAsync(query);

                        break;

                    case 1://查询警员信息
                        var clent1 = new MyService.DBServiceClient();
                        clent1.GetUserINFOCompleted += Clent1_GetUserINFOCompleted;
                        clent1.GetUserINFOAsync(string.Format("UserName = '{0}' or PNO = '{1}'",frmPointSearch.PName,frmPointSearch.PNO));
                        break;

                    case 2: //最近警局查询
                        //查询
                        ESRI.ArcGIS.Client.Tasks.Query query2 = new ESRI.ArcGIS.Client.Tasks.Query()
                        {
                            Where = "1>0",
                            OutSpatialReference = MainMap.SpatialReference,
                            ReturnGeometry = true
                        };
                        query2.OutFields.Add("*");

                        QueryTask queryTask2 =
                            new QueryTask(JingJuLayerURL);

                        queryTask2.ExecuteCompleted += (s, ags) =>
                        {
                            if (ags.FeatureSet == null)
                                return;
                            if (ags.FeatureSet.Count<Graphic>() > 0)
                            {
                                string sresult = "";
                                Graphic nearPS = null;
                                double distance = double.MaxValue;
                                //MessageBox.Show("查询到警局！");
                                double x = frmPointSearch.X;
                                double y = frmPointSearch.Y;
                                foreach (Graphic gra in ags.FeatureSet)
                                {
                                    //sresult += (gra.Attributes["地区"].ToString() + ",");
                                    double x2 = gra.Geometry.Extent.XMax / 2 + gra.Geometry.Extent.XMin / 2;
                                    double y2= gra.Geometry.Extent.YMax / 2 + gra.Geometry.Extent.YMin / 2;


                                    double dis = GetDistance(x, y, x2, x2);
                                    if (dis < distance)
                                    {
                                        distance = dis;
                                        nearPS = gra;
                                    }
                       
                                }
                                if (nearPS != null)
                                {
                                    MessageBox.Show("最近的警局是:"+nearPS.Attributes["Name"].ToString());
                                }
                                //MessageBox.Show(sresult);
                            }
                            else
                            {
                                MessageBox.Show("没有警局信息！");
                            }
                        };
                        queryTask2.Failed += QueryTask_Failed;

                        queryTask2.ExecuteAsync(query2);

                        break;

                }
                //MessageBox.Show(frmPointSearch.SearchType.ToString());
            };
             frmPointSearch.Show();
        }

        private double GetDistance(double x1, double y1, double x2, double y2)
        {
            return Math.Sqrt((x1-x2)* (x1 - x2)+(y1-y2)* (y1 - y2));
        }

        private void Clent1_GetUserINFOCompleted(object sender, MyService.GetUserINFOCompletedEventArgs e)
        {
            MessageBox.Show("查询结果:"+e.Result);
        }

        private void btPie_Click(object sender, RoutedEventArgs e)
        {
            //查询
            ESRI.ArcGIS.Client.Tasks.Query query = new ESRI.ArcGIS.Client.Tasks.Query()
            {
                Where = "1>0",
                OutSpatialReference = MainMap.SpatialReference,
                ReturnGeometry = true
            };
            query.OutFields.Add("*");

            QueryTask queryTask =
                new QueryTask(AnjianLayerURL);

            queryTask.ExecuteCompleted += (evtsender, args) =>
            {
                if (args.FeatureSet == null)
                    return;
                List<CaseItem> listCase = new List<CaseItem>();
                foreach (Graphic g in args.FeatureSet)
                {
                    listCase.Add(new CaseItem() { Name= g.Attributes["地区"].ToString(),Count=Convert.ToInt32(g.Attributes["案件数目"]) });
                }

                PieWindow frmPieWindow = new PieWindow();
                frmPieWindow.ListCase = listCase;
                //frmPieWindow.Width = 640;
                //frmPieWindow.Height = 480;
                frmPieWindow.Show();
            };
            queryTask.Failed += QueryTask_Failed;

            queryTask.ExecuteAsync(query);

            
        }
    }

    public struct ThematicItem
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string CalcField { get; set; }
        public double Min { get; set; }
        public double Max { get; set; }
        public string MinName { get; set; }
        public string MaxName { get; set; }
        public List<double> RangeStarts { get; set; }

    }
}
