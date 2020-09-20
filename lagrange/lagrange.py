# import math


# def interpolate(function_table, interpolation_point_x):
#
#     numerator_terms = [interpolation_point_x - node_point_x for (node_point_x, _) in function_table]
#     interpolation_point_y = 0.0
#
#     for i, (node_point_x_i, node_point_y_i) in enumerate(function_table):
#
#         numerator = math.prod(numerator_terms[:i] + numerator_terms[i + 1:])
#
#         denominator = math.prod([node_point_x_i - node_point_x for (node_point_x, _) in
#                                  function_table[:i] + function_table[i + 1:]])
#
#         interpolation_point_y += numerator / denominator * node_point_y_i
#
#     return interpolation_point_y

# Вычисляет значение интерполяционного многочлена Лагранжа, постороенного по таблице значений
# function_table, в точке interpolation_point_x
def interpolate(function_table, interpolation_point_x):

    interpolation_point_y = 0.0

    for i, (node_point_x_i, node_point_y_i) in enumerate(function_table):

        sum_term = node_point_y_i

        for j, (node_point_x_j, node_point_y_j) in enumerate(function_table):

            if i != j:
                sum_term *= (interpolation_point_x - node_point_x_j) / (node_point_x_i - node_point_x_j)

        interpolation_point_y += sum_term

    return interpolation_point_y
