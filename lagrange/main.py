import lagrange
import math


def function(x):
    return 2 * x  # math.cos(x) + 2 * x


separator = "________________________________________________________"


def print_error(message):
    print("ОШИБКА: " + message)


def float_to_string(value):
    return "{:30.20f}".format(value)


if __name__ == '__main__':

    print(f"""
ЛАБОРАТОРНАЯ РАБОТА №2

Задача алгебраического интерполирования
Интерполяционный многочлен в форме Лагранжа

Вариант №10

{separator}
    """)

    numberOfValues = 0

    while True:
        try:
            numberOfValues = int(input("Введите число значений в таблице функции: "))
            if numberOfValues < 2:
                raise ValueError
            break
        except ValueError:
            print_error("Некорректное значение: введите целое число, большее единицы")

    start = 0.0

    while True:
        try:
            start = float(input("Введите начало отрезка: "))
            break
        except ValueError:
            print_error("Некорректное значение: введите вещественное число")

    end = 0.0

    while True:
        try:
            end = float(input("Введите конец отрезка: "))
            if end <= start:
                raise ValueError
            break
        except ValueError:
            print_error("Некорректное значение: введите вещественное число, большее чем начало отрезка")

    valueTable = {}
    segmentLength = (end - start) / numberOfValues

    for i in range(numberOfValues):
        x = start + segmentLength * i
        valueTable[x] = function(x)

    print("Таблица значений функции:\n")

    for x, y in valueTable.items():
        print(float_to_string(x) + " | " + float_to_string(y))
