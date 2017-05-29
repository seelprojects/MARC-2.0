using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WekaClassifier.Enums
{
    /// <summary>
    /// Public enumerations
    /// </summary>
    public enum ClassifierName
    {
        SupportVectorMachine,
        NaiveBayes,
        RandomForest
    }

    /// <summary>
    /// Public enumerations for Text Filter Types
    /// </summary>
    public enum TextFilterType
    {
        NoFilter,
        StopwordsRemoval,
        Stemming,
        StopwordsRemovalStemming
    };
}
