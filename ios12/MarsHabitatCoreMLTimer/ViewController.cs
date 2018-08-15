using System;
using System.Linq;
using CoreML;
using Foundation;
using UIKit;
using Vision;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Diagnostics;

namespace MarsHabitatCoreMLTimer
{
    public partial class ViewController : UIViewController
    {
        #region constants
        const int numberOfInputs = 100000;
        const int smallNumber = numberOfInputs / 4;
        const int mediumNumber = numberOfInputs / 2;
        const int MaxSolarPanels = 100;
        const int MaxGreenHouses = 100;
        const int MaxAcres = 10000;
        #endregion

        #region fields
        MLModel model;
        MarsHabitatPricerInput[] inputs = new MarsHabitatPricerInput[numberOfInputs];
        long batchMilliseconds;
        long nonBatchMilliseconds;
        #endregion

        #region constructors
        protected ViewController(IntPtr handle) : base(handle) { }
        #endregion

        #region view controller overrides
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            SetupUI(false, "");
            SmallTestButton.SetTitle($"Timer: {smallNumber} predictions", UIControlState.Normal);
            MediumTestButton.SetTitle($"Timer: {mediumNumber} predictions", UIControlState.Normal);
            LargeTestButton.SetTitle($"Timer: {numberOfInputs} predictions", UIControlState.Normal);

            LoadMLModel();
            CreateInputs(numberOfInputs);
        }
        #endregion

        #region event handlers
        partial void LargeTestButton_TouchUpInside(UIButton sender)
        {
            RunTest(numberOfInputs);
        }

        partial void MediumTestButton_TouchUpInside(UIButton sender)
        {
            RunTest(mediumNumber);
        }

        partial void SmallTestButton_TouchUpInside(UIButton sender)
        {
            RunTest(smallNumber);
        }
        #endregion

        #region private methods
        void LoadMLModel()
        {
            var assetPath = NSBundle.MainBundle.GetUrlForResource("CoreMLModel/MarsHabitatPricer", "mlmodelc");
            model = MLModel.Create(assetPath, out NSError mlErr);
        }

        void SetupUI(bool busy, string statusLabelText)
        {
            if (busy)
            {
                Spinner.StartAnimating();
            }
            else
            {
                Spinner.StopAnimating();
            }

            SmallTestButton.Enabled = !busy;
            MediumTestButton.Enabled = !busy;
            LargeTestButton.Enabled = !busy;

            StatusLabel.Text = statusLabelText;
        }

        async void CreateInputs(int num)
        {
            SetupUI(true, "Generating sample data...");
            Random r = new Random();
            await Task.Run(() =>
            {
                for (int i = 0; i < num; i++)
                {
                    double solarPanels = r.NextDouble() * MaxSolarPanels;
                    double greenHouses = r.NextDouble() * MaxGreenHouses;
                    double acres = r.NextDouble() * MaxAcres;
                    inputs[i] = new MarsHabitatPricerInput(solarPanels, greenHouses, acres);
                }
            });
            SetupUI(false, "");
        }

        async void RunTest(int num)
        {
            SetupUI(true, $"Fetching {num} predictions with a loop...");
            await FetchNonBatchResults(num);
            SetupUI(true, $"Fetching {num} predictions with batch API...");
            await FetchBatchResults(num);
            SetupUI(false, $"Loop: {(float)nonBatchMilliseconds / 1000} sec; Batch: {(float)batchMilliseconds / 1000} sec");
        }

        async Task FetchNonBatchResults(int num)
        {
            Stopwatch stopWatch = Stopwatch.StartNew();
            await Task.Run(() =>
            {
                for (int i = 0; i < num; i++)
                {
                    model.GetPrediction(inputs[i], out NSError error);
                }
            });
            stopWatch.Stop();
            nonBatchMilliseconds = stopWatch.ElapsedMilliseconds;
        }

        async Task FetchBatchResults(int num)
        {
            var batch = new MLArrayBatchProvider(inputs.Take(num).ToArray());
            var options = new MLPredictionOptions()
            {
                UsesCpuOnly = false
            };

            Stopwatch stopWatch = Stopwatch.StartNew();
            await Task.Run(() =>
            {
                model.GetPredictions(batch, options, out NSError error);
            });
            stopWatch.Stop();
            batchMilliseconds = stopWatch.ElapsedMilliseconds;
        }
        #endregion
    }
}
