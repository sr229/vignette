// Copyright 2020 - 2021 Vignette Project
// Licensed under MIT. See LICENSE for details.
// This software implements Live2D. Copyright (c) Live2D Inc. All Rights Reserved.
// License for Live2D can be found here: http://live2d.com/eula/live2d-open-software-license-agreement_en.html

namespace Vignette.Live2D.Physics
{
    public struct CubismPhysicsNormalizationTuplet
    {
        public float Maximum { get; set; }

        public float Minimum { get; set; }

        public float Default { get; set; }
    }

    public struct CubismPhysicsNormalization
    {
        public CubismPhysicsNormalizationTuplet Position { get; set; }

        public CubismPhysicsNormalizationTuplet Angle { get; set; }
    }
}
