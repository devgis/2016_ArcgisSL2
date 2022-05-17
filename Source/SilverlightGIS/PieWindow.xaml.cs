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
using Visifire.Charts;

namespace SilverlightGIS
{
    public partial class PieWindow : ChildWindow
    {
        public PieWindow()
        {
            InitializeComponent();
            this.Loaded += PieWindow_Loaded;
        }
        public List<CaseItem> ListCase;
        private void PieWindow_Loaded(object sender, RoutedEventArgs e)
        {
            //设置图标的宽度和高度
            chart.Width = 580;
            chart.Height = 380;
            chart.Margin = new Thickness(100, 5, 10, 5);
            //是否启用打印和保持图片
            chart.ToolBarEnabled = false;

            //设置图标的属性
            chart.ScrollingEnabled = false;//是否启用或禁用滚动
            chart.View3D = true;//3D效果显示

            //创建一个标题的对象
            Title title = new Title();

            //设置标题的名称
            title.Text = "案件分布饼图";
            title.Padding = new Thickness(0, 10, 5, 0);

            //向图标添加标题
            chart.Titles.Add(title);

            //Axis yAxis = new Axis();
            ////设置图标中Y轴的最小值永远为0           
            //yAxis.AxisMinimum = 0;
            ////设置图表中Y轴的后缀          
            //yAxis.Suffix = "斤";
            //chart.AxesY.Add(yAxis);

            // 创建一个新的数据线。               
            DataSeries dataSeries = new DataSeries();

            // 设置数据线的格式
            dataSeries.RenderAs = RenderAs.Pie;//柱状Stacked


            // 设置数据点              
            DataPoint dataPoint;

            if (ListCase != null && ListCase.Count > 0)
            {
                foreach (CaseItem item in ListCase)
                {
                    // 创建一个数据点的实例。                   
                    dataPoint = new DataPoint();
                    // 设置X轴点                    
                    dataPoint.AxisXLabel = item.Name;

                    dataPoint.LegendText = item.Name;
                    //设置Y轴点                   
                    dataPoint.YValue = item.Count;
                    //添加一个点击事件        
                    //dataPoint.MouseLeftButtonDown += new MouseButtonEventHandler(dataPoint_MouseLeftButtonDown);
                    //添加数据点                   
                    dataSeries.DataPoints.Add(dataPoint);
                }
            }
            //for (int i = 0; i < 50; i++)
            //{
            //    // 创建一个数据点的实例。                   
            //    dataPoint = new DataPoint();
            //    // 设置X轴点                    
            //    dataPoint.AxisXLabel = i.ToString();

            //    dataPoint.LegendText = "##" + i.ToString();
            //    //设置Y轴点                   
            //    dataPoint.YValue = i;
            //    //添加一个点击事件        
            //    //dataPoint.MouseLeftButtonDown += new MouseButtonEventHandler(dataPoint_MouseLeftButtonDown);
            //    //添加数据点                   
            //    dataSeries.DataPoints.Add(dataPoint);
            //}

            // 添加数据线到数据序列。                
            chart.Series.Add(dataSeries);
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
    }
}

