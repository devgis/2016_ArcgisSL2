using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Devices.Geolocation;
using System.Threading.Tasks;
using System.Threading;

//“空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409 上有介绍

namespace MobileApp
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public static string UserName
        {
            get;
            set;
        }
        public MainPage()
        {
            this.InitializeComponent();
            this.Loaded += MainPage_Loaded;
        }

        private async void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            tbWelcome.Text = "欢迎使用：" + MainPage.UserName;
            //获取位置
            try
            {
                //异步获取位置，保存到变量中
                var position = await GetPosition(); //await
                                                    //维度
                double lat = position.Coordinate.Point.Position.Latitude;
                //经度
                double lon = position.Coordinate.Point.Position.Longitude;

                tbX.Text = lon.ToString();
                tbY.Text = lat.ToString();
            }
            catch(Exception ex)
            {
                //tbError.Text = "获取地址发生错误，随机产生一组经纬度";
                Random rd = new Random();
                double lat = 32 + rd.Next(1042, 1557) / 10000;
                //经度
                double lon = 118 + rd.Next(6942, 8999) / 10000;
                tbX.Text = lon.ToString();
                tbY.Text = lat.ToString();
            }
            

            //var accessStatus =  Geolocator.RequestAccessAsync();
            //switch (accessStatus.GetResults())
            //{
            //    case GeolocationAccessStatus.Allowed:
            //        tbError.Text = "Waiting for update...";

            //        // If DesiredAccuracy or DesiredAccuracyInMeters are not set (or value is 0), DesiredAccuracy.Default is used.
            //        Geolocator geolocator = new Geolocator();


            //        // Subscribe to the StatusChanged event to get updates of location status changes.
            //        //_geolocator.StatusChanged += OnStatusChanged;

            //        // Carry out the operation.
            //        var pos = geolocator.GetGeopositionAsync();

            //        //UpdateLocationData(pos);
            //        //_rootPage.NotifyUser("Location updated.", NotifyType.StatusMessage);
            //        tbX.Text = pos.GetResults().Coordinate.Point.Position.Longitude.ToString();
            //        tbY.Text = pos.GetResults().Coordinate.Point.Position.Latitude.ToString();
            //        break;

            //    case GeolocationAccessStatus.Denied:
            //        tbError.Text = "没有权限获取位置..."; //Access to location is denied.
            //        break;

            //    case GeolocationAccessStatus.Unspecified:
            //        tbError.Text = "获取位置发生错误..."; //_rootPage.NotifyUser("Unspecified error.", NotifyType.ErrorMessage);
            //        break;
            //}
        }

        private async void btUpLoad_Click(object sender, RoutedEventArgs e)
        {
            if (Check())
            {
                var client = new MyService.DBServiceClient();
                MyService.TrackInfo trackinfo = new MyService.TrackInfo();
                trackinfo.POSX = Convert.ToDouble(tbX.Text);
                trackinfo.POSY = Convert.ToDouble(tbY.Text);
                trackinfo.UserName = MainPage.UserName;
                var response = await client.AddTrackInfoAsync(trackinfo);
                if (response)
                {
                    tbX.Text = string.Empty;
                    tbY.Text = string.Empty;
                    tbError.Text = "位置上传成功！";
                }
                else
                {

                    tbError.Text = "位置上传失败！";
                    return;
                }
            }
        }

        bool Check()
        {
            if (string.IsNullOrEmpty(tbX.Text.Trim()))
            {
                tbError.Text = "经度不能为空！";
                tbX.Focus(FocusState.Pointer);
                return false;
            }
            else
            {
                try
                {
                    double d = Convert.ToDouble(tbX.Text);
                    if (d > 180 || d < -180)
                    {
                        tbError.Text = "经度必须在-180~180之间！";
                        tbX.Focus(FocusState.Pointer);
                        return false;
                    }
                }
                catch
                {
                    tbError.Text = "经度必须为-180~180之间的数字！";
                    tbX.Focus(FocusState.Pointer);
                    return false;
                }
            }

            if (string.IsNullOrEmpty(tbY.Text.Trim()))
            {
                tbError.Text = "纬度不能为空！";
                tbY.Focus(FocusState.Pointer);
                return false;
            }
            else
            {
                try
                {
                    double d = Convert.ToDouble(tbY.Text);
                    if (d > 90 || d < -90)
                    {
                        tbError.Text = "纬度必须在-90~90之间！";
                        tbY.Focus(FocusState.Pointer);
                        return false;
                    }
                }
                catch
                {
                    tbError.Text = "纬度必须为-90~90之间的数字！";
                    tbY.Focus(FocusState.Pointer);
                    return false;
                }
            }
            return true;
        }

        bool CheckContent()
        {
            if (string.IsNullOrEmpty(tbTitle.Text.Trim()))
            {
                tbError.Text = "标题不能为空！";
                tbTitle.Focus(FocusState.Pointer);
                return false;
            }
            
            if (string.IsNullOrEmpty(tbContent.Text.Trim()))
            {
                tbError.Text = "案件内容不能为空！";
                tbContent.Focus(FocusState.Pointer);
                return false;
            }
            return true;
        }

        public async Task<Geoposition> GetPosition()
        {
            //请求对位置的访问权
            var accessStatus = await Geolocator.RequestAccessAsync();
            //此时，窗口会弹出提示是否允许应用访问位置，如果用户不允许则抛出异常
            if (accessStatus != GeolocationAccessStatus.Allowed) throw new Exception();
            //实例化定位类，并设置经纬度精确度（单位：米），一般为零，为保护用户隐私，建议减少精确度
            var geolocator = new Geolocator { DesiredAccuracyInMeters = 0 };
            //异步获取设备位置，并将位置保存到变量中（Geoposition类型）
            var position = await geolocator.GetGeopositionAsync();
            //返回位置
            return position;
        }

        private async void btGetPosition_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //异步获取位置，保存到变量中
                var position = await GetPosition(); //await
                                                    //维度
                double lat = position.Coordinate.Point.Position.Latitude;
                //经度
                double lon = position.Coordinate.Point.Position.Longitude;

                tbX.Text = lon.ToString();
                tbY.Text = lat.ToString();
            }
            catch (Exception ex)
            {
                //tbError.Text = "获取地址发生错误，随机产生一组经纬度";
                Random rd = new Random();
                double lat = 34 + rd.NextDouble();
                //经度
                double lon = 109 + rd.NextDouble();
                tbX.Text = lon.ToString();
                tbY.Text = lat.ToString();
            }
        }

        private async void btAddInfo_Click(object sender, RoutedEventArgs e)
        {
            if (Check()&&CheckContent())
            {
                var client = new MyService.DBServiceClient();
                MyService.Info info = new MyService.Info();
                info.POSX = Convert.ToDouble(tbX.Text);
                info.POSY = Convert.ToDouble(tbY.Text);
                info.Author = MainPage.UserName;
                info.Title = tbTitle.Text;
                info.Content = tbContent.Text;
                var response = await client.AddInfoAsync(info);
                if (response)
                {
                    tbX.Text = string.Empty;
                    tbY.Text = string.Empty;
                    tbTitle.Text = string.Empty;
                    tbContent.Text = string.Empty;
                    tbError.Text = "案件上传成功！";
                }
                else
                {

                    tbError.Text = "案件上传失败！";
                    return;
                }
            }
        }
    }
}
