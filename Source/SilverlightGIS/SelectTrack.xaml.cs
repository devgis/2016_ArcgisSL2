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
using System.Windows.Navigation;
using SilverlightGIS.MyService;

namespace SilverlightGIS.Information
{
    public partial class SelectTrack : ChildWindow
    {
        public SelectTrack()
        {
            InitializeComponent();
            this.Loaded += SelectTrack_Loaded;
        }

        private void SelectTrack_Loaded(object sender, RoutedEventArgs e)
        {
            var client = new MyService.DBServiceClient();
            client.GetUserListCompleted += Client_GetUserListCompleted;
            client.GetUserListAsync();
        }

        private void Client_GetUserListCompleted(object sender, GetUserListCompletedEventArgs e)
        {
            cbPoliceList.ItemsSource = e.Result;
            cbPoliceList.DisplayMemberPath = "UserName";
            cbPoliceList.SelectedValuePath = "UserID";

            tbStartTime.Text = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd HH:mm:ss");
            tbEndTime.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

        }
        bool Check()
        {
            if (cbPoliceList.SelectedValue == null)
            {
                MessageBox.Show("请选择警员！");
                return false;
            }
            try
            {
                Convert.ToDateTime(tbStartTime.Text);
            }
            catch
            {
                MessageBox.Show("开始时间不是有效的时间格式！时间各式应该为 年-月-日 时:分:秒");
                return false;
            }

            try
            {
                Convert.ToDateTime(tbEndTime.Text);
            }
            catch
            {
                MessageBox.Show("结束时间不是有效的时间格式！时间各式应该为 年-月-日 时:分:秒");
                return false;
            }
            return true;
        }
        public string PoliceID;
        public DateTime dtStart;
        public DateTime dtEnd;
        private void btOK_Click(object sender, RoutedEventArgs e)
        {
            if(Check())
            {
                this.Visibility = Visibility.Collapsed;
                PoliceID = (cbPoliceList.SelectedItem as UserInfo).UserName.ToString();
                dtStart= Convert.ToDateTime(tbStartTime.Text);
                dtEnd= Convert.ToDateTime(tbEndTime.Text);
                this.Close();
            }
            //if (string.IsNullOrEmpty(tbTitle.Text.Trim()))
            //{
            //    MessageBox.Show("标题不能为空！");
            //    return;
            //}
            //if (string.IsNullOrEmpty(tbAuthor.Text.Trim()))
            //{
            //    MessageBox.Show("作者不能为空！");
            //    return;
            //}
            //if (string.IsNullOrEmpty(tbContent.Text.Trim()))
            //{
            //    MessageBox.Show("内容不能为空！");
            //    return;
            //}

            //Info info = new Info();
            //info.ID = Guid.NewGuid().ToString();
            //info.Time = DateTime.Now;
            //info.Title = tbTitle.Text;
            //info.Content = tbContent.Text;
            //info.Author = tbAuthor.Text;

            //MyService.DBServiceClient client = new DBServiceClient();
            //client.AddInfoCompleted += Client_AddInfoCompleted;
            //client.AddInfoAsync(info);
        }

        private void Client_AddInfoCompleted(object sender, AddInfoCompletedEventArgs e)
        {
            if (e.Result)
            {
                MessageBox.Show("保存成功！");
                this.Close();
            }
            else
            {
                MessageBox.Show("保存失败！");
            }
        }

        private void btCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btToday_Click(object sender, RoutedEventArgs e)
        {
            tbStartTime.Text = DateTime.Now.ToString("yyyy-MM-dd")+" 00:00:00";
            tbEndTime.Text = DateTime.Now.ToString("yyyy-MM-dd")+" 23:59:59";
        }

        private void btWeek_Click(object sender, RoutedEventArgs e)
        {
            tbStartTime.Text = DateTime.Now.Date.AddDays(-7).ToString("yyyy-MM-dd") + " 00:00:00";
            tbEndTime.Text = DateTime.Now.ToString("yyyy-MM-dd") + " 23:59:59";
        }
    }
}
