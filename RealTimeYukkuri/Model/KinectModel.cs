using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Prism.Mvvm;
using Microsoft.Kinect;
using Microsoft.Kinect.Face;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace RealTimeYukkuri.Model
{
    class KinectModel : INotifyPropertyChanged
    {
        private static KinectModel kinectModelInstance;
        public static KinectModel GetKinectModelInstance {
            get
            {
                if (kinectModelInstance != null)
                {
                    return kinectModelInstance;
                }
                kinectModelInstance = new KinectModel();
                return kinectModelInstance;
            }
        }

        /// <summary>
        /// コンストラクタ。シングルトンにしてるからそっちからのみ呼び出し
        /// </summary>
        private KinectModel()
        {
        }

        #region "メンバ変数"

        /// <summary>
        /// Kinectセンサーとの接続
        /// </summary>
        private KinectSensor Kinect;

        /// <summary>
        /// Kinectセンサーからの接続データを受け取るFrameReader
        /// </summary>
        private MultiSourceFrameReader Reader;

        /// <summary>
        /// Kinectの顔情報データの取得元
        /// </summary>
        private FaceFrameSource FaceSource;

        /// <summary>
        /// 顔情報の受け取りをするFaceRader
        /// </summary>
        private FaceFrameReader FaceReader;

        /// <summary>
        /// FaceFrameの解析用の項目を示す
        /// </summary>
        private const FaceFrameFeatures DefaultFaceframeFeatures = FaceFrameFeatures.PointsInColorSpace
            | FaceFrameFeatures.Happy
            | FaceFrameFeatures.LeftEyeClosed
            | FaceFrameFeatures.RightEyeClosed
            | FaceFrameFeatures.MouthOpen
            | FaceFrameFeatures.MouthMoved
            | FaceFrameFeatures.LookingAway
            | FaceFrameFeatures.Glasses
            | FaceFrameFeatures.RotationOrientation;

        /// <summary>
        /// Kinectから取得した骨格情報
        /// </summary>
        private Body[] Bodies;

        /// <summary>
        /// Kinectから取得した色情報用の一時的なバッファ
        /// </summary>
        private byte[] ColorPixels;

        /// <summary>
        /// Kinectから受信するピクセル単位のバイト数
        /// </summary>
        private readonly int BytePerPixel = (PixelFormats.Bgr32.BitsPerPixel + 7) / 8;

        #endregion

        #region "プロパティ"
        /// <summary>
        /// Kinectから取得した表示用の色情報。最終的には使わないかも
        /// </summary>
        private WriteableBitmap _ColorBitmap;
        public WriteableBitmap ColorBitmap
        {
            get { return _ColorBitmap; }
            private set { SetProperty(ref _ColorBitmap, value); }
        }

        /// <summary>
        /// Kinectから取得した顔パーツの座標をマッピングする情報。最終的には使わないかも
        /// </summary>
        private RenderTargetBitmap _FacePointBitmap;
        public RenderTargetBitmap FacePointBitmap
        {
            get { return _FacePointBitmap; }
            private set { SetProperty(ref _FacePointBitmap, value); }
        }

        /// <summary>
        /// 顔の回転情報
        /// </summary>
        private Vector4 _FaceRotation;
        public Vector4 FaceRotation
        {
            get { return _FaceRotation; }
            private set { SetProperty(ref _FaceRotation, value); }
        }


        #region 表情の情報
        private string _Happy;
        public string Happy
        {
            get { return _Happy; }
            private set { SetProperty(ref _Happy, value); }
        }

        private string _LeftEyeClosed;
        public string LeftEyeClosed
        {
            get { return _LeftEyeClosed; }
            private set { SetProperty(ref _LeftEyeClosed, value); }
        }

        private string _RightEyeClosed;
        public string RightEyeClosed
        {
            get { return _RightEyeClosed; }
            private set { SetProperty(ref _RightEyeClosed, value); }
        }

        private string _MouthOpen;
        public string MouthOpen
        {
            get { return _MouthOpen; }
            private set { SetProperty(ref _MouthOpen, value); }
        }

        private string _MouthMoved;
        public string MouthMoved
        {
            get { return _MouthMoved; }
            private set { SetProperty(ref _MouthMoved, value); }
        }

        private string _LookingAway;
        public string LookingAway
        {
            get { return _LookingAway; }
            private set { SetProperty(ref _LookingAway, value); }
        }

        private string _Glasses;
        public string Glasses
        {
            get { return _Glasses; }
            private set { SetProperty(ref _Glasses, value); }
        }
        #endregion

        #endregion


        #region メソッド

        /// <summary>
        /// 開始メソッド
        /// </summary>
        public void Start()
        {
            Init();
        }

        /// <summary>
        /// 終了メソッド
        /// </summary>
        public void Stop()
        {
            Reader?.Dispose();
            FaceSource?.Dispose();
            FaceReader?.Dispose();
            Kinect.Close();
            Kinect = null;
        }

        /// <summary>
        /// Kinectとかデータを扱う変数の初期化処理
        /// </summary>
        private void Init()
        {
            Kinect = KinectSensor.GetDefault();
            if(Kinect == null)
            {
                throw new Exception("Kinect Sensor is null");
            }
            /// Kinectの情報
            var colorDesc = Kinect.ColorFrameSource.FrameDescription;
            // var depthDesc = Kinect.DepthFrameSource.FrameDescription; // 奥行き情報。今後使用予定
            
            /// 各描画用変数の初期化
            ColorPixels = new byte[colorDesc.Width * colorDesc.Height * BytePerPixel];
            _ColorBitmap = new WriteableBitmap(colorDesc.Width, colorDesc.Height, 96.0, 96.0, PixelFormats.Bgr32, null);
            _FacePointBitmap = new RenderTargetBitmap(colorDesc.Width, colorDesc.Height, 96.0, 96.0, PixelFormats.Default);

            Reader = Kinect.OpenMultiSourceFrameReader(FrameSourceTypes.Body | FrameSourceTypes.Color);
            Reader.MultiSourceFrameArrived += OnMultiSourceFrameArrived;

            FaceSource = new FaceFrameSource(Kinect, 0, DefaultFaceframeFeatures);

            FaceReader = FaceSource.OpenReader();

            FaceReader.FrameArrived += OnFaceFrameArrived;
            FaceSource.TrackingIdLost += OnTrackingIdLost;

            Kinect.Open();
        }

        /// <summary>
        /// 顔のトラッキングがロストした時のイベント処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnTrackingIdLost(object sender, TrackingIdLostEventArgs e)
        {
            Happy = "NONE";
            LeftEyeClosed = "NONE";
            RightEyeClosed = "NONE";
            MouthOpen = "NONE";
            MouthMoved = "NONE";
            LookingAway = "NONE";
            Glasses = "NONE";
        }

        /// <summary>
        /// FaceFrameが使えるようになったときのイベント処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnFaceFrameArrived(object sender, FaceFrameArrivedEventArgs e)
        {
            using (var faceFrame = e.FrameReference.AcquireFrame())
            {
                var result = faceFrame?.FaceFrameResult;
                if(result == null)
                {
                    return;
                }

                Happy = result.FaceProperties[FaceProperty.Happy].ToString();
                LeftEyeClosed = result.FaceProperties[FaceProperty.LeftEyeClosed].ToString();
                RightEyeClosed = result.FaceProperties[FaceProperty.RightEyeClosed].ToString();
                MouthOpen = result.FaceProperties[FaceProperty.MouthOpen].ToString();
                MouthMoved = result.FaceProperties[FaceProperty.MouthMoved].ToString();
                LookingAway = result.FaceProperties[FaceProperty.LookingAway].ToString();
                Glasses = result.FaceProperties[FaceProperty.WearingGlasses].ToString();
                FaceRotation = result.FaceRotationQuaternion;
                
            }
        }

        /// <summary>
        /// 各種データの受け取りと処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnMultiSourceFrameArrived(object sender, MultiSourceFrameArrivedEventArgs e)
        {
            var frame = e.FrameReference.AcquireFrame();
            if (frame == null)
            {
                return;
            }

            using (var bodyFrame = frame.BodyFrameReference.AcquireFrame())
            {
                if (bodyFrame == null)
                {
                    return;
                }
                Bodies = new Body[bodyFrame.BodyCount];
                bodyFrame.GetAndRefreshBodyData(Bodies);

                if (!FaceSource.IsTrackingIdValid)
                {
                    var target = (from body in Bodies where body.IsTracked select body).FirstOrDefault();
                    FaceSource.TrackingId = target?.TrackingId ?? FaceSource.TrackingId;
                }
            }

            /// 画像の情報は以下で処理する。今は使ってないが参考に
            //using (var colorFrame = frame.ColorFrameReference.AcquireFrame())
            //{
            //
            //}
        }

        #endregion

        public event PropertyChangedEventHandler PropertyChanged;
        /// <summary>
        /// プロパティの変更処理
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="field"></param>
        /// <param name="value"></param>
        /// <param name="propertyName"></param>
        private void SetProperty<T>(ref T field, T value, [CallerMemberName]string propertyName = null)
        {
            field = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
