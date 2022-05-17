using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;

namespace MobileApp
{
    public class LocationManage
    {
        public async static Task<Geoposition> GetPosition()
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
    }
}
