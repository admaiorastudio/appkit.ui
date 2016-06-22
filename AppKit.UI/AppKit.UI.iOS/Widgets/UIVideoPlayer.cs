namespace AdMaiora.AppKit.UI
{
    using System;
    using System.Drawing;

    using AVFoundation;
    using CoreMedia;
    using Foundation;
    using UIKit;

    [Register("UIVideoPlayer")]
    public class UIVideoPlayer : UIView
    {
        private AVPlayer _mediaPlayer;
        private AVPlayerLayer _playerLayer;

        private NSObject _didPlayObserver;

        public event EventHandler ReadyToPlay;
        public event EventHandler FinishedPlaying;

        public UIVideoPlayer(RectangleF frame)
            : base(frame)
        {       
            Initialize();
        }

        public UIVideoPlayer(IntPtr handle)
            : base(handle)
        {
            Initialize();
        }

        public bool Looping
        {
            get
            {
                return _mediaPlayer.ActionAtItemEnd == AVPlayerActionAtItemEnd.None;
            }
            set
            {
                _mediaPlayer.ActionAtItemEnd = 
                    value ? AVPlayerActionAtItemEnd.None : AVPlayerActionAtItemEnd.Pause;
            }
        }

        public float Volume
        {
            get
            {
                return _mediaPlayer.Volume;
            }
            set
            {
                _mediaPlayer.Volume = value;
            }
        }

        public long Duration
        {
            get
            {
                if (_mediaPlayer.CurrentItem == null)
                    return 0;

                return (long)(_mediaPlayer.CurrentItem.Duration.Seconds * 1000);
            }
        }

        public long CurrentPosition
        {
            get
            {
                if (_mediaPlayer.CurrentItem == null)
                    return 0;

                return (long)(_mediaPlayer.CurrentItem.CurrentTime.Seconds * 1000);
            }
        }

        public bool IsPlaying
        {
            get
            {
                if (_mediaPlayer.CurrentItem == null)
                    return false;

                return _mediaPlayer.Rate > 0f;
            }
        }

        public void SetVideoURL(NSUrl url)
        {
            _mediaPlayer.ReplaceCurrentItemWithPlayerItem(new AVPlayerItem(url));

            // Observe did play to end time notification to notify finished play event

            if (_didPlayObserver != null)
            {
                _mediaPlayer.CurrentItem.RemoveObserver(_didPlayObserver);
                _didPlayObserver.Dispose();
            }

            _didPlayObserver = _mediaPlayer.CurrentItem.AddObserver(AVPlayerItem.DidPlayToEndTimeNotification, n =>
            {
                if(_mediaPlayer.ActionAtItemEnd == AVPlayerActionAtItemEnd.None)
                {
                    // Loop
                    _mediaPlayer.Seek(CMTime.FromSeconds(0, _mediaPlayer.CurrentTime.TimeScale));
                }
                else
                {
                    OnFinishedPlaying();
                }
            });

            _mediaPlayer.AddObserver(AVPlayerItem.NewErrorLogEntryNotification, n =>
            {
                System.Diagnostics.Debug.WriteLine(_mediaPlayer.CurrentItem.Error.ToString());

            });

            _mediaPlayer.AddObserver(AVPlayerItem.ItemFailedToPlayToEndTimeErrorKey, n =>
            {
                System.Diagnostics.Debug.WriteLine(_mediaPlayer.CurrentItem.Error.ToString());

            });

        }

        public void Play()
        {
            _mediaPlayer.Play();
        }

        public void Pause()
        {
            _mediaPlayer.Pause();
        }

        public void SeekTo(double milliseconds)
        {
            if (_mediaPlayer.CurrentItem == null)
                return;

            _mediaPlayer.Seek(CMTime.FromSeconds(milliseconds / 1000, _mediaPlayer.CurrentTime.TimeScale));
        }
            
        public override void ObserveValue(NSString keyPath, NSObject ofObject, NSDictionary change, IntPtr context)
        {
            if (_mediaPlayer.Status == AVPlayerStatus.ReadyToPlay)
                OnReadyToPlay();
        }
            
        protected void OnReadyToPlay()
        {
            if (ReadyToPlay != null)
                ReadyToPlay(this, EventArgs.Empty);
        }
            
        protected void OnFinishedPlaying()
        {
            if (FinishedPlaying != null)
                FinishedPlaying(this, EventArgs.Empty);
        }

        private void Initialize()
        {
            this.BackgroundColor = UIColor.Red;

            _mediaPlayer = new AVPlayer();

            _playerLayer = AVPlayerLayer.FromPlayer(_mediaPlayer);
            _playerLayer.VideoGravity = AVLayerVideoGravity.ResizeAspectFill;
            _playerLayer.NeedsDisplayOnBoundsChange = true;
            _playerLayer.Frame = this.Bounds;
            _playerLayer.Player = _mediaPlayer;
            this.Layer.AddSublayer(_playerLayer);


            // Observe status to notify ready to play event
            _mediaPlayer.AddObserver(this, "status");
        }

    }
}

