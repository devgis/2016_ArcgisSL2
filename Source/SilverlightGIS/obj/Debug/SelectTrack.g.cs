﻿#pragma checksum "D:\Work\2016\ArcgisSL2\SilverlightGIS33\SilverlightGIS\SilverlightGIS\SelectTrack.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "BF58C7D3D19691B85A24A0615BABFCE5"
//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由工具生成。
//     运行时版本:4.0.30319.42000
//
//     对此文件的更改可能会导致不正确的行为，并且如果
//     重新生成代码，这些更改将会丢失。
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Resources;
using System.Windows.Shapes;
using System.Windows.Threading;


namespace SilverlightGIS.Information {
    
    
    public partial class SelectTrack : System.Windows.Controls.ChildWindow {
        
        internal System.Windows.Controls.Grid LayoutRoot;
        
        internal System.Windows.Controls.ComboBox cbPoliceList;
        
        internal System.Windows.Controls.TextBox tbStartTime;
        
        internal System.Windows.Controls.TextBox tbEndTime;
        
        internal System.Windows.Controls.Button btToday;
        
        internal System.Windows.Controls.Button btWeek;
        
        internal System.Windows.Controls.Button btOK;
        
        internal System.Windows.Controls.Button btCancel;
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Windows.Application.LoadComponent(this, new System.Uri("/SilverlightGIS;component/SelectTrack.xaml", System.UriKind.Relative));
            this.LayoutRoot = ((System.Windows.Controls.Grid)(this.FindName("LayoutRoot")));
            this.cbPoliceList = ((System.Windows.Controls.ComboBox)(this.FindName("cbPoliceList")));
            this.tbStartTime = ((System.Windows.Controls.TextBox)(this.FindName("tbStartTime")));
            this.tbEndTime = ((System.Windows.Controls.TextBox)(this.FindName("tbEndTime")));
            this.btToday = ((System.Windows.Controls.Button)(this.FindName("btToday")));
            this.btWeek = ((System.Windows.Controls.Button)(this.FindName("btWeek")));
            this.btOK = ((System.Windows.Controls.Button)(this.FindName("btOK")));
            this.btCancel = ((System.Windows.Controls.Button)(this.FindName("btCancel")));
        }
    }
}

