﻿// Copyright 2020 - 2021 Vignette Project
// Licensed under MIT. See LICENSE for details.

using osuTK;

namespace Vignette.Camera
{
    /// <summary>
    /// An interface containing base logic for cameras.
    /// </summary>
    public interface ICamera
    {
        /// <summary>
        /// The output image width.
        /// </summary>
        public float Width { get; }

        /// <summary>
        /// The output image height.
        /// </summary>
        public float Height { get; }

        /// <summary>
        /// The output image size.
        /// </summary>
        public Vector2 Size { get; }

        /// <summary>
        /// The frequency of frames outputted by the device in seconds.
        /// </summary>
        public double FramesPerSecond { get; }

        /// <summary>
        /// The data output from the capture device.
        /// </summary>
        public byte[] Data { get; }

        /// <summary>
        /// The paused state of this <see cref="ICamera"/>.
        /// </summary>
        public bool Paused { get; }

        /// <summary>
        /// Whether this <see cref="ICamera"/> has started decoding.
        /// </summary>
        public bool Started { get; }

        /// <summary>
        /// Whether this <see cref="ICamera"/> has finished or stopped decoding.
        /// </summary>
        public bool Stopped { get; }

        /// <summary>
        /// Whether this <see cref="ICamera"/> is ready to start decoding.
        /// </summary>
        public bool Ready { get; }

        /// <summary>
        /// Starts the decoding process for this camera.
        /// </summary>
        void Start();

        /// <summary>
        /// Suspends the decoding process.
        /// </summary>
        void Pause();

        /// <summary>
        /// Resumes the decoding process.
        /// </summary>
        void Resume();

        /// <summary>
        /// Ends the decoding process for this camera and cleans up resources.
        /// </summary>
        /// <param name="waitForDecoder">Wait for the last tick to finish before proceeding to cleanup.</param>
        void Stop(bool waitForDecoder);
    }
}
