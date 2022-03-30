using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Logs;     // В этом классе будем объявлять статическое поле - ссылку на объект класса лог-файла


namespace Exeter7
{
    static class Program
    {
        /// <summary>
        /// Статический экземпляр класса для работы с лог-файлом
        /// </summary>
        public static SimpleTextLog Log = null;

        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // Application - статический класс .NET, запускающий главную форму приложения
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Единственная задача следующей строки - вызвать статический конструктор AppSettings
            // и проинициализировать статические поля класса значениями по-умолчанию
            ulong InitStaticClass = AppSettings.maxPN;
            // Загружаем настройки в AppSettings
            AppSettings.LoadSavedSettings();

            // Теперь создаем лог-файл
            Log = new SimpleTextLog(AppSettings.GetLogName());
            // Если в настройках установлено не создавать лог-файла...
            if (!AppSettings.useLogging)
            {
                // Закрываем лог-файл. Прием сообщений класс будет по-прежнему производить, но будет 
                // их игнорировать. Таким образом мы избавимся от необходимости проверять 
                // эту настройку каждый раз при выводе очередного сообщения
                Log.Close();
            }
            else
            {
                // ...иначе запишем заголовок в журнал
                Log.ShowMsgTypes = false;
                Log.ShowTime = false;
                Log.Write(" ***** Application: " + Application.ProductName +
                    " v." + Application.ProductVersion);
                Log.Write(" ***** ***** ***** ***** ***** ***** ***** *****");
                Log.ShowMsgTypes = true;
                Log.ShowTime = true;
            }

            Log.Write("Создаем и запускаем цикл обработки сообщений главной формы приложения.");

            // Создаем, загружаем (вызывается frmMain_Load()) и передаем управление главной форме
            Application.Run(new frmMain());

            Log.Write("Главная форма закрыта. Выходим из приложения.");
            Log.Close();
        }
    }
}
