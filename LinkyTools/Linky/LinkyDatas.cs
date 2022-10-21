using LinkyTools.DataReader;

namespace LinkyTools.Linky
{
    public class AddMeasurementEventArgs : EventArgs
    {
        public CustomLinkyData? LinkyData { get; set; }
        public int TotalMeasurementCount { get; set; }
    }

    public class LinkyDatas : List<CustomLinkyData>
    {
        #region Events
        public event EventHandler<AddMeasurementEventArgs>? OnAddMeasurement;
        #endregion

        /// <summary>
        /// Historical duration of measurements (in minutes).
        /// </summary>
        public int HistoricalDuration { get; set; }

        public LinkyDatas()
        {
            HistoricalDuration = 24 * 60;
        }

        /// <summary>
        /// Add a measurement to the measurement history.
        /// </summary>
        /// <param name="linkyData">The Linky measure</param>
        public void AddMeasurement(in CustomLinkyData linkyData)
        {
            // If the measure does not have a valid datetime then it is ignored
            if (linkyData.MeasureLinkyDateTime is object)
            {
                RemoveAll(ms => ms.MeasureLinkyDateTime < DateTime.Now.AddMinutes(HistoricalDuration * -1));
                Add(linkyData);
            }
            FireOnAddMeasurementEvent(linkyData);
        }

        private void FireOnAddMeasurementEvent(in CustomLinkyData linkyData)
        {
            var eventArgs = new AddMeasurementEventArgs
            {
                LinkyData = linkyData,
                TotalMeasurementCount = Count
            };
            OnAddMeasurement?.Invoke(this, eventArgs);
        }
    }

}
