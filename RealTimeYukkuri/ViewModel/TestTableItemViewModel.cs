using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealTimeYukkuri.ViewModel
{
    class TestTableItemViewModel : BindableBase
    {

        #region "プロパティ"
        private string happy;
        public string Happy
        {
            get { return happy; }
            private set { SetProperty(ref happy, value); }
        }

        private string leftEyeClosed;
        public string LeftEyeClosed
        {
            get { return leftEyeClosed; }
            private set { SetProperty(ref leftEyeClosed, value); }
        }

        private string rightEyeClosed;
        public string RightEyeClosed
        {
            get { return rightEyeClosed; }
            private set { SetProperty(ref rightEyeClosed, value); }
        }

        private string mouthOpen;
        public string MouthOpen
        {
            get { return mouthOpen; }
            private set { SetProperty(ref mouthOpen, value); }
        }

        private string mouthMoved;
        public string MouthMoved
        {
            get { return mouthMoved; }
            private set { SetProperty(ref mouthMoved, value); }
        }

        private string lookingAway;
        public string LookingAway
        {
            get { return lookingAway; }
            private set { SetProperty(ref lookingAway, value); }
        }

        private string glasses;
        public string Glasses
        {
            get { return glasses; }
            private set { SetProperty(ref glasses, value); }
        }

        #endregion
    }
}
