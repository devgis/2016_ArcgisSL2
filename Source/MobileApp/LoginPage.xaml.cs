using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上提供

namespace MobileApp
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class LoginPage : Page
    {
        public LoginPage()
        {
            this.InitializeComponent();
        }
        
        private async void btLogin_Click(object sender, RoutedEventArgs e)
        {
            //App.Current.RootVisual = new MainPage();

            if (string.IsNullOrEmpty(tbUserName.Text.Trim()))
            {
                tbMessage.Text = "请输入用户名！";
                return;
            }
            if (string.IsNullOrEmpty(tbPassword.Password.Trim()))
            {
                tbMessage.Text = "请输入密码！";
                return;
            }


            //client.CheckUserCompleted += Client_CheckUserCompleted;
            //client.CheckUserAsync(tbUserName.Text, tbPassword.Password);

            try
            {
                var client = new MyService.DBServiceClient();
                var response = await client.CheckUserAsync(tbUserName.Text, tbPassword.Password);
                if (response)
                {
                    this.Content = new MainPage();
                    MainPage.UserName = tbUserName.Text;
                }
                else
                {
                    tbMessage.Text = "用户名或者密码不正确！";
                    return;
                }
            }
            catch (Exception ex)
            {
                tbMessage.Text = "发生错误："+ex.Message;
            }
        }
        //public async Task<bool> CheckUserAsync(string UserName,string UserPassword)
        //{
        //   return await Task.Run(() => client.CheckUserAsync(UserName, UserPassword));
        //}
    }
}
