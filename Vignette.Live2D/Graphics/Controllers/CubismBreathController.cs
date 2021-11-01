// Copyright 2020 - 2021 Vignette Project
// Licensed under MIT. See LICENSE for details.
// This software implements Live2D. Copyright (c) Live2D Inc. All Rights Reserved.
// License for Live2D can be found here: http://live2d.com/eula/live2d-open-software-license-agreement_en.html

using System;
using System.Collections.Generic;
using System.Linq;

namespace Vignette.Live2D.Graphics.Controllers
{
    /// <summary>
    /// A controller that provides breathing motion to the <see cref="CubismModel"/>.
    /// </summary>
    public class CubismBreathController : CubismController
    {
        private readonly IEnumerable<CubismBreathParameter> settings = new List<CubismBreathParameter>();
        private Dictionary<string, CubismParameter> parameterMap;

        public CubismBreathController(params CubismBreathParameter[] parameters)
        {
            settings = parameters.Any() ? parameters : default_parameters;
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();
            parameterMap = settings.ToDictionary(s => s.Parameter, s => Model.Parameters.FirstOrDefault(p => p.Name == s.Parameter));
        }

        protected override void Update()
        {
            base.Update();

            float t = (float)(Clock.CurrentTime / 1000d);
            foreach (var setting in settings)
            {
                if (!parameterMap.TryGetValue(setting.Parameter, out var parameter))
                    continue; // Nitrous forgor 💀

                float peak = (parameter.Maximum - parameter.Minimum) / 2f;
                float value = peak * MathF.Tau * MathF.Cos(t * MathF.Tau / setting.Cycle) / setting.Cycle;
                parameter.Value += value * setting.Weight * (float)(Clock.ElapsedFrameTime) / 1000f;
            }
        }

        private static readonly CubismBreathParameter[] default_parameters = new[] { new CubismBreathParameter("ParamBreath", 0.5f, 0.5f, 3.2345f, 0.5f) };
    }

    public struct CubismBreathParameter
    {
        public string Parameter { get; set; }

        public float Offset { get; set; }

        public float Peak { get; set; }

        public float Cycle { get; set; }

        public float Weight { get; set; }

        public CubismBreathParameter(string parameterName, float offset, float peak, float cycle, float weight)
        {
            Parameter = parameterName;
            Offset = offset;
            Peak = peak;
            Cycle = cycle;
            Weight = weight;
        }
    }
}
