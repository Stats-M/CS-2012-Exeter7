using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Exeter7
{
    /// <summary>
    /// Класс, описывающий применимость запчасти в разных моделях, узлах и системах банкоматов
    /// </summary>
    class WhereUsed : IBinaryDataProvider
    {

        // DEP countATM_Enums dependancy, edit all entries at once
        #region Поля - перечисления с битовыми флагами.
        private ATM_FamilyList _familyList;
        private ATM_Models_Ancient _ancientModelsList;
        private ATM_Models_Old _oldModelsList;
        private ATM_Models_Current _currentModelsList;
        private ATM_Models_Future _futureModelsList;
        private ATM_SystemsList _systemsList;
        private ATM_SubystemsList _subsystemsList;
        private ATM_SystemTypesList _systemTypesList;
        private ATM_ItemTypesList _itemTypesList;
        #endregion

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
            if (_currentModelsList != ATM_Models_Current.Undefined)
            {
                return _currentModelsList.ToString();
            }
            else
            {
                return String.Empty;
            }
        }

        /// <summary>
        /// Переводит флаги перечислений ATM_... в массив типа bool[,] для работы с   
        /// CheckBox пользовательского интерфейса.
        /// </summary>
        /// <remarks>Массив bool не использутся внутри класса, он нужен вне класса, 
        /// как правило, при отображении данных в интерфейсе пользователя, где 
        /// элементы единого массива удобнее битовых полей разных перечислений-флагов.</remarks>
        /// <param name="usageArray">(ref) Двумерный массив значений флагов.</param>
        public void GetUsageArray(ref bool[,] usageArray)
        {

            Program.Log.Write("WhereUsed::GetUsageArray -> Перевод флагов-перечислений в массив bool");

            if (!UsageArrayHelper.Validate(usageArray))
            {
                Program.Log.Write(String.Format("WhereUsed::GetUsageArray -> Размерность массива ({0},{1}) не сответствует ожидаемой ({2},32).",
                        usageArray.GetLength(0).ToString(), 
                        usageArray.GetLength(1).ToString(), 
                        AppSettings.countATM_Enums.ToString()), Logs.MsgType.Error);
                // Серьезная ошибка. Останавливаем выполнение
                throw new System.Exception("WhereUsed::GetUsageArray -> 'usageArray' dimensions are (" +
                    usageArray.GetLength(0).ToString() + "," +
                    usageArray.GetLength(1).ToString() + ") instead of (" +
                    AppSettings.countATM_Enums.ToString() + ", 32).");
            }

            // Все нормально, заполняем массив значениями.

            // Декларируем временные переменные для битовых операций
            // DEP countATM_Enums dependancy, edit all entries at once
            // ВСЕГДА ПОДДЕРЖИВАЕМ ОДНУ И ТУ ЖЕ ОЧЕРЕДНОСТЬ для избежания ошибок
            ATM_FamilyList tmp_familyList_value;
            ATM_Models_Ancient tmp_ancientModelsList_value;
            ATM_Models_Old tmp_oldModelsList_value;
            ATM_Models_Current tmp_currentModelsList_value;
            ATM_Models_Future tmp_futureModelsList_value;
            ATM_SystemsList tmp_systemsList_value;
            ATM_SubystemsList tmp_subsystemsList_value;
            ATM_SystemTypesList tmp_systemTypesList_value;
            ATM_ItemTypesList tmp_itemTypesList_value;
           
            for (int i = 0; i < 32; i++)
            {
                // Схема проверки флагов: (перечисление & 1 бит) == этому 1 биту?
                // Если да, то соответствующий флаг установлен и в массив записывается "true",
                // иначе пишется "false". Т.е. мы каким-либо значением инициализируем все
                // элементы массива и неинициализированных элементов не останется.

                // DEP countATM_Enums dependancy, edit all entries at once
                // ВСЕГДА ПОДДЕРЖИВАЕМ ОДНУ И ТУ ЖЕ ОЧЕРЕДНОСТЬ для избежания ошибок
                tmp_familyList_value = (ATM_FamilyList)(1 << i);
                usageArray[0, i] = (((_familyList & tmp_familyList_value) == tmp_familyList_value) ? true : false);

                tmp_ancientModelsList_value = (ATM_Models_Ancient)(1 << i);
                usageArray[1, i] = (((_ancientModelsList & tmp_ancientModelsList_value) == tmp_ancientModelsList_value) ? true : false);

                tmp_oldModelsList_value = (ATM_Models_Old)(1 << i);
                usageArray[1, i] = (((_oldModelsList & tmp_oldModelsList_value) == tmp_oldModelsList_value) ? true : false);

                tmp_currentModelsList_value = (ATM_Models_Current)(1 << i);
                usageArray[2, i] = (((_currentModelsList & tmp_currentModelsList_value) == tmp_currentModelsList_value) ? true : false);

                tmp_futureModelsList_value = (ATM_Models_Future)(1 << i);
                usageArray[3, i] = (((_futureModelsList & tmp_futureModelsList_value) == tmp_futureModelsList_value) ? true : false);

                tmp_systemsList_value = (ATM_SystemsList)(1 << i);
                usageArray[4, i] = (((_systemsList & tmp_systemsList_value) == tmp_systemsList_value) ? true : false);

                tmp_subsystemsList_value = (ATM_SubystemsList)(1 << i);
                usageArray[5, i] = (((_subsystemsList & tmp_subsystemsList_value) == tmp_subsystemsList_value) ? true : false);

                tmp_systemTypesList_value = (ATM_SystemTypesList)(1 << i);
                usageArray[6, i] = (((_systemTypesList & tmp_systemTypesList_value) == tmp_systemTypesList_value) ? true : false);

                tmp_itemTypesList_value = (ATM_ItemTypesList)(1 << i);
                usageArray[7, i] = (((_itemTypesList & tmp_itemTypesList_value) == tmp_itemTypesList_value) ? true : false);
            }
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
        private int _versionData;
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
        /// <param name="bReader">Ссылка на открытый в двоичном режиме файловый поток</param>
        public void Load(BinaryReader bReader)
        {

            // 1) Читаем версию данных
            _versionData = bReader.ReadInt32();

            // 2) Проверяем, нужна ли конверсия
            if (_versionData == _versionCode)
            {
                // Нет, конверсия не нужна, просто загружаем данные

                // INFO Чтение данных для ТЕКУЩЕЙ версии кода WhereUsed._versionCode == 1

                // DEP countATM_Enums dependancy, edit all entries at once
                // ВСЕГДА ПОДДЕРЖИВАЕМ ОДНУ И ТУ ЖЕ ОЧЕРЕДНОСТЬ для избежания ошибок
                _familyList = (ATM_FamilyList)bReader.ReadInt64();
                _ancientModelsList = (ATM_Models_Ancient)bReader.ReadInt64();
                _oldModelsList = (ATM_Models_Old)bReader.ReadInt64();
                _currentModelsList = (ATM_Models_Current)bReader.ReadInt64();
                _futureModelsList = (ATM_Models_Future)bReader.ReadInt64();
                _systemsList = (ATM_SystemsList)bReader.ReadInt64();
                _subsystemsList = (ATM_SubystemsList)bReader.ReadInt64();
                _systemTypesList = (ATM_SystemTypesList)bReader.ReadInt64();
                _itemTypesList = (ATM_ItemTypesList)bReader.ReadInt64();
            }
            else if ((_versionData > _versionCode) || (_versionData < 1))
            {
                // Данные новее программы или версия данных = 0. Невозможно загрузить!
                throw new System.Exception("Data version is out of range. Current WhereUsed class version is " + _versionCode.ToString() + " and the data version is " + _versionData.ToString());
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

                // DEP countATM_Enums dependancy, edit all entries at once
                // ВСЕГДА ПОДДЕРЖИВАЕМ ОДНУ И ТУ ЖЕ ОЧЕРЕДНОСТЬ для избежания ошибок
                bWriter.Write((long)_familyList);
                bWriter.Write((long)_familyList);
                bWriter.Write((long)_ancientModelsList);
                bWriter.Write((long)_oldModelsList);
                bWriter.Write((long)_currentModelsList);
                bWriter.Write((long)_futureModelsList);
                bWriter.Write((long)_systemsList);
                bWriter.Write((long)_subsystemsList);
                bWriter.Write((long)_systemTypesList);
                bWriter.Write((long)_itemTypesList);
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
        WhereUsed()
        {
            // Присваиваем значения внутренним полям напрямую, т.к. 
            // нет необходимости пока отслеживать изменения

            // DEP countATM_Enums dependancy, edit all entries at once
            // ВСЕГДА ПОДДЕРЖИВАЕМ ОДНУ И ТУ ЖЕ ОЧЕРЕДНОСТЬ для избежания ошибок
            _familyList = ATM_FamilyList.Undefined;
            _ancientModelsList = ATM_Models_Ancient.Undefined;
            _oldModelsList = ATM_Models_Old.Undefined;
            _currentModelsList = ATM_Models_Current.Undefined;
            _futureModelsList = ATM_Models_Future.Undefined;
            _systemsList = ATM_SystemsList.Undefined;
            _subsystemsList = ATM_SubystemsList.Undefined;
            _systemTypesList = ATM_SystemTypesList.Undefined;
            _itemTypesList = ATM_ItemTypesList.Undefined;
        }
        #endregion

    }
}
