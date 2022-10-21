using System.Globalization;

namespace LinkyTools.Linky
{

    public class LinkyStandardData : CustomLinkyData
    {
        /// <summary>
        /// Classe statique avec les patterns à rechercher dans les trames 
        /// Linky pour récupérer les informations.
        /// </summary>
        private static class Patterns
        {
            /// <summary>
            /// Date courante du compteur Linky.
            /// </summary>
            public const string CurrentDateTime = "DATE";
            /// <summary>
            /// Energie active soutirée Fournisseur. Ce pattern doit être
            /// complété de "01" à "10".
            /// </summary>
            public const string ActiveEnergyConsumed = "EASF";
            /// <summary>
            /// Puissance Apparente Instantanée Soutirée.
            /// </summary>
            public const string ApparentPowerInstantaneousConsumed = "SINSTS";
            /// <summary>
            /// Puissance apparente de coupure.
            /// </summary>            
            public const string ApparentPowerCut = "PCOUP";
            /// <summary>
            /// Puissance apparente de référence. (Puissance souscrite aupré du fournisseur)
            /// </summary>            
            public const string ApparentPowerReference = "PREF";
            /// <summary>
            /// Tension efficace. Ce pattern doit être complété avec 1, 2 ou 3
            /// afin de récupérer la tension de la phase correspondante.
            /// En monophasé => 1
            /// </summary>
            public const string EffectiveVoltage = "URMS";
        }

        #region Fields   
        private string _CurrentDateAsString = string.Empty;
        private string _FormatedCurrentDateTime = string.Empty;
        private string _FormatedCurrentTime = string.Empty;
        private DateTime? _MeasureLinkyDateTime = null;
        private readonly int[] _ActiveConsumedEnergySupplier_Index;
        private readonly int[] _RmsVoltagePhase;
        private int _InstantConsumedPower;
        private int _PowerCut;
        private int _PowerRef;

        /// <summary>
        /// Date et heure Linky. Linky format H081225223518 
        /// </summary>
        public string CurrentDateAsString { get => GetCurrentDateAsString(); }
        /// <summary>
        ///  Date et heure Linky formattée avec la current culture
        /// </summary>
        public string FormatedCurrentDateTime { get => GetFormatedCurrentDateTime(); }
        /// <summary>
        ///  Heure Linky formattée au format HH:mm:ss
        /// </summary>
        public string FormatedCurrentTime { get => GetFormatedCurrentTime(); }
        /// <summary>
        /// Energie active soutirée Fournisseur, index 01 (EASF01) 
        /// </summary>
        public int ActiveConsumedEnergySupplier_Index01 { get => GetActiveEnergyConsumedSupplier_Index("01"); }
        /// <summary>
        /// Puissance app.Instantanée soutirée (SINSTS) 
        /// </summary>
        public int InstantConsumedPower { get => GetInstantConsumedPower(); }
        /// <summary>
        /// Puissance app. de coupure (PCOUP)
        /// </summary>
        public int PowerCut { get => GetPowerCut(); }
        /// <summary>
        /// PPuissance app. de référence (PREF)
        /// </summary>
        public int PowerRef { get => GetPowerRef(); }
        /// <summary>
        /// Tension efficace, phase 1 
        /// </summary>
        public int RmsVoltagePhase1 { get => GetRmsVoltagePhase("1"); }
        #endregion

        #region Ctor
        public LinkyStandardData() : base()
        {
            _ActiveConsumedEnergySupplier_Index = new int[10];
            _RmsVoltagePhase = new int[3];
        }
        #endregion

        private string GetCurrentDateAsString()
        {
            _CurrentDateAsString = ExtractFromRowDatas(Patterns.CurrentDateTime, 13) ?? _CurrentDateAsString;
            return _CurrentDateAsString;
        }

        private string GetFormatedCurrentDateTime()
        {
            var dt = CurrentDateAsString;
            if (dt.Length == 13)
            {
                if (DateTime.TryParseExact(dt.Right(12), "yyMMddHHmmss", CultureInfo.InvariantCulture, DateTimeStyles.None, out var resultDateTime))
                {
                    _FormatedCurrentDateTime = dt[0].ToString() + " " + resultDateTime.ToString(CultureInfo.CurrentCulture);
                }
            }
            return _FormatedCurrentDateTime;
        }

        private string GetFormatedCurrentTime()
        {
            var dt = CurrentDateAsString;
            if (dt.Length == 13)
            {
                if (DateTime.TryParseExact(dt.Right(12), "yyMMddHHmmss", CultureInfo.InvariantCulture, DateTimeStyles.None, out var resultDateTime))
                {
                    _FormatedCurrentTime = resultDateTime.ToString("HH:mm:ss");
                }
            }
            return _FormatedCurrentTime;
        }

        protected override DateTime? GetMeasureLinkyDateTime()
        {
            if (CurrentDateAsString.Length == 13 && DateTime.TryParseExact(CurrentDateAsString.Right(12), "yyMMddHHmmss", CultureInfo.InvariantCulture, DateTimeStyles.None, out var dt))
            {
                _MeasureLinkyDateTime = dt;
            }
            return _MeasureLinkyDateTime;
        }

        private int GetActiveEnergyConsumedSupplier_Index(in string index)
        {
            if (int.TryParse(ExtractFromRowDatas(Patterns.ActiveEnergyConsumed + index, 9), out int pwr))
            {
                _ActiveConsumedEnergySupplier_Index[Convert.ToInt32(index) - 1] = pwr;
            }
            return _ActiveConsumedEnergySupplier_Index[Convert.ToInt32(index) - 1];
        }

        private int GetInstantConsumedPower()
        {
            if (int.TryParse(ExtractFromRowDatas(Patterns.ApparentPowerInstantaneousConsumed, 5), out int pwr))
            {
                _InstantConsumedPower = pwr;
            }
            return _InstantConsumedPower;
        }

        private int GetPowerCut()
        {
            if (int.TryParse(ExtractFromRowDatas(Patterns.ApparentPowerCut, 2), out int pwr))
            {
                _PowerCut = pwr;
            }
            return _PowerCut;
        }

        private int GetPowerRef()
        {
            if (int.TryParse(ExtractFromRowDatas(Patterns.ApparentPowerReference, 2), out int pwr))
            {
                _PowerRef = pwr;
            }
            return _PowerRef;
        }

        private int GetRmsVoltagePhase(in string index)
        {
            if (int.TryParse(ExtractFromRowDatas(Patterns.EffectiveVoltage + index, 3), out int rms))
            {
                _RmsVoltagePhase[Convert.ToInt32(index) - 1] = rms;
            }
            return _RmsVoltagePhase[Convert.ToInt32(index) - 1];
        }

    }
}
