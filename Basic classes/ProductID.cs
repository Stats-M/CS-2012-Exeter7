using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Exeter7
{
    class ProductID : IBinaryDataProvider
    {
        private string _PID;
        public string PID
        {
            get
            {
                if (_PID == null)
                {
                    return String.Empty;
                }
                else
                {
                    return _PID;
                }
            }
            set
            {
                _PID = value;
            }
        }

        private string _PName;
        public string PName
        {
            get
            {
                if (_PName == null)
                {
                    return String.Empty;
                }
                else
                {
                    return _PName;
                }
            }
            set
            {
                _PName = value;
            }
        }

        private string _description;
        public string Description
        {
            get
            {
                if (_description == null)
                {
                    return String.Empty;
                }
                else
                {
                    return _description;
                }
            }
            set
            {
                _description = value;
            }
        }

        /// <summary>
        /// Поле, показывающее наличие несохраненных изменений
        /// </summary>
        public bool wasChanged = false;

        ProductID()
        {
            _PID = null;
            _PName = null;
            _description = null;
        }


        ProductID(string pid, string pname, string description)
        {
            _PID = pid;
            _PName = pname;
            _description = description;
        }

        // TODO Предусмотреть для каждого апгрейда генерацию поддельного партномера
        // Это необходимо для однообразной работы как с запчастями, так и с апгрейдами.
        // Может быть реализовано через ведение отдельной базы данных соответствия
        // номеров продуктов и поддельных партномеров, скажем, 222-ххххххх

        public override string ToString()
        {
            return PID;
        }

        #region Реализация интерфеса IBinaryDataProvider

        /// <summary>
        /// Версия кода класса. Внутреннее поле - константа, изменяется 
        /// только вручную при компилировании изменений.
        /// </summary>
        private const int _versionCode = 1;
        public int VersionCode
        {
            get
            {
                return _versionCode;
            }
        }

        /// <summary>
        /// Версия данных класса (версия класса, которой была произведена запись данных)
        /// </summary>
        private int _versionData = 0;
        public int VersionData
        {
            get
            {
                return _versionData;
            }
            set
            {
                _versionData = value;
                wasChanged = true;
            }

        }

        void IBinaryDataProvider.Load(System.IO.BinaryReader bReader)
        {
            throw new NotImplementedException();
        }

        void IBinaryDataProvider.Save(System.IO.BinaryWriter bWriter)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
