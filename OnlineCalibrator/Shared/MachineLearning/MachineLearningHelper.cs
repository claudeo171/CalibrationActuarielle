using Microsoft.ML.Data;
using Microsoft.ML;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tensorflow.Keras.Engine;

namespace OnlineCalibrator.Shared.MachineLearning
{
    public static class MachineLearningHelper
    {
        
        public static ITransformer GenerateModel(MLContext mlContext, string tag, string imagesFolder)
        {
            string _trainTagsTsv = Path.Combine(imagesFolder, tag);
            
            // <SnippetImageTransforms>
            IEstimator<ITransformer> pipeline = mlContext.Transforms.LoadImages(outputColumnName: "input", imageFolder: imagesFolder, inputColumnName: nameof(ImageData.ImagePath))
                            .Append(mlContext.Transforms.ResizeImages(outputColumnName: "input", imageWidth: InceptionSettings.ImageWidth, imageHeight: InceptionSettings.ImageHeight, inputColumnName: "input"))
                            .Append(mlContext.Transforms.ExtractPixels(outputColumnName: "input", interleavePixelColors: InceptionSettings.ChannelsLast, offsetImage: InceptionSettings.Mean))
                            .Append(mlContext.Model.LoadTensorFlowModel("./tensorflow_inception_graph.pb").
                                ScoreTensorFlowModel(outputColumnNames: new[] { "softmax2_pre_activation" }, inputColumnNames: new[] { "input" }, addBatchDimensionInput: true))
                            .Append(mlContext.Transforms.Conversion.MapValueToKey(outputColumnName: "LabelKey", inputColumnName: "Label"))
                            .Append(mlContext.MulticlassClassification.Trainers.LbfgsMaximumEntropy(labelColumnName: "LabelKey", featureColumnName: "softmax2_pre_activation"))
                            .Append(mlContext.Transforms.Conversion.MapKeyToValue("PredictedLabelValue", "PredictedLabel"))
                            .AppendCacheCheckpoint(mlContext);

            IDataView trainingData = mlContext.Data.LoadFromTextFile<ImageData>(path: _trainTagsTsv, hasHeader: false);

            // Create and train the model
            ITransformer model = pipeline.Fit(trainingData);


            
            return model;
        }

        public static ConfusionMatrix GetConfusionMatrix(MLContext mlContext, string tag, string imagesFolder, ITransformer model)
        {
            string _trainTagsTsv = Path.Combine(imagesFolder, tag);
            IDataView testData = mlContext.Data.LoadFromTextFile<ImageData>(path: _trainTagsTsv, hasHeader: false);
            IDataView predictions = model.Transform(testData);
            MulticlassClassificationMetrics metrics =
                mlContext.MulticlassClassification.Evaluate(predictions,
                    labelColumnName: "LabelKey",
                    predictedLabelColumnName: "PredictedLabel");
            return metrics.ConfusionMatrix;
        }
        
        public static ImagePrediction ClassifySingleImage(MLContext mlContext, ITransformer model, string path)
        {
            var imageData = new ImageData()
            {
                ImagePath = path
            };
            var predictor = mlContext.Model.CreatePredictionEngine<ImageData, ImagePrediction>(model);
            var prediction = predictor.Predict(imageData);
            return prediction;
        }

        private struct InceptionSettings
        {
            public const int ImageHeight = 224;
            public const int ImageWidth = 224;
            public const float Mean = 117;
            public const float Scale = 1;
            public const bool ChannelsLast = true;
        }
    }
}
