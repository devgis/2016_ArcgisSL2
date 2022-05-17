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
    public partial class PolygonSearch : ChildWindow
    {
        public string ZoneName = string.Empty;
        public PolygonSearch()
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
            if (string.IsNullOrEmpty(tbZoneName.Text.Trim()))
            {
                MessageBox.Show("请输入区县名称！");
            }
            else
            {
                ZoneName = tbZoneName.Text;
                this.Close();
            }
        }
    }
}

