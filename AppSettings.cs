using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;   // For access to Debug and Trace classes
using Logs;

namespace Exeter7
{
    /// <summary>
    /// Статический класс, к которому обращается приложение за всеми настройками. Имеет собственные 
    /// значения по-умолчанию, поверх которых загружает либо пользовательские из user.config, либо 
    /// исходные параметры из app.config через Properties.Settings.Default. См. remarks для подробного описания.
    /// </summary>
    /// <remarks>Формально, этот класс - лишнее звено в цепочке Settins - AppSettings - Application. 
    /// Является своего рода страховкой от удаления конфигурационных файлов и именно к этому классу 
    /// обращается код приложения в поисках настроек. Но в реальности, настроек в AppSettings может быть больше 
    /// чем их хранится в Settings. В данном случае большинство настрооек AppSettings имеют свой 
    /// эквивалент в Settings, но есть некоторые значения, не предназначенные для изменения извне 
    /// приложения, например, maxPN (его изменение требует пересмотра логики программы и выносить это 
    /// значение в настройки бессмысленно). Плюс, AppSettings предоставляет как дополнительную информацию для 
    /// IntelliSense, так и, помимо собственно настроек, дополнительные методы для удобства работы, например, GetLogName().
    /// Схема работы следующая: 1) Класс Application еще до создания главной формы приложения обращается 
    /// к AppSettings, чтобы при этом первом обращении к классу сработал статический конструктор и 
    /// инициализировались все поля. 2) Там же вызывается метод AppSettings.LoadSavedSettings(), использующий 
    /// Settings.Default для считывания сохраненных настроек из файла и перезаписывающий одноименные настройки AppSettings. 
    /// 3) frmMain создается и стартует, имея в своем распоряжении загруженные настройки.</remarks>
    static class AppSettings
    {

        #region Статический конструктор
        /// <summary>
        /// Статический конструктор
        /// </summary>
        static AppSettings()
        {
            Debug.WriteLine("****************** AppSettings constructor");
            //DEBUG Debug.Assert(false, "****************** AppSettings constructor");
        }
        #endregion

        #region Загрузчик настроек из Settings.Default
        /// <summary>
        /// Метод использует класс Properties.Settings.Default для загрузки всех
        /// сохраненных на диске настроек, как глобальных из app.exe.config, так и 
        /// из user.config, если таковые имеются.
        /// </summary>
        public static void LoadSavedSettings()
        {
            // Пример загрузки настроек: AppSettings.maxPN = Properties.Settings.Default.MaxPN;
            Debug.WriteLine("****************** AppSettings loader");
            if (Program.Log != null)
            {
                // Приложение приходит сюда как минимум 1 раз еще до инициализации лог-файла, поэтому 
                // обязательно делаем проверку, иначе можно получить исключение System.NullReferenceException
                Program.Log.Write("Начинаем применение настроек из Settings.Default к настройкам по-умолчанию в AppSettings");
            }
            AppSettings.autoSaveDataChanges = Properties.Settings.Default.AutoSaveDataChanges;
            AppSettings.autoSaveSettings = Properties.Settings.Default.AutoSaveSettings;
            AppSettings.fileCatalog = Properties.Settings.Default.FileCatalog;
            AppSettings.fileLog = Properties.Settings.Default.FileLog;
            AppSettings.miscFolder = Properties.Settings.Default.MiscFolder;
            AppSettings.useLogging = Properties.Settings.Default.UseLogging;
            AppSettings.workFolder = Properties.Settings.Default.WorkFolder;
            if (Program.Log != null)
            {
                // Приложение приходит сюда как минимум 1 раз еще до инициализации лог-файла, поэтому 
                // обязательно делаем проверку, иначе можно получить исключение System.NullReferenceException
                Program.Log.Write("Применение настроек завершено");
            }
        }
        #endregion

        #region Методы для облегчения работы с настройками программы
        /// <summary>
        /// Метод возвращает полное имя файла каталога запчастей.
        /// </summary>
        /// <returns>Полное имя файла каталога, включая путь к нему</returns>
        public static string GetCatalogName()
        {
            return workFolder + fileCatalog;
        }

        /// <summary>
        /// Метод возвращает полное имя лог-файла
        /// </summary>
        /// <returns>Полное имя лог-файла, включая путь к нему</returns>
        public static string GetLogName()
        {
            return miscFolder + fileLog;
        }
        #endregion

        #region Поля, НЕ хранимые в файлах настроек, (т.е. жестко закодированные в приложении)
        /// <summary>
        /// Максимально возможная величина партномеров PN, AS, SC
        /// </summary>
        public static ulong maxPN = 9999999999;

        /// <summary>
        /// Количество перечислений (32-битовых полей) вида ATM_xxx, используемых программой
        /// </summary>
        /// <remarks>Характеристики запчастей описываются перечислениями с флагами [Flags]. Это 
        /// константы из Definitions.cs вида ATM_... При работе с интерфейсом пользователя битовые 
        /// поля перечислений преобразуются в цикле в массив bool. Используем константу, чтобы эта 
        /// величина редактировалась в одном месте. ВНИМАНИЕ! При изменении числа констант 
        /// необходимо найти и корректно отредактировать все участки кода с комментариями "DEP countATM_Enums dependancy"
        /// </remarks>
        // DEP countATM_Enums dependancy, edit all entries at once
        public static int countATM_Enums = 9;
        #endregion

        #region Поля, имеющие свои эквиваленты в файле настроек (т.е. хранятся на диске)
       
        #region Папки и файлы
        /// <summary>
        /// Путь к папке с обрабатываемыми данными. 
        /// Должен заканчиваться слэшем "\", если задан.
        /// </summary>
        public static string workFolder = @"";

        /// <summary>
        /// Путь к папке с вспомогательными файлами (логи и т.п.). 
        /// Должен заканчиваться слэшем "\", если задан.
        /// </summary>
        public static string miscFolder = @"";

        /// <summary>
        /// Имя файла каталога запчастей
        /// </summary>
        public static string fileCatalog = @"Cat.dat";

        /// <summary>
        /// Имя лог-файла
        /// </summary>
        public static string fileLog = @"Exeter7log.txt";

        /// <summary>
        /// Вести ли файл журнала работы приложения
        /// </summary>
        public static bool useLogging = false;
        #endregion

        #region Настройки приложения
        /// <summary>
        /// Сохранять ли настройки приложения автоматически (при выходе 
        /// или при закрытии окна настроек)
        /// </summary>
        public static bool autoSaveSettings = true;

        /// <summary>
        /// Сохранять ли измененные данные автоматически при выходе
        /// </summary>
        public static bool autoSaveDataChanges = false;
        #endregion

        #endregion
    }
}
