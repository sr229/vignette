﻿// Copyright 2020 - 2021 Vignette Project
// Licensed under MIT. See LICENSE for details.

using System;
using System.Threading;
using System.Threading.Tasks;
using OpenCvSharp;
using osu.Framework.Logging;
using osuTK;

namespace Vignette.Camera
{
    /// <summary>
    /// A base class for camera components which handles device or file access, disposal, and update logic.
    /// </summary>
    public abstract class Camera : IDisposable, ICamera
    {
        public float Width => (Capture.IsDisposed) ? 0 : Capture.FrameWidth;

        public float Height => (Capture.IsDisposed) ? 0 : Capture.FrameHeight;

        public Vector2 Size => new Vector2(Width, Height);

        public double FramesPerSecond => (Capture.IsDisposed) ? 0 : Capture.Fps;

        public byte[] Data { get; private set; }

        /// <summary>
        /// Fired when a new update occurs. The frequency of invocations is tied to the <see cref="FramesPerSecond"/>.
        /// </summary>
        public event Action OnTick;

        public bool Paused => State == DecoderState.Paused;

        public bool Started => State == DecoderState.Started;

        public bool Stopped => State == DecoderState.Stopped;

        public bool Ready => State == DecoderState.Ready;

        /// <summary>
        /// Whether this <see cref="Camera"/> has been disposed.
        /// </summary>
        public bool IsDisposed { get; private set; }

        protected DecoderState State { get; private set; } = DecoderState.Ready;

        protected Mat Mat { get; private set; }

        private static Logger logger => Logger.GetLogger("performance-camera");

        private readonly EncodingFormat format;
        private readonly ImageEncodingParam[] encodingParams;
        private CancellationTokenSource decodingTaskCancellationToken;
        private Task decodingTask;

        internal VideoCapture Capture;

        public Camera(EncodingFormat format = EncodingFormat.PNG, ImageEncodingParam[] encodingParams = null)
        {
            this.format = format;
            this.encodingParams = encodingParams;
        }

        public void Start()
        {
            // throw if we encounter errors
            Capture.SetExceptionMode(true);

            if (!Ready)
                return;

            decodingTaskCancellationToken = new CancellationTokenSource();
            decodingTask = Task.Factory.StartNew(() => decodingLoop(decodingTaskCancellationToken), decodingTaskCancellationToken.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default);

            State = DecoderState.Started;
        }

        public virtual void Pause()
        {
            if (Ready || Paused)
                return;

            State = DecoderState.Paused;
        }

        public virtual void Resume()
        {
            if (Ready || !Paused)
                return;

            State = DecoderState.Started;
        }

        public void Stop(bool waitForDecoder)
        {
            // The capture has a reference to the device or file even if decoding hasn't started yet.
            Capture?.Release();

            if (Ready)
                return;

            decodingTaskCancellationToken.Cancel();
            if (waitForDecoder)
                decodingTask.Wait();

            OnTick = null;

            decodingTask.Dispose();
            decodingTask = null;

            decodingTaskCancellationToken.Dispose();
            decodingTaskCancellationToken = null;

            Capture?.Dispose();

            State = DecoderState.Stopped;
        }

        private void decodingLoop(CancellationTokenSource token)
        {
            while (true)
            {
                if (token.IsCancellationRequested)
                    break;

                PreTick();

                if (Paused)
                    continue;

                try
                {
                    // Don't do anything when there are no more frames or the device has been disconnected.
                    if (!Capture.Grab())
                        continue;

                    Mat = Capture.RetrieveMat();

                    if (!Mat.Empty())
                        Data = Mat.ToBytes(getStringfromEncodingFormat(format), encodingParams);

                    OnTick?.Invoke();
                }
                catch (OpenCVException e)
                {
                    logger.Add($@"{e.Status} {e.ErrMsg}", LogLevel.Verbose, e);
                }

                Thread.Sleep((int)Math.Round(1000 / Math.Max(FramesPerSecond, 1)));
            }
        }

        protected virtual void PreTick()
        {
        }

        protected virtual void Dispose(bool disposing)
        {
            if (IsDisposed)
                return;

            Stop(true);

            IsDisposed = true;
        }

        ~Camera()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private string getStringfromEncodingFormat(EncodingFormat format)
        {
            switch (format)
            {
                case EncodingFormat.PNG:
                    return ".png";

                case EncodingFormat.JPEG:
                    return ".jpg";

                case EncodingFormat.TIFF:
                    return ".tif";

                case EncodingFormat.WebP:
                    return ".webp";

                case EncodingFormat.Bitmap:
                    return ".bmp";

                case EncodingFormat.JPEG2000:
                    return ".jp2";

                case EncodingFormat.PBM:
                    return ".pbm";

                case EncodingFormat.Raster:
                    return ".ras";

                default:
                    throw new ArgumentOutOfRangeException($@"""{nameof(format)}"" is not a valid export format.");
            }
        }

        protected enum DecoderState
        {
            Ready,

            Started,

            Paused,

            Stopped,
        }
    }
}
