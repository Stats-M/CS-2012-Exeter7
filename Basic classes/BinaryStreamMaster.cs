using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Exeter7
{

    /// <summary>
    /// Класс, реализующий единый для всех классов файлов данных механизм работы с файлом и мастер-записью.</summary>
    /// 
    /// <remarks>Базовый класс BinaryStreamMaster предоставляет автономные законченные методы, каждый из которых 
    /// решает одну четко очерченную задачу (например, Открыть файл, или Прочитать мастер-запись). Если 
    /// методы не могут выполнить задачу, они либо возвращают признак ошибки для ее анализа классом-наследником, 
    /// либо генерируют исключение. Такая реализация сделана потому что класс-наследник не имеет 
    /// возможности быть оповещенным о самостоятельных дополнительных действиях методов, унаследованных 
    /// от базового класса, что может приводить к ошибкам (например, повторный вызов метода открытия 
    /// файла, если при ошибке доступа к файлу метод базового класса решил самостоятельно открыть файл данных).</remarks>
    public class BinaryStreamMaster
    {

        #region Поля класса - работа с файлом каталога
        /// <summary>
        /// Статус файла данных. Учитывается при выборе варианта действий.
        /// </summary>
        protected DataFileStatus _fileStatus = DataFileStatus.NotYetSelected;

        /// <summary>
        /// Класс, производящий чтение из файлового потока
        /// </summary>
        protected BinaryReader _bReader = null;

        /// <summary>
        /// Класс, производящий запись в файловый поток
        /// </summary>
        protected BinaryWriter _bWriter = null;

        /// <summary>
        /// Класс файлового потока
        /// </summary>
        protected FileStream _fs = null;

        /// <summary>
        /// Мастер-запись файла данных
        /// </summary>
        protected byte[] _masterFrame = new byte[256];

        /// <summary>
        /// Показывает можно ли записывать мастер-запись в файл. 
        /// Приобретает значение true после чтения мастер-записи или при формировании новой.
        /// </summary>
        private bool _isMasterFrameEncoded = false;

        /// <summary>
        /// Файл данных. После открытия имя файла фиксируется на время жизненного цикла 
        /// класса для изоляции класса от возможных изменений в настройках приложения.
        /// </summary>
        protected string _dataFile = "";

        /// <summary>
        /// Версия кода класса. Внутреннее поле - константа, изменяется только вручную 
        /// при компилировании изменений. Имеет для доступа соответствующее свойство (read-only).
        /// </summary>
        protected const int _masterVersionCode = 1;

        /// <summary>
        /// Версия схемы кодирования мастер-записи. Это значение определяет, какие параметры и 
        /// в какой последовательности записываются в заголовок файла данных. Имеет для доступа 
        /// соответствующее свойство (read-only). Изменяется только методами класса в момент 
        /// дешифровки прочитанной мастер-записи.
        /// </summary>
        /// <remarks>Если класс (класс-наследник) произодит обновление версии данных мастер записи, то 
        /// сперва кодируется мастер-запись в новой версии, затем она записывается в файл и тут же 
        /// считывается обратно и раскодируется. При раскодировании среди прочих устанавливается и значение 
        /// версии данных мастер-записи.</remarks>
        protected int _masterVersionData = 0;
        #endregion

        #region Свойства класса

        /// <summary>
        /// Свойство (только для чтения), позволяющее определить состояние байтового массива, 
        /// хранящего данные мастер-записи. Если поле имеет значение true, то массив готов к записи 
        /// в файл или к дешифровке. Необходимость такого отслеживания состоит в том, что при 
        /// файловых операциях происходит перестановка старших и младших разрядов.
        /// Позволяет автоматически производить обнуление массива при установке свойства в false. 
        /// После дешифровки массив обнуляется, т.к. в процессе происходит перестановка байтов местами 
        /// и писать их обратно нельзя, нужно заново формировать запись.</summary>
        protected bool IsMasterFrameEncoded
        {
            get
            {
                return _isMasterFrameEncoded;
            }
            set
            {
                if (value == false)
                {
                    // Устанавливается флаг false - нужно очистить массив мастер-записи
                    System.Array.Clear(_masterFrame, 0, 256);
                    Program.Log.Write(String.Format("{0}::IsMasterFrameValid -> Очищаем содержимое мастер-записи в памяти", this.GetType().Name));
                }
                _isMasterFrameEncoded = value;
            }
        }


        /// <summary>
        /// Свойство (read-only), возвращающее ссылку на объект типа BinaryReader, производящий
        /// операции чтения с файловым потоком данных, обрабатывемых текущим классом.
        /// </summary>
        public BinaryReader BinReader
        {
            get
            {
                return _bReader;
            }
        }

        /// <summary>
        /// Свойство (read-only), возвращающее ссылку на объект типа BinaryWriter, производящий
        /// операции записи в файловый поток данных, обрабатывемых текущим классом.
        /// </summary>
        public BinaryWriter BinWriter
        {
            get
            {
                return _bWriter;
            }
        }

        /// <summary>
        ///  Свойство (read-only), возвращающее состояние файла данных.
        /// </summary>
        public DataFileStatus FileStatus
        {
            get
            {
                return _fileStatus;
            }
        }

        /// <summary>
        /// (read-only) Версия кода класса, который создал мастер-запись
        /// </summary>
        public int MasterVersionCode
        {
            get
            {
                return _masterVersionCode;
            }
        }

        /// <summary>
        /// (read-only) Версия схемы кодирования (версия данных) мастер-записи. Это значение определяет, какие параметры и 
        /// в какой последовательности записываются в заголовок файла данных.  Изменяется только методами класса в момент 
        /// дешифровки прочитанной мастер-записи (см. описание _masterVersionData)
        /// /// </summary>
        public int MasterVersionData
        {
            get
            {
                return _masterVersionData;
            }
        }
        #endregion

        #region Методы класса, выполняющие конкретные операции независимо от класса-наследника
        /// <summary>
        /// Публичный метод, выполняющий общую проверку открыт ли уже файл данных. Если еще не открыт, 
        /// вызывает protected метод OpenDataFile(), который и реализует всю работу по открытию файла.
        /// Если уже открыт, то просто возвращает true. 
        /// Метод OpenFile() предназначен для его вызова конечными пользователями объектов, "извне".
        /// </summary>
        /// <returns>true, если файл успешно открыт (или уже был открыт ранее) и можно переходить к работе с данными.</returns>
        public bool OpenFile()
        {

            Program.Log.Write(String.Format("{0}::OpenFile() -> Попытка открыть файл данных", this.GetType().Name));

            // Проверяем статус файла каталога
            if (_fileStatus <= DataFileStatus.NotYetSelected)
            {
                // Файл еще не выбран или не найден. Нужно попытаться открыть его.
                return OpenDataFile();
            }
            else
            {
                // Файл открыт. Он может быть вообще пуст, может содержать только мастер-запись и 
                // ни одной записи данных, но это ведь не ошибка, файл открыт успешно, поэтому 
                // выходим и возвращаем true. Вызывающий класс может получить точный статус файла, 
                // проанализировав значение свойства FileStatus и сообщить об этом пользователю.
                Program.Log.Write(String.Format("{0}::OpenFile() -> Файл уже открыт, статус файла - {1}", this.GetType().Name, _fileStatus.ToString()), Logs.MsgType.Warning);
                return true;
            }
        }

        /// <summary>
        /// Метод занимается поиском, ассоциацией с потоками и открытием файла данных. Также осуществляет 
        /// перехват исключений, связанных с открытием файла. По итогам работы устанавливает флаг состояния 
        /// файла данных _fileStatus для его анализа вызывающими методами.
        /// </summary>
        /// <remarks>ОБЯЗАТЕЛЬНО в наследуемых классах при получении DataFileStatus.NoRecords произвести 
        /// принудительную запись мастер-записи!!! Пусть данных в файле нет, но мастер-запись при открытии 
        /// пустого файла должна быть записана автоматически! Что именно писать знает только класс-наследник, 
        /// поэтому после вызова метода OpenDataFile() базового класса нужно сделать проверку DataFileStatus. 
        /// Данный метод является изолированным (не вызывает другие методы класса). Это связано с тем, что управлять 
        /// работой с мастер-записью будет класс-наследник. Поэтому важно, чтобы для него все методы были прозрачны и 
        /// не вызывали скрытно другие методы базового класса.</remarks>
        /// <returns>true, если файл был успешно открыт</returns>
        protected bool OpenDataFile()
        {

            // Проверим на всякий случай нужно ли нам вообще открывать файл
            if (_fileStatus >= DataFileStatus.FileIsEmpty)
            {
                // Файл уже открыт. Не нужно открывать дважды, выходим.
                Program.Log.Write(String.Format("{0}::OpenDataFile() -> Файл уже открыт, статус файла - {1}", this.GetType().Name, _fileStatus.ToString()), Logs.MsgType.Warning);
                return true;
            }
            else
            {
                // Диагностическое сообщение
                Program.Log.Write(String.Format("{0}::OpenDataFile() -> Файл еще не открыт, статус файла - {1}", this.GetType().Name, _fileStatus.ToString()));
            }

            // Фиксируем имя и расположение файла каталога, если еще не зафиксировано
            if (_dataFile.Length == 0)
            {
                // Нулевая длина этого свойства может быть только при первом вызове метода, нужно 
                // определить имя файла каталога. НЕ ИЗМЕНЯЕМ единожды заданное имя!!! 
                // Чтобы изменения вступили в силу, нужно перезапустить приложение.
                _dataFile = AppSettings.GetCatalogName();
                Program.Log.Write(String.Format("{0}::OpenDataFile() -> Путь к файлу выбран - {1}", this.GetType().Name, _dataFile.ToString()));
            }

            // Проверим для диагностических целей, создавали ли мы уже файловый поток ранее
            if (_fs != null)
            {
                // Да, уже создавали... Была ошибка при предыдущем вызове (раз _fs!=null и статус<DataFileStatus.FileIsEmpty)?
                Program.Log.Write(String.Format("{0}::OpenDataFile() -> Объект файлового потока уже существует (?)", this.GetType().Name), Logs.MsgType.Warning);
                // НЕ выходим из метода, т.к. статус файла < DataFileStatus.FileIsEmpty и, следовательно,
                // нужно попытаться открыть его еще раз.
            }

            // Локальная переменная для хранения возвращаемого результата (должна быть доступна извне блока try)
            bool bResult = true;

            try
            {
                Program.Log.Write(String.Format("{0}::OpenDataFile() -> Создаем новый файловый поток", this.GetType().Name));
                _fs = new FileStream(_dataFile,
                    FileMode.OpenOrCreate,
                    FileAccess.ReadWrite,
                    FileShare.None);
            }
            catch (System.IO.DriveNotFoundException e)
            {
                // Дисковод не обнаружен - файл не может быть открыт
                _fileStatus = DataFileStatus.DriveError;
                Program.Log.Write(String.Format("{0}::OpenDataFile() -> Исключение. Дисковод не обнаружен. Невозможно открыть файл", this.GetType().Name), Logs.MsgType.Error);
                Program.Log.Write(String.Format("{0}::OpenDataFile() -> Описание: {1}", this.GetType().Name, e.Message), Logs.MsgType.Error);
                Console.WriteLine("{0} Exception caught in OpenDataFile()", e);
                //bResult = false;
            }
            catch (System.IO.PathTooLongException e)
            {
                // Слишком длинный путь к файлу - файл не может быть открыт
                _fileStatus = DataFileStatus.FolderNameTooLong;
                Program.Log.Write(String.Format("{0}::OpenDataFile() -> Исключение. Слишком длинный путь. Невозможно открыть файл", this.GetType().Name), Logs.MsgType.Error);
                Program.Log.Write(String.Format("{0}::OpenDataFile() -> Описание: {1}", this.GetType().Name, e.Message), Logs.MsgType.Error);
                Console.WriteLine("{0} Exception caught in OpenDataFile()", e);
                //bResult = false;
            }
            catch (System.IO.DirectoryNotFoundException e)
            {
                _fileStatus = DataFileStatus.FolderNotFound;
                // Папка не найдена - файл не может быть открыт
                Program.Log.Write(String.Format("{0}::OpenDataFile() -> Исключение. Папка не найдена. Невозможно открыть файл.", this.GetType().Name), Logs.MsgType.Error);
                Program.Log.Write(String.Format("{0}::OpenDataFile() -> Описание: {1}", this.GetType().Name, e.Message), Logs.MsgType.Error);
                Console.WriteLine("{0} Exception caught in OpenDataFile()", e);
                //bResult = false;
            }
            catch (System.IO.IOException e)
            {
                // Ловим все остальные исключения ввода-вывода в одном метсе
                _fileStatus = DataFileStatus.UndefinedError;
                Program.Log.Write(String.Format("{0}::OpenDataFile() -> Исключение ввода-вывода. См. отладочную информацию для более подробных сведений.", this.GetType().Name), Logs.MsgType.Error);
                Program.Log.Write(String.Format("{0}::OpenDataFile() -> Описание: {1}", this.GetType().Name, e.Message), Logs.MsgType.Error);
                Console.WriteLine("{0} Exception caught in OpenDataFile() and rethrown.", e);
                // Не обрабатываем никаких исключений здесь, перекидываем их выше по стеку вызовов.
                throw;
            }
            finally
            {
                // Раз мы здесь, произошло исключение. Необходимо подчистить переменные после неудачной попытки.
                _fs = null;
                _bWriter = null;
                _bReader = null;
                Program.Log.Write(String.Format("{0}::OpenDataFile() -> Завершаем обработку исключения. Обнуляем ссылки на объекты", this.GetType().Name), Logs.MsgType.Error);
                bResult = false;
            }


            // Следующий блок выполняется только если не возникло ранее исключений
            // (обработанных исключений, т.к. необработанные приведут к остановке приложения)
            if (bResult)
            {
                Program.Log.Write(String.Format("{0}::OpenDataFile() -> Создаем объект, производящий запись в поток", this.GetType().Name));
                _bWriter = new BinaryWriter(_fs);
                // Перемещаем указатель в начало файла
                _bWriter.BaseStream.Seek(0, SeekOrigin.Begin);

                Program.Log.Write(String.Format("{0}::OpenDataFile() -> Создаем объект, производящий чтение из потока", this.GetType().Name));
                _bReader = new BinaryReader(_fs);
                // Перемещаем указатель в начало файла
                _bReader.BaseStream.Seek(0, SeekOrigin.Begin);

                // Если мы здесь, то все операции выполнены успешно.
                
                // Обнуляем мастер-запись в памяти
                IsMasterFrameEncoded = false;
                // Очистка произведется автоматически внутри set свойства
                // System.Array.Clear(_masterFrame, 0, 256);

                // Устанавливаем флаги.
                if (_fs.Length == 0)
                {
                    _fileStatus = DataFileStatus.FileIsEmpty;
                    Program.Log.Write(String.Format("{0}::OpenDataFile() -> Файл пуст", this.GetType().Name));
                    // TODO _ОБЯЗАТЕЛЬНО_ !!! в наследуемых классах в OpenDataFile() при получении DataFileStatus.NoRecords произвести принудительную
                    // запись мастер-записи!!! Пусть данных в файле нет, но мастер-запись при открытии пустого файла должна быть записана автоматически!
                    // Что именно писать знает только класс-наследник, поэтому после вызова метода базового класса нужно сделать проверку этого случая.
                }
                else if (_fs.Length == 256)
                {
                    // TODO Проверить, выполняется ли этот вариант когда в файле есть только мастер-запись
                    _fileStatus = DataFileStatus.NoRecords;
                    Program.Log.Write(String.Format("{0}::OpenDataFile() -> Файл содержит только мастер-запись", this.GetType().Name));
                }
                else
                {
                    _fileStatus = DataFileStatus.OK;
                    Program.Log.Write(String.Format("{0}::OpenDataFile() -> Файл успешно открыт", this.GetType().Name));
                }
            }

            return bResult;
        }

        /// <summary>
        /// Метод, производящий проверку на возможность и считывание мастер-записи файла в память. Расшифровкой занимается 
        /// класс-наследник.
        /// </summary>
        /// <remarks>Данный метод является изолированным (не вызывает другие методы класса). Это связано с тем, что управлять 
        /// работой с мастер-записью будет класс-наследник. Поэтому важно, чтобы для него все методы были прозрачны и 
        /// не вызывали скрытно другие методы базового класса.</remarks>
        protected bool ReadMasterFrame()
        {
            // Проверим, есть ли в файле мастер-запись (это состояния DataFileStatus.NoRecords и .OK)
            if (_fileStatus < DataFileStatus.NoRecords)
            {
                // Файл пуст совсем или даже еще не открыт. Проверяем дополнительно.
                if (_fileStatus >= DataFileStatus.NotYetSelected)
                {
                    // Файл еще не открыт или пуст.
                    Program.Log.Write(String.Format("{0}::ReadMasterFrame() -> Попытка чтения мастер-записи из не открытого или пустого файла, статус файла - {1}",
                        this.GetType().Name, _fileStatus.ToString()), Logs.MsgType.Error);
                }
                else
                {
                    // Файл уже пытались открыть, но произошла какая-то ошибка
                    Program.Log.Write(String.Format("{0}::ReadMasterFrame() -> Попытка чтения мастер-записи из файла со статусом \"Ошибка\" - {1}", 
                        this.GetType().Name, _fileStatus.ToString()), Logs.MsgType.Error);
                }
                
                // Ошибка, нечего читать
                return false;
            }

            // Мастер-запись присутствует. Подготовим массив для считывания
            IsMasterFrameEncoded = false;
            // Очистка произведется автоматически внутри set свойства
            // System.Array.Clear(_masterFrame, 0, 256);

            Program.Log.Write(String.Format("{0}::ReadMasterFrame() -> Попытка чтения мастер-записи", this.GetType().Name));
            // Локальная переменная для хранения возвращаемого результата (должна быть доступна извне блока try)
            bool bResult = true;
            // Локальная переменная для хранения числа прочитанных байт
            int count = 0;
            // Перемещаем указатель в начало файла
            _bReader.BaseStream.Seek(0, SeekOrigin.Begin);

            try
            {
                count = _bReader.Read(_masterFrame, 0, 256);
            }
            catch (System.IO.EndOfStreamException e)
            {
                // Достигнут конец потока - статус файла был ошибочен.
                Program.Log.Write(String.Format("{0}::ReadMasterFrame() -> Исключение. Достигнут конец потока. Ошибочный статус файла - {1}. Невозможно прочесть мастер-запись", 
                    this.GetType().Name, _fileStatus.ToString()), Logs.MsgType.Error);
                Program.Log.Write(String.Format("{0}::ReadMasterFrame() -> Описание: {1}", this.GetType().Name, e.Message), Logs.MsgType.Error);
                // Устанавливаем статус файла в "файл пуст", т.к. данных не хватило даже на мастер-запись, т.е. в файле нет ничего ценного
                _fileStatus = DataFileStatus.FileIsEmpty;
                Program.Log.Write(String.Format("{0}::ReadMasterFrame() -> Установлен новый статус файла - {1}. ",
                    this.GetType().Name, _fileStatus.ToString()), Logs.MsgType.Error);
                Console.WriteLine("{0} Exception caught in ReadMasterFrame()", e);
            }
            catch (System.IO.IOException e)
            {
                // Ловим все остальные исключения ввода-вывода в одном месте
                Program.Log.Write(String.Format("{0}::ReadMasterFrame() -> Исключение ввода-вывода, переброс выше по стеку вызовов. См. отладочную информацию для более подробных сведений", this.GetType().Name), Logs.MsgType.Error);
                Program.Log.Write(String.Format("{0}::ReadMasterFrame() -> Описание: {1}", this.GetType().Name, e.Message), Logs.MsgType.Error);
                Console.WriteLine("{0} Exception caught in ReadMasterFrame() and rethrown.", e);
                // Не обрабатываем никаких исключений здесь, перекидываем их выше по стеку вызовов.
                throw;
            }
            finally
            {
                // Если мы здесь, то произошло исключение. Запоминаем ошибку.
                bResult = false;
            }

            // Следующий блок выполняется только если не было исключений
            if (bResult)
            {

                // Проверим все ли 256 байт прочитаны
                if (count != 256)
                {
                    // Ошибка! Прочитано менее 256 байт, хотя исключение "конец потока" не было.
                    bResult = false;
                    Program.Log.Write(String.Format("{0}::ReadMasterFrame() -> Прочитано менее 256 байт ({1} байт прочитано)", this.GetType().Name, count.ToString()), Logs.MsgType.Error);
                }
                else
                {
                    // Если мы здесь, чтение мастер-записи произошло успешно, массив готов к дешифровке. Устанавливаем флаг
                    IsMasterFrameEncoded = true;
                    Program.Log.Write(String.Format("{0}::ReadMasterFrame() -> Мастер-запись успешно прочитана", this.GetType().Name));
                }
            }

            return bResult;
        }


        /// <summary>
        /// Метод, производящий проверку на возможность записи и саму запись заголовка в файл. Предшествующим этой операции 
        /// кодированием мастер-записи занимается класс-наследник.
        /// </summary>
        /// <remarks>Данный метод является изолированным (не вызывает другие методы класса). Это связано с тем, что управлять 
        /// работой с мастер-записью будет класс-наследник. Поэтому важно, чтобы для него все методы были прозрачны и 
        /// не вызывали скрытно другие методы базового класса.</remarks>
        protected bool WriteMasterFrame()
        {

            // Есть что записывать? Подготовлен ли массив к записи?
            if (!IsMasterFrameEncoded)
            {
                Program.Log.Write(String.Format("{0}::WriteMasterFrame() -> Массив с данными мастер-записи не подготовлен к записи", this.GetType().Name), Logs.MsgType.Error);
                return false;
            }

            // Проверим , есть ли у нас открытый файл, куда мы собираемся писать
            if (_fileStatus < DataFileStatus.FileIsEmpty)
            {
                Program.Log.Write(String.Format("{0}::WriteMasterFrame() -> Попытка записи заголовка в неоткрытый файл, статус файла - {1}",
                        this.GetType().Name, _fileStatus.ToString()), Logs.MsgType.Error);
                return false;
            }

            // Если мы здесь, то у нас есть что и куда записывать. Пытаемся произвести запись
            Program.Log.Write(String.Format("{0}::WriteMasterFrame() -> Попытка записи мастер-записи", this.GetType().Name));
            // Локальная переменная для хранения возвращаемого результата (должна быть доступна извне блока try)
            bool bResult = true;

            try
            {
                // Используем метод с указанием количества записываемых байт, чтобы изменение 
                // объявленной размерности массива в сторону увеличения не привело к перезаписи данных
                // Write(buffer, startIndex, NumberOfBytesToWrite)
                _bWriter.Write(_masterFrame, 0, 256);
            }
            catch (System.ArgumentNullException e)
            {
                // буфер = null
                Program.Log.Write(String.Format("{0}::WriteMasterFrame() -> Исключение. Буфер == null", this.GetType().Name), Logs.MsgType.Error);
                Program.Log.Write(String.Format("{0}::WriteMasterFrame() -> Описание: {1}", this.GetType().Name, e.Message), Logs.MsgType.Error);
                Console.WriteLine("{0} Exception caught in WriteMasterFrame()", e);
            }
            catch (System.ArgumentOutOfRangeException e)
            {
                // startIndex или NumberOfBytesToWrite меньше нуля
                Program.Log.Write(String.Format("{0}::WriteMasterFrame() -> Исключение. Параметры вызова BinaryWriter.Write имеют отрицательное значение", this.GetType().Name), Logs.MsgType.Error);
                Program.Log.Write(String.Format("{0}::WriteMasterFrame() -> Описание: {1}", this.GetType().Name, e.Message), Logs.MsgType.Error);
                Console.WriteLine("{0} Exception caught in WriteMasterFrame()", e);
            }
            catch (System.ArgumentException e)
            {
                // Число байт для записи превышает число байт от startIndex до конца буфера ИЛИ иная ошибка в аргументе
                Program.Log.Write(String.Format("{0}::WriteMasterFrame() -> Исключение. Попытка произвести запись большего числа байт чем есть в буфере от точки старта, либо иная ошибка в аргументе", this.GetType().Name), Logs.MsgType.Error);
                Program.Log.Write(String.Format("{0}::WriteMasterFrame() -> Описание: {1}", this.GetType().Name, e.Message), Logs.MsgType.Error);
                Console.WriteLine("{0} Exception caught in WriteMasterFrame()", e);
            }
            catch (System.IO.IOException e)
            {
                // Ловим все исключения ввода-вывода в одном месте
                Program.Log.Write(String.Format("{0}::WriteMasterFrame() -> Исключение ввода-вывода, переброс выше по стеку вызовов. См. отладочную информацию для более подробных сведений", this.GetType().Name), Logs.MsgType.Error);
                Program.Log.Write(String.Format("{0}::WriteMasterFrame() -> Описание: {1}", this.GetType().Name, e.Message), Logs.MsgType.Error);
                Console.WriteLine("{0} Exception caught in WriteMasterFrame() and rethrown.", e);
                // Не обрабатываем никаких исключений здесь, перекидываем их выше по стеку вызовов.
                throw;
            }
            catch (System.ObjectDisposedException e)
            {
                // Файловый поток закрыт
                Program.Log.Write(String.Format("{0}::WriteMasterFrame() -> Исключение, переброс выше по стеку вызовов. Файловый поток закрыт. См. отладочную информацию для более подробных сведений", this.GetType().Name), Logs.MsgType.Error);
                Program.Log.Write(String.Format("{0}::WriteMasterFrame() -> Описание: {1}", this.GetType().Name, e.Message), Logs.MsgType.Error);
                Console.WriteLine("{0} Exception caught in WriteMasterFrame() and rethrown.", e);
                // Не обрабатываем никаких исключений здесь, перекидываем их выше по стеку вызовов.
                throw;
            }
            finally
            {
                // Раз мы зашли в блок finally, значит произошло исключение
                // Ничего не исправлем, просто констатируем факт произошедшей ошибки
                bResult = false;
            }

            return bResult;
        }
        #endregion

        #region Методы класса - заготовки для переопределения в классах-наследниках
        /// <summary>
        /// Метод, производящий расшифровку служебной информации из начала файла (мастер-запись). 
        /// При расшифровке необходимо учитывать версию данных, записанных в файл.
        /// Вызов метода напрямую ЗАПРЕЩЕН, используйте перегруженный метод класса-наследника.
        /// </summary>
        /// <returns>true, если мастер-запись успешно раскодирована</returns>
        protected virtual bool DecodeMasterFrame()
        {
            // Метод базового класса не предназначен для прямого вызова. Должен использоваться виртуальный метод класса-наследника
            Program.Log.Write("BinaryStreamMaster::DecodeMasterFrame() -> Вызов метода базового класса запрещен. Используйте перегруженный метод", Logs.MsgType.Error);
            Program.Log.Write("BinaryStreamMaster::DecodeMasterFrame() -> Генерация исключения, останов программы", Logs.MsgType.Error);
            throw new NotImplementedException();
        }

        /// <summary>
        /// Метод, производящий кодирование служебной информации в байтовый массив для его 
        /// последующей записи в начало файла данных (мастер-запись).
        /// Вызов метода напрямую ЗАПРЕЩЕН, используйте перегруженный метод класса-наследника.
        /// </summary>
        /// <param name="VersionData">Версия данных, в которой следуют производить кодировку мастер-записи</param>
        /// <returns>true, если конверсия успешно произведена и можно сохранять изменения</returns>
        protected virtual bool EncodeMasterFrame(int VersionData)
        {
            // Метод базового класса не предназначен для прямого вызова. Должен использоваться виртуальный метод класса-наследника
            Program.Log.Write("BinaryStreamMaster::EncodeMasterFrame() -> Вызов метода базового класса запрещен. Используйте перегруженный метод", Logs.MsgType.Error);
            Program.Log.Write("BinaryStreamMaster::EncodeMasterFrame() -> Генерация исключения, останов программы", Logs.MsgType.Error);
            throw new NotImplementedException();
        }
        
        /// <summary>
        /// Метод, производящий перевод старой версии мастер-записи в новую версию 
        /// (в текущую версию кода).
        /// Вызов метода напрямую ЗАПРЕЩЕН, используйте перегруженный метод класса-наследника.
        /// </summary>
        /// <returns>true, если конверсия успешно произведена и можно сохранять изменения</returns>
        protected virtual bool UpgradeMasterFrame()
        {
            // Метод базового класса не предназначен для прямого вызова. Должен использоваться виртуальный метод класса-наследника
            Program.Log.Write("BinaryStreamMaster::UpgradeMasterFrame() -> Вызов метода базового класса запрещен. Используйте перегруженный метод", Logs.MsgType.Error);
            Program.Log.Write("BinaryStreamMaster::UpgradeMasterFrame() -> Генерация исключения, останов программы", Logs.MsgType.Error);
            throw new NotImplementedException();
        }
        #endregion
    }
}
