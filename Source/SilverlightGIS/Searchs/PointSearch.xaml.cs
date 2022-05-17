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

namespace SilverlightGIS.Searchs
{
    public partial class PointSearch : ChildWindow
    {
        public int SearchType = -1; //0 案件地点查询 //1 警员信息查询 //2最近警局查询

        public string CaseType = string.Empty;

        public string PName = string.Empty;
        public string PNO = string.Empty;

        public double X = 0;
        public double Y = 0;
        public PointSearch()
        {
            InitializeComponent();
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void tbSearchCaseInfo_Click(object sender, RoutedEventArgs e)
        {
            SearchType = 0;
            if (Check())
            {
                this.Visibility = Visibility.Collapsed;
                CaseType = tbCaseType.Text;
                this.Close();
            }
        }

        private void tbSearchPoliceInfo_Click(object sender, RoutedEventArgs e)
        {
            SearchType = 1;
            if (Check())
            {
                this.Visibility = Visibility.Collapsed;
                PName = tbName.Text;
                PNO = tbPNO.Text;
                this.Close();
            }

        }

        private void tbNearPoliceStation_Click(object sender, RoutedEventArgs e)
        {
            SearchType = 2;
            if (Check())
            {
                this.Visibility = Visibility.Collapsed;
                X = Convert.ToDouble(tbX.Text);
                Y = Convert.ToDouble(tbY.Text);
                this.Close();
            }
        }

        private bool Check()
        {
            switch (SearchType)
            {
                case 0:
                    if (string.IsNullOrEmpty(tbCaseType.Text.Trim()))
                    {
                        MessageBox.Show("请输入案件类型！");
                        return false;
                    }
                    break;
                case 1:
                    if (string.IsNullOrEmpty(tbName.Text.Trim()))
                    {
                        MessageBox.Show("请输入警员名称！");
                        return false;
                    }
                    
                    if (string.IsNullOrEmpty(tbPNO.Text.Trim()))
                    {
                        MessageBox.Show("请输入警员编号！");
                        return false;
                    }
                    break;
                case 2:

                    if (string.IsNullOrEmpty(tbX.Text.Trim()))
                    {
                        MessageBox.Show("请输入经度！");
                        return false;
                    }
                    else
                    {
                        try
                        {
                            double d = Convert.ToDouble(tbX.Text);
                            if (d < -180 || d > 180)
                            {
                                MessageBox.Show("经度在-180~180之间！");
                                return false;
                            }
                        }
                        catch
                        {
                            MessageBox.Show("经度是-180~180之间的数值！");
                            return false;
                        }
                    }

                    if (string.IsNullOrEmpty(tbY.Text.Trim()))
                    {
                        MessageBox.Show("请输入纬度！");
                        return false;
                    }
                    else
                    {
                        try
                        {
                            double d = Convert.ToDouble(tbY.Text);
                            if (d < -90 || d > 90)
                            {
                                MessageBox.Show("纬度在-90~90之间！");
                                return false;
                            }
                        }
                        catch
                        {
                            MessageBox.Show("纬度是-90~90之间的数值！");
                            return false;
                        }
                    }
                    break;
            }
            return true;
        }
    }
}

