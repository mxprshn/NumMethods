import lagrange
import math


def function(x):
    return x ** 2 + 7 * (x ** 4) + 4 * (x ** 6)  # math.cos(x) + 2 * x


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

    valueTable = []
    segmentLength = (end - start) / (numberOfValues - 1)

    for i in range(numberOfValues):
        x = start + segmentLength * i
        valueTable.append((x, function(x)))

    print("Таблица значений функции:")

    for x, y in valueTable:
        print(float_to_string(x) + " | " + float_to_string(y))

    interpolationPointX = 0.0

    while True:
        try:
            interpolationPointX = float(input("Введите точку интерполяции: "))
            break
        except ValueError:
            print_error("Некорректное значение: введите вещественное число")

            interpolationPointX = 0.0

    interpolationDegree = 0

    while True:
        try:
            interpolationDegree = int(input(f"Введите степень интерполяционного многочлена (< {numberOfValues}): "))
            if interpolationDegree < 1 or interpolationDegree >= numberOfValues:
                raise ValueError
            break
        except ValueError:
            print_error("Некорректное значение: введите целое число, большее нуля и меньшее числа значений в таблице "
                        "функции")

    valueTable.sort(key=lambda xy: abs(xy[0] - interpolationPointX))
    interpolationNodes = valueTable[:interpolationDegree + 1]

    print("Узлы интерполяции:")

    for x, y in interpolationNodes:
        print(float_to_string(x) + " | " + float_to_string(y))

    print(f"""
Точка интерполирования: {float_to_string(interpolationPointX)}
Степень интерполяционного многочлена: {interpolationDegree}

{separator}
    """)

    interpolation_point_y = lagrange.interpolate(interpolationNodes, interpolationPointX)
    print(f"Значение интерполяционного многочлена в точке {float_to_string(interpolationPointX)}"
          f": {float_to_string(interpolation_point_y)}")

    value_error = abs(function(interpolationPointX) - interpolation_point_y)
    print(f"Абсолютная фактическая погрешность: {float_to_string(value_error)}")
