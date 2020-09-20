import lagrange
import math
from format import format_float
from format import format_float_table
from format import format_error


def function(x):
    return 2 * x  # x ** 2 + 7 * (x ** 4) + 4 * (x ** 6)  # math.cos(x) + 2 * x


def interpolation_ui():
    # Число значений в таблице функции
    number_of_values = 0

    # Начало отрезка
    start = 0.0

    # Конец отрезка
    end = 0.0

    # Таблица значений функции
    value_table = []

    # Точка интерполяции
    interpolation_point_x = 0.0

    # Степень интерполяционного многочлена
    interpolation_degree = 0

    while True:
        try:
            number_of_values = int(input("Введите число значений в таблице функции: "))
            if number_of_values < 2:
                raise ValueError
            break
        except ValueError:
            print(format_error("Некорректное значение: введите целое число, большее единицы"))

    while True:
        try:
            start = float(input("Введите начало отрезка: "))
            break
        except ValueError:
            print(format_error("Некорректное значение: введите вещественное число"))

    while True:
        try:
            end = float(input("Введите конец отрезка: "))
            if end <= start:
                raise ValueError
            break
        except ValueError:
            print(format_error("Некорректное значение: введите вещественное число, большее чем начало отрезка"))

    segment_length = (end - start) / (number_of_values - 1)

    for i in range(number_of_values):
        x = start + segment_length * i
        value_table.append((x, function(x)))

    print(f"Таблица значений функции (число значений: {number_of_values}):")

    print(format_float_table(value_table, "x", "f(x)"))

    while True:
        try:
            interpolation_point_x = float(input("Введите точку интерполяции: "))
            break
        except ValueError:
            print(format_error("Некорректное значение: введите вещественное число"))

            interpolation_point_x = 0.0

    while True:
        try:
            interpolation_degree = int(input(f"Введите степень интерполяционного многочлена (< {number_of_values}): "))
            if interpolation_degree < 1 or interpolation_degree >= number_of_values:
                raise ValueError
            break
        except ValueError:
            print(format_error("Некорректное значение: введите целое число, большее нуля и меньшее числа значений в "
                               "таблице функции"))

    value_table.sort(key=lambda xy: abs(xy[0] - interpolation_point_x))
    interpolation_nodes = value_table[:interpolation_degree + 1]

    print("Узлы интерполяции:")

    print(format_float_table(interpolation_nodes, "x", "f(x)"))

    print(f"""Точка интерполирования: {format_float(interpolation_point_x)}
Степень интерполяционного многочлена: {interpolation_degree}

РЕЗУЛЬТАТ:""")

    interpolation_point_y = lagrange.interpolate(interpolation_nodes, interpolation_point_x)
    print(f"Значение интерполяционного многочлена степени {interpolation_degree} в точке интерполирования {format_float(interpolation_point_x)}"
          f": {format_float(interpolation_point_y)}")

    value_error = abs(function(interpolation_point_x) - interpolation_point_y)
    print(f"Абсолютная фактическая погрешность: {format_float(value_error)}")

    if input("\nЧтобы выйти из программы, введите q: ") == "q":
        return True

    return False


if __name__ == '__main__':

    print(f"""
ЛАБОРАТОРНАЯ РАБОТА №2

Задача алгебраического интерполирования
Интерполяционный многочлен в форме Лагранжа

Вариант №10
""")

    while True:
        if interpolation_ui():
            break
