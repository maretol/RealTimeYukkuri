using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealTimeYukkuri.ViewModel
{
    class InfomationTabItemViewModel : BindableBase
    {
        public InfomationTabItemViewModel()
        {
        }
        
        public string MainLogo { get { return "リアルタイムゆっくり（べーたばん）"; } }

        public string Version { get { return "Version : 現在バージョン未管理"; } }
    }
}
