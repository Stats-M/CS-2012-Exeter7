using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Exeter7
{

    /// <summary>
    /// Делегат (класс указателя на функцию) для индикации прогресса 
    /// выполнения какой-либо задачи. Вычислением процентов выполнения занимается 
    /// функция, на которую указывает делегат.
    /// </summary>
    /// <param name="CurrentValue">Текущее значение параметра</param>
    /// <param name="MaxValue">Максимальное значение параметра (прогресс = 100%)</param>
    public delegate void ProgressCallback(int CurrentValue, int MaxValue);

    /// <summary>
    /// Делегат (класс указателя на функцию) для индикации прогресса (от 0 до 100%)
    /// выполнения какой-либо задачи. Вычислением процентов выполнения занимается класс,
    /// который создает экземпляр делегата.
    /// </summary>
    /// <param name="Progress">Текущее значение хода выполнения работы от 0 до 100</param>
    public delegate void ProgressCallback100(int Progress);

    /// <summary>
    /// Перечисление поколений моделей банкоматов
    /// </summary>
    [Flags]
    public enum ATM_FamilyList : long
    {
        Undefined = 0,
        NCR_50xx = 1 << 0,
        NCR_56xx = 1 << 1,
        NCR_Personas = 1 << 2,
        NCR_M = 1 << 3,
        NCR_SelfServ = 1 << 4,
        Reserved01 = 1 << 5,
        Reserved02 = 1 << 6,
        Reserved03 = 1 << 7,
        Reserved04 = 1 << 8,
        Reserved05 = 1 << 9,
        Reserved06 = 1 << 10,
        Reserved07 = 1 << 11,
        Reserved08 = 1 << 12,
        Reserved09 = 1 << 13,
        Reserved10 = 1 << 14,
        Reserved11 = 1 << 15,
        Reserved12 = 1 << 16,
        Reserved13 = 1 << 17,
        Reserved14 = 1 << 18,
        Reserved15 = 1 << 19,
        Reserved16 = 1 << 20,
        Reserved17 = 1 << 21,
        Reserved18 = 1 << 22,
        Reserved19 = 1 << 23,
        Reserved20 = 1 << 24,
        Reserved21 = 1 << 25,
        Reserved22 = 1 << 26,
        Reserved23 = 1 << 27,
        Reserved24 = 1 << 28,
        Reserved25 = 1 << 29,
        Reserved26 = 1 << 30,
        Reserved27 = 1 << 31
    }

    /// <summary>
    /// Перечисление сильно устаревших и неиспользуемых моделей банкоматов
    /// </summary>
    [Flags]
    public enum ATM_Models_Ancient : long
    {
        Undefined = 0,
        NCR5070 = 1 << 0,
        NCR5080 = 1 << 1,
        NCR5084 = 1 << 2,
        NCR5085 = 1 << 3,
        NCR5670 = 1 << 4,
        NCR5674 = 1 << 5,
        NCR5675 = 1 << 6,
        NCR5684 = 1 << 7,
        NCR5685 = 1 << 8,
        NCR5688 = 1 << 9,
        Reserved01 = 1 << 10,
        Reserved02 = 1 << 11,
        Reserved03 = 1 << 12,
        Reserved04 = 1 << 13,
        Reserved05 = 1 << 14,
        Reserved06 = 1 << 15,
        Reserved07 = 1 << 16,
        Reserved08 = 1 << 17,
        Reserved09 = 1 << 18,
        Reserved10 = 1 << 19,
        Reserved11 = 1 << 20,
        Reserved12 = 1 << 21,
        Reserved13 = 1 << 22,
        Reserved14 = 1 << 23,
        Reserved15 = 1 << 24,
        Reserved16 = 1 << 25,
        Reserved17 = 1 << 26,
        Reserved18 = 1 << 27,
        Reserved19 = 1 << 28,
        Reserved20 = 1 << 29,
        Reserved21 = 1 << 30,
        Reserved22 = 1 << 31
    };

        /// <summary>
    /// Перечисление устаревших, но иногда встречающихся еще моделей банкоматов
    /// </summary>
    [Flags]
    public enum ATM_Models_Old : long
    {
        Undefined = 0,
        NCR5870 = 1 << 0,
        NCR5872 = 1 << 1,
        NCR5873 = 1 << 2,
        NCR5874 = 1 << 3,
        NCR5875 = 1 << 4,
        NCR5877 = 1 << 5,
        NCR5884 = 1 << 6,
        NCR5885 = 1 << 7,
        NCR5886 = 1 << 8,
        NCR5887 = 1 << 9,
        Reserved01 = 1 << 10,
        Reserved02 = 1 << 11,
        Reserved03 = 1 << 12,
        Reserved04 = 1 << 13,
        Reserved05 = 1 << 14,
        Reserved06 = 1 << 15,
        Reserved07 = 1 << 16,
        Reserved08 = 1 << 17,
        Reserved09 = 1 << 18,
        Reserved10 = 1 << 19,
        Reserved11 = 1 << 20,
        Reserved12 = 1 << 21,
        Reserved13 = 1 << 22,
        Reserved14 = 1 << 23,
        Reserved15 = 1 << 24,
        Reserved16 = 1 << 25,
        Reserved17 = 1 << 26,
        Reserved18 = 1 << 27,
        Reserved19 = 1 << 28,
        Reserved20 = 1 << 29,
        Reserved21 = 1 << 30,
        Reserved22 = 1 << 31
    };

    /// <summary>
    /// Перечисление текущих (актуальных) моделей банкоматов
    /// </summary>
    [Flags]
    public enum ATM_Models_Current : long
    {
        Undefined = 0,
        NCR6674 = 1 << 0,
        NCR6676 = 1 << 1,
        NCR6622 = 1 << 2,
        NCR6625 = 1 << 3,
        NCR6626 = 1 << 4,
        NCR6631 = 1 << 5,
        NCR6632 = 1 << 6,
        NCR6634 = 1 << 7,
        NCR6636 = 1 << 8,
        NCR6638 = 1 << 9,
        Reserved01 = 1 << 10,
        Reserved02 = 1 << 11,
        Reserved03 = 1 << 12,
        Reserved04 = 1 << 13,
        Reserved05 = 1 << 14,
        Reserved06 = 1 << 15,
        Reserved07 = 1 << 16,
        Reserved08 = 1 << 17,
        Reserved09 = 1 << 18,
        Reserved10 = 1 << 19,
        Reserved11 = 1 << 20,
        Reserved12 = 1 << 21,
        Reserved13 = 1 << 22,
        Reserved14 = 1 << 23,
        Reserved15 = 1 << 24,
        Reserved16 = 1 << 25,
        Reserved17 = 1 << 26,
        Reserved18 = 1 << 27,
        Reserved19 = 1 << 28,
        Reserved20 = 1 << 29,
        Reserved21 = 1 << 30,
        Reserved22 = 1 << 31
    };

    /// <summary>
    /// Перечисление будущих (после Self Serv) моделей банкоматов
    /// </summary>
    [Flags]
    public enum ATM_Models_Future : long
    {
        Undefined = 0,
        Reserved01 = 1 << 0,
        Reserved02 = 1 << 1,
        Reserved03 = 1 << 2,
        Reserved04 = 1 << 3,
        Reserved05 = 1 << 4,
        Reserved06 = 1 << 5,
        Reserved07 = 1 << 6,
        Reserved08 = 1 << 7,
        Reserved09 = 1 << 8,
        Reserved10 = 1 << 9,
        Reserved11 = 1 << 10,
        Reserved12 = 1 << 11,
        Reserved13 = 1 << 12,
        Reserved14 = 1 << 13,
        Reserved15 = 1 << 14,
        Reserved16 = 1 << 15,
        Reserved17 = 1 << 16,
        Reserved18 = 1 << 17,
        Reserved19 = 1 << 18,
        Reserved20 = 1 << 19,
        Reserved21 = 1 << 20,
        Reserved22 = 1 << 21,
        Reserved23 = 1 << 22,
        Reserved24 = 1 << 23,
        Reserved25 = 1 << 24,
        Reserved26 = 1 << 25,
        Reserved27 = 1 << 26,
        Reserved28 = 1 << 27,
        Reserved29 = 1 << 28,
        Reserved30 = 1 << 29,
        Reserved31 = 1 << 30,
        Reserved32 = 1 << 31
    };

    /// <summary>
    /// Перечисление систем банкоматов (депозит, монитор, презентер...)
    /// </summary>
    [Flags]
    public enum ATM_SystemsList : long
    {
        Undefined = 0,
        Alarm = 1 << 0,
        Audio = 1 << 1,
        Cabinetry = 1 << 2,
        CardReader = 1 << 3,
        Computer = 1 << 4,
        Container = 1 << 5,
        Depository = 1 << 6,
        Dispenser = 1 << 7,
        Display = 1 << 8,
        EPP = 1 << 9,
        Fascia = 1 << 10,
        Harness = 1 << 11,
        Misc = 1 << 12,
        Power = 1 << 13,
        Printer = 1 << 14,
        Security = 1 << 15,
        Reserved17 = 1 << 16,
        Reserved18 = 1 << 17,
        Reserved19 = 1 << 18,
        Reserved20 = 1 << 19,
        Reserved21 = 1 << 20,
        Reserved22 = 1 << 21,
        Reserved23 = 1 << 22,
        Reserved24 = 1 << 23,
        Reserved25 = 1 << 24,
        Reserved26 = 1 << 25,
        Reserved27 = 1 << 26,
        Reserved28 = 1 << 27,
        Reserved29 = 1 << 28,
        Reserved30 = 1 << 29,
        Reserved31 = 1 << 30,
        Reserved32 = 1 << 31
    }

    /// <summary>
    /// Перечисление подсистем; используется совместно с ATM_SystemsList 
    /// для расширения возможностей группировки и поиска запчастей
    /// </summary>
    [Flags]
    public enum ATM_SubystemsList : long
    {
        Undefined = 0,
        DepositCoin,
        DepositNotes,
        DispenserCoin,
        DispenserNotes
        // TODO Завершить список-4!
    }

    /// <summary>
    /// Перечисление типов запчастей (ремень, шестерня, плата, узел (сложный элемент), стекло......)
    /// </summary>
    [Flags]
    public enum ATM_ItemTypesList : long
    {
        Undefined = 0,
        Belt = 1 << 0,
        Gear = 1 << 1,
        PCB = 1 << 2,
        Electric = 1 << 3,
        Nuts = 1 << 4,
        ConstrMetal = 1 << 5,
        ConstrPlastic = 1 << 6,
        ConstrRubber = 1 << 7,
        ConstrGlass = 1 << 8,
        ComplexUnit = 1 << 9
        // TODO Завершить список-3!
    }

    /// <summary>
    /// Перечисление разновидностей систем банкоматов (NID разновидность презентера, ЭЛТ разновидность монитора...)
    /// </summary>
    [Flags]
    public enum ATM_SystemTypesList : long
    {
        Undefined = 0,
        NID = 1 << 0,
        Enhanced = 1 << 1,
        ARIA3 = 1 << 2,
        LCD = 1 << 3,
        CRT = 1 << 4
        // TODO Завершить список-2!
    }

    /// <summary>
    /// Перечисление, показывающее статус файла данных. Помогает отследить ситуацию и принять решение либо 
    /// о дополнительных действиях, либо о генерации исключения. Используется в классах, реализующих 
    /// интерфес IBinaryStreamMaster, а также при их коммуникации с родительскими классами.
    /// </summary>
    public enum DataFileStatus : int
    {
        /// <summary>
        /// Иная ошибка, не предусмотренная этим перечислением
        /// </summary>
        UndefinedError = -1000,
        /// <summary>
        /// Дисковод не найден или не готов
        /// </summary>
        DriveError = -100,
        /// <summary>
        /// Имя папки слишком длинное
        /// </summary>
        FolderNameTooLong = -75,
        /// <summary>
        /// Папка не существует
        /// </summary>
        FolderNotFound = -50,
        /// <summary>
        /// Файл с заданным именем не найден
        /// </summary>
        FileNotFound = -40,
        /// <summary>
        /// Используется при загрузке класса, когда файл еще не открывался
        /// </summary>
        NotYetSelected = -30,
        /// <summary>
        /// Файл открыт, но он пуст (нет даже мастер-записи)
        /// </summary>
        FileIsEmpty = 0,
        /// <summary>
        /// Файл открыт, но в нем нет ни одной записи данных (кроме мастер-записи)
        /// </summary>
        NoRecords = 10,
        /// <summary>
        /// Нет ошибок
        /// </summary>
        OK = 20
    }
}