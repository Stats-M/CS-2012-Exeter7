using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Exeter7
{
    /// <summary>
    /// Объект, описывающий запасную часть с уникальным партномером.
    /// </summary>
    /// <remarks>Для апгрейдов классом ProductID предусмотрена мимикрия
    /// номера продукта под партномер для унифицирования методов работы.</remarks>
    class SparePart : IBinaryDataProvider
    {

        #region Поля класса - обрабатываемые данные
        /// <summary>
        /// Экземпляр класса, обрабатывающего данные о номере продукта запчасти
        /// </summary>
        private ProductID _productID;
        /// <summary>
        /// Экземпляр класса, обрабатывающего данные о партномере запчасти
        /// </summary>
        private Partnumber _partnumber;
        /// <summary>
        /// Экземпляр класса, обрабатывающего данные о применимости запчасти в банкоматах
        /// </summary>
        private WhereUsed _whereUsed;
        #endregion

        #region Поля класса
        /// <summary>
        /// Поле, показывающее наличие несохраненных изменений. Реализовано как
        /// свойство, т.к. классу нужно опрашивать аналогичные поля у подклассов 
        /// после попыток внесения в них изменений (т.е. выдается команда подклассу 
        /// изменить поле и считывается его поле _wasChanged чтобы определить, 
        /// были ли в действительности сделаны изменения подклассом или нет.
        /// </summary>
        private bool _wasChanged = false;


        /// <summary>
        /// Свойство (read-only), показывающее наличие несохраненных изменений как в самом 
        /// классе, так и в классах обрабатываемых данных (т.к. данные не видны с более 
        /// высоких уровней иерархии классов)
        /// </summary>
        public bool WasChanged
        {
            get
            {
                // TODO Проверяем все подклассы на наличие изменений при помощи (condition ? first_expression : second_expression)
                return (_wasChanged || (_productID == null ? false : _productID.wasChanged) || (_partnumber == null ? false : _partnumber.wasChanged));
            }
            // Нет нужды прописывать set? Методы класса сами знают когда изменять флаг, 
            // а снаружи изменять флаг бессмысленно, т.к. непонятно к какой части класса
            // должно относиться такое изменение...
        }
        #endregion

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
                _wasChanged = true;
            }

        }

        /// <summary>
        /// Метод, реализующий загрузку данных из двоичного потока
        /// </summary>
        /// <param name="bReader">Ссылка на открытый в двоичном режиме файловый поток</param>
        public void Load(BinaryReader bReader)
        {

            // 1) Читаем версию данных
            _versionData = bReader.ReadInt32();

            // 2) Проверяем, нужна ли конверсия
            if (_versionData == _versionCode)
            {
                // Нет, конверсия не нужна, просто загружаем данные
                
                // TODO INFO Чтение данных для ТЕКУЩЕЙ версии кода SparePart._versionCode == 1
                // Partnumber pnumber = new(Partnumber)
                // pnumber.Load(bReader)
            }
            else if ((_versionData > _versionCode) || (_versionData < 1))
            {
                // Данные новее программы или версия данных = 0. Невозможно загрузить!
                throw new System.Exception("Data version is out of range. Current SparePart class version is " + _versionCode.ToString() + " and the data version is " + _versionData.ToString());
            }
            else
            {
                // Нужна конверсия. Используем нестандартную последовательность загрузки.
            }

            // Сбрасываем флаг изменений, т.к. мы только-только загрузили данные
            _wasChanged = false;

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
            // TODO реализовать метод
        }

        #endregion

        #region Конструкторы
        /// <summary>
        /// Конструктор по-умолчанию
        /// </summary>
        SparePart()
        {
            _productID = null;
            _partnumber = null;
            _whereUsed = null;
        }

        #endregion
    }
}
