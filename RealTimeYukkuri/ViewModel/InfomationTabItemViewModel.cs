using Prism.Commands;
using Prism.Mvvm;
using RealTimeYukkuri.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealTimeYukkuri.ViewModel
{
    class InfomationTabItemViewModel : BindableBase
    {
        #region "メンバ"
        private KinectModel kinectModel;
        #endregion

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public InfomationTabItemViewModel()
        {
            kinectModel = KinectModel.GetKinectModelInstance;
            KinectButton = new DelegateCommand(KinectButtonExecute, CanKinectButtonExecute);
            KinectButton.RaiseCanExecuteChanged();
            SetStatus();
        }

        #region "バインドされるプロパティ"
        public string MainLogo { get { return "リアルタイムゆっくり（仮）"; } }

        public string Version { get { return "Version : 現在バージョン未管理"; } }

        public string ButtonText { get { return "Kinect起動・終了"; } }

        private string statusText;
        public string StatusText {
            get { return statusText; }
            private set { SetProperty(ref statusText, value); }
        }

        public DelegateCommand KinectButton { get; private set; }
        #endregion

        #region "ボタンのコマンド"
        private void KinectButtonExecute()
        {
            kinectModel.ExecCommand();
            SetStatus();
        }

        private bool CanKinectButtonExecute()
        {
            return StatusText=="起動可能" || StatusText=="起動中";
        }
        #endregion

        #region "その他メソッド"
        private void SetStatus()
        {
            if(kinectModel == null)
            {
                StatusText = "Kinectが動かせません";
            }
            else if (kinectModel.IsAwaken)
            {
                StatusText = "起動中";
            }
            else
            {
                StatusText = "起動可能";
            }
            KinectButton.RaiseCanExecuteChanged();
        }

        #endregion
    }
}
