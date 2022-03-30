using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Exeter7
{
    /// <summary>
    /// Класс, описывающий такую характеристику запчасти как партномер, включая его вариации
    /// </summary>
    class Partnumber : IBinaryDataProvider
    {

        /// <summary>
        /// Партномер
        /// </summary>
        private ulong _PN;
        public ulong PN
        {
            get
            {
                return _PN;
            }
            set
            {
                _PN = value;
                wasChanged = true;
            }
        }

        /// <summary>
        /// AS-код, зачастую соответствующий партномеру
        /// </summary>
        private ulong _AS;
        public ulong AS
        {
            get
            {
                return _AS;
            }
            set
            {
                _AS = value;
                wasChanged = true;
            }
        }

        /// <summary>
        /// SC-код, не равный партномеру, но на который часто ссылаются
        /// </summary>
        private ulong _SC;
        public ulong SC
        {
            get
            {
                return _SC;
            }
            set
            {
                _SC = value;
                wasChanged = true;
            }
        }

        /// <summary>
        /// Поле, показывающее наличие несохраненных изменений
        /// </summary>
        public bool wasChanged = false;

        /// <summary>
        /// Метод, формирующий текстовое представление данных класса. Часто используется 
        /// для автоприведения типов к типу String
        /// </summary>
        /// <returns>Строка с данными класса.</returns>
        public new string ToString()
        {
            if (_PN > 0)
            {
                //return new String(_PN);
                return _PN.ToString();
            }
            else
                return String.Empty;
        }

        #region Реализация интерфейса IBinaryDataProvider
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

        /// <summary>
        /// Метод, реализующий загрузку данных из двоичного потока
        /// </summary>
        /// <param name="bReader">Ссылка на открытый в двоичном режиме файловый поток - reader</param>
        public void Load(BinaryReader bReader)
        {

            // 1) Читаем версию данных
            _versionData = bReader.ReadInt32();

            // 2) Проверяем, нужна ли конверсия
            if (_versionData == _versionCode)
            {
                // Нет, конверсия не нужна, просто загружаем данные

                // INFO Чтение данных для ТЕКУЩЕЙ версии кода Partnumber._versionCode == 1
                _PN = bReader.ReadUInt64();
                _AS = bReader.ReadUInt64();
                _SC = bReader.ReadUInt64();
            }
            else if ((_versionData > _versionCode) || (_versionData < 1))
            {
                // Данные новее программы или версия данных = 0. Невозможно загрузить!
                throw new System.Exception("Data version is out of range. Current Partnumber class version is " + _versionCode.ToString() + " and the data version is " + _versionData.ToString());
            }
            else
            {
                // Нужна конверсия. Используем нестандартную последовательность загрузки.
            }

            // Сбрасываем флаг изменений, т.к. мы только-только загрузили данные
            wasChanged = false;
        }


        /// <summary>
        /// Метод производит сохранение данных в двоичном формате без проверок. 
        /// Сохранение производится с учетом версии ДАННЫХ.
        /// </summary>
        /// <remarks>Решение о необходимости конверсии данных до версии КОДА принимается в 
        /// верхних классах иерархии. Там же ведется учет произведенных изменений. В классах же, 
        /// реализующих IBinaryDataProvider, стоит задача просто произвести запись на диск.</remarks>
        /// <param name="bWriter">Ссылка на открытый в двоичном режиме файловый поток - writer</param>
        public void Save(BinaryWriter bWriter)
        {
            
            // Если версии данных и кода совпадают, просто сохраняем данные
            if (_versionData == _versionCode)
            {
                // INFO Запись данных для ТЕКУЩЕЙ версии кода Partnumber._versionCode == 1
                bWriter.Write(_versionData);
                bWriter.Write(_PN);
                bWriter.Write(_AS);
                bWriter.Write(_SC);
           }
//            else if ()
//            {
//                // ...иначе нужно произвести запись в соответствии с более старой версией данных
//            }

            // Сбрасываем флаг изменений, т.к. мы только-только сохранили данные
            wasChanged = false;
        }

        #endregion

        #region Конструкторы
        /// <summary>
        /// Конструктор по-умолчанию
        /// </summary>
        Partnumber()
        {
            // Присваиваем значения внутренним полям напрямую, т.к. 
            // нет необходимости пока отслеживать изменения
            _PN = 0;
            _AS = 0;
            _SC = 0;
        }

        /// <summary>
        /// Конструктор объекта, у которого известен лишь партномер
        /// </summary>
        /// <param name="partnumber">Партномер</param>
        Partnumber(ulong partnumber)
        {

            // Присваиваем значения внутренним полям напрямую, т.к. 
            // нет необходимости пока отслеживать изменения
            if ((partnumber > 0) && (partnumber < AppSettings.maxPN))
            {
                _PN = partnumber;
            }
            else
            {
                _PN = 0;
            }
            _AS = 0;
            _SC = 0;
        }

        /// <summary>
        /// Конструктор для объекта, у которого известны лишь AS и SC коды.
        /// </summary>
        /// <remarks>Как правило, эти данные находятся на стикерах, прикрепленных к запчасти.</remarks>
        /// <param name="asValue">AS-код (зачастую равен партномеру)</param>
        /// <param name="scValue">SC-код</param>
        Partnumber(ulong asValue, ulong scValue)
        {

            // Присваиваем значения внутренним полям напрямую, т.к. 
            // нет необходимости пока отслеживать изменения
            if ((asValue > 0) && (asValue < AppSettings.maxPN))
            {
                _AS = asValue;
                _PN = asValue;
            }
            else
            {
                _PN = 0;
                _AS = 0;
            }

            if ((scValue > 0) && (scValue < AppSettings.maxPN))
            {
                _SC = scValue;
            }
            else
            {
                _SC = 0;
            }
        }

        /// <summary>
        /// Конструктор для объекта, у которого все 3 значения известны
        /// </summary>
        /// <param name="partnumber">Партномер</param>
        /// <param name="asValue">AS-код (зачастую равен партномеру)</param>
        /// <param name="scValue">SC-код</param>
        Partnumber(ulong partnumber, ulong asValue, ulong scValue)
        {

            // Присваиваем значения внутренним полям напрямую, т.к. 
            // нет необходимости пока отслеживать изменения
            if ((partnumber > 0) && (partnumber < AppSettings.maxPN))
            {
                _PN = partnumber;
            }
            else
            {
                _PN = 0;
            }

            if ((asValue > 0) && (asValue < AppSettings.maxPN))
            {
                _AS = asValue;
            }
            else
            {
                _AS = 0;
            }

            if ((scValue > 0) && (scValue < AppSettings.maxPN))
            {
                _SC = scValue;
            }
            else
            {
                _SC = 0;
            }
        }
        #endregion

    }
}
