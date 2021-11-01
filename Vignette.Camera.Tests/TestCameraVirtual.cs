// Copyright 2020 - 2021 Vignette Project
// Licensed under MIT. See LICENSE for details.

using OpenCvSharp;
using Vignette.Camera.Tests.Resources;

namespace Vignette.Camera.Tests
{
    internal class TestCameraVirtual : CameraVirtual
    {
        public new string FilePath => base.FilePath;

        public new string State => base.State.ToString();

        public TestCameraVirtual()
            : base(TestResources.GetStream(@"earth.mp4"), EncodingFormat.JPEG, new[]
            {
                new ImageEncodingParam(ImwriteFlags.JpegQuality, 20),
                new ImageEncodingParam(ImwriteFlags.JpegOptimize, 1),
            })
        {
        }
    }
}
