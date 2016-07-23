using System;

namespace Wheatech.Modulize
{
    public class FeatureEventArgs : EventArgs
    {
        public FeatureEventArgs(FeatureDescriptor feature)
        {
            Feature = feature;
        }

        public FeatureDescriptor Feature { get; }
    }
}
