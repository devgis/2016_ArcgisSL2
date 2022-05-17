using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using SilverlightGIS.Common;

namespace SilverlightGIS.Web
{
    // 注意: 使用“重构”菜单上的“重命名”命令，可以同时更改代码和配置文件中的接口名“IDBService”。
    [ServiceContract]
    public interface IDBService
    {
        #region  信息

        [OperationContract]
        List<Info> GetInfoList(string Where);
        [OperationContract]
        bool AddInfo(Info info);
        [OperationContract]
        bool EditInfo(Info info);
        [OperationContract]
        bool DeleteInfo(string ID);
        #endregion

        #region 位置
        [OperationContract]
        List<TrackInfo> GetTrackList(string Where);
        [OperationContract]
        bool AddTrackInfo(TrackInfo trackInfo);
        [OperationContract]
        bool DeleteTrackInfo(string ID);
        #endregion

        #region 用户
        [OperationContract]
        bool AddUser(string UserName, string UserPassword, string PNO, double PAGE, double age, string POSITION);
        [OperationContract]
        bool EditUser(string UserName, string UserPassword);
        [OperationContract]
        bool CheckUser(string UserName, string UserPassword);
        [OperationContract]
        bool UserExists(string UserName, string UserPassword);
        [OperationContract]
        string GetUserINFO(string where);
        [OperationContract]
        List<UserInfo> GetUserList();
        #endregion

    }
}
