using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Exeter7
{
    /// <summary>
    /// Статический специализированный класс, упрощающий работу с двумерным массивом типа bool, 
    /// при помощи которого передаются свойства запчастей (перечисления-битовые поля).
    /// </summary>
    /// <remarks>Массив bool не использутся внутри класса WhereUsed, он нужен вне класса, 
    /// как правило, при отображении данных в интерфейсе пользователя, где 
    /// элементы единого массива удобнее битовых полей разных перечислений-флагов. 
    /// Кроме того, поскольку число перечислений-флагов ATM_xxx жестко задано, 
    /// класс позволяет также провести некоторые проверки на допустимость.</remarks>
    public static class UsageArrayHelper
    {

        /// <summary>
        /// Статический метод, создающий новый массив bool необходимой размерности
        /// </summary>
        public static bool[,] Create()
        {
            // Создаем массив требуемой размерности
            return new bool[AppSettings.countATM_Enums, 32];
        }

        /// <summary>
        /// Статический метод, проверяющий соответствие размерности проверяемого массива 
        /// заданной размерности.
        /// </summary>
        /// <param name="testArray">Массив для проверки</param>
        /// <returns>true, если размерность соответствует заданной</returns>
        public static bool Validate(bool[,] testArray)
        {

            // Проверяем правильных ли размеров массив нам передали
            // 1) В текущей версии кода у нас AppSettings.countATM_Enums (=9) переменных-перечислений.
            // 2) В текущей версии кода у нас по 32 флага в каждом перечислении.
            if ((testArray.GetLength(0) != AppSettings.countATM_Enums) && (testArray.GetLength(1) != 32))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Статический метод, складывающий array1 и array2 (логическое ИЛИ).
        /// </summary>
        /// <param name="array1">(ref) Массив 1, он же результат операции.</param>
        /// <param name="array2">Массив 2</param>
        /// <returns>Результирующий массив</returns>
        public static void AddArray(ref bool[,] array1, bool[,] array2)
        {
            if (UsageArrayHelper.Validate(array1))
            {
                if (UsageArrayHelper.Validate(array2))
                {
                    // Для каждого перечисления
                    for (int i = 0; i < AppSettings.countATM_Enums; i++)
                    {
                        // Для каждого поля в перечислении (32 поля в перечислении)
                        for (int j = 0; j < 32; j++)
                        {
                            // Складываем элементы типа bool
                            array1[i, j] = array1[i, j] | array2[i, j];
                        }
                    }
                }
                else
                {
                    // Поскольку мы не знаем кто вызвал этот метод и не можем продолжать работу 
                    // (ошибка в размерности array2 весьма серьезная), генерируем исключение. 
                    // Так мы хотя бы сможем увидеть стек вызовов.
                    throw new System.Exception("UsageArray::AddArray -> 'array2' dimensions are (" + 
                        array2.GetLength(0).ToString() + "," + 
                        array2.GetLength(1).ToString() + ") instead of (" + 
                        AppSettings.countATM_Enums.ToString() + ", 32).");
                }
            }
            else
            {
                // Поскольку мы не знаем кто вызвал этот метод и не можем продолжать работу 
                // (ошибка в размерности array1 весьма серьезная), генерируем исключение. 
                // Так мы хотя бы сможем увидеть стек вызовов.
                throw new System.Exception("UsageArray::AddArray -> 'array1' dimensions are (" + 
                    array1.GetLength(0).ToString() + "," + 
                    array1.GetLength(1).ToString() + ") instead of (" + 
                    AppSettings.countATM_Enums.ToString() + ", 32).");
            }
        }

        /// <summary>
        /// Статический метод, умножающий array1 и array2 (логическое И).
        /// </summary>
        /// <param name="array1">(ref) Массив 1, он же результат операции.</param>
        /// <param name="array2">Массив 2</param>
        /// <returns>Результирующий массив</returns>
        public static void MultiplyArray(ref bool[,] array1, bool[,] array2)
        {
            if (UsageArrayHelper.Validate(array1))
            {
                if (UsageArrayHelper.Validate(array2))
                {
                    // Для каждого перечисления
                    for (int i = 0; i < AppSettings.countATM_Enums; i++)
                    {
                        // Для каждого поля в перечислении (32 поля в перечислении)
                        for (int j = 0; j < 32; j++)
                        {
                            // Складываем элементы типа bool
                            array1[i, j] = array1[i, j] & array2[i, j];
                        }
                    }
                }
                else
                {
                    // Поскольку мы не знаем кто вызвал этот метод и не можем продолжать работу 
                    // (ошибка в размерности array2 весьма серьезная), генерируем исключение. 
                    // Так мы хотя бы сможем увидеть стек вызовов.
                    throw new System.Exception("UsageArray::MultiplyArray -> 'array2' dimensions are (" +
                        array2.GetLength(0).ToString() + "," +
                        array2.GetLength(1).ToString() + ") instead of (" +
                        AppSettings.countATM_Enums.ToString() + ", 32).");
                }
            }
            else
            {
                // Поскольку мы не знаем кто вызвал этот метод и не можем продолжать работу 
                // (ошибка в размерности array1 весьма серьезная), генерируем исключение. 
                // Так мы хотя бы сможем увидеть стек вызовов.
                throw new System.Exception("UsageArray::MultiplyArray -> 'array1' dimensions are (" +
                    array1.GetLength(0).ToString() + "," +
                    array1.GetLength(1).ToString() + ") instead of (" +
                    AppSettings.countATM_Enums.ToString() + ", 32).");
            }
        }

    }
}
