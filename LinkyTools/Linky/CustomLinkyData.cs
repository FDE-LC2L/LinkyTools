using System.Text;

namespace LinkyTools.Linky
{
    public class InstantConsumptionMesure
    {
        public double InstantConsumption { get; set; }
        public string FormattedTimeMeasure { get; set; } = string.Empty;
        public DateTime DateTimeMeasure { get; set; }

    }


    public static class StringExtension
    {
        public static string Right(this string str, int count)
        {
            return str.Substring(str.Length - count, count);
        }
    }

    public abstract class CustomLinkyData
    {
        #region Fields
        private readonly StringBuilder? _ReceiveBuffer;
        public string RawDatas { get; set; } = string.Empty;

        /// <summary>
        ///  Date et heure Linky en DateTime
        /// </summary>
        public DateTime? MeasureLinkyDateTime { get => GetMeasureLinkyDateTime(); }

        protected abstract DateTime? GetMeasureLinkyDateTime();

        #endregion

        public CustomLinkyData()
        {
            _ReceiveBuffer = new StringBuilder();
        }

        public void AppendRawDatas(in string rawDatas)
        {
            _ReceiveBuffer!.Append(@rawDatas);
            while (_ReceiveBuffer.Length > 4096)
            {
                _ReceiveBuffer.Remove(0, 100);
            }
            RawDatas = _ReceiveBuffer.ToString();
        }

        protected string? ExtractFromRowDatas(in string label, int charCount)
        {
            var i = RawDatas.IndexOf(label);
            if (i > 0 && i + label.Length + charCount <= RawDatas.Length)
            {
                return RawDatas.Substring(i + label.Length + 1, charCount);
            }
            return null;
        }
    }
}
